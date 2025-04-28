using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ahmadi.Controllers
{
    public class IndexController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            if (Request.RawUrl.ToLower().Contains("index"))
                return RedirectPermanent("~/");
            return View();
        }
    }
}