using Bookstore.Data;
using Bookstore.Repository.IRepository;

namespace Bookstore.Repository {
    public class UnitOfWork : IUnitOfWork {

        private readonly Context _dbcontext;

        public UnitOfWork(Context dbcontext) {
            _dbcontext = dbcontext;
            Category = new CategoryRepository(dbcontext);
        }

        public ICategoryRepository Category { get; private set; }

        public void Save() {
            _dbcontext.SaveChanges();
        }

    }
}
