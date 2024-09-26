namespace ProductManager.Models
{
    public class ProductCategory
    {
        public int ProductID { get; set; }
        public required Product Product { get; set; }

        public int CategoryID { get; set; }
        public required Category Category { get; set; }
    }
}
