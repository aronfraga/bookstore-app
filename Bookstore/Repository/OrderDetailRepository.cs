using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Repository.IRepository;
using System.Linq.Expressions;

namespace Bookstore.Repository {
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository { 

        private readonly Context _dbcontext;

        public OrderDetailRepository(Context dbcontext) : base(dbcontext) {
            _dbcontext = dbcontext;
        }

		public void Update(OrderDetail orderDetail) {
			_dbcontext.OrderDetails.Update(orderDetail);
		}

	}
}
