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
			//_dbcontext.Products.Include(data => data.Category).Include(data => data.CoverType);
			this.dbSet = _dbcontext.Set<T>();
		}

		public void Add(T entity) {
			dbSet.Add(entity);
			_dbcontext.SaveChanges();
		}

		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null) {
			IQueryable<T> query = dbSet;

			if(filter != null) query = query.Where(filter);

			if (includeProperties != null) {
				foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
					query = query.Include(item);
				}
			}
			return query.ToList();
		}

		public T GetOne(Expression<Func<T, bool>> filter, string? includeProperties = null) {
			IQueryable<T> query = dbSet;
			if (includeProperties != null) {
				foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
					query = query.Include(item);
				}
			}
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