using Bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data {
    public class Context : DbContext {

        private readonly DbContext _context;

        public Context(DbContextOptions<Context> options) : base(options) {
            _context = context; 
        }

        public DbSet<Category> Categories { get; set; }
    }
}
