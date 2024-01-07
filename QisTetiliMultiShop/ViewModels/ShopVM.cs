using QisTetiliMultiShop.Areas.Admin.ViewModel;
using QisTetiliMultiShop.Models;

namespace QisTetiliMultiShop.ViewModels
{
    public class ShopVM
    {
        public ICollection<Product> Products { get; set; }
        public PaginationVM<Product> PaginationVM { get; set; }
        public int? Order { get; set; }
    }
}
