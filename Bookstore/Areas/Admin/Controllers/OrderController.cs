using Bookstore.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Repository.IRepository;
using Bookstore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace Bookstore.Areas.Admin.Controllers {

	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller {

		private readonly IUnitOfWork _unitOfWork;
		private object shoppingCardViewModel;

		[BindProperty]
		public OrderViewModel OrderViewModel { get; set; }

		public OrderController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			return View();
		}

		public IActionResult Details(int orderId) {
			OrderViewModel = new OrderViewModel() {
				OrderHeader = _unitOfWork.OrderHeader.GetOne(data => data.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(data => data.OrderId == orderId, includeProperties: "Product"),
			};
			return View(OrderViewModel);
		}

		[ActionName("Details")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Details_PAY_NOW() {

			OrderViewModel.OrderHeader = _unitOfWork.OrderHeader.GetOne(data => data.Id == OrderViewModel.OrderHeader.Id, includeProperties: "ApplicationUser");
			OrderViewModel.OrderDetail = _unitOfWork.OrderDetail.GetAll(data => data.OrderId == OrderViewModel.OrderHeader.Id, includeProperties: "Product");

			var domain = "https://localhost:44300/";

			var options = new SessionCreateOptions {
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderid={OrderViewModel.OrderHeader.Id}",
				CancelUrl = domain + $"admin/order/details?orderId={OrderViewModel.OrderHeader.Id}",
			};

			foreach (var item in OrderViewModel.OrderDetail) {
				var sessionLineItem = new SessionLineItemOptions {
					PriceData = new SessionLineItemPriceDataOptions {
						UnitAmount = (long)(item.Product.Price * 100), //20.00 -> 2000
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

			_unitOfWork.OrderHeader.UpdateStripePaymentID(OrderViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);

		}

		public IActionResult PaymentConfirmation(int orderHeaderid) {

			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetOne(u => u.Id == orderHeaderid);

			if (orderHeader.PaymentStatus == Status.PaymentStatusDelayedPayment) {
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid") {
					_unitOfWork.OrderHeader.UpdateStatus(orderHeaderid, orderHeader.OrderStatus, Status.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}

			return View(orderHeaderid);
		}


		[HttpPost]
		[Authorize(Roles = Roles.Role_Admin + "," + Roles.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetail() {

			var orderHeaderDB = _unitOfWork.OrderHeader.GetOne(data => data.Id == OrderViewModel.OrderHeader.Id, tracked:false);

			orderHeaderDB.Name = OrderViewModel.OrderHeader.Name;
			orderHeaderDB.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
			orderHeaderDB.StreetAddress = OrderViewModel.OrderHeader.StreetAddress;
			orderHeaderDB.City = OrderViewModel.OrderHeader.City;
			orderHeaderDB.State = OrderViewModel.OrderHeader.State;
			orderHeaderDB.PostalCode = OrderViewModel.OrderHeader.PostalCode;

			if (OrderViewModel.OrderHeader.Carrier != null) {
				orderHeaderDB.Carrier = OrderViewModel.OrderHeader.Carrier;
			}

			if (OrderViewModel.OrderHeader.TrackingNumber != null) {
				orderHeaderDB.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
			}

			_unitOfWork.OrderHeader.Update(orderHeaderDB);
			_unitOfWork.Save();

			TempData["Success"] = "Order Details Updated Successfully.";
			return RedirectToAction("Details", "Order", new { orderId = orderHeaderDB.Id });
		}

		[HttpPost]
		[Authorize(Roles = Roles.Role_Admin + "," + Roles.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult StartProcessing() {

			var orderHeaderDB = _unitOfWork.OrderHeader.GetOne(data => data.Id == OrderViewModel.OrderHeader.Id, tracked: false);

			_unitOfWork.OrderHeader.UpdateStatus(OrderViewModel.OrderHeader.Id, Status.StatusInProcess);
			_unitOfWork.Save();

			TempData["Success"] = "Order Status Updated Successfully.";
			return RedirectToAction("Details", "Order", new { orderId = OrderViewModel.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = Roles.Role_Admin + "," + Roles.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult ShipOrder() {
			var orderHeader = _unitOfWork.OrderHeader.GetOne(u => u.Id == OrderViewModel.OrderHeader.Id, tracked: false);
			
			orderHeader.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderViewModel.OrderHeader.Carrier;
			orderHeader.OrderStatus = Status.StatusShipped;
			orderHeader.ShippingDate = DateTime.Now;
			
			if (orderHeader.PaymentStatus == Status.PaymentStatusDelayedPayment) {
				orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
			}
			
			_unitOfWork.OrderHeader.Update(orderHeader);
			_unitOfWork.Save();
			TempData["Success"] = "Order Shipped Successfully.";
			return RedirectToAction("Details", "Order", new { orderId = OrderViewModel.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = Roles.Role_Admin + "," + Roles.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder() {
			var orderHeader = _unitOfWork.OrderHeader.GetOne(u => u.Id == OrderViewModel.OrderHeader.Id, tracked: false);
			
			if(orderHeader.PaymentStatus == Status.PaymentStatusApproved) {

				var options = new RefundCreateOptions {
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId,
				};

				var service = new RefundService();
				Refund refund = service.Create(options);

				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, Status.StatusCancelled, Status.StatusRefunded);
			} else {
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, Status.StatusCancelled, Status.StatusCancelled);
			}
			
			_unitOfWork.Save();
			TempData["Success"] = "Order Cancelled Successfully.";
			return RedirectToAction("Details", "Order", new { orderId = OrderViewModel.OrderHeader.Id });
		}


		[HttpGet]
		public IActionResult GetAllOrders(string status) {

			IEnumerable<OrderHeader> orderHeaders;

			if (User.IsInRole(Roles.Role_Admin) || User.IsInRole(Roles.Role_Employee)) {
				orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
			} else {
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(data => data.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }

			switch (status) {
                case "pending":
					orderHeaders = orderHeaders.Where(data => data.PaymentStatus == Status.PaymentStatusDelayedPayment);
					break;
				case "inprocess":
					orderHeaders = orderHeaders.Where(data => data.OrderStatus == Status.StatusInProcess);
                    break;
                case "completed":
					orderHeaders = orderHeaders.Where(data => data.OrderStatus == Status.StatusShipped);
                    break;
                case "approveed":
					orderHeaders = orderHeaders.Where(data => data.OrderStatus == Status.StatusApproved);
					break;
                default:
					break;
            }

			return Json(new { data = orderHeaders });

		}

	}
}
