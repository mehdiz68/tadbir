using System.Web.Mvc;

namespace ahmadi.Areas.Admin.Controllers
{
    public partial class ErrorController : Controller
    {
        // GET: Admin/Error
        public virtual ActionResult Index()
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}