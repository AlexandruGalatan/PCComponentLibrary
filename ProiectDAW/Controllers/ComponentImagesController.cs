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
    public class ComponentImagesController : Controller
    {
        private ComponentStuff db = new ComponentStuff();

        // GET: ComponentImages
        public ActionResult Index()
        {
            var componentImages = db.ComponentImages.Include(c => c.Component);
            return View(componentImages.ToList());
        }

        // GET: ComponentImages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComponentImage componentImage = db.ComponentImages.Find(id);
            if (componentImage == null)
            {
                return HttpNotFound();
            }
            return View(componentImage);
        }

        // GET: ComponentImages/Create
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

            ViewBag.ComponentId = component;

            ComponentImage c = new ComponentImage();
            return View(c);
        }

        // POST: ComponentImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ComponentId,Image")] ComponentImage componentImage, HttpPostedFileBase image1)
        {
            if (ModelState.IsValid && image1 != null)
            {
                componentImage.Image = new byte[image1.ContentLength];
                image1.InputStream.Read(componentImage.Image, 0, image1.ContentLength);

                db.ComponentImages.Add(componentImage);
                db.SaveChanges();
                return RedirectToAction("Details", "Components", new { id = componentImage.ComponentId });
            }

            ViewBag.ComponentId = new SelectList(db.Components, "Id", "Title", componentImage.ComponentId);
            return View(componentImage);
        }

        // GET: ComponentImages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComponentImage componentImage = db.ComponentImages.Find(id);
            if (componentImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.ComponentId = new SelectList(db.Components, "Id", "Title", componentImage.ComponentId);
            return View(componentImage);
        }

        // POST: ComponentImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ComponentId,Image")] ComponentImage componentImage, HttpPostedFileBase image1)
        {
            if (ModelState.IsValid && image1 != null)
            {
                componentImage.Image = new byte[image1.ContentLength];
                image1.InputStream.Read(componentImage.Image, 0, image1.ContentLength);

                db.Entry(componentImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ComponentId = new SelectList(db.Components, "Id", "Title", componentImage.ComponentId);
            return View(componentImage);
        }

        // GET: ComponentImages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComponentImage componentImage = db.ComponentImages.Find(id);
            if (componentImage == null)
            {
                return HttpNotFound();
            }
            return View(componentImage);
        }

        // POST: ComponentImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ComponentImage componentImage = db.ComponentImages.Find(id);
            db.ComponentImages.Remove(componentImage);
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
