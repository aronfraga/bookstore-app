using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Repository.IRepository;
using System.Linq.Expressions;

namespace Bookstore.Repository {
    public class ProductRepository : Repository<Product>, IProductRepository {

        private readonly Context _dbcontext;

        public ProductRepository(Context dbcontext) : base(dbcontext) {
            _dbcontext = dbcontext;
        }

        public void Update(Product product) {
            var dbResponse = _dbcontext.Products.FirstOrDefault(data => data.Id == product.Id);
            if(dbResponse != null) {
                dbResponse.Title = product.Title;
                dbResponse.ISBN = product.ISBN;
                dbResponse.PromotionPrice = product.PromotionPrice;
                dbResponse.Description = product.Description;
                dbResponse.Price = product.Price;
                dbResponse.Author = product.Author;
                dbResponse.CategoryId = product.CategoryId;
                dbResponse.CoverTypeId = product.CoverTypeId;
                if(product.ImageUrl != null) dbResponse.ImageUrl = product.ImageUrl;
            }
        }
    }
}
