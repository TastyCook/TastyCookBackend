using TastyCook.RecipesAPI.Entities;

namespace TastyCook.RecipesAPI.Services;

public class CategoriesService
{
    private readonly RecipesContext _db;

    public CategoriesService(RecipesContext db)
    {
        _db = db;
    }

    public IEnumerable<Category> GetAll()
    {
        return _db.Categories.ToList();
    }

    public Category? GetById(int id)
    {
        return _db.Categories.FirstOrDefault(r => r.Id == id);
    }

    public void Add(Category category)
    {
        var categoryDb = _db.Categories.FirstOrDefault(c => c.Name == category.Name);
        if (categoryDb != null) throw new Exception("There is already category with the same name");

        _db.Categories.Add(category);
        _db.SaveChanges();
    }

    public void Update(Category category)
    {
        var categoryDb = _db.Categories.Find(category.Id);
        categoryDb.Name = category.Name;
        _db.SaveChanges();
    }

    public void DeleteById(int id)
    {
        var categoryToDelete = _db.Categories.FirstOrDefault(r => r.Id == id);
        if (categoryToDelete != null)
        {
            _db.Categories.Remove(categoryToDelete);
            _db.SaveChanges();
        }
    }
}