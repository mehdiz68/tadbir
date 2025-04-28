using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ahmadi.Models;
using Domain;

namespace ahmadi.Infrastructure.Security
{

    public class IdentityManager
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public IdentityManager()
        {
        }

        public IdentityManager(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        //public bool RoleExists(string name)
        //{

        //    var rm = new RoleManager<IdentityRole>(
        //        new RoleStore<IdentityRole>(new ApplicationDbContext()));
        //    return rm.RoleExists(name);
        //}

        public bool IsInRole(string userid, string RoleName)
        {
            return UserManager.IsInRole(userid, RoleName);
        }


        public ApplicationUser GetUser(string ID)
        {
            return UserManager.FindById(ID);

        }
        public ApplicationUser GetUserByName(string UserName)
        {

            return UserManager.FindByName(UserName);

        }
        public bool AddUserToRole(string userId, string roleName)
        {
            IdentityResult ir = UserManager.AddToRole(userId, roleName);
            if (ir.Succeeded)
                return true;
            else
                return false;
        }
        public bool ClearRole(string userId)
        {
            var roles = UserManager.GetRoles(userId);
            foreach (var item in roles)
            {
                UserManager.RemoveFromRole(userId, item);
            }
                return true;
        }

        public bool DeleteUser(string ID)
        {
            try
            {
                ApplicationUser cUser = UserManager.FindById(ID);
                var idResult = UserManager.Delete(cUser);
                if (idResult.Succeeded)
                    return true;
                else
                    return false;
            }
            catch (Exception x)
            {
                return false;
            }
        }


        public bool CreateUser(ApplicationUser user, string password)
        {
            var idResult = UserManager.Create(user, password);
            return idResult.Succeeded;
        }
        public bool ChangePassword(string userId, string newPassword)
        {
            try
            {

                var result = UserManager.RemovePassword(userId);
                if (result.Succeeded)
                {
                    result = UserManager.AddPassword(userId, newPassword);
                    if (result.Succeeded)
                        return true;
                    else
                        return false;

                }
                else
                    return false;
           
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}