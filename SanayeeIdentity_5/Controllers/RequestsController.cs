using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SanayeeIdentity_5.Data;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using SanayeeIdentity_5.Controllers;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Infrastructure;
using SanayeeIdentity_5.Models;
using SanayeeIdentity_5;

namespace SanayeeIdentity_5.Controllers
{
    [Authorize(Roles = "Client")]

    public class RequestsController : Controller
    {
        //حل مشكلة الركويس خلى الركويس كى هوا البريمى كى وهتلاقى السبب تحت



        private ApplicationDbContext dbc = new ApplicationDbContext();
        private Entities db = new Entities();

        // private Entities dbc = new Entities();
        private ApplicationUserManager _userManager;
        public ActionResult SearchWorkerByUsers(string SearchKey)
        {
            if (SearchKey != "")
            {
                int MaxRate = int.Parse(db.Workers.Max(e => e.TotalRate).ToString());
                //i have to make two diffrent quires cuz the data belong to diffrent  edml file or dbcontex
                var helpWithUserList = dbc.Users.ToList();
                var ws = helpWithUserList.Join(db.Workers, user => user.Id, worker => worker.UserID, (user, worker) => new WorkerViewCardModelcs
                {

                    MaxRateofWorkers = MaxRate,
                    Name = user.Name,
                    WorkerId = worker.WorkerId,
                    AddressId = user.AddressId,
                    UserName = user.UserName,
                    SBIN = worker.SBIN,
                    PhoneNumber = user.PhoneNumber,
                    PhotoBin = worker.PhotoBin,
                    TotalRate = worker.TotalRate,
                    Email = user.Email,
                    //extra to remove buttoms
                    Available = worker.Available,
                    Onwork = worker.Onwork,
                    TypeName = db.Workers.Join(db.Types, w => w.TypeId, t => t.TypeId, (w, t) => t).First().Name.ToString()
                }
                        //here the update
                        ).ToList().Where(e=>e.Name.Contains(SearchKey)).ToList();
                        
                      //var s= Workers.Where(worker => worker.Name.Contains(SearchKey)).ToList();
                return PartialView("_WorkerCard", ws);
            }
            else return View();
        }

        public ActionResult InsertComplain(string WorkerId, string head, string body)
        {
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());


            if (WorkerId != "" && body != "")
            {
                Complaint cmp = new Complaint();
                cmp.datetime = DateTime.Now;
                cmp.Workerid = WorkerId;
                cmp.MessageHead = head;
                cmp.Text = body;
                cmp.Userid = user.Id;

                db.Complaints.Add(cmp);
                db.SaveChanges();
                return Json(new { message = $"Your Compline Is Done at" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { message = $"Your compline Isn't Done " }, JsonRequestBehavior.AllowGet);



        }
        public ActionResult InsertingRateofWorker(string workerid, string rate)
        {

            int wi = int.Parse(workerid);
            db.Workers.Where(e => e.WorkerId == wi).SingleOrDefault().TotalRate += int.Parse(rate);
            db.SaveChanges();
            return null;
        }
        public JsonResult GetAreas(string City)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                int cityid = Convert.ToInt32(City);
                var Areas = db.Areas.Where(e => e.CityId == cityid).ToList();
                //ViewBag.AreasID = new SelectList(db.Areas, "Areaid", "Name");
                return Json(Areas, JsonRequestBehavior.AllowGet);
            }
            catch (FormatException)
            {
                this.Dispose();
                return null;
            }
        }
        public ActionResult GetWorkerByCityAreaType(string City, string Area, string Type)
        {
            if (City != "" && Area != "" && Type != "")
            {
                try
                {


                    db.Configuration.ProxyCreationEnabled = false;
                    //الشروط اتلاته متاح بيشتغل بى الفعل و اخر اسكدول ليه  لسه مفتوح المده
                    var datenow = DateTime.Now;
                    var Workers = db.Scheduals.Where(e => e.EndDate > datenow && e.Worker.Onwork == true && e.Worker.Available == true)
                        .Select(z => z.Worker).Distinct().ToList();
                    //this will return html you need to bind this to your model  when accepted as response
                    int cityid = int.Parse(City);
                    int areaid = int.Parse(Area);
                    int typeint = int.Parse(Type);
                    var WorkersThatHasUser = Workers.Join(dbc.Users, e => e.UserID, u => u.Id, (e, u) => new { e, u.AddressId }).ToList();
                    //City Area type here search
                    var moreeasy = WorkersThatHasUser.Join(db.AdressDetails, z => z.AddressId, p => p.AdressId, (e, u) => new { e.e, u.AreaId }).Join(db.Areas, u => u.AreaId, e => e.AreaId, (y, u) => new { y.e, u })
                       .Where(z => z.u.CityId == cityid && z.u.AreaId == areaid && z.e.TypeId == typeint).Select(z => z.e).ToList();
                    // Note that Annonmous object will cauze runtime problem cuz it just local you must made a modelclass

                    var SchedualOfWorkers = moreeasy.Join(db.Scheduals, e => e.WorkerId, s => s.WorkerId, (e, s) => s).ToList();
                    int MaxRate = int.Parse(db.Workers.Max(e => e.TotalRate).ToString());

                    var WorkerFullinfo = moreeasy.Join(dbc.Users, e => e.UserID, i => i.Id, (e, i) => new WorkerViewCardModelcs
                    {
                        MaxRateofWorkers = MaxRate,
                        Name = i.Name,
                        WorkerId = e.WorkerId,
                        AddressId = i.AddressId,
                        UserName = i.UserName,
                        SBIN = e.SBIN,
                        PhoneNumber = i.PhoneNumber,
                        PhotoBin = e.PhotoBin,
                        TotalRate = e.TotalRate,
                        Email = i.Email,
                        ScedualId = SchedualOfWorkers.Where(w => w.WorkerId == e.WorkerId).ToList().Last().ScedualId,
                        //TypeName = moreeasy.Join(db.Types, w => w.TypeId, t => t.TypeId, (w, t) => t).First().Name.ToString()
                        TypeName = db.Types.Where(z => z.TypeId == e.TypeId).SingleOrDefault().Name
                    }

                     ).ToList();

                    ViewBag.All =WorkerFullinfo;
                    //Make sure that you have not error in syntax in the view cuz it will not render
                    return PartialView("_WorkerCard", WorkerFullinfo);
                }
                catch (FormatException)
                {
                    this.Dispose();
                    return null;
                }
            }
            else /*if(City == "" || Area == "")*/
            {
                if (Type == null || Type == "")
                {
                    return PartialView("_WorkerCard", "");
                }
                else
                {
                    //search egypt by type
                    db.Configuration.ProxyCreationEnabled = false;
                    //الشروط اتلاته متاح بيشتغل بى الفعل و اخر اسكدول ليه  لسه مفتوح المده
                    var datenow = DateTime.Now;
                    var Workers = db.Scheduals.Where(e => e.EndDate > datenow && e.Worker.Onwork == true && e.Worker.Available == true)
                        .Select(z => z.Worker).Distinct().ToList();
                    //this will return html you need to bind this to your model  when accepted as response

                    int typeint = int.Parse(Type);

                    var WorkersThatHasUser = Workers.Join(dbc.Users, e => e.UserID, u => u.Id, (e, u) => new { e, u.AddressId }).ToList();
                    //here the type only
                    var moreeasy = WorkersThatHasUser.Join(db.AdressDetails, z => z.AddressId, p => p.AdressId, (e, u) => new { e.e, u.AreaId }).Join(db.Areas, u => u.AreaId, e => e.AreaId, (y, u) => new { y.e, u })
                       .Where(z => z.e.TypeId == typeint).Select(z => z.e).ToList();
                    // Note that Annonmous object will cauze runtime problem cuz it just local you must made a modelclass

                    var SchedualOfWorkers = moreeasy.Join(db.Scheduals, e => e.WorkerId, s => s.WorkerId, (e, s) => s).ToList();

                    int MaxRate = int.Parse(db.Workers.Max(e => e.TotalRate).ToString());
                    var WorkerFullinfo = moreeasy.Join(dbc.Users, e => e.UserID, i => i.Id, (e, i) => new WorkerViewCardModelcs
                    {
                        MaxRateofWorkers = MaxRate,
                        Name = i.Name,
                        WorkerId = e.WorkerId,
                        AddressId = i.AddressId,
                        UserName = i.UserName,
                        SBIN = e.SBIN,
                        PhoneNumber = i.PhoneNumber,
                        PhotoBin = e.PhotoBin,
                        TotalRate = e.TotalRate,
                        Email = i.Email,
                        ScedualId = SchedualOfWorkers.Where(w => w.WorkerId == e.WorkerId).ToList().Last().ScedualId,
                        //TypeName = moreeasy.Join(db.Types, w => w.TypeId, t => t.TypeId, (w, t) => t).First().Name.ToString()
                        TypeName = db.Types.Where(z => z.TypeId == e.TypeId).SingleOrDefault().Name
                    }

                     ).ToList();

                    ViewBag.All = WorkerFullinfo;
                    //Make sure that you have not error in syntax in the view cuz it will not render
                    return PartialView("_WorkerCard", WorkerFullinfo);
                }

            }
        }



        public ActionResult GetWorkersByCity(string City, string Type)
        {
            if (Type == null || Type == "")
            {

                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    ////var Workers = db.Workers.Join(db.Scheduals, e => e.WorkerId, w => w.WorkerId, (e, w) => e)
                    ////    .Where(w => w.Onwork == true && w.Available == true).ToList();
                    //Solved like this 
                    //To see it choose workerid only and go to script in sql write
                    //select e.WorkerId,s.EndDate
                    //from Workers as e join Scheduals as s
                    //on e.WorkerId = s.WorkerId
                    //where s.EndDate >= GETDATE() and e.Onwork = 1 and e.Available = 1




                    //الشروط اتلاته متاح بيشتغل بى الفعل و اخر اسكدول ليه  لسه مفتوح المده
                    var datenow = DateTime.Now;
                    var Workers = db.Scheduals.Where(e => e.EndDate > datenow && e.Worker.Onwork == true && e.Worker.Available == true)
                        .Select(z => z.Worker).Distinct().ToList();
                    //this will return html you need to bind this to your model  when accepted as response
                    int cityid = int.Parse(City);
                    var WorkersThatHasUser = Workers.Join(dbc.Users, e => e.UserID, u => u.Id, (e, u) => new { e, u.AddressId }).ToList();
                    var moreeasy1 = WorkersThatHasUser.Join(db.AdressDetails, z => z.AddressId, p => p.AdressId, (e, u) => new { e.e, u.AreaId }).ToList();
                    var moreeasy2 = moreeasy1.Join(db.Areas, u => u.AreaId, e => e.AreaId, (y, u) => new { y.e, u.CityId }).ToList();
                    //Type Eddting is here
                    //int typeint = int.Parse(Type);
                    var moreeasy = moreeasy2.Where(z => z.CityId == cityid).Select(z => z.e).ToList();

                    var SchedualOfWorkers = moreeasy.Join(db.Scheduals, e => e.WorkerId, s => s.WorkerId, (e, s) => s).ToList();
                    //19/4/2019 new update the schedual id will be in request will be last scheduql of the worker
                    int MaxRate = int.Parse(db.Workers.Max(e => e.TotalRate).ToString());
                    var WorkerFullinfo = moreeasy.Join(dbc.Users, e => e.UserID, i => i.Id, (e, i) => new WorkerViewCardModelcs
                    {
                        MaxRateofWorkers = MaxRate,
                        Name = i.Name,
                        WorkerId = e.WorkerId,
                        AddressId = i.AddressId,
                        UserName = i.UserName,
                        SBIN = e.SBIN,
                        PhoneNumber = i.PhoneNumber,
                        PhotoBin = e.PhotoBin,
                        TotalRate = e.TotalRate,
                        Email = i.Email,
                        ScedualId = SchedualOfWorkers.Where(w => w.WorkerId == e.WorkerId).ToList().Last().ScedualId,
                        //the modfication i did to fix the type name error copy past to all if the problem expand
                        TypeName = db.Types.Where(z => z.TypeId == e.TypeId).SingleOrDefault().Name
                    }

                    ).ToList();
                    //moreeasy.Join(db.Types, w => w.TypeId, t => t.TypeId, (w, t) => t).First().Name.ToString()


                    //select user that goes to specific city

                    //extra info city and area
                    //var ExtraInfo = WorkersThatHasUser.Join(db.AdressDetails, z => z.AddressId, p => p.AdressId, (e, u) => new { e.e, u.AreaId }).Join(db.Areas, u => u.AreaId, e => e.AreaId, (y, u) => new { y.e, u })
                    //   .Where(z => z.u.CityId == cityid).Select(z => new { z.e, AreaName = z.u.Name, CityName = z.u.City.Name }).ToList();

                    return PartialView("_WorkerCard", WorkerFullinfo);
                }
                catch (FormatException)
                {
                    this.Dispose();
                    return null;

                }
            }
            else
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    ////var Workers = db.Workers.Join(db.Scheduals, e => e.WorkerId, w => w.WorkerId, (e, w) => e)
                    ////    .Where(w => w.Onwork == true && w.Available == true).ToList();
                    //Solved like this 
                    //To see it choose workerid only and go to script in sql write
                    //select e.WorkerId,s.EndDate
                    //from Workers as e join Scheduals as s
                    //on e.WorkerId = s.WorkerId
                    //where s.EndDate >= GETDATE() and e.Onwork = 1 and e.Available = 1




                    //الشروط اتلاته متاح بيشتغل بى الفعل و اخر اسكدول ليه  لسه مفتوح المده
                    var datenow = DateTime.Now;
                    var Workers = db.Scheduals.Where(e => e.EndDate > datenow && e.Worker.Onwork == true && e.Worker.Available == true)
                        .Select(z => z.Worker).Distinct().ToList();
                    //this will return html you need to bind this to your model  when accepted as response
                    int cityid = int.Parse(City);
                    var WorkersThatHasUser = Workers.Join(dbc.Users, e => e.UserID, u => u.Id, (e, u) => new { e, u.AddressId }).ToList();
                    var moreeasy1 = WorkersThatHasUser.Join(db.AdressDetails, z => z.AddressId, p => p.AdressId, (e, u) => new { e.e, u.AreaId }).ToList();
                    var moreeasy2 = moreeasy1.Join(db.Areas, u => u.AreaId, e => e.AreaId, (y, u) => new { y.e, u.CityId }).ToList();
                    //Type Eddting is here
                    int typeint = int.Parse(Type);
                    var moreeasy = moreeasy2.Where(z => z.CityId == cityid && z.e.TypeId == typeint).Select(z => z.e).ToList();

                    var SchedualOfWorkers = moreeasy.Join(db.Scheduals, e => e.WorkerId, s => s.WorkerId, (e, s) => s).ToList();
                    //19/4/2019 new update the schedual id will be in request will be last scheduql of the worker
                    int MaxRate = int.Parse(db.Workers.Max(e => e.TotalRate).ToString());
                    var WorkerFullinfo = moreeasy.Join(dbc.Users, e => e.UserID, i => i.Id, (e, i) => new WorkerViewCardModelcs
                    {
                        MaxRateofWorkers = MaxRate,
                        Name = i.Name,
                        WorkerId = e.WorkerId,
                        AddressId = i.AddressId,
                        UserName = i.UserName,
                        SBIN = e.SBIN,
                        PhoneNumber = i.PhoneNumber,
                        PhotoBin = e.PhotoBin,
                        TotalRate = e.TotalRate,
                        Email = i.Email,
                        ScedualId = SchedualOfWorkers.Where(w => w.WorkerId == e.WorkerId).ToList().Last().ScedualId,
                        TypeName = db.Types.Where(z => z.TypeId == e.TypeId).SingleOrDefault().Name
                    }

                    ).ToList();
                    //select user that goes to specific city

                    //extra info city and area
                    //var ExtraInfo = WorkersThatHasUser.Join(db.AdressDetails, z => z.AddressId, p => p.AdressId, (e, u) => new { e.e, u.AreaId }).Join(db.Areas, u => u.AreaId, e => e.AreaId, (y, u) => new { y.e, u })
                    //   .Where(z => z.u.CityId == cityid).Select(z => new { z.e, AreaName = z.u.Name, CityName = z.u.City.Name }).ToList();

                    return PartialView("_WorkerCard", WorkerFullinfo);
                }
                catch (FormatException)
                {
                    this.Dispose();
                    return null;

                }
            }

            //ViewBag.Workers = ExtraInfo;
            //Make sure that you have not error in syntax in the view cuz it will not render

        }


        public ActionResult GetWorkersByCityAndArea(string City, string Area, string Type)
        {
            if (Type == null || Type == "")
            {
                try
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    //الشروط اتلاته متاح بيشتغل بى الفعل و اخر اسكدول ليه  لسه مفتوح المده
                    var datenow = DateTime.Now;
                    var Workers = db.Scheduals.Where(e => e.EndDate > datenow && e.Worker.Onwork == true && e.Worker.Available == true)
                        .Select(z => z.Worker).Distinct().ToList();
                    //this will return html you need to bind this to your model  when accepted as response
                    int cityid = int.Parse(City);
                    int areaid = int.Parse(Area);
                    var WorkersThatHasUser = Workers.Join(dbc.Users, e => e.UserID, u => u.Id, (e, u) => new { e, u.AddressId }).ToList();

                    var moreeasy = WorkersThatHasUser.Join(db.AdressDetails, z => z.AddressId, p => p.AdressId, (e, u) => new { e.e, u.AreaId }).Join(db.Areas, u => u.AreaId, e => e.AreaId, (y, u) => new { y.e, u })
                       .Where(z => z.u.CityId == cityid && z.u.AreaId == areaid).Select(z => z.e).ToList();
                    // Note that Annonmous object will cauze runtime problem cuz it just local you must made a modelclass

                    var SchedualOfWorkers = moreeasy.Join(db.Scheduals, e => e.WorkerId, s => s.WorkerId, (e, s) => s).ToList();
                    int MaxRate = int.Parse(db.Workers.Max(e => e.TotalRate).ToString());
                    var WorkerFullinfo = moreeasy.Join(dbc.Users, e => e.UserID, i => i.Id, (e, i) => new WorkerViewCardModelcs
                    {
                        MaxRateofWorkers = MaxRate,
                        Name = i.Name,
                        WorkerId = e.WorkerId,
                        AddressId = i.AddressId,
                        UserName = i.UserName,
                        SBIN = e.SBIN,
                        PhoneNumber = i.PhoneNumber,
                        PhotoBin = e.PhotoBin,
                        TotalRate = e.TotalRate,
                        Email = i.Email,
                        ScedualId = SchedualOfWorkers.Where(w => w.WorkerId == e.WorkerId).ToList().Last().ScedualId,
                        //TypeName = moreeasy.Join(db.Types, w => w.TypeId, t => t.TypeId, (w, t) => t).First().Name.ToString()
                        TypeName = db.Types.Where(z => z.TypeId == e.TypeId).SingleOrDefault().Name
                    }

                     ).ToList();

                    ViewBag.All = WorkerFullinfo;
                    //Make sure that you have not error in syntax in the view cuz it will not render
                    return PartialView("_WorkerCard", WorkerFullinfo);
                }
                catch (FormatException)
                {
                    this.Dispose();
                    return null;
                }
            }
            else
            {
                try
                {
                    int TypeInt = int.Parse(Type);
                    db.Configuration.ProxyCreationEnabled = false;
                    //الشروط اتلاته متاح بيشتغل بى الفعل و اخر اسكدول ليه  لسه مفتوح المده
                    var datenow = DateTime.Now;
                    var Workers = db.Scheduals.Where(e => e.EndDate > datenow && e.Worker.Onwork == true && e.Worker.Available == true)
                        .Select(z => z.Worker).Distinct().ToList();
                    //this will return html you need to bind this to your model  when accepted as response
                    int cityid = int.Parse(City);
                    int areaid = int.Parse(Area);
                    var WorkersThatHasUser = Workers.Join(dbc.Users, e => e.UserID, u => u.Id, (e, u) => new { e, u.AddressId }).ToList();
                    ///The Modification of the Type inserted is here
                    var moreeasy = WorkersThatHasUser.Join(db.AdressDetails, z => z.AddressId, p => p.AdressId, (e, u) => new { e.e, u.AreaId }).Join(db.Areas, u => u.AreaId, e => e.AreaId, (y, u) => new { y.e, u })
                       .Where(z => z.u.CityId == cityid && z.u.AreaId == areaid && z.e.TypeId == TypeInt).Select(z => z.e).ToList();
                    // Note that Annonmous object will cauze runtime problem cuz it just local you must made a modelclass

                    var SchedualOfWorkers = moreeasy.Join(db.Scheduals, e => e.WorkerId, s => s.WorkerId, (e, s) => s).ToList();
                    int MaxRate = int.Parse(db.Workers.Max(e => e.TotalRate).ToString());
                    var WorkerFullinfo = moreeasy.Join(dbc.Users, e => e.UserID, i => i.Id, (e, i) => new WorkerViewCardModelcs
                    {
                        MaxRateofWorkers = MaxRate,
                        Name = i.Name,
                        WorkerId = e.WorkerId,
                        AddressId = i.AddressId,
                        UserName = i.UserName,
                        SBIN = e.SBIN,
                        PhoneNumber = i.PhoneNumber,
                        PhotoBin = e.PhotoBin,
                        TotalRate = e.TotalRate,
                        Email = i.Email,
                        ScedualId = SchedualOfWorkers.Where(w => w.WorkerId == e.WorkerId).ToList().Last().ScedualId,
                        //TypeName = moreeasy.Join(db.Types, w => w.TypeId, t => t.TypeId, (w, t) => t).First().Name.ToString()
                        TypeName = db.Types.Where(z => z.TypeId == e.TypeId).SingleOrDefault().Name
                    }

                     ).ToList();

                    ViewBag.All = WorkerFullinfo;
                    //Make sure that you have not error in syntax in the view cuz it will not render
                    return PartialView("_WorkerCard", WorkerFullinfo);
                }
                catch (FormatException)
                {
                    this.Dispose();
                    return null;
                }
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
        public async Task<string> CreateUser(string PhoneNumber, string Name)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbc));
            var user = new ApplicationUser();
            //var r = Guid.NewGuid().ToString(); error cotain only number or letter
            //error of name taken
            char[] letter = "abcdefghijklmnopqrstuvwxyz123456789".ToCharArray();
            Random r = new Random();
            string randomstring = null;
            for (int i = 0; i < 10; i++)
            {
                randomstring += letter[r.Next(0, 35)].ToString();
            }
            user.Name = Name;
            user.UserName = "Annoymous" + randomstring;
            user.Email = "AnnoymousUser@yahoo.com";
            user.PhoneNumber = PhoneNumber;// defult user  defult info no need for thir info for now
            //if you need to get the information about ther the most area with most user get it from request addressid  not from useraddress it add only headeck
            user.AddressId = 5;
            var check = await UserManager.CreateAsync(user, "MO@hamed123");
            if (check.Succeeded)
            {
                UserManager.AddToRole(user.Id, "Annoymous");
                dbc.Dispose();
                return user.Id;

            }
            return null;


        }



        public ActionResult InsertRequest(string WorkerId, string PhoneNumber, string Name, string Latitude, string longitude)
        {

            //searching first for the user if exist get it's id if not insert it
            //var User = dbc.Users.SingleOrDefault(u => u.PhoneNumber == PhoneNumber);


            //string UserId = null;
            //if (User == null)
            //{

            //    UserId = CreateUser(PhoneNumber,Name).Result;


            //}
            //else
            //{
            //    UserId = User.Id;
            //}
            //Should Write this but for seek of another team member reading i wrote the first
            /* UserId = (User == null) ? dbc.AspNetUsers.Add(User).Id : User.Id; */
            int Workerid = Convert.ToInt32(WorkerId);


            var request = new Request();
            //get current user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            request.Userid = user.Id;
            //the date from server so the user don't  lie us
            request.RequestDateTime = DateTime.Now;

            //2-geting the address of this worker 
            var f0 = db.Workers.Where(w => w.WorkerId == Workerid).ToList();
            var ad = f0.Join(dbc.Users, w => w.UserID, u => u.Id, (w, u) => new { u.AddressId, w }).First();
            request.AddressId = ad.AddressId;

            //request.RequestId = ++Requestincrementkey;
            //insert the request to the last schedual of the worker
            request.ScedualId = ad.w.Scheduals.Last().ScedualId;
            Map lastmap = new Map();
            try
            {
 decimal lat = decimal.Parse(Latitude);
            decimal lng = decimal.Parse(longitude);
            var map = new Map()
            {
                Latitude = lat,
                Longitude = lng
            };
             lastmap = db.Maps.Add(map);
            db.SaveChanges();
            }
            catch (Exception)
            {

                lastmap = null;
            }
           
            ////address details has error in designing the datbase that make it null every time i need to get the request address details


            //request.Schedual = new Schedual() { WorkerId=Workerid} ;
            //request.AdressDetail = new AdressDetail();

            var lastrequest = db.Requests.Add(request);
            //the request must be created first then the map should be set  very important fixed by mohamed feto

            //the request must be created first then the map should be set  very important fixed by mohamed feto

            //request.AdressDetail.Mapid = lastmap.MapId;
            db.SaveChanges();
            //solve problem i have to select the address detils
            //lastrequest.AdressDetail.Map.MapId = lastmap.MapId;
            if (lastmap != null)
            {
                var Addressdetilsofthisrequest = db.AdressDetails.Where(e => e.AdressId == lastrequest.AddressId);
                //will get the only one request by id
                Addressdetilsofthisrequest.SingleOrDefault().Mapid = lastmap.MapId;

                db.SaveChanges();
            }



            //    I got this same error because part of the PK was a datetime column, and the record being inserted used DateTime.Now as
            //     the value for that column. Entity framework would insert the value with millisecond precision, and then look for the value
            //         it just inserted also with millisecond precision.However SqlServer had rounded the value to second precision, and thus
            //         entity framework was unable to find the millisecond precision value.

            //The solution was to truncate the milliseconds from DateTime.Now before inserting.

            //Create(request);

            //var worker = db.Workers.Where(w => w.WorkerId == Workerid).Select(e => e).ToList().First();


            return Json(new { message = $"Your Request Is Done at {new DateTime().ToLongDateString()}  Please Be Patient He Will Contact You " }, JsonRequestBehavior.AllowGet);
        }












        // GET: Requests
        public ActionResult Index()
        {

            ViewBag.CityID = new SelectList(db.Citys, "Cityid", "Name");
            ViewBag.TypeID = new SelectList(db.Types, "Typeid", "Name");

            var requests = db.Requests.Include(r => r.AdressDetail).Include(r => r.Schedual);
            return View(requests.ToList());
        }

        // GET: Requests/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // GET: Requests/Create
        public ActionResult Create()
        {
            ViewBag.AddressId = new SelectList(db.AdressDetails, "AdressId", "Text");
            ViewBag.ScedualId = new SelectList(db.Scheduals, "ScedualId", "PaidRate");
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(Request request)
        {
            //check for user by phone if there is user get id if there is no user insert and get it's id
            //[Bind(Include = "ScedualId,RequestDateTime,AddressId")]
            if (ModelState.IsValid)
            {

                //dispose the entity and use it agine cuz of errors update conncurencyy 
                using (var context = new Entities())
                {
                    var blog = context.Requests.Find(9);
                    blog.Done = true;

                    bool saveFailed;
                    do
                    {
                        saveFailed = false;
                        try
                        {
                            db.Requests.Add(request);
                            context.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;

                            // Get the current entity values and the values in the database
                            // as instances of the entity type
                            var entry = ex.Entries.Single();
                            var databaseValues = entry.GetDatabaseValues();
                            var databaseValuesAsBlog = (Schedual)databaseValues.ToObject();

                            // Choose an initial set of resolved values. In this case we
                            // make the default be the values currently in the database.
                            var resolvedValuesAsBlog = (Schedual)databaseValues.ToObject();

                            // Have the user choose what the resolved values should be

                            // Update the original values with the database values and
                            // the current values with whatever the user choose.
                            entry.OriginalValues.SetValues(databaseValues);
                            entry.CurrentValues.SetValues(resolvedValuesAsBlog);
                        }

                    } while (saveFailed);
                }


            }


            //ViewBag.AddressId = new SelectList(db.AdressDetails, "AdressId", "Text", request.AddressId);
            //ViewBag.ScedualId = new SelectList(db.Scheduals, "ScedualId", "PaidRate", request.ScedualId);
            return View(request);
        }


        // GET: Requests/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            ViewBag.AddressId = new SelectList(db.AdressDetails, "AdressId", "Text", request.AddressId);
            ViewBag.ScedualId = new SelectList(db.Scheduals, "ScedualId", "PaidRate", request.ScedualId);
            return View(request);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RequestId,Userid,ScedualId,RequestDateTime,Done,AddressId")] Request request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AddressId = new SelectList(db.AdressDetails, "AdressId", "Text", request.AddressId);
            ViewBag.ScedualId = new SelectList(db.Scheduals, "ScedualId", "PaidRate", request.ScedualId);
            return View(request);
        }

        // GET: Requests/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Request request = db.Requests.Find(id);
            db.Requests.Remove(request);
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
