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

namespace MyBlog.WebUI.Controllers
{
    [Auth]
    [AuthAdmin]
    [HandleException]
    public class AdminBlogUserController : Controller
    {
        BlogUserManager _manager = new BlogUserManager();

        // GET: AdminBlogUser
        public IActionResult Index()
        {
            return View(_manager.List());
        }

        // GET: AdminBlogUser/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogUser = _manager.Find(m => m.Id == id);
            if (blogUser == null)
            {
                return NotFound();
            }

            return View(blogUser);
        }

        // GET: AdminBlogUser/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminBlogUser/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BlogUser blogUser)
        {
            ModelState.Remove("UserProfileImage");
            ModelState.Remove("Notes");
            ModelState.Remove("Comments");
            ModelState.Remove("Likes");
            ModelState.Remove("ModifiedUserName");

            string currentUserJson = HttpContext.Session.GetString("currentUser");

            BlogUser currentUser = JsonSerializer.Deserialize<BlogUser>(currentUserJson);

            blogUser.CreatedDate = DateTime.Now;
            blogUser.ModifiedDate = DateTime.Now;
            blogUser.ActivateGuid = Guid.NewGuid();
            blogUser.UserProfileImage = "user-profile.jpg";
            blogUser.ModifiedUserName = currentUser.UserName;

            if (ModelState.IsValid)
            {  
                _manager.Insert(blogUser);
                return RedirectToAction(nameof(Index));
            }

            return View(blogUser);
        }

        // GET: AdminBlogUser/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogUser = _manager.GetById(id.Value);
            if (blogUser == null)
            {
                return NotFound();
            }
            return View(blogUser);
        }

        // POST: AdminBlogUser/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BlogUser blogUser)
        {
            ModelState.Remove("UserProfileImage");
            ModelState.Remove("Notes");
            ModelState.Remove("Comments");
            ModelState.Remove("Likes");
            ModelState.Remove("ModifiedUserName");

            string currentUserJson = HttpContext.Session.GetString("currentUser");

            BlogUser currentUser = JsonSerializer.Deserialize<BlogUser>(currentUserJson);

            blogUser.ModifiedUserName = currentUser.UserName;
            blogUser.ModifiedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _manager.Update(blogUser);
                return RedirectToAction(nameof(Index));
            }
            return View(blogUser);
        }

        // GET: AdminBlogUser/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogUser = _manager.Find(m => m.Id == id);
            if (blogUser == null)
            {
                return NotFound();
            }

            return View(blogUser);
        }

        // POST: AdminBlogUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            BlogUser blogUser = _manager.GetById(id);
            if (blogUser != null)
            {
                _manager.Delete(blogUser);
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
