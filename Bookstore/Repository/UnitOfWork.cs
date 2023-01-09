using Bookstore.Data;
using Bookstore.Repository.IRepository;

namespace Bookstore.Repository {
    public class UnitOfWork : IUnitOfWork {

        private readonly Context _dbcontext;

        public UnitOfWork(Context dbcontext) {
            _dbcontext = dbcontext;
            Category = new CategoryRepository(dbcontext);
            CoverType = new CoverTypeRepository(dbcontext);
            Product = new ProductRepository(dbcontext);
        }

        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public IProductRepository Product { get; private set; }

        public void Save() {
            _dbcontext.SaveChanges();
        }

    }
}
