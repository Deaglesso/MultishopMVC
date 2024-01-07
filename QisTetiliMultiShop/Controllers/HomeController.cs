using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QisTetiliMultiShop.DAL;
using QisTetiliMultiShop.ViewModels;

namespace QisTetiliMultiShop.Controllers
{

    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            HomeVM vm = new HomeVM 
            {
                Slides = await _db.Slides.ToListAsync(),
                Categories = await _db.Categories.ToListAsync(),
                Products = await _db.Products.Include(x=>x.ProductImages).ToListAsync(),
            };   
            return View(vm);
        }
    }
}
