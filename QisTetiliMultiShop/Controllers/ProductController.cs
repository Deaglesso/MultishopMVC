using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QisTetiliMultiShop.DAL;
using QisTetiliMultiShop.Models;
using QisTetiliMultiShop.ViewModels;

namespace QisTetiliMultiShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;


        public ProductController(AppDbContext context)
        {
            _db = context;
        }


        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = _db.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductColors).ThenInclude(pt => pt.Color)
                .FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();


            ProductVM vm = new ProductVM
            {
                Product = product,
                RelatedProducts = _db.Products.Where(p => p.Category.Id == product.CategoryId && p.Id != product.Id).Include(x => x.ProductImages).ToList(),
            };


            return View(vm);
        }
    }
}
