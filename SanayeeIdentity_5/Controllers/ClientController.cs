using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SanayeeIdentity_5.Data;
using SanayeeIdentity_5.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SanayeeIdentity_5.Controllers
{
    [Authorize(Roles = "Client")]

    public class ClientController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        Entities db = new Entities();
        ApplicationDbContext db2 = new ApplicationDbContext();

        public ClientController()
        {
        }

        public ClientController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Client
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ClientProfile(string Userid)
        {

            ApplicationUser user = db2.Users.Find(Userid);
            AdressDetail address = db.AdressDetails.Find(user.AddressId);
            ClientViewModel model = new ClientViewModel();
            if (address!=null)
            {
             model.AreaId = (int)address.AreaId;
            model.CityId = address.Area.CityId;
            }
            else
            {
                model.AreaId = 0;
                model.CityId = 0;
            }
            model.Uid = Userid;
            model.Name = user.Name;               
                model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            ViewBag.Citys = new SelectList(db.Citys, "CityId", "Name", model.CityId);
            ViewBag.Areas = new SelectList(db.Areas.Where(ar => ar.CityId == model.CityId).ToList(), "AreaId", "Name", model.AreaId);
            return View(model);
        }

        [HttpPost]
        public ActionResult ClientProfile(ClientViewModel model)
        {
            ApplicationUser cu = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            ApplicationUser user = db2.Users.Find(cu.Id);
            List<ApplicationUser> list = new List<ApplicationUser>();
            list = db2.Users.ToList();
            if (list.Where(u => u.Email == model.Email).Count() <= 1 && model.Email != null || user.Email == model.Email)
            {
                user.Name = model.Name;
                user.Email = model.Email;
                user.UserName = model.Email;
                if ((list.Where(u => u.PhoneNumber == model.PhoneNumber).Count() < 1 || model.PhoneNumber == user.PhoneNumber) && model.PhoneNumber != null )
                {
                    user.PhoneNumber = model.PhoneNumber;


                    AdressDetail add = db.AdressDetails.Find(user.AddressId);
                    if (add!=null&&model.AreaId!=0)
                    {
                      add.AreaId = model.AreaId;
                      db.Entry(add).State = EntityState.Modified;
                      db.SaveChanges();
                    }
                    if (add==null&&model.AreaId!=0)
                    {
                        AdressDetail newadd = new AdressDetail() { AreaId = model.AreaId };
                        db.AdressDetails.Add(newadd);
                        db.SaveChanges();
                        user.AddressId = newadd.AdressId;                   
                    }

                    db2.Entry(user).State = EntityState.Modified;
                    db2.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }





            }
            ViewBag.Citys = new SelectList(db.Citys, "CityId", "Name", model.CityId);
            //        ViewBag.Areas = new SelectList(db.Areas.Where(ar => ar.CityId == model.CityId).ToList(), "AreaId", "Name", model.AreaId);


            return View(model);




        }
        public JsonResult GetAreaList(string CityId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            int x = int.Parse(CityId);
            List<Area> Arealist = db.Areas.Where(p => p.CityId == x).ToList();

            return Json(Arealist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClientRequsts(string ClientId)
        {
         List<Request>  model=   db.Requests.Where(r => r.Userid == ClientId).ToList();


            return View(model);
        }

        public ActionResult DeleteClientRequest(string Reqid,string ClientId)
        {
            try
            {
     Request r = db.Requests.Find(int.Parse(Reqid));
            if (r!=null)
            {
            db.Requests.Remove(r);
                db.SaveChanges();

            }

            }
            catch (Exception)
            {
                return RedirectToAction("ClientRequsts",new { ClientId = ClientId });

            }

            return RedirectToAction("ClientRequsts",new { ClientId = ClientId });

        }
    }
}