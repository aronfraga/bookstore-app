using Bookstore.Data;
using Bookstore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookstore.Repository {

    public class Repository<T> : IRepository<T> where T : class {

        private readonly Context _dbcontext;
        internal DbSet<T> dbSet;

        public Repository(Context dbcontext) {
            _dbcontext = dbcontext;
            this.dbSet = _dbcontext.Set<T>();
        }

        public void Add(T entity) {
            dbSet.Add(entity);
            _dbcontext.SaveChanges();
        }

        public IEnumerable<T> GetAll() {
            IQueryable<T> query = dbSet;
            return query.ToList();
        }

        public T GetOne(Expression<Func<T, bool>> filter) {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public void Remove(T entity) {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity) {
            dbSet.RemoveRange(entity);
        }
    }

}
