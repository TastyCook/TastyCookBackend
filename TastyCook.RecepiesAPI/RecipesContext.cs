using Microsoft.EntityFrameworkCore;
using TastyCook.RecepiesAPI.Models;

namespace TastyCook.RecepiesAPI
{
    public class RecipesContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }

        public RecipesContext(DbContextOptions<RecipesContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"");
        }
    }
}
