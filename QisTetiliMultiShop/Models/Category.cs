using System.ComponentModel.DataAnnotations;

namespace QisTetiliMultiShop.Models
{
    public class Category : BaseModel
    {
        [Required]
        [MaxLength(25, ErrorMessage = "Max length is 25")]
        [MinLength(2, ErrorMessage = "Min length is 2")]
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;
        public ICollection<Product>? Products { get; set; }

    }
}
