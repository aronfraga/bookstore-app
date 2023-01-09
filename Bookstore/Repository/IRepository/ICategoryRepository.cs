using Bookstore.Models;

namespace Bookstore.Repository.IRepository {
    public interface ICategoryRepository : IRepository<Category> {
        
        void Update(Category category);
        void Save();

    }
}
