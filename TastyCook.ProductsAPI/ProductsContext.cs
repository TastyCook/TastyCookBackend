using Microsoft.EntityFrameworkCore;
using TastyCook.ProductsAPI.Entities;

namespace TastyCook.ProductsAPI
{
    public class ProductsContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductUser> ProductUsers { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeProduct> RecipeProducts { get; set; }

        public ProductsContext(DbContextOptions<ProductsContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ProductUser>()
        //        .HasKey(sc => new { sc.ProductId, sc.UserId });

        //    modelBuilder.Entity<ProductUser>()
        //        .HasOne<Product>(sc => sc.Product)
        //        .WithMany(s => s.ProductUsers)
        //        .HasForeignKey(sc => sc.ProductId);


        //    modelBuilder.Entity<ProductUser>()
        //        .HasOne<User>(sc => sc.User)
        //        .WithMany(s => s.ProductUsers)
        //        .HasForeignKey(sc => sc.UserId);
        //}
    }
}
