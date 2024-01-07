namespace QisTetiliMultiShop.Models
{
    public class Product:BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public ICollection<ProductColor> ProductColors { get; set; }

    }
}
