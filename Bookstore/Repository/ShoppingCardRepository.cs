using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Repository.IRepository;
using System.Linq.Expressions;

namespace Bookstore.Repository {
    public class ShoppingCardRepository : Repository<ShoppingCard>, IShoppingCardRepository { 

        private readonly Context _dbcontext;

        public ShoppingCardRepository(Context dbcontext) : base(dbcontext) {
            _dbcontext = dbcontext;
        }

		public int DecrementQty(ShoppingCard shoppingCard, int count) {
			if(shoppingCard.Count > 1) shoppingCard.Count -= count;
			return shoppingCard.Count;
		}

		public int IncrementQty(ShoppingCard shoppingCard, int count) {
			shoppingCard.Count += count;
			return shoppingCard.Count;
		}
	}
}
