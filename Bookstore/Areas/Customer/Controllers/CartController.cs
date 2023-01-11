using Bookstore.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookstore.Areas.Customer.Controllers {

	[Area("Customer")]
	[Authorize]
	public class CartController : Controller {

		private readonly IUnitOfWork _unitOfWork;
		public ShoppingCardViewModel ShoppingCardViewModel { get; set; }

		public CartController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}
		
		public IActionResult Index() {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCardViewModel = new ShoppingCardViewModel() {
				ListCard = _unitOfWork.ShoppingCard.GetAll(data => data.ApplicationUserId == claim.Value, includeProperties: "Product")
			};

			foreach (var item in ShoppingCardViewModel.ListCard) {
				ShoppingCardViewModel.CardTotal += (item.Product.Price * item.Count);
			}
			
			return View(ShoppingCardViewModel);
		}

		public IActionResult Summary() {
			//var claimsIdentity = (ClaimsIdentity)User.Identity;
			//var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			//ShoppingCardViewModel = new ShoppingCardViewModel() {
			//	ListCard = _unitOfWork.ShoppingCard.GetAll(data => data.ApplicationUserId == claim.Value, includeProperties: "Product")
			//};

			//foreach (var item in ShoppingCardViewModel.ListCard) {
			//	ShoppingCardViewModel.CardTotal += (item.Product.Price * item.Count);
			//}

			//return View(ShoppingCardViewModel);
			return View();
		}

		public IActionResult Plus(int cardId) {
			var cart = _unitOfWork.ShoppingCard.GetOne(data => data.Id == cardId);
			_unitOfWork.ShoppingCard.IncrementQty(cart, 1);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cardId) {
			var cart = _unitOfWork.ShoppingCard.GetOne(data => data.Id == cardId);
			_unitOfWork.ShoppingCard.DecrementQty(cart, 1);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cardId) {
			var cart = _unitOfWork.ShoppingCard.GetOne(data => data.Id == cardId);
			_unitOfWork.ShoppingCard.Remove(cart);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

	}
}
