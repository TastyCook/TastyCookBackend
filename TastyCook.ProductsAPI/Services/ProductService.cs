using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                productsQuery = _db.Products.Where(p => p.Name.Contains(request.SearchValue));
            }

            var products = GetByPagination(productsQuery, request.Limit, request.Offset);
            return products;
        }

        public int GetAllCount(string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return _db.Products.Count();
            }

            var productsNumber = _db.Products.Count(p => p.Name.Contains(searchValue));
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

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(request.SearchValue));
            }

            productsQuery = productsQuery.Where(p => p.ProductUsers.Any(pu => pu.UserId == user.Id));
            var products = GetByPagination(productsQuery, request.Limit, request.Offset);

            return products;
        }


        public int GetUserProductsCount(string searchValue, string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            var products = _db.Products.Include(p => p.ProductUsers)
                .Where(p => p.ProductUsers.Any(pu => pu.UserId == user.Id));

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

        public void AddNewProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                throw new ArgumentException("Name shouldn't be empty");
            }

            _db.Products.Add(product);
            _db.SaveChanges();
        }

        public void AddUserProduct(int productId, string amount, string type, string userEmail)
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

            _db.ProductUsers.Add(new ProductUser()
            {
                ProductId = productId,
                UserId = user.Id,
                Amount = amount,
                Type = type
            });

            _db.SaveChanges();
        }

        public void UpdateUserProduct(int productId, string amount, string type, string userEmail)
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
        }

        public void Update(ProductModel model)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == model.Id);
            product.Name = model.Name;
            product.Calories = model.Calories;
            _db.SaveChanges();
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
