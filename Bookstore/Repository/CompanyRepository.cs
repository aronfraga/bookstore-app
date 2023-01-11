using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Repository;
using Bookstore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookstore.Repository {
    public class CompanyRepository : Repository<Company>, ICompanyRepository {

        private readonly Context _dbcontext;

        public CompanyRepository(Context dbcontext) : base(dbcontext) {
            _dbcontext = dbcontext;
        }

        public void Update(Company company) {
            _dbcontext.Companies.Update(company);
        }
    }
}
