using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Repository.IRepository;
using System.Linq.Expressions;

namespace Bookstore.Repository {
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository { 

        private readonly Context _dbcontext;

        public ApplicationUserRepository(Context dbcontext) : base(dbcontext) {
            _dbcontext = dbcontext;
        }

    }
}
