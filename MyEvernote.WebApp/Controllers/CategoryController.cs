﻿using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using System.Net;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Controllers
{
    [Auth] // Tüm metdolara yazılması gerektiği için her bir metoda yazmak yerine controller classı na ekledik
    [AuthAdmin]
    [ErrorExc]
    public class CategoryController : Controller
    {
        CategoryManager categoryManager = new CategoryManager();

        public ActionResult Index()
        {
            //return View(categoryManager.List());   // database den gelen
            return View(CacheHelper.GetCategoriesFromCache()); // Cache ten gelen
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = categoryManager.Find(x => x.Id == id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsurname");

            if (ModelState.IsValid)
            {
                categoryManager.Insert(category);
                CacheHelper.RemoveCategoriesFromCache();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = categoryManager.Find(x => x.Id == id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsurname");

            if (ModelState.IsValid)
            {
                Category cat = categoryManager.Find(x => x.Id == category.Id);
                cat.Title = category.Title;
                cat.Description = category.Description;

                categoryManager.Update(cat);
                CacheHelper.RemoveCategoriesFromCache();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = categoryManager.Find(x => x.Id == id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            Category category = categoryManager.Find(x => x.Id == id.Value);

            categoryManager.Delete(category);
            CacheHelper.RemoveCategoriesFromCache();

            return RedirectToAction("Index");
        }
    }
}
