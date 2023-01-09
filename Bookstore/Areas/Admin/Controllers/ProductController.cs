using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index() {
            IEnumerable<Product> dbResponse = _unitOfWork.Product.GetAll();
            return View(dbResponse);
        }


        public IActionResult Upsert(int id) {

            ProductViewModel pvm = new() {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(
                    data => new SelectListItem {
                        Text = data.Name,
                        Value = data.Id.ToString()
                    }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
                    data => new SelectListItem {
                        Text = data.Name,
                        Value = data.Id.ToString()
                    }),
            };

            if (id == null || id == 0) {
                return View(pvm);
            } else {

            }
            return View(pvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product product) {
            if (ModelState.IsValid) {
                _unitOfWork.Product.Update(product);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type updated successfully";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public IActionResult Delete(int id) {
            if (id == null || id == 0) return NotFound();
            var dbResponse = _unitOfWork.Product.GetOne(data => data.Id == id);
            if (dbResponse == null) return NotFound();
            return View(dbResponse);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int id) {
            var dbResponse = _unitOfWork.Product.GetOne(data => data.Id == id);
            if (dbResponse == null) return NotFound();

            _unitOfWork.Product.Remove(dbResponse);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
