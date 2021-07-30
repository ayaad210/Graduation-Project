using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SanayeeIdentity_5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using SanayeeIdentity_5.Data;
using System.Data.Entity;
using SanayeeIdentity_5;

namespace SanayeeIdentity4.Controllers
{
    [Authorize(Roles = "Worker")]

    public class MainWorkerController : Controller
    {
        // GET: MainWorker
        
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        Entities db = new Entities();
        ApplicationDbContext db2 = new ApplicationDbContext();

        public MainWorkerController()
        {
        }

        public MainWorkerController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [AllowAnonymous]
        public ActionResult WorkerRegister()
        {
            ViewBag.Citys = new SelectList(db.Citys.ToList(), "CityId", "Name");
            ViewBag.Areas = new SelectList(db.Areas.Where(r => r.CityId ==0), "AreaId", "Name");
            ViewBag.Types= new SelectList(db.Types.ToList(), "TypeId", "Name");
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public  ActionResult WorkerRegister(WorkerRegisterViewModel model, HttpPostedFileBase image1)
        {
            ViewBag.Citys = new SelectList(db.Citys.ToList(), "CityId", "Name");
            ViewBag.Areas = new SelectList(db.Areas.Where(r => r.CityId == 0), "AreaId", "Name");
            ViewBag.Types = new SelectList(db.Types.ToList(), "TypeId", "Name");
            if (ModelState.IsValid)
            {
                ApplicationUser User = db2.Users.Where(u => u.PhoneNumber == model.PhoneNumber).FirstOrDefault();
                string Uid = null;
                if (User == null)
                {

                    AdressDetail adress = new AdressDetail() { AreaId = model.AreaId };
                    db.AdressDetails.Add(adress);
                    db.SaveChanges();

                    var user = new ApplicationUser() {Name=model.Name, AddressId = adress.AdressId, UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                    var result = UserManager.Create(user, model.Password);
                    if (result.Succeeded)
                    {

                    //    SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);

                        Uid = db2.Users.Where(u => u.PhoneNumber == model.PhoneNumber).First().Id;

                    }
                    else
                    {
                        ViewBag.Citys = new SelectList(db.Citys, "CityId", "Name", model.CityId);
                        ViewBag.Areas = new SelectList(db.Areas.Where(r => r.CityId == model.CityId), "AreaId", "Name", model.AreaId);

                        AddErrors(result);
                        return View(model);
                    }


                }

                else
                {
                    Uid = User.Id;

                }

                UserManager.AddToRoles(Uid, "Worker");

                Worker w = new Worker() {TypeId=model.typeid, Available = false, Onwork = false, SBIN = model.ISBN, UserID = Uid };
                if (image1 != null)
                {                   
                    w.PhotoBin = new byte[image1.ContentLength];
                    image1.InputStream.Read(w.PhotoBin, 0, image1.ContentLength);
                }
                db.Workers.Add(w);
                db.SaveChanges();
                return  RedirectToAction("Login", "Account");





            }
       ViewBag.Citys = new SelectList(db.Citys, "CityId", "Name",model.CityId);
       ViewBag.Areas = new SelectList(db.Areas.Where(r=>r.CityId==model.CityId), "AreaId", "Name",model.AreaId);

            return View(model);
        }


        public ActionResult Index()
        {



            return View();

        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [AllowAnonymous]

        public JsonResult GetAreaList(string CityId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            int x = int.Parse(CityId);
            List<Area> Arealist = db.Areas.Where(p => p.CityId ==x) .ToList();

            return Json(Arealist, JsonRequestBehavior.AllowGet);
        }


 


        public ActionResult WorkerSceduasls(string UserId)
        {



            Worker worker = db.Workers.Where(w => w.UserID == UserId).FirstOrDefault();
            List<Schedual> List = new List<Schedual>();
            List = db.Scheduals.Where(s => s.WorkerId == worker.WorkerId) .ToList();
            
            return View(List);
        }
        [AllowAnonymous]
        public JsonResult GetReqInlastTwoMin()
        {
            int x = 0;
            try
            {
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                int wid = db.Workers.Where(w => w.UserID == user.Id).FirstOrDefault().WorkerId;
                int s = (db.Scheduals.Where(ss=>ss.WorkerId == wid)).Select(ss => ss.ScedualId).Max();


                x = db.Requests.Where(r => r.ScedualId == s && r.RequestDateTime.Day == DateTime.Now.Day && r.Done == false).Count();

            }
            catch (Exception e)
            {
                x = 0;
    return Json("To Day Reqests: "+ x.ToString(), JsonRequestBehavior.AllowGet);

            }
            return Json("To Day Reqests: " + x.ToString(), JsonRequestBehavior.AllowGet);


        }
        public ActionResult scedialRequsts(string ScedualId)
        {
            int x = 0;
            x = int.Parse(ScedualId);

            Schedual scedual = db.Scheduals.Find(x);
            List<Request> List = new List<Request>();
            List = db.Requests.Where(r => r.ScedualId == x).ToList();

            return View(List);
        }
       
        public PartialViewResult RequstFinish(string RequestId)
        {
            int x = 0;
            x = int.Parse(RequestId);

            Request req = db.Requests.Where(r => r.RequestId == x).FirstOrDefault();

            req.Done = true;

            db.Entry(req).State = EntityState.Modified;
            db.SaveChanges();

            return PartialView("RequestPartial",req);
        }
        [HttpGet]
        public ActionResult WorkerProfile(string UserId)
        {
            
            ApplicationUser user=    db2.Users.Find(UserId);
            AdressDetail address = db.AdressDetails.Find(user.AddressId);
            Worker worker = db.Workers.Where(w => w.UserID == user.Id).First();
            WorkerRegisterViewModel model = new WorkerRegisterViewModel()
            {
                Name = user.Name
                ,
                Email = user.Email
                ,
                AreaId = address.Area.AreaId
                ,
                CityId = address.Area.CityId
               ,
                PhoneNumber = user.PhoneNumber
               ,
                userid = UserId
               ,
                ISBN = worker.SBIN
               ,
                Password = user.PasswordHash
                ,
                ConfirmPassword = user.PasswordHash
                ,
                IsAvailable = (bool)worker.Available
                ,
                PhotoBin = worker.PhotoBin
                , typeid = (int)worker.TypeId


            };
            ViewBag.Citys = new SelectList(db.Citys, "CityId", "Name",model.CityId);
            ViewBag.Areas = new SelectList(db.Areas .Where(ar=>ar.CityId==model.CityId).ToList(), "AreaId", "Name", model.AreaId);
          

            return View(model);
            

          
           
        }
        [HttpPost]
        public ActionResult WorkerProfile(WorkerRegisterViewModel model, HttpPostedFileBase image1)
        {
          
                ApplicationUser user = db2.Users.Find(model.userid);
                List<ApplicationUser> list = new List<ApplicationUser>();
                list = db2.Users.ToList();
            if ((list.Where(u => u.Email == model.Email&&u.Id!=model.userid).Count() < 1||user.Email==model.Email )&& model.Email != null)
            {
                user.Name = model.Name;
                user.Email = model.Email;
                user.UserName = model.Email;
                                if (((list.Where(u => u.PhoneNumber == model.PhoneNumber&&u.Id!=model.userid).Count() < 1 ||model.PhoneNumber==user.PhoneNumber)&&model.PhoneNumber!=null)&&model.AreaId!=0)
                    {
                        user.PhoneNumber = model.PhoneNumber;
                        AdressDetail add = db.AdressDetails.Find(user.AddressId);
                        add.AreaId = model.AreaId;
                        db.Entry(add).State = EntityState.Modified;
                    Worker worker = db.Workers.Where(w => w.UserID == model.userid).First();
                    worker.Available = model.IsAvailable;
                    if (image1 != null)
                    {
                        worker.PhotoBin = new byte[image1.ContentLength];
                        image1.InputStream.Read(worker.PhotoBin, 0, image1.ContentLength);
                    }
                     db2.Entry(user).State = EntityState.Modified;
                        db2.SaveChanges();

                    db.Entry(worker).State= EntityState.Modified;

                    db.SaveChanges();

                      

                        return RedirectToAction("Index", "Home");
                    }

                


               
        }
            ViewBag.Citys = new SelectList(db.Citys, "CityId", "Name", model.CityId);
           ViewBag.Areas = new SelectList(db.Areas.Where(ar => ar.CityId == model.CityId).ToList(), "AreaId", "Name", model.AreaId);
            model.PhoneNumber = user.PhoneNumber;

            return View(model);



        }

    }
}
