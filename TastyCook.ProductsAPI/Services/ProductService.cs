using Microsoft.EntityFrameworkCore;
using TastyCook.ProductsAPI.Entities;

namespace TastyCook.ProductsAPI.Services
{
    public class ProductService
    {
        private readonly ProductsContext _db;

        public ProductService(ProductsContext db)
        {
            _db = db;
        }

        public IEnumerable<Product> GetAll(int limit, int offset)
        {
            var products = _db.Products.Skip(offset).Take(limit).ToList();
            return products;
        }

        public IEnumerable<Product> GetUserProducts(string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return Enumerable.Empty<Product>();
            }

            var products = _db.Products.Include(p => p.ProductUsers)
                .Where(p => p.ProductUsers.Any(pu => pu.UserId == user.Id)).ToList();

            return products;
        }

        public Product GetById(int id)
        {
            return _db.Products.FirstOrDefault(r => r.Id == id);
        }

        public void AddNewProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                throw new ArgumentException("Name shouldn't be empty");
            }

            _db.Products.Add(product);
            _db.SaveChanges();
        }

        public void AddUserProduct(int productId, string amount, string userEmail)
        {
            var user = _db.Users.Include(u => u.ProductUsers).FirstOrDefault(u => u.Email == userEmail);
            _db.ProductUsers.Add(new ProductUser()
            {
                ProductId = productId,
                UserId = user.Id,
                Amount = amount
            });

            _db.SaveChanges();
        }

        public void Update(int productId, string amount, string userEmail)
        {
            var user = _db.Users.Include(u => u.ProductUsers).FirstOrDefault(u => u.Email == userEmail);
            var productUser = _db.ProductUsers.Find(productId);
            productUser.Amount = amount;

            _db.SaveChanges();
        }

        //public void DeleteById(int id, string userEmail)
        //{
        //    var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
        //    var ProductToDelete = _db.Products.FirstOrDefault(r => r.Id == id && r.UserId == user.Id);
        //    if (ProductToDelete != null)
        //    {
        //        _db.Products.Remove(ProductToDelete);
        //        _db.SaveChanges();
        //    }
        //}
    }
}
