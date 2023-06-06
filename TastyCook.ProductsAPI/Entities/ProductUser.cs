using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TastyCook.ProductsAPI.Entities
{
    [PrimaryKey(nameof(ProductId), nameof(UserId))]
    public class ProductUser
    {
        [MaxLength(225)]
        public string UserId { get; set; }
        public User User { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string Amount { get; set; }
        public string Type { get; set; }
    }
}
