using QisTetiliMultiShop.Models;

namespace QisTetiliMultiShop.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
    }
}
