using System.ComponentModel.DataAnnotations;

namespace QisTetiliMultiShop.Areas.Admin.ViewModel
{
    public class UpdateSettingVM
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Max length is 50")]
        [MinLength(1, ErrorMessage = "Min length is 1")]
        public string Key { get; set; } = null!;
        [Required]
        [MaxLength(100, ErrorMessage = "Max length is 100")]
        [MinLength(1, ErrorMessage = "Min length is 1")]
        public string Value { get; set; } = null!;
    }
}
