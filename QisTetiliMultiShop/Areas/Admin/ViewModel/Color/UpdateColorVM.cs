using QisTetiliMultiShop.Models;
using System.ComponentModel.DataAnnotations;

namespace QisTetiliMultiShop.Areas.Admin.ViewModel
{
    public class UpdateColorVM
    {
        [Required]
        [MaxLength(25, ErrorMessage = "Max length is 25")]
        [MinLength(2, ErrorMessage = "Min length is 2")]
        public string Name { get; set; }
        public ICollection<ProductColor>? ProductColors { get; set; }
    }
}
