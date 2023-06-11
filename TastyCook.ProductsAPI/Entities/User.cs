using System.ComponentModel.DataAnnotations;

namespace TastyCook.ProductsAPI.Entities;

public class User
{
    [MaxLength(225)]
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public IEnumerable<ProductUser> ProductUsers { get; set; }
}