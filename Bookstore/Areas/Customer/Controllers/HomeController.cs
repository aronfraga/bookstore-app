using Bookstore.Models;
using Bookstore.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bookstore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index() {
            IEnumerable<Product> productsList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return View(productsList);
        }

		public IActionResult Details(int id) {
			ShoppingCard cartObj = new() {
				Count = 1,
				Product = _unitOfWork.Product.GetOne(u => u.Id == id, includeProperties: "Category,CoverType"),
			};
			return View(cartObj);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}