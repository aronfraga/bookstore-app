﻿using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Controllers {
    public class CategoryController : Controller {

        private readonly ICategoryRepository _dbcontext;

        public CategoryController(ICategoryRepository dbcontext) { 
            _dbcontext = dbcontext;
        }
        
        public IActionResult Index() {
            IEnumerable<Category> dbResponse = _dbcontext.GetAll();
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
                _dbcontext.Add(category);
                _dbcontext.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Edit(int id) {
            if(id == null || id == 0) return NotFound();
            var dbResponse = _dbcontext.GetOne(data => data.Id == id);
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
                _dbcontext.Update(category);
                _dbcontext.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Delete(int id) {
            if (id == null || id == 0) return NotFound();
            var dbResponse = _dbcontext.GetOne(data => data.Id == id);
            if (dbResponse == null) return NotFound();
            return View(dbResponse);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int id) {
            var dbResponse = _dbcontext.GetOne(data => data.Id == id);
            if (dbResponse == null) return NotFound();
            
            _dbcontext.Remove(dbResponse);
            _dbcontext.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }

    }
}