using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ahmadi.Infrastructure.Security;
using Domain;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class SliderImagesController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uof = null;

        //GET: Admin/SliderImages
        public ActionResult Index(int? id)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 6);
                if (p.Where(x => x == true).Any())
                {

                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var sliderImages = uof.SliderImageRepository.Get(x => x, x => x.SliderId == id.Value, null, "attachment,Slider").OrderBy(x => x.DisplaySort);
                    ViewBag.SliderTitle = uof.SliderRepository.GetByID(id).Title;
                    ViewBag.SliderId = id;
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SliderImages", "Index", true, 200, " نمایش صفحه مدیریت تصاویر اسلایدرها", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(sliderImages.ToList());
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تصاویر اسلایدرها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SliderImages", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        //POST: Admin/Categories/Sort/5
        [HttpPost]
        public virtual JsonResult Sort(string ids)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var SliderImage = uof.SliderImageRepository.GetByID(id);
                    SliderImage.DisplaySort = i;
                    uof.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SliderImages", "Sort", false, 200, "مرتب سازی تصاویرِ اسلایدرها", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SliderImages", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //GET: Admin/SliderImages/Create
        public ActionResult Create(int? id)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {


                if (ModulePermission.check(User.Identity.GetUserId(), 6, 1))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    ViewBag.SliderTitle = uof.SliderRepository.GetByID(id).Title;
                    ViewBag.SliderId = id;

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SliderImages", "Create", true, 200, " نمایش صفحه ایجاد تصویر برای اسلایدر", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تصاویر اسلایدرها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SliderImages", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        //POST: Admin/SliderImages/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( SliderImage sliderImage)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    if (uof.SliderImageRepository.Get(x => x, x => x.SliderId == sliderImage.SliderId).Any())
                        sliderImage.DisplaySort = uof.SliderImageRepository.Get(x => x, x => x.SliderId == sliderImage.SliderId).Max(x => x.DisplaySort) + 1;
                    else
                        sliderImage.DisplaySort = 0;
                    uof.SliderImageRepository.Insert(sliderImage);
                    uof.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "SliderImages", "Create", false, 200, "   ایجاد تصویر اسلایدر", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index", new { id = sliderImage.SliderId });
                }

                ViewBag.SliderTitle = uof.SliderImageRepository.GetByID(sliderImage.SliderId).Title;
                ViewBag.SliderId = sliderImage.SliderId;

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SliderImages", "Create", false, 500, "   خطا در ایجاد تصویر اسلایدر ", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(sliderImage);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SliderImages", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        //GET: Admin/SliderImages/Edit/5
        public ActionResult Edit(int? id)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {


                if (ModulePermission.check(User.Identity.GetUserId(), 6, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    SliderImage sliderImage = uof.SliderImageRepository.Get(x => x, x => x.Id == id, null, "attachment").Single();
                    if (sliderImage == null)
                    {
                        return HttpNotFound();
                    }
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SliderImages", "Edit", true, 200, " نمایش صفحه ویرایش تصویر اسلایدر ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(sliderImage);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تصاویر اسلایدر ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SliderImages", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        //POST: Admin/SliderImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( SliderImage sliderImage)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    uof.SliderImageRepository.Update(sliderImage);
                    uof.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "SliderImages", "Edit", false, 200, "   ویرایش تصویر اسلایدرِ ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index", new { id = sliderImage.SliderId });
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SliderImages", "Edit", false, 500, "   خطا در ویرایش تصویرِ اسلایدرِ ", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(sliderImage);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SliderImages", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        //GET: Admin/SliderImages/Delete/5
        public ActionResult Delete(int? id)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 6, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    SliderImage sliderImage = uof.SliderImageRepository.Get(x=>x,x=>x.Id==id,null, "attachment").Single();
                    if (sliderImage == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SliderImages", "Delete", true, 200, " نمایش صفحه حذف تصویر اسلایدرِ ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(sliderImage);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تصاویر اسلایدر ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SliderImages", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        //POST: Admin/SliderImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {
                SliderImage sliderImage = uof.SliderImageRepository.GetByID(id);
                uof.SliderImageRepository.Delete(sliderImage);
                uof.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "SliderImages", "DeleteConfirmed", false, 200, "حذف اسلایدر ", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", new { id = sliderImage.SliderId });

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SliderImages", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
