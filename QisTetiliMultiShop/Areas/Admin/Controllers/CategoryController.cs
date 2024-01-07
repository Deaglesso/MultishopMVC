using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QisTetiliMultiShop.Areas.Admin.ViewModel;
using QisTetiliMultiShop.DAL;
using QisTetiliMultiShop.Models;
using QisTetiliMultiShop.Utilities.Extensions;

namespace QisTetiliMultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int limit = 4;
            double count = await _db.Categories.CountAsync();
            if (count != 0)
            {
                if (page > (int)Math.Ceiling(count / limit) || page <= 0)
                {
                    return BadRequest();
                }
            }
            
            List<Category> CategoryList = await _db.Categories.Skip((page - 1) * limit).Take(limit).Include(x=>x.Products).ToListAsync();
            PaginationVM<Category> paginationVM = new PaginationVM<Category>
            {
                Items = CategoryList,
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
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = await _db.Categories.IgnoreQueryFilters().AnyAsync(x => x.Name == categoryVM.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "Already exists.");
                return View();
            }
            if (!categoryVM.File.CheckFileSize(2))
            {
                ModelState.AddModelError("File", "Max file size is 2MB.");
                return View();
            }
            if (!categoryVM.File.CheckFileType("image"))
            {
                ModelState.AddModelError("File", "Only image files supported.");
                return View();
            }
            Category category = new Category { Name = categoryVM.Name, Image = await categoryVM.File.CreateFileAsync(_env.WebRootPath, "assets", "img") };
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Update(int id)
        {

            if (id <= 0)
            {
                return BadRequest();
            }
            Category category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            UpdateCategoryVM categoryVM = new UpdateCategoryVM
            {
                Name = category.Name,
                Products = category.Products,
                Image = category.Image

            };
            return View(categoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM newCategory)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Category oldCategory = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (oldCategory == null) return NotFound();

            bool result = await _db.Categories.IgnoreQueryFilters().AnyAsync(x => x.Name == newCategory.Name && x.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This name already used in other category");
                return View();

            }
            if (newCategory.File is not null)
            {
                if (!newCategory.File.CheckFileType("image"))
                {
                    ModelState.AddModelError("Photo", "You need to choose image file.");
                    return View(oldCategory);
                }
                if (!newCategory.File.CheckFileSize(2))
                {
                    ModelState.AddModelError("Photo", "You need to choose up to 2MB.");
                    return View(oldCategory);
                }
                string newimage = await newCategory.File.CreateFileAsync(_env.WebRootPath, "assets", "img");
                oldCategory.Image.Delete(_env.WebRootPath, "assets", "img");
                oldCategory.Image = newimage;
            }
            oldCategory.Name = newCategory.Name;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Delete(int id)
        {

            Category category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category is null) return NotFound();

            //slide.Image.Delete(_env.WebRootPath, "assets", "img");
            //_db.Slides.Remove(slide);

            category.DeletedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Category category = await _db.Categories
            .Include(x => x.Products)
                .ThenInclude(x => x.ProductColors)
                    .ThenInclude(x => x.Color)
            .Include(x => x.Products)
                .ThenInclude(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
    }
}
