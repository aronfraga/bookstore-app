using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Repository.IRepository;
using System.Linq.Expressions;

namespace Bookstore.Repository {
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository { 

        private readonly Context _dbcontext;

        public OrderHeaderRepository(Context dbcontext) : base(dbcontext) {
            _dbcontext = dbcontext;
        }

		public void Update(OrderHeader orderHeader) {
			_dbcontext.OrderHeaders.Update(orderHeader);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null) {
			var orderFromDb = _dbcontext.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if (orderFromDb != null) {
				orderFromDb.OrderStatus = orderStatus;
				if (paymentStatus != null) {
					orderFromDb.PaymentStatus = paymentStatus;
				}
			}
		}

		public void UpdateStripePaymentID(int id, string sessionId, string paymentItentId) {
			var orderFromDb = _dbcontext.OrderHeaders.FirstOrDefault(u => u.Id == id);
			orderFromDb.PaymentDate = DateTime.Now;
			orderFromDb.SessionId = sessionId;
			orderFromDb.PaymentIntentId = paymentItentId;
		}

	}
}
