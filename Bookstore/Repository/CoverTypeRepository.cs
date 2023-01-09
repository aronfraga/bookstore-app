using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Repository;
using Bookstore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookstore.Repository {
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository {

        private readonly Context _dbcontext;

        public CoverTypeRepository(Context dbcontext) : base(dbcontext) {
            _dbcontext = dbcontext;
        }

        public void Update(CoverType coverType) {
            _dbcontext.CoverTypes.Update(coverType);
        }
    }
}
