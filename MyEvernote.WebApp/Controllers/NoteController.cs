﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;

namespace MyEvernote.WebApp.Controllers
{
    [ErrorExc]
    public class NoteController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        private CategoryManager categoryManager = new CategoryManager();
        private LikedManager likedManager = new LikedManager();

        [Auth]
        public ActionResult Index()
        {
            //var notes = noteManager.ListQueryable().Include("Category").Where(x => x.Owner.Id == CurrentSession.Get<EvernoteUser>("login").Id).OrderByDescending(x=>x.ModifiedOn);
            var notes = noteManager.ListQueryable().Include("Category").Include("Owner").Where(x => x.Owner.Id == CurrentSession.User.Id).OrderByDescending(x => x.ModifiedOn);
            return View(notes.ToList());
        }

        [Auth]
        public ActionResult MyLikedNotes()
        {
            var likeNotes = likedManager.ListQueryable().Include("LikedUser").Include("Note").Where(
                x => x.LikedUser.Id == CurrentSession.User.Id).Select(
                x => x.Note).OrderByDescending(
                x => x.ModifiedOn);

            return View("Index", likeNotes.ToList());
        }

        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [Auth]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title");
            return View();
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Note note)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsurname");

            if (ModelState.IsValid)
            {
                note.Owner = CurrentSession.User;
                noteManager.Insert(note);
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Note note)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsurname");

            if (ModelState.IsValid)
            {
                Note db_note = noteManager.Find(x => x.Id == note.Id);
                db_note.IsDraft = note.IsDraft;
                db_note.CategoryId = note.CategoryId;
                db_note.Title = note.Title;
                db_note.Text = note.Text;

                noteManager.Update(db_note);
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [Auth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            Note note = noteManager.Find(x => x.Id == id.Value);
            noteManager.Delete(note);
            return RedirectToAction("Index");
        }

        [Auth]
        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            List<int> likedNoteIds = likedManager.List(x => x.LikedUser.Id == CurrentSession.User.Id && ids.Contains(x.Note.Id)).Select(x => x.Note.Id).ToList();

            return Json(new { result = likedNoteIds });
        }

        [Auth]
        [HttpPost]
        public ActionResult SetLikeState(int noteid, bool liked)
        {
            int res = 0;
            Liked like = likedManager.Find(x => x.Note.Id == noteid && x.LikedUser.Id == CurrentSession.User.Id);
            Note note = noteManager.Find(x => x.Id == noteid);

            if (like != null && liked == false) // like null değil ise unlike yapılarak like silinir
            {
               res = likedManager.Delete(like);
            }
            else if(like == null && liked == true) // like null ise like yapılarak insert edilir
            {
                res = likedManager.Insert(new Liked
                {
                    LikedUser = CurrentSession.User,
                    Note = note
                });
            }

            if(res > 0)
            {
                if(liked == true) // if(liked)
                {
                    note.LikeCount++;
                }
                else
                {
                    note.LikeCount--;
                }

                if(noteManager.Update(note) > 0)
                         return Json(new { hasError = false, errorMessage = string.Empty, result = note.LikeCount });
            }

            return Json(new { hasError = true, errorMessage = "Beğenme işlemi gerçekleşmedi.", result = note.LikeCount });
        }

        public ActionResult GetNoteDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PartialNoteDetail", note);
        }
    }
}
