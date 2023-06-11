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
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeProduct>()
                .HasKey(sc => new { sc.ProductId, sc.RecipeId });

            modelBuilder.Entity<RecipeProduct>()
                .HasOne<Product>(sc => sc.Product)
                .WithMany(s => s.RecipeProducts)
                .HasForeignKey(sc => sc.ProductId);

            modelBuilder.Entity<RecipeProduct>()
                .HasOne<Recipe>(sc => sc.Recipe)
                .WithMany(s => s.RecipeProducts)
                .HasForeignKey(sc => sc.RecipeId);
        }
    }
}
