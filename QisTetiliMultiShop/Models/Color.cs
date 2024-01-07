using System.ComponentModel.DataAnnotations;

namespace QisTetiliMultiShop.Models
{
    public class Color:BaseModel
    {
        [Required]
        [MaxLength(25, ErrorMessage = "Max length is 25")]
        [MinLength(2, ErrorMessage = "Min length is 2")]
        public string Name { get; set; }
        public ICollection<ProductColor>? ProductColors { get; set; }
    }
}
