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
    public class ComponentListsController : Controller
    {
        private ComponentStuff db = new ComponentStuff();

        // GET: ComponentLists
        public ActionResult Index()
        {
            var componentLists = db.ComponentLists.Include(c => c.AspNetUser);

            if (User.IsInRole("Admin"))
            {
                return View(componentLists.ToList());
            }

            var userId = User.Identity.GetUserId();
            componentLists = db.ComponentLists.Where(i => i.AspNetUserId == userId).Include(c => c.AspNetUser);


            return View(componentLists.ToList());
        }

        // GET: ComponentLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComponentList componentList = db.ComponentLists.Find(id);
            if (componentList == null)
            {
                return HttpNotFound();
            }

            var cLE = db.ComponentListEntries.Where(i => i.ComponentListId == id).ToList();
            var components = new List<Component>();

            for (var i = 0; i < cLE.Count; i++)
            {
                var compId = cLE[i].ComponentId;
                Component comp = db.Components.Find(compId);
                comp.ListEntryId = cLE[i].Id;
                if (comp != null)
                {
                    components.Add(comp);
                }
            }

            ViewBag.Components = components;

            return View(componentList);
        }

        // GET: ComponentLists/Create
        public ActionResult Create()
        {
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: ComponentLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AspNetUserId,Title,Description")] ComponentList componentList)
        {
            componentList.AspNetUserId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                db.ComponentLists.Add(componentList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", componentList.AspNetUserId);
            return View(componentList);
        }

        // GET: ComponentLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComponentList componentList = db.ComponentLists.Find(id);
            if (componentList == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.GetUserId() != componentList.AspNetUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", componentList.AspNetUserId);
            return View(componentList);
        }

        // POST: ComponentLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AspNetUserId,Title,Description")] ComponentList componentList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(componentList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", componentList.AspNetUserId);
            return View(componentList);
        }

        // GET: ComponentLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComponentList componentList = db.ComponentLists.Find(id);
            if (componentList == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.GetUserId() != componentList.AspNetUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(componentList);
        }

        // POST: ComponentLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ComponentList componentList = db.ComponentLists.Find(id);
            db.ComponentLists.Remove(componentList);
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
