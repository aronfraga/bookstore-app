using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Repository.IRepository;
using Bookstore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Areas.Admin.Controllers {

    [Area("Admin")]
	[Authorize(Roles = Roles.Role_Admin)]
	public class ProductController : Controller {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment) {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index() {
            return View();
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
                pvm.Product = _unitOfWork.Product.GetOne(data => data.Id == id);
                return View(pvm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productViewModel, IFormFile? file) {
            if (ModelState.IsValid) {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if(file != null) {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    if(productViewModel.Product.ImageUrl != null) {
                        var oldImagePath = Path.Combine(wwwRootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create)) {
                        file.CopyTo(fileStreams);
                    }
					productViewModel.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                if (productViewModel.Product.Id == 0) {
                    _unitOfWork.Product.Add(productViewModel.Product);
                } else {
                    _unitOfWork.Product.Update(productViewModel.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            return View(productViewModel);
        }

        [HttpGet]
        public IActionResult GetAllProduct() {
            var dbResponse = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = dbResponse });
        }

        [HttpDelete]
        public IActionResult Delete(int id) {
            var dbResponse = _unitOfWork.Product.GetOne(data => data.Id == id);
            if (dbResponse == null) return Json(new { success = false, message = "Error while deleting" });
            
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, dbResponse.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
            _unitOfWork.Product.Remove(dbResponse);
            _unitOfWork.Save();

            //TempData["success"] = "Category deleted successfully";
            return Json(new { success = true, message = "Delete Successful" });
        }

    }
}
