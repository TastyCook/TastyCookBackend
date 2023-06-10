﻿namespace TastyCook.RecipesAPI.Models
{
    public class RecipeModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        //public IEnumerable<Products> Products { get; set; }
        public Localization Localization { get; set; }
        public int? Likes { get; set; }
        public bool? IsUserLiked { get; set; }
        public string? UserId { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public string ImageUrl { get; set; }
    }
}
