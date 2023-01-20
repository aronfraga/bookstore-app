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
	public class CompanyController : Controller {

        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index() {
            return View();
        }


        public IActionResult Upsert(int id) {

            Company company = new();

            if (id == null || id == 0) {
                return View(company);
            } else {
				company = _unitOfWork.Company.GetOne(data => data.Id == id);
                return View(company);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company) {
            if (ModelState.IsValid) {
                if (company.Id == 0) {
                    _unitOfWork.Company.Add(company);
					TempData["success"] = "Company created successfully";
				} else {
                    _unitOfWork.Company.Update(company);
					TempData["success"] = "Company updated successfully";
				}
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        [HttpGet]
        public IActionResult GetAllCompany() {
            var dbResponse = _unitOfWork.Company.GetAll();
            return Json(new { data = dbResponse });
        }

        [HttpDelete]
        public IActionResult Delete(int id) {
            var dbResponse = _unitOfWork.Company.GetOne(data => data.Id == id);
            if (dbResponse == null) return Json(new { success = false, message = "Error while deleting" });
            
            _unitOfWork.Company.Remove(dbResponse);
            _unitOfWork.Save();

            //TempData["success"] = "Category deleted successfully";
            return Json(new { success = true, message = "Delete Successful" });
        }

    }
}
