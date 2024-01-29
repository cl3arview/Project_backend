using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Project_backend.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual List<Product> Items { get; set; } = new List<Product>();

        /*

                [NotMapped]
               public decimal TotalPrice
               {
                   get
                   {
                       return Items.Sum(item => item.Price);
                   }
               }

         */
    }
}
