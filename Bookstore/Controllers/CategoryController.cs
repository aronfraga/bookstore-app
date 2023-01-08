using Bookstore.Data;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Controllers {
    public class CategoryController : Controller {

        private readonly Context _dbcontext;

        public CategoryController(Context dbcontext) { 
            _dbcontext = dbcontext;
        }
        
        public IActionResult Index() {
            IEnumerable<Category> dbResponse = _dbcontext.Categories;
            return View(dbResponse);
        }

        public IActionResult Create() { 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category) {
            if(category.Name == category.DisplayOrder.ToString()) {
                ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the name");
            }
            if (ModelState.IsValid) {
                _dbcontext.Categories.Add(category);
                _dbcontext.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Edit(int id) {
            if(id == null || id == 0) return NotFound();
            var dbResponse = _dbcontext.Categories.Find(id);
            if(dbResponse == null) return NotFound();
            return View(dbResponse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category) {
            if (category.Name == category.DisplayOrder.ToString()) {
                ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the name");
            }
            if (ModelState.IsValid) {
                _dbcontext.Categories.Update(category);
                _dbcontext.SaveChanges();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Delete(int id) {
            if (id == null || id == 0) return NotFound();
            var dbResponse = _dbcontext.Categories.Find(id);
            if (dbResponse == null) return NotFound();
            return View(dbResponse);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int id) {
            var dbResponse = _dbcontext.Categories.Find(id);
            if (dbResponse == null) return NotFound();
            
            _dbcontext.Categories.Remove(dbResponse);
            _dbcontext.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
