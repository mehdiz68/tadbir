using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ahmadi.Infrastructure.Helper;

namespace ahmadi.Controllers
{
    public class ThumbnailController : Controller
    {
        // GET: Thumbnail
        [OutputCache(Duration = 112000, VaryByParam = "f;w;h;size")]
        public ActionResult Index(string f, int? w, int? h, string size, int? q)
        {

            if (size != "LG")
            {
                if (f.IndexOf("/") > 0)//file uploaded in folder
                {
                    if (System.IO.File.Exists(Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}/{1}_{2}", f.Substring(0, f.LastIndexOf("/")), size, f.Substring(f.LastIndexOf("/") + 1).Remove(0, 3)))))
                        f = string.Format("{0}/{1}_{2}", f.Substring(0, f.LastIndexOf("/")), size, f.Substring(f.LastIndexOf("/") + 1).Remove(0, 3));
                }
                else
                {
                    if (System.IO.File.Exists(Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}_{1}", size, f.Remove(0, 3)))))
                        f = string.Format("{0}_{1}", size, f.Remove(0, 3));
                }
            }
            WebImage img = new WebImage(Server.MapPath("~/Content/UploadFiles/" + f));
            img.Resize(w.HasValue ? w.Value : img.Width, h.HasValue ? h.Value : img.Height);
            img.FileName = f;
            img.Crop(1, 1, 1, 1);
            //img.Write();
            return File(img.GetBytes(), img.ImageFormat);

            //using (Image image = Image.FromFile(Server.MapPath("~/Content/UploadFiles/" + f)))
            //{
            //    Image bitmap = image.BestFit(w, h);

            //    return new ImageResult(image.BestFit(w, h));
            //    //return File(bitmap.ConvertToByteArray(), System.Web.MimeMapping.GetMimeMapping(Server.MapPath("~/Content/UploadFiles/" + f)));
            //}
        }

        //[OutputCache(Duration = 3600, VaryByParam = "f")]
        //public void Thumbnail(string f, int w, int h, int q)
        //{
        //    WebImage img = new WebImage(Server.MapPath("~/Content/UploadFiles/" + f));
        //    img.Resize(w, h);
        //    img.Write();
        //}
    }
}