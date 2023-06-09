﻿using System.ComponentModel.DataAnnotations;

namespace TastyCook.RecipesAPI.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Likes { get; set; }
        public string? ImageUrl { get; set; }
        public Localization Localization { get; set; }

        [MaxLength(225)]
        public string UserId { get; set; }
        public User User { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<RecipeUser> RecipeUsers { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<RecipeProduct> RecipeProducts { get; set; }

        public Recipe() { }

        public Recipe(Recipe recipe)
        {
            Id = recipe.Id;
            Name = recipe.Name;
            Description = recipe.Description;
            Likes = recipe.Likes;
            Localization = recipe.Localization;
            UserId = recipe.UserId;
            User = recipe.User;
            RecipeUsers = recipe.RecipeUsers;
            Comments = recipe.Comments;
            ImageUrl = recipe.ImageUrl;
            Categories = recipe.Categories;
            RecipeProducts = recipe.RecipeProducts;
        }
    }
}
