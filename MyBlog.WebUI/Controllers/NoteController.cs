using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBlog.BusinessLayer;
using MyBlog.Entities.Concrete;
using MyBlog.WebUI.Data;
using MyBlog.WebUI.Filters;
using MyBlog.WebUI.Models;

namespace MyBlog.WebUI.Controllers
{
    [HandleException]
    public class NoteController : Controller
    {
        NoteManager _noteManager = new NoteManager();

        LikedManager _likedManager = new LikedManager();

        CategoryManager _categoryManager = new CategoryManager();


        // GET: Note
        [Auth]
        public IActionResult Index()
        {
            //string currentUserJson = HttpContext.Session.GetString("currentUser");

            //BlogUser currentUser = JsonSerializer.Deserialize<BlogUser>(currentUserJson);

            var notes = _noteManager.ListQueryable().Include("Category").Include("BlogUser").Where(x => x.BlogUser.Id == CurrentSession.CurrentUser.Id).OrderByDescending(x => x.ModifiedDate).ToList();

            return View(notes.ToList());
        }

        [Auth]
        public IActionResult MyLikedNotes()
        {
            //string currentUserJson = HttpContext.Session.GetString("currentUser");

            //BlogUser currentUser = JsonSerializer.Deserialize<BlogUser>(currentUserJson);

            var notes = _likedManager.ListQueryable().Include("LikedUser").Include("Note")
                .Where(x => x.LikedUser.Id == CurrentSession.CurrentUser.Id)
                .Select(x => x.Note)
                .OrderByDescending(x => x.ModifiedDate)
                .ToList();

            return View("Index", notes.ToList());
        }

        // GET: Note/Details/5
        [Auth]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = _noteManager.ListQueryable()
                .Include(n => n.Category)
                .FirstOrDefault(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Note/Create
        [Auth]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_categoryManager.List(), "Id", "Description");
            return View();
        }

        // POST: Note/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Auth]
        public IActionResult Create(Note note)
        {
            ModelState.Remove("ModifiedUserName");
            ModelState.Remove("BlogUser");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                note.ModifiedUserName = CurrentSession.CurrentUser.UserName;
                note.CreatedDate = DateTime.Now;
                note.ModifiedDate = DateTime.Now;
                note.BlogUserId = CurrentSession.CurrentUser.Id;
                _noteManager.Insert(note);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_categoryManager.List(), "Id", "Description", note.CategoryId);
            return View(note);
        }

        // GET: Note/Edit/5
        [Auth]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = _noteManager.GetById(id.Value);
            if (note == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_categoryManager.List(), "Id", "Description", note.CategoryId);
            return View(note);
        }

        // POST: Note/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Auth]
        public IActionResult Edit(Note note)
        {
            ModelState.Remove("BlogUser");
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                note.ModifiedUserName = CurrentSession.CurrentUser.UserName;
                note.ModifiedDate = DateTime.Now;
                note.BlogUserId = CurrentSession.CurrentUser.Id;
                _noteManager.Update(note);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_categoryManager.List(), "Id", "Description", note.CategoryId);
            return View(note);
        }

        // GET: Note/Delete/5
        [Auth]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = _noteManager.ListQueryable()
                .Include(n => n.Category)
                .FirstOrDefault(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Note/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Auth]
        public IActionResult DeleteConfirmed(int id)
        {
            Note note = _noteManager.GetById(id);

            if (note != null)
            {
                _noteManager.Delete(note);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult NoteDetails(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            List<Note> notes = _noteManager.ListQueryable().Include("BlogUser").Where(n => n.Id == id).ToList();

            if (notes == null)
            {
                return NotFound();
            }

            return PartialView("_PartialNoteDetails", notes);
        }


        [HttpPost]
        [Auth]
        public IActionResult GetLiked(int[] ids)
        {
            List<int> likedNoteIds = null;
            if (CurrentSession.CurrentUser != null)
            {
                likedNoteIds = _likedManager.List(x => x.LikedUserId == CurrentSession.CurrentUser.Id && ids.Contains(x.NoteId)).Select(x => x.NoteId).ToList();
            }
            return Json(new { result = likedNoteIds });
        }

        [HttpPost]
        [Auth]
        public IActionResult SetNoteLike(int noteid, bool liked)
        {
            int likeCount = 0;

            if (CurrentSession.CurrentUser != null)
            {
                int result = 0;

                // kullanıcının beğendiği/etkileşime getiği kayıt veritabanında var mı yok mu kontrol edilir.
                // 
                Liked like = _likedManager.Find(x => x.Note.Id == noteid && x.LikedUser.Id == CurrentSession.CurrentUser.Id);

                Note note = _noteManager.GetById(noteid);
                if (like != null && liked == false)
                {
                    // silme işlemi yap.. Beğeni ilgili nottan kaldırılıyor/siliniyor.
                    result = _likedManager.Delete(like);
                }
                else if (like == null && liked == true)
                {
                    // beğeni Likes tablosuna eklenecek...
                    result = _likedManager.Insert(
                        new Liked()
                        {
                            LikedUserId = CurrentSession.CurrentUser.Id,
                            NoteId = noteid
                        });
                }

                if (result > 0)
                {
                    if (liked)
                    {
                        note.LikeCount++;
                    }
                    else
                    {
                        note.LikeCount--;
                    }
                    _noteManager.Update(note);

                    likeCount = note.LikeCount;

                    return Json(new { hasError = false, errorMessage = string.Empty, result = likeCount });
                }
            }


            return Json(new { hasError = true, errorMessage = "Beğenilme işlemi gerçekleştirilemedi.", result = likeCount });



        }
    }
}
