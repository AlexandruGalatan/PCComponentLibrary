using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProiectDAW.Models;

namespace ProiectDAW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ComponentsController : Controller
    {
        private ComponentStuff db = new ComponentStuff();

        // GET: Components
        [AllowAnonymous]
        public ActionResult Index(int? id) //category id
        {
            var cats = db.Categories.ToList();
            ViewBag.ShowCategories = cats;

            if (id == null || id == 0)
            {
                var components = db.Components.Include(c => c.Category);
                ViewBag.DisplayTitle = "All Components";
                return View(components.ToList());
            }

            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            ViewBag.DisplayTitle = "All Components - " + category.Title;

            var components2 = db.Components.Where(i => i.CategoryId == id).Include(c => c.Category);
            return View(components2.ToList());
        }

        // GET: Components/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Component component = db.Components.Find(id);
            if (component == null)
            {
                return HttpNotFound();
            }

            var comms = db.Comments.Where(i => i.ComponentId == component.Id);
            var comments = comms.ToList();

            for (var i = 0; i < comments.Count; i++)
            {
                var userId = comments[i].AspNetUserId;
                AspNetUser usr = db.AspNetUsers.Find(userId);
                if (usr != null)
                {
                    comments[i].AspNetUserName = usr.UserName;
                }
            }

            ViewBag.Comments = comments;


            var imgs = db.ComponentImages.Where(i => i.ComponentId == component.Id).ToList();

            ViewBag.Images = imgs;

            return View(component);
        }

        // GET: Components/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Title");
            return View();
        }

        // POST: Components/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryId,Title,Description")] Component component)
        {
            if (ModelState.IsValid)
            {
                db.Components.Add(component);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Title", component.CategoryId);
            return View(component);
        }

        // GET: Components/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Component component = db.Components.Find(id);
            if (component == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Title", component.CategoryId);
            return View(component);
        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,Title,Description")] Component component)
        {
            if (ModelState.IsValid)
            {
                db.Entry(component).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Title", component.CategoryId);
            return View(component);
        }

        // GET: Components/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Component component = db.Components.Find(id);
            if (component == null)
            {
                return HttpNotFound();
            }
            return View(component);
        }

        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Component component = db.Components.Find(id);
            db.Components.Remove(component);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
