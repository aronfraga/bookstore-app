using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models
{
    public class ShoppingCard
    {

        public Product Product { get; set; }

        [Range(1, 10, ErrorMessage = "Please enter a value between 1 and 10")]
        public int Count { get; set; }

    }
}
