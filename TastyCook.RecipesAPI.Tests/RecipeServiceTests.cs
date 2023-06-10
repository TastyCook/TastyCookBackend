using Microsoft.EntityFrameworkCore;
using Moq;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;
using TastyCook.RecipesAPI.Services;

namespace TastyCook.RecipesAPI.Tests
{
    public class Tests
    {
        private List<Recipe> _recipes;
        private IQueryable<Recipe> _recipesQueryable;
        private Mock<DbSet<Recipe>> _recipesDbSet;

        [SetUp]
        public void Setup()
        {
            _recipes = new List<Recipe>();
            _recipes.Add(new Recipe()
            {
                Id = 1, Description = "Fresh salad", Name = "Greek Salad", Localization = Localization.English, Categories = Enumerable.Empty<Category>()
            });
            _recipes.Add(new Recipe()
            {
                Id = 2, Description = "Суп", Name = "Томатний суп", Localization = Localization.Ukrainian, Categories = Enumerable.Empty<Category>()
            });
            _recipesQueryable = _recipes.AsQueryable();

            _recipesDbSet = new Mock<DbSet<Recipe>>();
            _recipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.Provider).Returns(_recipesQueryable.Provider);
            _recipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.Expression).Returns(_recipesQueryable.Expression);
            _recipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.ElementType).Returns(_recipesQueryable.ElementType);
            _recipesDbSet.As<IQueryable<Recipe>>().Setup(m => m.GetEnumerator()).Returns(() => _recipesQueryable.GetEnumerator());
        }

        [Test]
        public void GetAllReturnsCorrectValues()
        {
            var options = new DbContextOptionsBuilder<RecipesContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var recipeContextMock = new Mock<RecipesContext>(options);
            recipeContextMock.Setup(m => m.Recipes).Returns(_recipesDbSet.Object);
            var fileServiceMock = new Mock<FileService>();
            var emptyRequest = new GetAllRecipesRequest();

            var recipeService = new RecipeService(recipeContextMock.Object, fileServiceMock.Object);
            var recipesGetAllResult = recipeService.GetAll(emptyRequest).ToList();

            Assert.AreEqual(_recipes.Count, recipesGetAllResult.Count);
            Assert.AreEqual(_recipes[0].Name, recipesGetAllResult[0].Name);
            Assert.AreEqual(_recipes[1].Name, recipesGetAllResult[1].Name);
        }
    }
}