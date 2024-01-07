using QisTetiliMultiShop.Models;
using System.ComponentModel.DataAnnotations;

namespace QisTetiliMultiShop.Areas.Admin.ViewModel
{
    public class UpdateCategoryVM
    {
        [Required]
        [MaxLength(25, ErrorMessage = "Max length is 25")]
        [MinLength(2, ErrorMessage = "Min length is 2")]
        public string Name { get; set; }

        public ICollection<Product>? Products { get; set; }
        public string? Image { get; set; }
        public IFormFile? File { get; set; }
    }
}
