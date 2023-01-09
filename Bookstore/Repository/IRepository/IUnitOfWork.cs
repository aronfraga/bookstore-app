using Bookstore.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Repository.IRepository {
    public interface IUnitOfWork {

        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get; }
        IProductRepository Product { get; }

        void Save();

    }
}
