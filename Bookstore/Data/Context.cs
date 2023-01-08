using Bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data {
    public class Context : DbContext {

        public Context(DbContextOptions<Context> options) : base(options) {
        }

        public DbSet<Category> Categories { get; set; }
    }
}
