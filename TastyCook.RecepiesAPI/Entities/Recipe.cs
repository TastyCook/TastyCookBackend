﻿using TastyCook.RecepiesAPI.Entities;

namespace TastyCook.RecepiesAPI.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public IEnumerable<Products> Products { get; set; }
        public int Likes { get; set; }
        //public byte[] Image { get; set; }
        //public IEnumerable<RecipeUser> RecipeUsers { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
