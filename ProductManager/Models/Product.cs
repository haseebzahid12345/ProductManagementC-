using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [MaxLength(100)]
        public string ProductName { get; set; } = "";

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Store tags as a comma-separated string
        public string Tags { get; set; } = "";

        // Add this navigation property for the many-to-many relationship
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}

