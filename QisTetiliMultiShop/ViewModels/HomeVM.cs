using QisTetiliMultiShop.Models;

namespace QisTetiliMultiShop.ViewModels
{
    public class HomeVM
    {
        public ICollection<Slide> Slides { get; set; } = null!;
        public ICollection<Category>? Categories { get; set; }
        public ICollection<Product>? Products { get; set; }

    }
}
