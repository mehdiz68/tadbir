using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using PagedList;
using ahmadi.Infrastructure.Security;
using System.Collections.Generic;
using CoreLib.ViewModel.Xml;
using CoreLib.Infrastructure.ModelBinder;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class ProductAttributeTabsController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;

        // GET: Admin/ProductAttributeGroups
        public virtual ActionResult Index()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    List<ProductAttributeTab> ProductAttributeTabs = uow.ProductAttributeTabRepository.Get(x => x).ToList();


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributeTabs", "Index", true, 200, " نمایش صفحه مدیریت تب", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductAttributeTabs.OrderBy(x => x.DisplayOrder));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تب" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeTabs", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        public virtual JsonResult Sort(string ids)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (Int16 i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var social = uow.ProductAttributeTabRepository.GetByID(id);
                    social.DisplayOrder = i;
                    uow.Save();
                }
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "SortItems", false, 200, "   مرتب سازی مقادیر خصوصیت", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "SortItems", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public virtual ActionResult Create(int? Id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {

                    return View();

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تب" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeGroups", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create([Bind(Include = "Id,Name,Icon,DisplayOrder,LanguageId")] ProductAttributeTab ProductAttributeTab)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    ProductAttributeTab.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductAttributeTab.Name);
                    uow.ProductAttributeTabRepository.Insert(ProductAttributeTab);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductAttributeTabs", "Create", false, 200, "   ایجاد تب " + ProductAttributeTab.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeTabs", "Create", false, 500, "   خطا در ایجاد تب " + ProductAttributeTab.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductAttributeTab);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeTabs", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductAttributeTab);
            }

        }

        public virtual ActionResult Edit(int? Id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {
                    var ProductAttributeTab = uow.ProductAttributeTabRepository.GetByID(Id.Value);
                    if (ProductAttributeTab != null)
                    {
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributeTabs", "Edit", true, 200, " نمایش صفحه ویرایش تب", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(ProductAttributeTab);
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تب" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeTabs", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit([Bind(Include = "Id,Name,Icon,DisplayOrder,LanguageId")] ProductAttributeTab ProductAttributeTab)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    ProductAttributeTab.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductAttributeTab.Name);
                    uow.ProductAttributeTabRepository.Update(ProductAttributeTab);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductAttributeTabs", "Edit", false, 200, "   ویرایش تب" + ProductAttributeTab.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeTabs", "Edit", false, 500, "   خطا در ویرایش تب" + ProductAttributeTab.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductAttributeTab);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeTabs", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductAttributeTab);
            }

        }


        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    ProductAttributeTab ProductAttributeTab = uow.ProductAttributeTabRepository.Get(x => x, x => x.Id == id).FirstOrDefault();
                    if (ProductAttributeTab == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributeTabs", "Delete", true, 200, " نمایش صفحه حذف مقدار خصوصیت " + ProductAttributeTab.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductAttributeTab);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تب" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeTabs", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.TabId == id).ToList().ForEach(s => s.TabId = null);
                uow.Save();

                ProductAttributeTab ProductAttributeTab = uow.ProductAttributeTabRepository.GetByID(id);
                uow.ProductAttributeTabRepository.Delete(ProductAttributeTab);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductAttributeTabs", "Delete", false, 200, "   حذف تب" + ProductAttributeTab.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeTabs", "Delete", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
