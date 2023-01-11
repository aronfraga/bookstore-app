using Bookstore.Models;

namespace Bookstore.Repository.IRepository {
    public interface IShoppingCardRepository : IRepository<ShoppingCard> {
        int IncrementQty (ShoppingCard shoppingCard, int count);
        int DecrementQty (ShoppingCard shoppingCard, int count);
    }
}
