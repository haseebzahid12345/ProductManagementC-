using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        [MaxLength(100)]
        public string CategoryName { get; set; } = "";

        // Add this navigation property for the many-to-many relationship
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
