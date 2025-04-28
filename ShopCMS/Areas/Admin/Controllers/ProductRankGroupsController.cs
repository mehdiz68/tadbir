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
using ahmadi.Infrastructure.Helper;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class ProductRankGroupsController : Controller
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



                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductRankGroups", "Index", true, 200, " نمایش صفحه مدیریت آیتم های امتیازدهی", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(uow.ProductRankGroupRepository.Get(x => x, null, null, "ProductRankGroupSelects").ToList().OrderBy(x => x.DisplayOrder));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroups", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public virtual JsonResult SortGroup(string ids)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (Int16 i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var ProductRankGroup = uow.ProductRankGroupRepository.GetByID(id);
                    ProductRankGroup.DisplayOrder = i;
                    uow.Save();
                }
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "SortGroup", false, 200, "   مرتب سازی گروههای آیتم امتیازدهی", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "SortGroup", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual JsonResult Sort(string ids, int GroupId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (Int16 i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var ProductRankGroup = uow.ProductRankGroupSelectRepository.Get(x => x, x => x.GroupId == GroupId && x.Id == id).First();
                    ProductRankGroup.DisplayOrder = i;
                    uow.Save();
                }
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "Sort", false, 200, "   مرتب سازی مقادیر خصوصیت", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public virtual ActionResult SortGroup(int? page)
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

                    var ProductRankGroups = uow.ProductRankGroupRepository.Get(x => x, null, x => x.OrderBy(o => o.DisplayOrder));

                    int pageSize = 100;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 17 && x.Name == "مرتب سازی گروه", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductRankGroups", "SortGroup", true, 200, " نمایش صفحه مرتب سازی گروههای آیتم امتیازدهی", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductRankGroups.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroups", "SortGroup", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
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
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create([Bind(Include = "Id,Title,DisplayOrder,LanguageId")] ProductRankGroup ProductRankGroup)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    ProductRankGroup.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductRankGroup.Title);
                    uow.ProductRankGroupRepository.Insert(ProductRankGroup);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductRankGroup", "Create", false, 200, "   ایجاد گروه آیتم امتیاز دهی " + ProductRankGroup.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "Create", false, 500, "   خطا در ایجاد گروه آیتم امتیازدهی " + ProductRankGroup.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductRankGroup);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductRankGroup);
            }

        }

        public virtual ActionResult Edit(int? Id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                #region Check License


                #endregion

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {
                    var ProductRankGroup = uow.ProductRankGroupRepository.GetByID(Id.Value);
                    if (ProductRankGroup != null)
                    {
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductRankGroup", "Edit", true, 200, " نمایش صفحه ویرایش گروه آیتم امتیازدهی", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(ProductRankGroup);
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit([Bind(Include = "Id,Title,DisplayOrder,LanguageId")] ProductRankGroup ProductRankGroup)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    ProductRankGroup.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductRankGroup.Title);
                    uow.ProductRankGroupRepository.Update(ProductRankGroup);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductAttributes", "Edit", false, 200, "   ویرایش گروه آیتم امتیازدهی" + ProductRankGroup.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "Edit", false, 500, "   خطا در ویرایش گروه آیتم امتیازدهی" + ProductRankGroup.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductRankGroup);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductRankGroup);
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
                    ProductRankGroup ProductRankGroup = uow.ProductRankGroupRepository.Get(x => x, x => x.Id == id).FirstOrDefault();
                    if (ProductRankGroup == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductRankGroup", "Delete", true, 200, " نمایش صفحه حذف آیتم امتیازدهی " + ProductRankGroup.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductRankGroup);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گروه آیتم های امتیازدهی" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
                ProductRankGroup ProductRankGroup = uow.ProductRankGroupRepository.Get(x => x, x => x.Id == id, null, "ProductRankGroupSelects").FirstOrDefault();
                if (ProductRankGroup.ProductRankGroupSelects.Any())
                {
                    uow.ProductRankGroupSelectRepository.Delete(ProductRankGroup.ProductRankGroupSelects);
                    uow.Save();
                }
                uow.ProductRankGroupRepository.Delete(ProductRankGroup);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductRankGroup", "Delete", false, 200, "   حذف گروه آیتم امتیازدهی" + ProductRankGroup.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "Delete", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        #region Product Rank


        // GET: Admin/ProductAttributeGroups/Ranks
        public virtual ActionResult Ranks(int? Id)
        {
            if (Id.HasValue)
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

                        ViewBag.Group = uow.ProductRankGroupRepository.Get(x => x, x => x.Id == Id.Value, null, "ProductRankGroupSelects.ProductRankGroup,ProductRankGroupSelects.ProductRank").SingleOrDefault();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductRankGroups", "Ranks", true, 200, " نمایش صفحه مدیریت آیتم های امتیازدهی گروه" + Id.Value, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(uow.ProductRankGroupSelectRepository.Get(x => x, x => x.ProductRankGroup.Id == Id.Value).ToList().OrderBy(x => x.DisplayOrder));
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

                }
                catch (Exception x)
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroups", "Ranks", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index", "Error");
                }
            }
            else
                return RedirectToAction("Index", "ProductRankGroups");
        }

        public virtual ActionResult CreateRank(int? Id)
        {
            if (Id.HasValue)
            {
                uow = new UnitOfWork.UnitOfWorkClass();
                var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                try
                {

                    if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                    {
                        var group = uow.ProductRankGroupRepository.GetByID(Id.Value);
                        if (group != null)
                        {
                            ViewBag.group = group;
                            return View();
                        }
                        else
                            return RedirectToAction("Index", "ProductRankGroup");
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

                }
                catch (Exception x)
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "CreateRank", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index", "Error");
                }
            }
            else
                return RedirectToAction("Index", "ProductRankGroup");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateRank([Bind(Include = "Id,Name,DisplaySort,LanguageId")] ProductRank ProductRank, int GroupId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    ProductRank.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductRank.Name);
                    uow.ProductRankRepository.Insert(ProductRank);
                    uow.Save();
                    ProductRank.ProductRankGroupSelects = new List<ProductRankGroupSelect>();
                    ProductRank.ProductRankGroupSelects.Add(new ProductRankGroupSelect { RankId = ProductRank.Id, GroupId = GroupId, DisplayOrder = 0, DisplayGroupOrder = 0 });
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductRankGroup", "CreateRank", false, 200, "   ایجاد آیتم امتیاز دهی " + ProductRank.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Ranks", "ProductRankGroups", new { Id = GroupId });
                }

                ViewBag.group = uow.ProductRankGroupRepository.GetByID(GroupId);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "CreateRank", false, 500, "   خطا در ایجاد آیتم امتیازدهی " + ProductRank.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductRank);
            }
            catch (Exception x)
            {
                ViewBag.group = uow.ProductRankGroupRepository.GetByID(GroupId);
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "CreateRank", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductRank);
            }

        }

        public virtual ActionResult EditRank(int? Id, int GroupId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {
                    var ProductRank = uow.ProductRankRepository.GetByID(Id.Value);
                    if (ProductRank != null)
                    {
                        ViewBag.group = uow.ProductRankGroupRepository.GetByID(GroupId);
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductRankGroup", "EditRank", true, 200, " نمایش صفحه ویرایش آیتم امتیازدهی" + ProductRank.Name, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(ProductRank);
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "EditRank", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditRank([Bind(Include = "Id,Name,DisplaySort,LanguageId")] ProductRank ProductRank, int GroupId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    ProductRank.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductRank.Name);
                    uow.ProductRankRepository.Update(ProductRank);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductAttributes", "EditRank", false, 200, "   ویرایش آیتم امتیازدهی" + ProductRank.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Ranks", "ProductRankGroups", new { Id = GroupId });
                }


                ViewBag.group = uow.ProductRankGroupRepository.GetByID(GroupId);
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "EditRank", false, 500, "   خطا در ویرایش آیتم امتیازدهی" + ProductRank.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductRank);
            }
            catch (Exception x)
            {
                ViewBag.group = uow.ProductRankGroupRepository.GetByID(GroupId);
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "EditRank", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductRank);
            }

        }


        public virtual ActionResult DeleteRank(int? id, int GroupId)
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
                    ProductRank ProductRank = uow.ProductRankRepository.Get(x => x, x => x.Id == id).FirstOrDefault();
                    if (ProductRank == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.group = uow.ProductRankGroupRepository.GetByID(GroupId);
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductRankGroup", "DeleteRank", true, 200, " نمایش صفحه حذف آیتم امتیازدهی " + ProductRank.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductRank);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "DeleteRank", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteRank(int id, int GroupId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                ProductRank ProductRank = uow.ProductRankRepository.GetByID(id);
                uow.ProductRankSelectValueRepository.Delete(uow.ProductRankSelectValueRepository.Get(x => x, x => x.ProductRankSelect.ProductRankGroupSelect.RankId == id));
                uow.ProductRankSelectRepository.Delete(uow.ProductRankSelectRepository.Get(x => x, x => x.ProductRankGroupSelect.RankId == id));
                uow.ProductRankRepository.Delete(ProductRank);
                uow.Save();

                ViewBag.group = uow.ProductRankGroupRepository.GetByID(GroupId);
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductRankGroup", "DeleteRank", false, 200, "   حذف آیتم امتیازدهی" + ProductRank.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Ranks", "ProductRankGroups", new { Id = GroupId });
            }
            catch (Exception x)
            {
                ViewBag.group = uow.ProductRankGroupRepository.GetByID(GroupId);
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductRankGroup", "DeleteRank", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }

}
