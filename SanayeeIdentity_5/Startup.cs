using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using SanayeeIdentity_5.Models;

[assembly: OwinStartupAttribute(typeof(SanayeeIdentity_5.Startup))]
namespace SanayeeIdentity_5
{
    public partial class Startup
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoles();
            CreateUsers();
        }
        public void CreateUsers()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            ApplicationUser user = new ApplicationUser();
            user.Email = "Elso@gmail.com";
            user.UserName = "Elso@gmail.com";
            user.Name = "MasterAdmin";
            var check = userManager.Create(user, "Elso1104+");
            if (check.Succeeded)
            {
                userManager.AddToRoles(user.Id, "Admins");
            }



        }


        public void CreateRoles()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            IdentityRole role;
            if (!roleManager. RoleExists("Admins"))
            {
                role = new IdentityRole();

                role.Name = "Admins";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Worker"))
            {
                role = new IdentityRole();
                role.Name = "Worker";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Client"))
            {
                role = new IdentityRole();
                role.Name = "Client";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("Employee"))
            {
                role = new IdentityRole();
                role.Name = "Employee";
                roleManager.Create(role);
            }



        }

    }
}
