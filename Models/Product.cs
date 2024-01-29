using System.ComponentModel.DataAnnotations;

namespace Project_backend.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, Range(0.1, 10000, ErrorMessage = "Insert a valid price.")]
        public decimal Price { get; set; }

        public string Description { get; set; } // Add the Description property

        // Property to store image file path
        public string ImagePath { get; set; }

    }
}
