namespace QisTetiliMultiShop.Areas.Admin.ViewModel
{
    public class CreateSlideVM
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ButtonText { get; set; } = null!;
        public string ButtonLink { get; set; } = null!;
        public int Order { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
