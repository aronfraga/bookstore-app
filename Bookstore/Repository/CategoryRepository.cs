using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Repository.IRepository;
using System.Linq.Expressions;

namespace Bookstore.Repository {
    public class CategoryRepository : Repository<Category>, ICategoryRepository { 

        private readonly Context _dbcontext;

        public CategoryRepository(Context dbcontext) : base(dbcontext) {
            _dbcontext = dbcontext;
        }

        public void Save() {
            _dbcontext.SaveChanges();
        }

        public void Update(Category category) {
            _dbcontext.Categories.Update(category);
        }

    }
}
