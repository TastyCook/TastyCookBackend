using Microsoft.EntityFrameworkCore;
using TastyCook.RecipesAPI;
using TastyCook.RecipesAPI.Entities;

namespace TastyCook.RecipesAPI.Services
{
    public class ProductService
    {
        private readonly RecipesContext _db;

        public ProductService(RecipesContext db)
        {
            _db = db;
        }

        //public IEnumerable<Product> GetAll(ProductsRequest request)
        //{
        //    var products = GetByPagination(productsQuery, request.Limit, request.Offset);
        //    return products;
        //}

        //public int GetAllCount(string searchValue, Localization localization)
        //{
        //    IQueryable<Product> productsQuery = _db.Products;

        //    if (localization != Localization.None)
        //    {
        //        productsQuery = productsQuery.Where(r => r.Localization == localization);
        //    }

        //    if (string.IsNullOrEmpty(searchValue))
        //    {
        //        return productsQuery.Count();
        //    }

        //    var productsNumber = productsQuery.Count(p => p.Name.Contains(searchValue));
        //    return productsNumber;
        //}

        //public IEnumerable<Product> GetUserProducts(ProductsRequest request, string email)
        //{
        //    var user = _db.Users.FirstOrDefault(u => u.Email == email);
        //    IQueryable<Product> productsQuery = _db.Products.Include(p => p.ProductUsers);
        //    if (user == null)
        //    {
        //        return Enumerable.Empty<Product>();
        //    }

        //    if (request.Localization != Localization.None)
        //    {
        //        productsQuery = productsQuery.Where(r => r.Localization == request.Localization);
        //    }

        //    if (!string.IsNullOrEmpty(request.SearchValue))
        //    {
        //        productsQuery = productsQuery.Where(p => p.Name.Contains(request.SearchValue));
        //    }

        //    productsQuery = productsQuery.Where(p => p.ProductUsers.Any(pu => pu.UserId == user.Id));
        //    var products = GetByPagination(productsQuery, request.Limit, request.Offset);

        //    return products;
        //}


        //public int GetUserProductsCount(string searchValue, string email, Localization localization)
        //{
        //    var user = _db.Users.FirstOrDefault(u => u.Email == email);
        //    var products = _db.Products.Include(p => p.ProductUsers)
        //        .Where(p => p.ProductUsers.Any(pu => pu.UserId == user.Id));

        //    if (localization != Localization.None)
        //    {
        //        products = products.Where(r => r.Localization == localization);
        //    }

        //    if (string.IsNullOrEmpty(searchValue))
        //    {
        //        return products.Count(p => p.Name.Contains(searchValue));
        //    }

        //    var productsNumber = products.Count(p => p.Name.Contains(searchValue));
        //    return productsNumber;
        //}

        public Product GetById(int id)
        {
            return _db.Products.FirstOrDefault(r => r.Id == id);
        }

        //public ProductUser GetProductsByUser(string userEmail)
        //{
        //    var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
        //    var products = _db.ProductUsers.Where(r => r.UserId == user.Id).ToList();
        //    return products;
        //}

        public ProductUser GetProductUserByIds(int productId, string userId)
        {
            return _db.ProductUsers.FirstOrDefault(r => r.ProductId == productId && r.UserId == userId);
        }

        public void AddNewProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                throw new ArgumentException("Name shouldn't be empty");
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                _db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Products] ON;");
                _db.Products.Add(product);
                _db.SaveChanges();
                _db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Products] OFF;");
                transaction.Commit();
            }
        }

        public void AddUserProduct(ProductUser productUser)
        {

            var product = GetById(productUser.ProductId);
            var user = _db.Users.Include(u => u.ProductUsers).FirstOrDefault(u => productUser.UserId == u.Id);
            if (product is null || user is null)
            {
                throw new Exception("There is no product or user with this id");
            }

            if (_db.ProductUsers.FirstOrDefault(pu => pu.UserId == user.Id && pu.ProductId == productUser.ProductId) is not null)
            {
                throw new Exception("The user already has this product");
            }

            _db.ProductUsers.Add(new ProductUser()
            {
                ProductId = productUser.ProductId,
                UserId = user.Id,
                Amount = productUser.Amount,
            });

            _db.SaveChanges();
        }

        public void UpdateUserProduct(ProductUser productUser)
        {
            var user = _db.Users.Include(u => u.ProductUsers).FirstOrDefault(u => u.Id == productUser.UserId);
            var userProduct = _db.ProductUsers.FirstOrDefault(pu => pu.UserId == user.Id && pu.ProductId == productUser.ProductId);
            if (userProduct is null)
            {
                throw new Exception("There is no product for this user");
            }

            userProduct.Amount = productUser.Amount;
            _db.SaveChanges();
        }

        public void Update(Product model)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == model.Id);
            product.Name = model.Name;
            product.Calories = model.Calories;
            product.Localization = model.Localization;
            _db.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var product = _db.Products.Find(id);
            if (product is null)
            {
                throw new Exception("There is no product with this id");
            }

            _db.Products.Remove(product);
            _db.SaveChanges();
        }

        public void DeleteUserProductsById(int productId, string userId)
        {
            var userProduct = _db.ProductUsers.FirstOrDefault(pu => pu.ProductId == productId && pu.UserId == userId);
            if (userProduct is null)
            {
                throw new Exception("There is no user product with this id");
            }

            _db.ProductUsers.Remove(userProduct);
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
