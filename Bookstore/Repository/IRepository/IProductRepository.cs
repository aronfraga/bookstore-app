using Bookstore.Models;

namespace Bookstore.Repository.IRepository {
    public interface IProductRepository : IRepository<Product> {

        void Update(Product product);

    }
}
