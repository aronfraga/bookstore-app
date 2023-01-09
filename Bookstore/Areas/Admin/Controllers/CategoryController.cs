using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> dbResponse = _unitOfWork.Category.GetAll();
            return View(dbResponse);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Edit(int id)
        {
            if (id == null || id == 0) return NotFound();
            var dbResponse = _unitOfWork.Category.GetOne(data => data.Id == id);
            if (dbResponse == null) return NotFound();
            return View(dbResponse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the name");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Delete(int id)
        {
            if (id == null || id == 0) return NotFound();
            var dbResponse = _unitOfWork.Category.GetOne(data => data.Id == id);
            if (dbResponse == null) return NotFound();
            return View(dbResponse);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int id)
        {
            var dbResponse = _unitOfWork.Category.GetOne(data => data.Id == id);
            if (dbResponse == null) return NotFound();

            _unitOfWork.Category.Remove(dbResponse);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
