using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Repository.IRepository;
using Bookstore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = Roles.Role_Admin)]
	public class CoverTypeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index() {
            IEnumerable<CoverType> dbResponse = _unitOfWork.CoverType.GetAll();
            return View(dbResponse);
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType coverType) {
            if (ModelState.IsValid) {
                _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type created successfully";
                return RedirectToAction("Index");
            }
            return View(coverType);
        }

        public IActionResult Edit(int id) {
            if (id == null || id == 0) return NotFound();
            var dbResponse = _unitOfWork.CoverType.GetOne(data => data.Id == id);
            if (dbResponse == null) return NotFound();
            return View(dbResponse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType coverType) {
            if (ModelState.IsValid) {
                _unitOfWork.CoverType.Update(coverType);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type updated successfully";
                return RedirectToAction("Index");
            }
            return View(coverType);
        }

        public IActionResult Delete(int id) {
            if (id == null || id == 0) return NotFound();
            var dbResponse = _unitOfWork.CoverType.GetOne(data => data.Id == id);
            if (dbResponse == null) return NotFound();
            return View(dbResponse);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int id) {
            var dbResponse = _unitOfWork.CoverType.GetOne(data => data.Id == id);
            if (dbResponse == null) return NotFound();

            _unitOfWork.CoverType.Remove(dbResponse);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
