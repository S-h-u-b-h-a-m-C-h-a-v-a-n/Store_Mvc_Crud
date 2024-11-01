using Microsoft.AspNetCore.Mvc;
using Store_Mvc_Crud.Models;
using Store_Mvc_Crud.Services;

namespace Store_Mvc_Crud.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationdbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationdbContext context,IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            var prodects = context.Products.OrderByDescending(p => p.Id).ToList();

            return View(prodects);
        }
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(ProductDto productdto)
        {
            if (productdto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile","The Image File is Required");
            }
            if (!ModelState.IsValid)
            {
                return View(productdto);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName = Path.GetExtension(productdto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/products" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productdto.ImageFile.CopyTo(stream);
            }

            Product product = new Product()
            {
                Name = productdto.Name,
                Brand = productdto.Brand,
                Category = productdto.Category,
                Price = productdto.Price,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,

            };
            context.Products.Add(product);
            context.SaveChanges();

                return RedirectToAction("Index", "Products");
        }


        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index","Products");
            }

            var productdto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Description = product.Description,
            };


            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyyy");

            return View(productdto);

         
        }


        [HttpPost]
        public IActionResult Edit(int id, ProductDto productdto)
        {
            var product = context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
                return View(productdto);
            }

            string newFileName = product.ImageFileName;

            // Check if a new image file has been uploaded
            if (productdto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productdto.ImageFile.FileName);

                // Save the new image file
                string imageFullPath = Path.Combine(environment.WebRootPath, "products", newFileName);
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productdto.ImageFile.CopyTo(stream);
                }

                // Delete the old image file, if it exists
                string oldImageFullPath = Path.Combine(environment.WebRootPath, "products", product.ImageFileName);
                if (System.IO.File.Exists(oldImageFullPath))
                {
                    System.IO.File.Delete(oldImageFullPath);
                }
            }

            // Update product properties
            product.Name = productdto.Name;
            product.Brand = productdto.Brand;
            product.Category = productdto.Category;
            product.Price = productdto.Price;
            product.Description = productdto.Description;
            product.ImageFileName = newFileName;

            // Save changes to the database
            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            string imageFullPath = environment.WebRootPath + "/products" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Products.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index","Products");
        }
    }
}
