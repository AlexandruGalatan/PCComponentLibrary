using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProiectDAW.Models;
using Microsoft.AspNet.Identity;

namespace ProiectDAW.Controllers
{
    [Authorize]
    public class ComponentListEntriesController : Controller
    {
        private ComponentStuff db = new ComponentStuff();

        // GET: ComponentListEntries
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var componentListEntries = db.ComponentListEntries.Include(c => c.ComponentList).Include(c => c.Component);
            return View(componentListEntries.ToList());
        }

        // GET: ComponentListEntries/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComponentListEntry componentListEntry = db.ComponentListEntries.Find(id);
            if (componentListEntry == null)
            {
                return HttpNotFound();
            }
            return View(componentListEntry);
        }

        // GET: ComponentListEntries/Create
        public ActionResult Create(int? id)
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

            var userId = User.Identity.GetUserId();
            var UserComponentLists = db.ComponentLists.Where(i => i.AspNetUserId == userId);

            if (UserComponentLists.Count() <= 0)
            {
                return RedirectToAction("Create", "ComponentLists");
            }

            ViewBag.ComponentListId = new SelectList(UserComponentLists, "Id", "Title");
            ViewBag.ComponentId = component;
            return View();
        }

        // POST: ComponentListEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ComponentListId,ComponentId")] ComponentListEntry componentListEntry)
        {
            if (ModelState.IsValid)
            {
                db.ComponentListEntries.Add(componentListEntry);
                db.SaveChanges();
                return RedirectToAction("Details", "Components", new { id = componentListEntry.ComponentId });
            }

            ViewBag.ComponentListId = new SelectList(db.ComponentLists, "Id", "Title", componentListEntry.ComponentListId);
            ViewBag.ComponentId = new SelectList(db.Components, "Id", "Title", componentListEntry.ComponentId);
            return View(componentListEntry);
        }

        // GET: ComponentListEntries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComponentListEntry componentListEntry = db.ComponentListEntries.Find(id);
            if (componentListEntry == null)
            {
                return HttpNotFound();
            }
            ViewBag.ComponentListId = new SelectList(db.ComponentLists, "Id", "Title", componentListEntry.ComponentListId);
            ViewBag.ComponentId = new SelectList(db.Components, "Id", "Title", componentListEntry.ComponentId);
            return View(componentListEntry);
        }

        // POST: ComponentListEntries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ComponentListId,ComponentId")] ComponentListEntry componentListEntry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(componentListEntry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ComponentListId = new SelectList(db.ComponentLists, "Id", "Title", componentListEntry.ComponentListId);
            ViewBag.ComponentId = new SelectList(db.Components, "Id", "Title", componentListEntry.ComponentId);
            return View(componentListEntry);
        }

        // GET: ComponentListEntries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComponentListEntry componentListEntry = db.ComponentListEntries.Find(id);
            if (componentListEntry == null)
            {
                return HttpNotFound();
            }
            return View(componentListEntry);
        }

        // POST: ComponentListEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ComponentListEntry componentListEntry = db.ComponentListEntries.Find(id);
            db.ComponentListEntries.Remove(componentListEntry);
            db.SaveChanges();
            return RedirectToAction("Index", "ComponentLists");
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
