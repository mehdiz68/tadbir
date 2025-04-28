using System.Web.Http;
using System.Web.Mvc;

namespace ahmadi.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {

        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            //context.Routes.MapHttpRoute(
            //    name: "Admin_DefaultApi",
            //    routeTemplate: "Admin/api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);


            context.MapRoute(
                "Admin_home",
                "Admin/{controller}/{action}/{id}",
                new { Controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "ahmadi.Areas.Admin.Controllers" }
            );
        }
    }
}