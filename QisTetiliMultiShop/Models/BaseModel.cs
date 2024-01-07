namespace QisTetiliMultiShop.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public BaseModel()
        {
            CreatedAt = DateTime.Now;
            CreatedBy = "said";
        }
    }
}
