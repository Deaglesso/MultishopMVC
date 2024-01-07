using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QisTetiliMultiShop.Areas.Admin.ViewModel;
using QisTetiliMultiShop.DAL;
using QisTetiliMultiShop.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QisTetiliMultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController : Controller
    {
        private readonly AppDbContext _db;

        public SettingController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int limit = 4;
            double count = await _db.Settings.CountAsync();
            if (count != 0)
            {
                if (page > (int)Math.Ceiling(count / limit) || page <= 0)
                {
                    return BadRequest();
                }
            }
            List<Setting> SettingList = await _db.Settings.Skip((page - 1) * limit).Take(limit).ToListAsync();

            PaginationVM<Setting> paginationVM = new PaginationVM<Setting>
            {
                Items = SettingList,
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
        public async Task<IActionResult> Create(CreateSettingVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            bool result = await _db.Settings.IgnoreQueryFilters().AnyAsync(x => x.Key == vm.Key);
            if (result)
            {
                ModelState.AddModelError("Key", "Already exists.");
                return View();
            }
            Setting setting = new Setting { Key = vm.Key, Value = vm.Value };
            await _db.Settings.AddAsync(setting);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Setting setting = await _db.Settings.FirstOrDefaultAsync(x => x.Id == id);
            if (setting == null)
            {
                return NotFound();
            }
            UpdateSettingVM updateVM = new UpdateSettingVM
            {
                Key = setting.Key,

                Value = setting.Value
            };

            return View(updateVM);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSettingVM updateVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Setting oldSetting = await _db.Settings.FirstOrDefaultAsync(x => x.Id == id);
            if (oldSetting == null)
            {
                return NotFound();
            }
            bool result = await _db.Settings.IgnoreQueryFilters().AnyAsync(x => x.Key == updateVM.Key && x.Id != id);
            if (result)
            {
                ModelState.AddModelError("Key", "Already exists.");
                return View();
            }
            oldSetting.Key = updateVM.Key;
            oldSetting.Value = updateVM.Value;
            oldSetting.ModifiedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");


        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Setting setting = await _db.Settings.FirstOrDefaultAsync(x => x.Id == id);
            if (setting == null)
            {
                return NotFound();
            }
            
            setting.DeletedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
