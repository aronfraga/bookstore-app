using Bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data {
    public class Context : DbContext {

        public Context(DbContextOptions<Context> options) : base(options) {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }
        public DbSet<Product> Products { get; set; } //volver hacer la migracion con la SSSSS

    }
}
