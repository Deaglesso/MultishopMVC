using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QisTetiliMultiShop.Areas.Admin.ViewModel;
using QisTetiliMultiShop.DAL;
using QisTetiliMultiShop.Models;

namespace QisTetiliMultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorController : Controller
    {
        private readonly AppDbContext _db;

        public ColorController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int limit = 4;
            double count = await _db.Colors.CountAsync();
            if (count != 0)
            {
                if (page > (int)Math.Ceiling(count / limit) || page <= 0)
                {
                    return BadRequest();
                }
            }
            List<Color> ColorList = await _db.Colors.Skip((page - 1) * limit).Take(limit).ToListAsync();

            PaginationVM<Color> paginationVM = new PaginationVM<Color>
            {
                Items = ColorList,
                TotalPage = (int)Math.Ceiling(count / limit),
                CurrentPage = page,
                Limit = limit
            };
            return View(paginationVM);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = await _db.Colors.IgnoreQueryFilters().AnyAsync(x => x.Name == colorVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Already exists.");
                return View();
            }
            Color Color = new Color
            {
                Name = colorVM.Name,
            };
            await _db.Colors.AddAsync(Color);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Color color = await _db.Colors.FirstOrDefaultAsync(s => s.Id == id);

            if (color is null) return NotFound();

            UpdateColorVM colorVM = new UpdateColorVM
            {
                Name = color.Name,
                ProductColors = color.ProductColors,
            };

            return View(colorVM);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateColorVM colorVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Color existed = await _db.Colors.FirstOrDefaultAsync(e => e.Id == id);
            if (existed is null) return NotFound();

            bool result = _db.Colors.IgnoreQueryFilters().Any(c => c.Name == colorVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Color already exists");
                return View();
            }


            existed.Name = colorVM.Name;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            Color color = await _db.Colors.FirstOrDefaultAsync(x => x.Id == id);
            if (color is null) return NotFound();

            //slide.Image.Delete(_env.WebRootPath, "assets", "img");
            //_db.Slides.Remove(slide);

            color.DeletedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var Color = await _db.Colors.Include(s => s.ProductColors).ThenInclude(p => p.Product).ThenInclude(pi => pi.ProductImages).FirstOrDefaultAsync(x => x.Id == id);

            if (Color is null) return NotFound();

            return View(Color);
        }

    }
}
