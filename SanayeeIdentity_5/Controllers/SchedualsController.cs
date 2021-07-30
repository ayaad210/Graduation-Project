using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SanayeeIdentity_5.Data;

namespace SanayeeIdentity_5.Controllers
{
    public class SchedualsController : Controller
    {
        private Entities db = new Entities();

        // GET: Scheduals
        public ActionResult Index()
        {
            var scheduals = db.Scheduals.Include(s => s.Worker);
            return View(scheduals.ToList());
        }

        // GET: Scheduals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedual schedual = db.Scheduals.Find(id);
            if (schedual == null)
            {
                return HttpNotFound();
            }
            return View(schedual);
        }

        // GET: Scheduals/Create
        public ActionResult Create()
        {
            ViewBag.WorkerId = new SelectList(db.Workers, "WorkerId", "UserID");
            return View();
        }

        // POST: Scheduals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ScedualId,WorkerId,StartDate,EndDate,Rate,PaidRate,Price")] Schedual schedual)
        {
            if (ModelState.IsValid)
            {
                db.Scheduals.Add(schedual);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.WorkerId = new SelectList(db.Workers, "WorkerId", "UserID", schedual.WorkerId);
            return View(schedual);
        }

        // GET: Scheduals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedual schedual = db.Scheduals.Find(id);
            if (schedual == null)
            {
                return HttpNotFound();
            }
            ViewBag.WorkerId = new SelectList(db.Workers, "WorkerId", "UserID", schedual.WorkerId);
            return View(schedual);
        }

        // POST: Scheduals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ScedualId,WorkerId,StartDate,EndDate,Rate,PaidRate,Price")] Schedual schedual)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schedual).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.WorkerId = new SelectList(db.Workers, "WorkerId", "UserID", schedual.WorkerId);
            return View(schedual);
        }

        // GET: Scheduals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedual schedual = db.Scheduals.Find(id);
            if (schedual == null)
            {
                return HttpNotFound();
            }
            return View(schedual);
        }

        // POST: Scheduals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Schedual schedual = db.Scheduals.Find(id);
            db.Scheduals.Remove(schedual);
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
