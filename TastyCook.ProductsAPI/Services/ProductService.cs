using Microsoft.EntityFrameworkCore;
using TastyCook.ProductsAPI.Entities;
using TastyCook.ProductsAPI.Models;

namespace TastyCook.ProductsAPI.Services
{
    public class ProductService
    {
        private readonly ProductsContext _db;

        public ProductService(ProductsContext db)
        {
            _db = db;
        }

        public IEnumerable<Product> GetAll(ProductsRequest request)
        {
            IQueryable<Product> productsQuery = _db.Products;

            if (request.Localization != Localization.None)
            {
                productsQuery = productsQuery.Where(r => r.Localization == request.Localization);
            }

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(request.SearchValue));
            }

            var products = GetByPagination(productsQuery, request.Limit, request.Offset);
            return products;
        }

        public int GetAllCount(string searchValue, Localization localization)
        {
            IQueryable<Product> productsQuery = _db.Products;

            if (localization != Localization.None)
            {
                productsQuery = productsQuery.Where(r => r.Localization == localization);
            }

            if (string.IsNullOrEmpty(searchValue))
            {
                return productsQuery.Count();
            }

            var productsNumber = productsQuery.Count(p => p.Name.Contains(searchValue));
            return productsNumber;
        }

        public IEnumerable<Product> GetUserProducts(ProductsRequest request, string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            IQueryable<Product> productsQuery = _db.Products.Include(p => p.ProductUsers);
            if (user == null)
            {
                return Enumerable.Empty<Product>();
            }

            if (request.Localization != Localization.None)
            {
                productsQuery = productsQuery.Where(r => r.Localization == request.Localization);
            }

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(request.SearchValue));
            }

            productsQuery = productsQuery.Where(p => p.ProductUsers.Any(pu => pu.UserId == user.Id));
            var products = GetByPagination(productsQuery, request.Limit, request.Offset);

            return products;
        }


        public int GetUserProductsCount(string searchValue, string email, Localization localization)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            var products = _db.Products.Include(p => p.ProductUsers)
                .Where(p => p.ProductUsers.Any(pu => pu.UserId == user.Id));

            if (localization != Localization.None)
            {
                products = products.Where(r => r.Localization == localization);
            }

            if (string.IsNullOrEmpty(searchValue))
            {
                return products.Count(p => p.Name.Contains(searchValue));
            }

            var productsNumber = products.Count(p => p.Name.Contains(searchValue));
            return productsNumber;
        }

        public Product GetById(int id)
        {
            return _db.Products.FirstOrDefault(r => r.Id == id);
        }

        public Product AddNewProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                throw new ArgumentException("Name shouldn't be empty");
            }

            _db.Products.Add(product);
            _db.SaveChanges();

            return product;
        }

        public ProductUser AddUserProduct(int productId, string amount, string type, string userEmail)
        {

            var product = GetById(productId);
            var user = _db.Users.Include(u => u.ProductUsers).FirstOrDefault(u => u.Email == userEmail);
            if (product is null || user is null)
            {
                throw new Exception("There is no product or user with this id");
            }

            if (_db.ProductUsers.FirstOrDefault(pu => pu.UserId == user.Id && pu.ProductId == productId) is not null)
            {
                throw new Exception("The user already has this product");
            }

            var productUser = new ProductUser()
            {
                ProductId = productId,
                UserId = user.Id,
                Amount = amount,
                Type = type
            };
            
            _db.ProductUsers.Add(productUser);
            _db.SaveChanges();

            return productUser;
        }

        public ProductUser UpdateUserProduct(int productId, string amount, string type, string userEmail)
        {
            var user = _db.Users.Include(u => u.ProductUsers).FirstOrDefault(u => u.Email == userEmail);
            var userProduct = _db.ProductUsers.FirstOrDefault(pu => pu.UserId == user.Id && pu.ProductId == productId);
            if (userProduct is null)
            {
                throw new Exception("There is no product for this user");
            }

            userProduct.Amount = amount;
            userProduct.Type = type;
            _db.SaveChanges();

            return userProduct;
        }

        public Product Update(ProductModel model)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == model.Id);
            product.Name = model.Name;
            product.Calories = model.Calories;
            product.Localization = model.Localization;
            _db.SaveChanges();

            return product;
        }


        private IEnumerable<Product> GetByPagination(IQueryable<Product> products, int? limit, int? offset)
        {
            if (limit.HasValue && offset.HasValue)
            {
                products = products.Skip(offset.Value).Take(limit.Value);
            }
            else if (limit.HasValue && !offset.HasValue)
            {
                products = products.Take(limit.Value);
            }
            else if (!limit.HasValue && offset.HasValue)
            {
                products = products.Skip(offset.Value);
            }

            return products.ToList();
        }

        public void DeleteById(int id)
        {
            var productToDelete = _db.Products.FirstOrDefault(r => r.Id == id);
            if (productToDelete != null)
            {
                _db.Products.Remove(productToDelete);
                _db.SaveChanges();
            }
        }
    }
}
