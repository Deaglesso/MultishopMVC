using Microsoft.AspNetCore.Mvc.Rendering;
using QisTetiliMultiShop.Models;
using System.ComponentModel.DataAnnotations;

namespace QisTetiliMultiShop.Areas.Admin.ViewModel
{
    public class UpdateProductVM
    {
        [Required(ErrorMessage = "You must include name")]
        [MaxLength(25, ErrorMessage = "The name must be up to 25 characters")]
        [MinLength(3, ErrorMessage = "The name must be at least 3 characters")]
        public string Name { get; set; }
        [Range(1, 2147483647, ErrorMessage = "The price must be at least 0.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "You must include description")]
        [MaxLength(320, ErrorMessage = "The description must be up to 320 characters")]
        [MinLength(5, ErrorMessage = "The description must be at least 5 characters")]
        public string Description { get; set; }

        

        [Required]
        public int? CategoryId { get; set; }
        public ICollection<int> ColorIds { get; set; }
        

        public ICollection<int>? ImageIds { get; set; }


        public IFormFile? MainPhoto { get; set; }
        public ICollection<IFormFile>? Photos { get; set; }

        public ICollection<ProductImage>? ProductImages { get; set; }


        public SelectList? Categories { get; set; }
        public SelectList? Colors { get; set; }
        
    }
}
