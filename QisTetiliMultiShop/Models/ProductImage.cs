namespace QisTetiliMultiShop.Models
{
    public class ProductImage:BaseModel
    {
        public string Url { get; set; }
        public bool? IsPrimary { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
