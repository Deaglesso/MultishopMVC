namespace QisTetiliMultiShop.Models
{
    public class Slide:BaseModel
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ButtonText { get; set; } = null!;
        public string ButtonLink { get; set; } = null!;
        public string Image { get; set; } = null!;
        public int Order { get; set; } 
    }
}
