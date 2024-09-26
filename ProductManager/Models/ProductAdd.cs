using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class ProductAdd
    {
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } = "";

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        public string Tags { get; set; } = "";

        [Required]
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
