using System.Linq.Expressions;

namespace Bookstore.Repository.IRepository {
    
    public interface IRepository<T> where T : class {

        IEnumerable<T> GetAll();
        T GetOne(Expression<Func<T, bool>> filter);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);

    }
}
