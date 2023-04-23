using Microsoft.EntityFrameworkCore;
using TastyCook.RecepiesAPI.Entities;
using TastyCook.RecepiesAPI.Models;

namespace TastyCook.RecepiesAPI
{
    public class RecipesContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<User> Users { get; set; }
        //public DbSet<RecipeUser> RecipesUsers { get; set; }

        public RecipesContext(DbContextOptions<RecipesContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
            //optionsBuilder.UseSqlServer(@"");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<RecipeUser>()
            //    .HasKey(ru => new { ru.RecipeId, ru.UserId });
            //modelBuilder.Entity<RecipeUser>()
            //    .HasOne(ru => ru.Recipe)
            //    .WithMany(r => r.RecipeUsers)
            //    .HasForeignKey(ru => ru.RecipeId);
            //modelBuilder.Entity<RecipeUser>()
            //    .HasOne(ru => ru.User)
            //    .WithMany(r => r.RecipeUsers)
            //    .HasForeignKey(ru => ru.UserId);
        }
    }
}
