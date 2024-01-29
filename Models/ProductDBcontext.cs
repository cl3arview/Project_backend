    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    namespace Project_backend.Models
    {
        public class ProductDBContext : IdentityDbContext<Users>
        {
            public ProductDBContext(DbContextOptions<ProductDBContext> options) : base(options)
            {
            }

            public DbSet<Cart> Carts { get; set; }
            public DbSet<Product> Products { get; set; }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder);

                // Define the relationship between Users and Cart
                builder.Entity<Users>()
                    .HasOne(u => u.Cart)
                    .WithOne()
                    .HasForeignKey<Cart>(c => c.UserId);

                // Additional configurations...

            }
        }
    }
