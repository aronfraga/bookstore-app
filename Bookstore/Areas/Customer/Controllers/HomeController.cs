using Bookstore.Models;
using Bookstore.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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

        public IActionResult Details(int productId) {
            ShoppingCard cartObj = new() {
                Count = 1,
                ProductId = productId,
                Product = _unitOfWork.Product.GetOne(u => u.Id == productId, includeProperties: "Category,CoverType"),
            };
            return View(cartObj);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public IActionResult Details(ShoppingCard shoppingCard) {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCard.ApplicationUserId = claim.Value;

            ShoppingCard cardFromDb = _unitOfWork.ShoppingCard.GetOne(data => data.ApplicationUserId == claim.Value && 
                                                                              data.ProductId == shoppingCard.ProductId);

            if(cardFromDb== null) {
				_unitOfWork.ShoppingCard.Add(shoppingCard);
			} else {
                _unitOfWork.ShoppingCard.IncrementQty(cardFromDb, shoppingCard.Count);
            }
            _unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}