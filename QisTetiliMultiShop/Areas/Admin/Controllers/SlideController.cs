using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QisTetiliMultiShop.Areas.Admin.ViewModel;
using QisTetiliMultiShop.DAL;
using QisTetiliMultiShop.Models;
using QisTetiliMultiShop.Utilities.Extensions;

namespace QisTetiliMultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public SlideController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int limit = 4;
            double count = await _db.Slides.CountAsync();
            if (count != 0)
            {
                if (page > (int)Math.Ceiling(count / limit) || page <= 0)
                {
                    return BadRequest();
                }
            }
            List<Slide> SlideList = await _db.Slides.Skip((page - 1) * limit).Take(limit).ToListAsync();

            PaginationVM<Slide> paginationVM = new PaginationVM<Slide>
            {
                Items = SlideList,
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
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (await _db.Slides.AnyAsync(x=>x.Order == slideVM.Order))
            {
                slideVM.Order = (await _db.Slides.OrderByDescending(x => x.Order).FirstOrDefaultAsync()).Order+1;
            }
            if (!slideVM.File.CheckFileSize(2))
            {
                ModelState.AddModelError("File", "Max file size is 2MB.");
                return View();
            }
            if (!slideVM.File.CheckFileType("image"))
            {
                ModelState.AddModelError("File", "Only image files supported.");
                return View();
            }

            Slide slide = new Slide
            {
                Title = slideVM.Title,
                Description = slideVM.Description,
                Order = slideVM.Order,
                ButtonLink = slideVM.ButtonLink,
                ButtonText = slideVM.ButtonText,
                Image = await slideVM.File.CreateFileAsync(_env.WebRootPath, "assets", "img")
            };


            await _db.Slides.AddAsync(slide);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            Slide slide = await _db.Slides.FirstOrDefaultAsync(x => x.Id == id);
            return View(slide);
        }
        public async Task<ActionResult> Delete(int id)
        {

            Slide slide = await _db.Slides.FirstOrDefaultAsync(x => x.Id == id);
            if (slide is null) return NotFound();

            //slide.Image.Delete(_env.WebRootPath, "assets", "img");
            //_db.Slides.Remove(slide);

            slide.DeletedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Slide existed = await _db.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
            UpdateSlideVM slideVM = new UpdateSlideVM
            {
                Title = existed.Title,
                ButtonLink = existed.ButtonLink,
                ButtonText = existed.ButtonText,
                Description = existed.Description,
                Order = existed.Order,
                Image = existed.Image,

            };

            return View(slideVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSlideVM slideVM)
        {

            Slide existed = await _db.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(existed);
            }

            if (slideVM.File is not null)
            {
                bool result = _db.Slides.Any(s => s.Order < 0);
                if (result)
                {
                    ModelState.AddModelError("Order", "Order can't be smaller than 0.");
                    return View(existed);
                }
                if (await _db.Slides.AnyAsync(x => x.Order == slideVM.Order))
                {
                    slideVM.Order = (await _db.Slides.OrderByDescending(x => x.Order).FirstOrDefaultAsync()).Order + 1;
                }
                if (!slideVM.File.CheckFileType("image"))
                {
                    ModelState.AddModelError("Photo", "You need to choose image file.");
                    return View(existed);
                }
                if (!slideVM.File.CheckFileSize(2))
                {
                    ModelState.AddModelError("Photo", "You need to choose up to 2MB.");
                    return View(existed);
                }
                string newimage = await slideVM.File.CreateFileAsync(_env.WebRootPath, "assets", "img");
                existed.Image.Delete(_env.WebRootPath, "assets", "img");
                existed.Image = newimage;
            }

            existed.Title = slideVM.Title;
            existed.ButtonText = slideVM.ButtonText;
            existed.ButtonLink = slideVM.ButtonLink;
            existed.Description = slideVM.Description;
            existed.Order = slideVM.Order;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
