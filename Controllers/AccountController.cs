using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Project_backend.Models;
using Project_backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<Users> _userManager;
    private readonly ProductDBContext _context;
    private readonly IConfiguration _configuration;

    public AccountController(UserManager<Users> userManager, ProductDBContext context, IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterDto model)
    {
        var user = new Users { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Create a cart for the new user
            var cart = new Cart { UserId = user.Id };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // Now update the user entity with the newly created cart
            user.Cart = cart;
            _context.Update(user);
            await _context.SaveChangesAsync();

            // Add user to "Client" role
            var roleResult = await _userManager.AddToRoleAsync(user, "Client");
            if (!roleResult.Succeeded)
            {
                // Handle the error in adding to role
                return BadRequest(roleResult.Errors);
            }

            var UserRole = await _userManager.GetRolesAsync(user);
            return Ok(new { userId = user.Id, cartId = cart.CartId, role = UserRole});
        }

        // Handle failure in user creation
        return BadRequest(result.Errors);
    }


    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                // Add other claims as needed
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            var UserRole = await _userManager.GetRolesAsync(user);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                userId = user.Id,
                role = UserRole[0],
                userName = user.UserName
            })  ;
        }
        return Unauthorized();
    }

    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        // Add any other properties you need for registration
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
