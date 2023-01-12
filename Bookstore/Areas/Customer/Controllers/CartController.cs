﻿using Bookstore.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Bookstore.Utility;
using Stripe.Checkout;
using Microsoft.Extensions.Options;

namespace Bookstore.Areas.Customer.Controllers {

	[Area("Customer")]
	[Authorize]
	public class CartController : Controller {

		private readonly IUnitOfWork _unitOfWork;
		
		[BindProperty]
		public ShoppingCardViewModel ShoppingCardViewModel { get; set; }

		public CartController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}
		
		public IActionResult Index() {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCardViewModel = new ShoppingCardViewModel() {
				ListCard = _unitOfWork.ShoppingCard.GetAll(data => data.ApplicationUserId == claim.Value, includeProperties: "Product"),
				OrderHeader = new()
			};

			foreach (var item in ShoppingCardViewModel.ListCard) {
				ShoppingCardViewModel.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
			}
			
			return View(ShoppingCardViewModel);
		}

		public IActionResult Summary() {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCardViewModel = new ShoppingCardViewModel() {
				ListCard = _unitOfWork.ShoppingCard.GetAll(data => data.ApplicationUserId == claim.Value, includeProperties: "Product"),
				OrderHeader = new()
			};

			ShoppingCardViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetOne(data => data.Id == claim.Value);

			ShoppingCardViewModel.OrderHeader.Name = ShoppingCardViewModel.OrderHeader.ApplicationUser.Name;
			ShoppingCardViewModel.OrderHeader.PhoneNumber = ShoppingCardViewModel.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCardViewModel.OrderHeader.StreetAddress = ShoppingCardViewModel.OrderHeader.ApplicationUser.StreetAddress;
			ShoppingCardViewModel.OrderHeader.City = ShoppingCardViewModel.OrderHeader.ApplicationUser.City;
			ShoppingCardViewModel.OrderHeader.State = ShoppingCardViewModel.OrderHeader.ApplicationUser.State;
			ShoppingCardViewModel.OrderHeader.PostalCode = ShoppingCardViewModel.OrderHeader.ApplicationUser.PostalCode;

			foreach (var item in ShoppingCardViewModel.ListCard) {
				ShoppingCardViewModel.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
			}

			return View(ShoppingCardViewModel);
		}

		[HttpPost]
		[ActionName("Summary")]
		[ValidateAntiForgeryToken]
		public IActionResult SummaryPost(ShoppingCardViewModel shoppingCardViewModel) {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			shoppingCardViewModel.ListCard = _unitOfWork.ShoppingCard.GetAll(data => data.ApplicationUserId == claim.Value, includeProperties: "Product");

			shoppingCardViewModel.OrderHeader.PaymentStatus = Status.PaymentStatusPending;
			shoppingCardViewModel.OrderHeader.OrderStatus = Status.StatusPending;
			shoppingCardViewModel.OrderHeader.OrderDate = DateTime.Now;
			shoppingCardViewModel.OrderHeader.ApplicationUserId = claim.Value;

			foreach (var item in shoppingCardViewModel.ListCard) {
				shoppingCardViewModel.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
			}

			_unitOfWork.OrderHeader.Add(shoppingCardViewModel.OrderHeader);
			_unitOfWork.Save();

			foreach (var item in shoppingCardViewModel.ListCard) {
				OrderDetail orderDetail = new() {
					ProductId = item.ProductId,
					OrderId = shoppingCardViewModel.OrderHeader.Id,
					Price = item.Product.Price,
					Count = item.Count,
				};
				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}

			var domain = "https://localhost:44300/";

			var options = new SessionCreateOptions {
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = domain+$"customer/cart/OrderConfirmation?id={shoppingCardViewModel.OrderHeader.Id}",
				CancelUrl = domain+"customer/cart/index",
			};

			foreach (var item in shoppingCardViewModel.ListCard) {
				var sessionLineItem = new SessionLineItemOptions {
					PriceData = new SessionLineItemPriceDataOptions {
						UnitAmount = (long)(item.Product.Price*100), //20.00 -> 2000
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions {
							Name = item.Product.Title,
						},
					},
					Quantity = item.Count,
				};
				options.LineItems.Add(sessionLineItem);
			}

			var service = new SessionService();
			Session session = service.Create(options);
			shoppingCardViewModel.OrderHeader.SessionId = session.Id;
			shoppingCardViewModel.OrderHeader.PaymentIntentId = session.PaymentIntentId;

			_unitOfWork.OrderHeader.UpdateStripePaymentID(shoppingCardViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
		}

		public IActionResult OrderConfirmation(int id) {
			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetOne(data => data.Id == id);
			var service = new SessionService();
			Session session = service.Get(orderHeader.SessionId);
			
			if(session.PaymentStatus.ToLower() == "paid") {
				_unitOfWork.OrderHeader.UpdateStatus(id, Status.StatusApproved, Status.PaymentStatusApproved);
				_unitOfWork.Save();
			}

			List<ShoppingCard> shoppingCards = _unitOfWork.ShoppingCard.GetAll(data => data.ApplicationUserId ==
																					   orderHeader.ApplicationUserId).ToList();
			_unitOfWork.ShoppingCard.RemoveRange(shoppingCards);
			_unitOfWork.Save();
			return View(id);
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