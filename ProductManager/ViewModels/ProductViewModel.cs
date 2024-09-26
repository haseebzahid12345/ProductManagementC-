namespace ProductManager.ViewModels
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Tags { get; set; }
        public List<string> Categories { get; set; } // List of category names
    }
}
