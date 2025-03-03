using Microsoft.AspNetCore.Mvc;
using Restaurant.Data;
using Restaurant.Models;

namespace Restaurant.Controllers
{
    public class ProductController : Controller
    {
        private Repository<Product> products;
        private Repository<Ingredient> ingredients;
        private Repository<Category> categories;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            products = new Repository<Product>(context);
            ingredients = new Repository<Ingredient>(context);
            categories = new Repository<Category>(context);
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await products.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            if (id == 0)
            {
                ViewBag.Operation = "Add";
                ViewBag.Ingredients = await ingredients.GetAllAsync();
                ViewBag.Categories = await categories.GetAllAsync();
                return View(new Product());
            } else
            {
                ViewBag.Operation = "Edit";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(Product product, int[] ingredientIds, int catId)
        {
            if (ModelState.IsValid)
            {
                if(product.ImageFile != null)
                {
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFilename = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string filepath = Path.Combine(uploadFolder, uniqueFilename);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = uniqueFilename;
                }

                if(product.ProductId == 0)
                {
                    ViewBag.Ingredients = await ingredients.GetAllAsync();
                    ViewBag.Categories = await categories.GetAllAsync();
                    product.CategoryId = catId;

                    foreach(int id in ingredientIds)
                    {
                        product.ProductIngredients?.Add(new ProductIngredient { IngredientId = id, ProductId = product.ProductId });
                    }

                    await products.AddAsync(product);
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    return RedirectToAction("Index", "Product");
                }
            }
            else
            {
                return View(product);
            }
        }
    }
}
