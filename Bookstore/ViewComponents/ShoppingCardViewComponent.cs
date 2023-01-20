using Bookstore.Repository.IRepository;
using Bookstore.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookstore.ViewComponents {
    public class ShoppingCardViewComponent : ViewComponent {
        
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCardViewComponent(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            var claimsIndentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIndentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null) {
                if(HttpContext.Session.GetInt32(Status.SessionCart) != null) {
                    return View(HttpContext.Session.GetInt32(Status.SessionCart));
                } else {
                    HttpContext.Session.SetInt32(Status.SessionCart,
                        _unitOfWork.ShoppingCard.GetAll(data => data.ApplicationUserId == claim.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32(Status.SessionCart));
                }
            } else {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
