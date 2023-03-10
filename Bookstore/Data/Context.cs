using Bookstore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data {
    public class Context : IdentityDbContext {

        public Context(DbContextOptions<Context> options) : base(options) {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }
        public DbSet<Product> Products { get; set; }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCard> ShoppingCards { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }

	}
}
