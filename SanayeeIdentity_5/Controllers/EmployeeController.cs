using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SanayeeIdentity_5.Data;
using SanayeeIdentity_5.Models;

namespace SanayeeIdentity_5.Controllers
{
   // [Authorize(Roles ="Employee")]
    public class EmployeeController : Controller
    {
        private Entities db = new Entities();
        private ApplicationDbContext db2 = new ApplicationDbContext();
        // GET: Workers
        public ActionResult Index()
        {
            var workers = db.Workers.Include(w => w.Type);

            return View(workers.ToList());
        }

        // GET: Workers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worker worker = db.Workers.Find(id);
            if (worker == null)
            {
                return HttpNotFound();
            }
            return View(worker);
        }
        [HttpGet]
        // GET: Workers/Create
        public ActionResult Create()
        {
            ViewBag.TypeId = new SelectList(db.Types, "TypeId", "Name");
            return View();
        }

        // POST: Workers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WorkerId,UserID,SBIN,Onwork,Available,TotalRate,PhotoBin,TypeId")] Worker worker)
        {
            if (ModelState.IsValid)
            {
                db.Workers.Add(worker);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TypeId = new SelectList(db.Types, "TypeId", "Name", worker.TypeId);
            return View(worker);
        }

        // GET: Workers/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worker worker = db.Workers.Find(id);
            if (worker == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeId = new SelectList(db.Types, "TypeId", "Name", worker.TypeId);
            return View(worker);
        }

   
        [HttpPost]
        public ActionResult Edit([Bind(Include = "WorkerId,UserID,SBIN,Onwork,Available,TotalRate,PhotoBin,TypeId")] Worker worker)
        {
            if (ModelState.IsValid)
            {
                db.Entry(worker).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeId = new SelectList(db.Types, "TypeId", "Name", worker.TypeId);
            return View(worker);
        }

        // GET: Workers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Worker worker = db.Workers.Find(id);
            if (worker == null)
            {
                return HttpNotFound();
            }
            return View(worker);
        }

        // POST: Workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Worker worker = db.Workers.Find(id);
            db.Workers.Remove(worker);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult WorkerScheduals(string WorkId)
        {
                List<Schedual> List = new List<Schedual>();

            int x = 0;
            if (int.TryParse(WorkId, out x))
            {
                Worker worker = db.Workers.Where(w => w.WorkerId == x).FirstOrDefault();
                List = db.Scheduals.Where(s => s.WorkerId == worker.WorkerId).ToList();

            }
            workerSceduasModel mo = new Models.workerSceduasModel();
            mo.List = List;
            mo.workerid = WorkId;


            return View(mo);
        }

        [HttpGet]
        public ActionResult CreateScedualForWorker(string WorkerId)
        {
            ViewBag.WorkerId = new SelectList(db.Workers, "WorkerId", "UserID", WorkerId);
            return View();
        }
        [HttpPost]
        public ActionResult CreateScedualForWorker([Bind(Include = "ScedualId,WorkerId,StartDate,EndDate,Rate,PaidRate,Price")] Schedual schedual)
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
        [HttpGet]
        public ActionResult EditScedual(int? Scedualid)
        {
            if (Scedualid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedual schedual = db.Scheduals.Find(Scedualid);
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
        public ActionResult EditScedual([Bind(Include = "ScedualId,WorkerId,StartDate,EndDate,Rate,PaidRate,Price")] Schedual schedual)
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
        [HttpGet]
        public ActionResult DeleteScedual(int? Scedualid)
        {
            if (Scedualid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Schedual schedual = db.Scheduals.Find(Scedualid);
            if (schedual == null)
            {
                return HttpNotFound();
            }
            return View(schedual);
        }

        // POST: Scheduals/Delete/5
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeleteScedual(int Scedualid)
        {
            Schedual schedual = db.Scheduals.Find(Scedualid);
            db.Scheduals.Remove(schedual);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public PartialViewResult WorkerAjaxSearch(string NameAjax)
        {
           

         List<ApplicationUser>  users = db2.Users.Where(p => p.Name.ToLower().StartsWith(NameAjax)).ToList();

          
          List<Worker> model = new  List<Worker>();


            foreach (var item in db.Workers)
            {

                foreach (var item2 in users)
                {
                    if (item.UserID==item2.Id)
                    {
                        model.Add(item);

                    }
                }

            }
      



            return PartialView("_WorkerSearchResutPartial", model);
        }

       public ActionResult WorkerComplimaints(string WorkerId)
        {

            List<Complaint> model = db.Complaints.Where(c => c.Workerid == WorkerId).ToList() ;

            return View(model);

        }
        public ActionResult DeleteComplimant(string Compid,string workerid)
        {
            try
            {
                db.Complaints.Remove(db.Complaints.Find(int.Parse(Compid)));
                db.SaveChanges();
            }
            catch (Exception)
            {

                return RedirectToAction("WorkerComplimaints", new { WorkerId = workerid });
            }
            return RedirectToAction("WorkerComplimaints", new { WorkerId = workerid });

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
