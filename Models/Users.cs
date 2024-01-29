using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Project_backend.Models
{
    public class Users : IdentityUser
    {
        [Required]
        public bool isAdmin { get; set; } = false;

        public int CartId { get; set; }
        public Cart Cart { get; set; }

    }
}
