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
			Company = new CompanyRepository(dbcontext);
            ShoppingCard = new ShoppingCardRepository(dbcontext);
            ApplicationUser = new ApplicationUserRepository(dbcontext);
            OrderDetail = new OrderDetailRepository(dbcontext);
            OrderHeader = new OrderHeaderRepository(dbcontext);
		}

        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public IProductRepository Product { get; private set; }
		public ICompanyRepository Company { get; private set; }
        public IShoppingCardRepository ShoppingCard { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }

		public void Save() {
            _dbcontext.SaveChanges();
        }

    }
}
