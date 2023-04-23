using Microsoft.AspNetCore.Mvc;
using TastyCook.RecepiesAPI.Models;

namespace TastyCook.RecepiesAPI;

public class UserService
{
    private readonly RecipesContext _db;

    public UserService(RecipesContext db)
    {
        _db = db;
    }

    public IEnumerable<User> GetAll()
    {
        return _db.Users.ToList();
    }

    public User GetById(string id)
    {
        return _db.Users.FirstOrDefault(r => r.Id == id);
    }

    public void Add(User user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();
    }

    public void Update(User user)
    {
        _db.Users.Update(user);
        _db.SaveChanges();
    }

    public void DeleteById(string id)
    {
        var userToDelete = _db.Users.FirstOrDefault(r => r.Id == id);
        if (userToDelete != null)
        {
            _db.Users.Remove(userToDelete);
            _db.SaveChanges();
        }
    }
}