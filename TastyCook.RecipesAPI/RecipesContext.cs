using Microsoft.EntityFrameworkCore;
using TastyCook.RecipesAPI.Entities;

namespace TastyCook.RecipesAPI
{
    public class RecipesContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }

        public RecipesContext(DbContextOptions<RecipesContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder
            //    .UseLazyLoadingProxies();
        }
    }
}
