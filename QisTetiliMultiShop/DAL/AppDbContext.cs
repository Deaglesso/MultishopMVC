using Microsoft.EntityFrameworkCore;
using QisTetiliMultiShop.Models;

namespace QisTetiliMultiShop.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }



        
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<Slide>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<Category>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<ProductColor>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<ProductImage>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<Product>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<Color>().HasQueryFilter(e => e.DeletedAt == null);




        }
    }
}
