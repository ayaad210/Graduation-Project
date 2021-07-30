using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SanayeeIdentity_5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using SanayeeIdentity_5.Data;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Threading;
using SanayeeIdentity_5;

namespace SanayeeIdentity4.Controllers
{
    [Authorize(Roles = "Admins")]
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        Entities db = new Entities();
        ApplicationDbContext db2 = new ApplicationDbContext();
        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult EmployeeeList()
        {
            IdentityRole role = db2.Roles.Where(r => r.Name == "Employee").First();

            List<ApplicationUser> model = db2.Users.Where(u => u.Roles.Where(r => r.RoleId == role.Id).ToList().Count > 0).ToList();

            return View(model);
        }
        [HttpGet]

        public ActionResult AddEmployee()
        {
            return View();
        }


        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddEmployee(EmployeeRegisterViewModel model)
        {
            List<ApplicationUser> Users = db2.Users.Where(u => u.PhoneNumber == model.PhoneNumber).ToList();

            if (ModelState.IsValid&&Users.Count()==0)
            {

          

                var user = new ApplicationUser { Name = model.Name, UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    UserManager.AddToRoles(user.Id, "Employee");

                    return RedirectToAction("EmployeeeList", "Admin");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public ActionResult EditEmployee(string EmpId)
        {
            ApplicationUser user = db2.Users.Where(u => u.Id == EmpId).First();


            EmployeeRegisterViewModel model = new EmployeeRegisterViewModel()
            { id = user.Id, Email = user.Email, Password = user.PasswordHash, ConfirmPassword = user.PasswordHash, PhoneNumber = user.PhoneNumber };
            return View(model);
        }
        [HttpPost]
        public ActionResult EditEmployee(EmployeeRegisterViewModel model)
        {
            List<ApplicationUser> list = new List<ApplicationUser>();

            list = db2.Users.ToList();

            ApplicationUser user = db2.Users.Where(u => u.Id == model.id).FirstOrDefault();
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.UserName = model.Email;
            if ((list.Where(u => u.PhoneNumber == model.PhoneNumber&&user.Id!=model.id).Count() < 1 && model.PhoneNumber != null || model.PhoneNumber == user.PhoneNumber) && (list.Where(u => u.Email == model.Email&&user.Id!=model.id).Count() < 1 && model.Email != null || model.Email == user.Email))
            {

                db2.Entry(user).State = EntityState.Modified;
                db2.SaveChanges();
                return RedirectToAction("EmployeeeList");
            }

            return View(model);
        }


       
        public ActionResult DeleteEmployee(string EmpId)
        {
            ApplicationUser user = db2.Users.Find(EmpId);
            db2.Users.Remove(user);
            db2.SaveChanges();
          return  RedirectToAction("EmployeeeList");
        }
        [HttpPost]
        public PartialViewResult EmployeeAjaxSearch(string NameAjax)
        {
            Thread.Sleep(1000);
            IdentityRole role = db2.Roles.Where(r => r.Name == "Employee").First();

            List<ApplicationUser> model = db2.Users.Where(u => u.Roles.Where(r => r.RoleId == role.Id).ToList().Count > 0).ToList();
            model = model.Where(p => p.UserName.ToLower().StartsWith(NameAjax)).ToList();

            return PartialView("_EmployeeSearchResutPartial", model.ToList());
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


    }
}