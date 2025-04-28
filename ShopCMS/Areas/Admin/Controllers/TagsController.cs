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
    public partial class TagsController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;

        // GET: Admin/Tags
        [CorrectArabianLetter(new string[] { "TagnameString", "TagnameFilter" })]
        public virtual ActionResult Index(string sortOrder, string TagnameFilter, string TagnameString, string LanguagenameFilter, string LanguagenameString, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
              
                    var p = ModulePermission.check(User.Identity.GetUserId(), 2);
                    if (p.Where(x => x == true).Any())
                    {
                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();

                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                        #region search
                        if (string.IsNullOrEmpty(TagnameString))
                            TagnameString = TagnameFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(LanguagenameString))
                            LanguagenameString = LanguagenameFilter;
                        else
                            page = 1;

                        ViewBag.TagnameFilter = TagnameString;
                        ViewBag.LanguagenameFilter = LanguagenameString;

                        var tags = uow.TagRepository.GetByReturnQueryable(x=>x);
                        if (!String.IsNullOrEmpty(TagnameString))
                            tags = tags.Where(s => s.TagName.Contains(TagnameString));
                        if (!String.IsNullOrEmpty(LanguagenameString))
                        {
                            int langId = Convert.ToInt32(LanguagenameString);
                            tags = tags.Where(s => s.LanguageId == langId);
                        }

                        #endregion

                        #region Sort
                        switch (sortOrder)
                        {
                            case "TagName":
                                tags = tags.OrderBy(s => s.TagName);
                                ViewBag.CurrentSort = "TagName";
                                break;
                            case "TagName_desc":
                                tags = tags.OrderByDescending(s => s.TagName);
                                ViewBag.CurrentSort = "TagName_desc";
                                break;
                            case "Languagename":
                                tags = tags.OrderBy(s => s.LanguageId);
                                ViewBag.CurrentSort = "Languagename";
                                break;
                            case "Languagename_desc":
                                tags = tags.OrderByDescending(s => s.LanguageId);
                                ViewBag.CurrentSort = "Languagename_desc";
                                break;
                            default:  // Name ascending 
                                tags = tags.OrderByDescending(s => s.Id);
                                break;
                        }

                        #endregion

                        int pageSize = 8;
                        int pageNumber = (page ?? 1);


                        ViewBag.HelpModule = uow.HelpModuleRepository.Get(x=>x,x => x.Name == "برچسب ها",null, "HelpModuleSections").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Tags", "Index", true, 200, " نمایش صفحه مدیریت برچسب ها", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(tags.ToPagedList(pageNumber, pageSize));
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت برچسب ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tags", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Tags/Details/5
        public virtual ActionResult Details(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 2).Where(x => x == true).Any())
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        Tag tag = uow.TagRepository.Get(x=>x,x=>x.Id==id,null, "Content").FirstOrDefault();
                        if (tag == null)
                        {
                            return HttpNotFound();
                        }
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Tags", "Details", true, 200, " نمایش صفحه جزئیات برچسب" + tag.TagName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(tag);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت برچسب ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tags", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Tags/Create
        public virtual ActionResult Create()
        {
            UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
              
                    if (ModulePermission.check(User.Identity.GetUserId(), 2, 1))
                    {
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Tags", "Create", true, 200, " نمایش صفحه ایجاد برچسب", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت برچسب ها" }));
              
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tags", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Tags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(Tag tag)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    tag.TagName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(tag.TagName);
                    tag.TagName = tag.TagName.Trim();
                    uow.TagRepository.Insert(tag);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Tags", "Create", false, 200, "   ایجاد برچسبِ " + tag.TagName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", tag.LanguageId);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tags", "Create", false, 500, "   خطا در ایجاد برچسبِ " + tag.TagName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(tag);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tags", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", tag.LanguageId);
                ViewBag.Error = "تگ وارد شده تکراری است.";
                return View(tag);
            }

        }

        // GET: Admin/Tags/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 2, 2))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        Tag tag = uow.TagRepository.GetByID(id);
                        if (tag == null)
                        {
                            return HttpNotFound();
                        }


                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", tag.LanguageId);

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Tags", "Edit", true, 200, " نمایش صفحه ویرایش برچسبِ " + tag.TagName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(tag);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت برچسب ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tags", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Tags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(Tag tag)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            XMLReader readXML = new XMLReader(setting.StaticContentDomain);
            ViewBag.setting = setting;
            ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", tag.LanguageId);
            try
            {
                if (ModelState.IsValid)
                {
                    if (!uow.TagRepository.Get(x=>x,x => x.Id!=tag.Id && x.TagName == tag.TagName.Trim(),null).Any())
                    {
                        tag.TagName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(tag.TagName);
                        tag.TagName = tag.TagName.Trim();
                        uow.TagRepository.Update(tag);
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(3, "Tags", "Edit", false, 200, "   ویرایش برچسبِ " + tag.TagName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
                    else
                    {

                        ViewBag.Error = "تگ وارد شده تکراری است.";
                    }
                }


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tags", "Edit", false, 500, "   خطا در ویرایش برچسبِ " + tag.TagName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(tag);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tags", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Tags/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                
                    if (ModulePermission.check(User.Identity.GetUserId(), 2, 3))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        Tag tag = uow.TagRepository.Get(x=>x,x=>x.Id==id,null, "Content").FirstOrDefault();
                        if (tag == null)
                        {
                            return HttpNotFound();
                        }

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Tags", "Delete", true, 200, " نمایش صفحه حذف برچسبِ " + tag.TagName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(tag);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت برچسب ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tags", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                Tag tag = uow.TagRepository.GetByID(id);
                uow.TagRepository.Delete(tag);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Tags", "DeleteConfirmed", false, 200, "   حذف برچسبِ " + tag.TagName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tags", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
