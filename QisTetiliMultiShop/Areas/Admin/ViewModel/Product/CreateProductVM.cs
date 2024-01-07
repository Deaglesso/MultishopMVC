using QisTetiliMultiShop.Models;
using System.ComponentModel.DataAnnotations;

namespace QisTetiliMultiShop.Areas.Admin.ViewModel
{
    public class CreateProductVM
    {
        [Required]
        [MaxLength(25, ErrorMessage = "Max length is 25")]
        [MinLength(2, ErrorMessage = "Min length is 2")]
        public string Name { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Min length is 2")]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        public ICollection<int>? ColorIds { get; set; }
        public IFormFile MainImage { get; set; }
        public IFormFile HoverImage { get; set; }
        public List<IFormFile>? AddImages { get; set; }


        public Category? Category { get; set; }
        public ICollection<Category>? CategoryList { get; set; }
        public ICollection<Color>? ColorList { get; set; }

    }
}
