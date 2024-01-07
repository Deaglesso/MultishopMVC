namespace QisTetiliMultiShop.Models
{
    public class ProductColor:BaseModel
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ColorId { get; set; }
        public Color Color { get; set; }
    }
}
