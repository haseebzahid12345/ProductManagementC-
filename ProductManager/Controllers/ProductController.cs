//using Microsoft.AspNetCore.Mvc;
//using ProductManager.Services;

//namespace ProductManager.Controllers
//{
//    public class ProductController : Controller
//    {
//        private readonly ApplicationDbContext context;
//        public ProductController(ApplicationDbContext context)
//        {
//            this.context = context;
//        }
//        public IActionResult Index()
//        {

//            return View();
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For Include
using ProductManager.Models;
using ProductManager.Services;
using ProductManager.ViewModels; // Add this using directive for your ViewModel

namespace ProductManager.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;

        public ProductController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            // Fetch products with their associated categories
            var products = context.Prodct
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category) // Assuming you have a navigation property in ProductCategory to Category
                .ToList();

            // Create a ViewModel list
            var productViewModels = products.Select(p => new ProductViewModel
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Price = p.Price,
                Quantity = p.Quantity,
                Tags = p.Tags,
                Categories = p.ProductCategories.Select(pc => pc.Category.CategoryName).ToList() // Assuming Category has a CategoryName property
            }).ToList();

            return View(productViewModels);
        }


        [HttpGet]
        public IActionResult Add()
        {
            // Fetch the list of categories from the database
            var categories = context.Categories
                .Select(c => new { c.CategoryID, c.CategoryName })
                .ToList();

            // Pass the list of categories to the ViewBag
            ViewBag.Categories = categories;

            return View();
        }

        // POST: Add Product
        [HttpPost]
        public IActionResult Add(ProductAdd model)
        {
            if (ModelState.IsValid)
            {
                var newProduct = new Product
                {
                    ProductName = model.ProductName,
                    Price = model.Price,
                    Quantity = model.Quantity,
                    Tags = model.Tags
                };

                foreach (var categoryId in model.CategoryIds)
                {
                    var category = context.Categories.FirstOrDefault(c => c.CategoryID == categoryId);
                    if (category != null)
                    {
                        var productCategory = new ProductCategory
                        {
                            Product = newProduct,
                            Category = category
                        };
                        context.ProductCategories.Add(productCategory);
                    }
                }

                context.Prodct.Add(newProduct);
                context.SaveChanges();

                return RedirectToAction("Index");
            }

            // If ModelState is invalid, reload the categories and redisplay the form
            ViewBag.Categories = context.Categories
                .Select(c => new { c.CategoryID, c.CategoryName })
                .ToList();

            return View(model);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = context.Prodct.FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            context.Prodct.Remove(product);
            context.SaveChanges();

            return RedirectToAction("Index"); // This refreshes the page to show updated data
        }


        public IActionResult Edit(int id)
        {
            // Fetch the product by ID and include associated categories
            var product = context.Prodct
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            // Create a ViewModel with product details
            var productViewModel = new ProductViewModel()
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = product.Quantity,
                Tags = product.Tags,
                Categories = product.ProductCategories.Select(pc => pc.Category.CategoryName).ToList()
            };

            // Load all categories for the view (dropdown, etc.)
            ViewBag.AllCategories = context.Categories.ToList();

            return View(productViewModel);
        }


        [HttpPost]
        public IActionResult Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload categories in case of validation error
                ViewBag.AllCategories = context.Categories.ToList();
                return View(model);
            }

            // Find the product by ID, including related categories
            var product = context.Prodct
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefault(p => p.ProductID == model.ProductID);

            if (product == null)
            {
                return NotFound();
            }

            // Update product details
            product.ProductName = model.ProductName;
            product.Price = model.Price;
            product.Quantity = model.Quantity;
            product.Tags = model.Tags;

            // Clear existing categories and update with new ones
            product.ProductCategories.Clear();

            if (model.Categories != null && model.Categories.Count > 0)
            {
                foreach (var categoryName in model.Categories)
                {
                    var category = context.Categories.FirstOrDefault(c => c.CategoryName == categoryName);
                    if (category != null)
                    {
                        product.ProductCategories.Add(new ProductCategory
                        {
                            ProductID = product.ProductID,
                            CategoryID = category.CategoryID,
                            Product = product,
                            Category = category
                        });
                    }
                }
            }

            // Save changes to the database
            context.SaveChanges();

            return RedirectToAction("Index");
        }

      [Route("Product")]



        // For search functionality
        [HttpGet("Search")]
        public IActionResult Search(string searchTag, string searchCategory)
        {
            // Start by querying the database for products and their associated categories
            var query = context.Prodct
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .AsQueryable();

            // Filter by tag if provided
            if (!string.IsNullOrEmpty(searchTag))
            {
                query = query.Where(p => p.Tags != null && p.Tags.Contains(searchTag, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by category if provided
            if (!string.IsNullOrEmpty(searchCategory))
            {
                query = query.Where(p => p.ProductCategories.Any(pc => pc.Category.CategoryName.Equals(searchCategory, StringComparison.OrdinalIgnoreCase)));
            }

            // Execute the query and project it into the view model
            var productViewModels = query.Select(p => new ProductViewModel
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Price = p.Price,
                Quantity = p.Quantity,
                Tags = p.Tags,
                Categories = p.ProductCategories.Select(pc => pc.Category.CategoryName).ToList()
            }).ToList();

            // Return the Index view and pass the search results to it
            return View("Index", productViewModels);
        }


    }


}



