using Bookstore.Models;

namespace Bookstore.Repository.IRepository {
    public interface ICompanyRepository : IRepository<Company> {
        
        void Update(Company company);

    }
}
