using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ProiectDAW.Models;
using Microsoft.AspNet.Identity;

namespace ProiectDAW.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private ComponentStuff db = new ComponentStuff();

        // GET: Comments
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.AspNetUser).Include(c => c.Component);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create(int? id) // id is component id
        {
            //get component
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Component component = db.Components.Find(id);
            if (component == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.ComponentId = component; // new SelectList(db.Components, "Id", "Title");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ComponentId,AspNetUserId,Message,TimeStamp")] Comment comment)
        {
            comment.TimeStamp = DateTime.Now;
            comment.AspNetUserId = User.Identity.GetUserId();

            //get component
            Component component = db.Components.Find(comment.ComponentId);
            if (component == null)
            {
                return RedirectToAction("Index");
            }


            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "Components", new { id=comment.ComponentId });
            }

            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", comment.AspNetUserId);
            ViewBag.ComponentId = new SelectList(db.Components, "Id", "Title", comment.ComponentId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            if (User.Identity.GetUserId() != comment.AspNetUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", comment.AspNetUserId);
            ViewBag.ComponentId = new SelectList(db.Components, "Id", "Title", comment.ComponentId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ComponentId,AspNetUserId,Message,TimeStamp")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Components", new { id = comment.ComponentId });
            }
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", comment.AspNetUserId);
            ViewBag.ComponentId = new SelectList(db.Components, "Id", "Title", comment.ComponentId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            if (User.Identity.GetUserId() != comment.AspNetUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            var compId = comment.ComponentId;
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", "Components", new { id = compId });
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
