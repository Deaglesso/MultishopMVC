using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QisTetiliMultiShop.Areas.Admin.ViewModel;
using QisTetiliMultiShop.DAL;
using QisTetiliMultiShop.Models;
using QisTetiliMultiShop.ViewModels;

namespace QisTetiliMultiShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(int page, int? id, int? order)
        {
            IQueryable<Product> productsQuery = _context.Products
                .Include(y => y.ProductColors)
                .Include(pi => pi.ProductImages)
                .AsQueryable();

            if (id is not null)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == id);
            }

            switch (order)
            {
                case 1:
                    productsQuery = productsQuery.OrderBy(p => p.Name);
                    break;
                case 2:
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case 3:
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                case 4:
                    productsQuery = productsQuery.OrderByDescending(p => p.Id);
                    break;
            }

            double count = await productsQuery.CountAsync();
            int limit = 6;

            IQueryable<Product> products = productsQuery.Skip(page * limit).Take(limit);

            PaginationVM<Product> paginationVM = new PaginationVM<Product>
            {
                TotalPage = (int)Math.Ceiling(count / limit),
                CurrentPage = page + 1,
                Items = await products.ToListAsync()
            };

            ShopVM shopVM = new ShopVM
            {
                Products = paginationVM.Items,
                PaginationVM = paginationVM,
                Order = order
            };

            return View(shopVM);
        }
    }
}
