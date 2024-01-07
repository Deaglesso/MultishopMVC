using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QisTetiliMultiShop.Areas.Admin.ViewModel;
using QisTetiliMultiShop.DAL;
using QisTetiliMultiShop.Models;
using QisTetiliMultiShop.Utilities.Extensions;

namespace QisTetiliMultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        
        public ProductController(IWebHostEnvironment env,AppDbContext db)
        {
            _env = env;
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int limit = 4;
            double count = await _db.Products.CountAsync();
            if (count != 0)
            {
                if (page > (int)Math.Ceiling(count / limit) || page <= 0)
                {
                    return BadRequest();
                }
            }


            List<Product> products = await _db.Products.Skip((page - 1) * limit).Take(limit)
                .Include(x => x.ProductImages)
                .Include(x => x.Category).ToListAsync();
            PaginationVM<Product> paginationVM = new PaginationVM<Product>
            {
                Items = products,
                TotalPage = (int)Math.Ceiling(count / limit),
                CurrentPage = page,
                Limit = limit
            };
            return View(paginationVM);
        }
        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM
            {
                CategoryList = await _db.Categories.ToListAsync(),
                ColorList = await _db.Colors.ToListAsync(),

            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.CategoryList = await _db.Categories.ToListAsync();

                productVM.ColorList = await _db.Colors.ToListAsync();
                
                return View(productVM);
            }


            if (!(await _db.Categories.AnyAsync(x => x.Id == productVM.CategoryId)))
            {
                productVM.CategoryList = await _db.Categories.ToListAsync();

                productVM.ColorList = await _db.Colors.ToListAsync();

                ModelState.AddModelError("CategoryId", "This category does not exist.");

                return View(productVM);
            }
            
            if (!productVM.MainImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("MainImage", "Only images allowed.");
                productVM.CategoryList = await _db.Categories.ToListAsync();

                productVM.ColorList = await _db.Colors.ToListAsync();
                return View(productVM);
            }
            if (!productVM.MainImage.CheckFileSize(1))
            {
                ModelState.AddModelError("MainImage", "Only images below 1MB allowed.");
                productVM.CategoryList = await _db.Categories.ToListAsync();

                productVM.ColorList = await _db.Colors.ToListAsync();
                return View(productVM);
            }

            if (!productVM.HoverImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("HoverImage", "Only images allowed.");
                productVM.CategoryList = await _db.Categories.ToListAsync();

                productVM.ColorList = await _db.Colors.ToListAsync();
                return View(productVM);
            }
            if (!productVM.HoverImage.CheckFileSize(1))
            {
                ModelState.AddModelError("HoverImage", "Only images below 1MB allowed.");
                productVM.CategoryList = await _db.Categories.ToListAsync();

                productVM.ColorList = await _db.Colors.ToListAsync();
                return View(productVM);
            }

            ProductImage mainImage = new ProductImage
            {
                IsPrimary = true,
                Url = await productVM.MainImage.CreateFileAsync(_env.WebRootPath, "assets", "img"),
            };
            ProductImage hoverImage = new ProductImage
            {
                IsPrimary = false,
                Url = await productVM.HoverImage.CreateFileAsync(_env.WebRootPath, "assets", "img"),
            };

            Product product = new Product
            {
                Name = productVM.Name,
                Description = productVM.Description,
                Price = productVM.Price,
                SKU = productVM.SKU,
                CategoryId = productVM.CategoryId,
                Category = productVM.Category,
                ProductColors = new List<ProductColor>(),
                ProductImages = new List<ProductImage> { mainImage, hoverImage }

            };
            TempData["ImageMessage"] = "";
            foreach (IFormFile image in productVM.AddImages)
            {
                if (!image.CheckFileType("image/"))
                {
                    TempData["ImageMessage"] += $" <p class=\"btn btn-inverse-danger btn-fw myParagraph\" style=\"display: inline-flex; align-items: center;\" >{image.FileName} file's type is not image.<span style=\"margin-top: -1px; margin-left: 3px\" class=\"close-button text-white\" onclick=\"closeParagraph()\"><i class=\"mdi mdi-close-circle-outline\"></i></span></p>\r\n\r\n                    <script> function closeParagraph() {{ var paragraphs = document.getElementsByClassName(\"myParagraph\");  for (var i = 0; i < paragraphs.length; i++) {{ paragraphs[i].style.display = \"none\"; }} }} setTimeout(closeParagraph, 10000); </script>";
                    continue;
                }
                if (!image.CheckFileSize(1))
                {
                    TempData["ImageMessage"] += $" <p class=\"btn btn-inverse-danger btn-fw myParagraph\" style=\"display: inline-flex; align-items: center;\" >{image.FileName} file's size is larger than 1MB.<span style=\"margin-top: -1px; margin-left: 3px\" class=\"close-button text-white\" onclick=\"closeParagraph()\"><i class=\"mdi mdi-close-circle-outline\"></i></span></p>\r\n\r\n                    <script> function closeParagraph() {{ var paragraphs = document.getElementsByClassName(\"myParagraph\");  for (var i = 0; i < paragraphs.length; i++) {{ paragraphs[i].style.display = \"none\"; }} }} setTimeout(closeParagraph, 10000); </script>";

                    continue;
                }

                product.ProductImages.Add(new ProductImage { IsPrimary = null, Url = await image.CreateFileAsync(_env.WebRootPath, "assets", "img") });
            }
            
            if (productVM.ColorIds is not null)
            {
                foreach (var item in productVM.ColorIds)
                {
                    ProductColor productEdition = new ProductColor
                    {
                        ColorId = item
                    };
                    product.ProductColors.Add(productEdition);
                }

            }
            



            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _db.Products.Include(y => y.ProductColors).Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = existed.Name,
                Price = existed.Price,
                Description = existed.Description,
                

                CategoryId = existed.CategoryId,
                ColorIds = existed.ProductColors.Select(pt => pt.ColorId).ToList(),
                ProductImages = existed.ProductImages
            };
            GetSelectList(ref productVM);
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            Product existed = await _db.Products
                .Include(y => y.ProductColors)
                .Include(pi => pi.ProductImages)
                .FirstOrDefaultAsync(e => e.Id == id);

            productVM.ProductImages = existed.ProductImages;
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                GetSelectList(ref productVM);

                return View(productVM);
            }


            bool result = await _db.Products.AnyAsync(c => c.Name == productVM.Name && c.Id != id);
            if (result)
            {
                GetSelectList(ref productVM);
                ModelState.AddModelError("Name", "Product already exists");
                return View(productVM);
            }

            bool result1 = await _db.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result1)
            {
                GetSelectList(ref productVM);
                ModelState.AddModelError("CategoryId", "Category not found, choose another one.");
                return View(productVM);
            }

            ///////

            // Update ProductColors
            var colorsToRemove = existed.ProductColors.Where(pc => !productVM.ColorIds.Contains(pc.ColorId)).ToList();
            foreach (var colorToRemove in colorsToRemove)
            {
                existed.ProductColors.Remove(colorToRemove);
            }





            List<int> colorcreatable = productVM.ColorIds
                                            .Except(existed.ProductColors.Select(pc => pc.ColorId)).ToList();


            foreach (int colorid in colorcreatable)
            {
                bool colorresult = await _db.Colors.AnyAsync(c => c.Id == colorid);

                if (!colorresult)
                {
                    GetSelectList(ref productVM);
                    ModelState.AddModelError("ColorIds", "Color not found.");
                    return View();
                }
                existed.ProductColors.Add(new ProductColor
                {
                    ColorId = colorid
                });
            }

            ///////
            



            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.CheckFileType("image/"))
                {
                    GetSelectList(ref productVM);
                    ModelState.AddModelError("MainPhoto", "File type is not valid");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.CheckFileSize(1))
                {
                    GetSelectList(ref productVM);
                    ModelState.AddModelError("MainPhoto", "Size is not valid, you need to choose up to 2MB");
                    return View(productVM);
                }
            }

            if (productVM.MainPhoto is not null)
            {
                string filename = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "img");
                ProductImage mainimg = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                mainimg.Url.Delete(_env.WebRootPath, "img");
                _db.ProductImages.Remove(mainimg);

                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    Url = filename
                });
            }


            if (productVM.ImageIds is null)
            {
                productVM.ImageIds = new List<int>();
            }

            List<ProductImage> removable = existed.ProductImages.Where(pi => !productVM.ImageIds.Contains(pi.Id) && pi.IsPrimary == null).ToList();

            foreach (var pig in removable)
            {
                pig.Url.Delete(_env.WebRootPath, "img");
                existed.ProductImages.Remove(pig);
            }

            TempData["Message"] = "";

            if (productVM.Photos is not null)
            {
                foreach (IFormFile photo in productVM.Photos)
                {
                    if (!photo.CheckFileType("image/"))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName}'s  type is not suitable<p/>";
                        continue;
                    }
                    if (!photo.CheckFileSize(1))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName}'s  size is not suitable<p/>";
                        continue;
                    }

                    existed.ProductImages.Add(new ProductImage
                    {
                        IsPrimary = null,
                        Url = await photo.CreateFileAsync(_env.WebRootPath, "img")
                    });
                }
            }


            existed.Name = productVM.Name;
            existed.Price = productVM.Price;
            existed.Description = productVM.Description;
            

            existed.CategoryId = (int)productVM.CategoryId;


            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _db.Products
                .Include(x => x.ProductImages)
                .Include(x => x.ProductColors)
                .ThenInclude(c => c.Color)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null) return NotFound();

            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var product = await _db.Products
                .Include(x => x.ProductImages)
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductColors)
                .ThenInclude(c => c.Color)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null) return NotFound();

            foreach (var item in product.ProductImages)
            {
                item.Url.Delete(_env.WebRootPath, "img");
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        


        private void GetSelectList(ref UpdateProductVM vm)
        {
            vm.Categories = new(_db.Categories, "Id", "Name");
            vm.Colors = new(_db.Colors, "Id", "Name");
        }
    }
}
