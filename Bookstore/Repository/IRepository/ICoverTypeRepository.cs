using Bookstore.Models;

namespace Bookstore.Repository.IRepository {
    public interface ICoverTypeRepository : IRepository<CoverType> {
        
        void Update(CoverType coverType);

    }
}
