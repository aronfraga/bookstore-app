using Bookstore.Models;

namespace Bookstore.Repository.IRepository {
	public interface IOrderDetailRepository : IRepository<OrderDetail> {

		void Update(OrderDetail orderDetail);

	}
}