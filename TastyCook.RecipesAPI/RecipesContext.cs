﻿using Microsoft.EntityFrameworkCore;
using TastyCook.RecipesAPI.Entities;

namespace TastyCook.RecipesAPI
{
    public class RecipesContext : DbContext
    {
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<RecipeUser> RecipeUsers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<RecipeProduct> RecipeProducts { get; set; }
        public virtual DbSet<ProductUser> ProductUsers { get; set; }

        public RecipesContext(DbContextOptions<RecipesContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder
            //    .UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeUser>().HasKey(sc => new { sc.UserId, sc.RecipeId });

            modelBuilder.Entity<RecipeUser>()
                .HasOne<Recipe>(sc => sc.Recipe)
                .WithMany(s => s.RecipeUsers)
                .HasForeignKey(sc => sc.RecipeId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<RecipeUser>()
                .HasOne<User>(sc => sc.User)
                .WithMany(s => s.RecipeUsers)
                .HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);


            modelBuilder.Entity<ProductUser>().HasKey(sc => new { sc.UserId, sc.ProductId });

            modelBuilder.Entity<ProductUser>()
                .HasOne<Product>(sc => sc.Product)
                .WithMany(s => s.ProductUsers)
                .HasForeignKey(sc => sc.ProductId);

            modelBuilder.Entity<ProductUser>()
                .HasOne<User>(sc => sc.User)
                .WithMany(s => s.ProductUsers)
                .HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);


            modelBuilder.Entity<RecipeProduct>().HasKey(sc => new { sc.RecipeId, sc.ProductId });

            modelBuilder.Entity<RecipeProduct>()
                .HasOne<Recipe>(sc => sc.Recipe)
                .WithMany(s => s.RecipeProducts)
                .HasForeignKey(sc => sc.RecipeId);

            modelBuilder.Entity<RecipeProduct>()
                .HasOne<Product>(sc => sc.Product)
                .WithMany(s => s.RecipeProducts)
                .HasForeignKey(sc => sc.ProductId)
                .OnDelete(DeleteBehavior.ClientCascade);


            modelBuilder.Entity<User>()
                .HasMany<Comment>(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.ClientNoAction);
        }
    }
}
