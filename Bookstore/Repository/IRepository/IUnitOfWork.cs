using Bookstore.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Repository.IRepository {
    public interface IUnitOfWork {

        ICategoryRepository Category { get; }
        void Save();

    }
}
