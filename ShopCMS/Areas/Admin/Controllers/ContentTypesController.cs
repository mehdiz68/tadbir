using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using System;
using System.Collections.Generic;
using CoreLib.ViewModel.Xml;
using UnitOfWork;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperUser,Admin,Support")]
    public partial class ContentTypesController : Controller
    {
        private UnitOfWorkClass uow;

        // GET: Admin/ContentTypes
        public virtual ActionResult Index()
        {
            uow = new UnitOfWorkClass();
            #region Get Language
            short langId = 0;
            if (Session["langId"] == null)
            {
                langId = uow.SettingRepository.Get(x => x, null, null, "attachment,Faviconattachment").FirstOrDefault().LanguageId.Value;
                Session["langId"] = langId;
            }
            else
                langId = Convert.ToInt16(Session["langId"]);

            #endregion
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == langId, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                
                    var p = ModulePermission.check(User.Identity.GetUserId(), 20);
                    if (p != null)
                    {
                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();

                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        var data = readXML.ListOfXContentType();


                        IdentityManager im = new IdentityManager();
                        if (im.IsInRole(User.Identity.GetUserId(), "SuperUser"))
                            ViewBag.SuperUserPermission = true;


                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(1, "ContentTypes", "Index", true, 200, " نمایش صفحه انواع محتوا", DateTime.Now, User.Identity.GetUserId());
                        #endregion;
                        return View(data.ToList());


                        //List<XContentType> xcontentTypes = new List<XContentType>();
                        //foreach (var item in contentTypes)
                        //    xcontentTypes.Add(new XContentType(item.Id, item.Name, item.Title, item.SettingId, item.Abstract));

                        //XDocument oxdContentTypes = new XDocument(new XDeclaration("1.0", "UTF - 8", "yes"),
                        //new XElement("ContentTypes",
                        //from x in xcontentTypes
                        //select new XElement("ContentType",
                        //new XElement("Id", x.Id),
                        //new XElement("Name", x.Name),
                        //new XElement("Tite", x.Title),
                        //new XElement("SettingId", x.SettingId),
                        //new XElement("Abstract", x.Abstract))));
                        //oxdContentTypes.Save(Server.MapPath("~/Content/Xml/Theme.xml"));

                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت انواع محتوا" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ContentTypes", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/ContentTypes/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWorkClass();
            #region Get Language
            short langId = 0;
            if (Session["langId"] == null)
            {
                langId = uow.SettingRepository.Get(x => x, null, null, "attachment,Faviconattachment").FirstOrDefault().LanguageId.Value;
                Session["langId"] = langId;
            }
            else
                langId = Convert.ToInt16(Session["langId"]);

            #endregion
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == langId, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                IdentityManager im = new IdentityManager();
                if (!im.IsInRole(User.Identity.GetUserId(), "SuperUser"))
                    return RedirectToAction("Index");
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 20, 1))
                    {
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        var Languages = readXML.ListOfXLanguage();
                        ViewBag.LanguageId = new SelectList(Languages, "Id", "Name");

                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(1, "ContentTypes", "Create", true, 200, " نمایش صفحه ایجاد نوع محتوای", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت انواع محتوا" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ContentTypes", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ContentTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create([Bind(Include = "Id,Name,Title,LanguageId,Abstract,InSearch,ShortName,IsVideo,Cover")] XContentType XContentType)
        {
            uow = new UnitOfWorkClass();
            #region Get Language
            short langId = 0;
            if (Session["langId"] == null)
            {
                langId = uow.SettingRepository.Get(x => x, null, null, "attachment,Faviconattachment").FirstOrDefault().LanguageId.Value;
                Session["langId"] = langId;
            }
            else
                langId = Convert.ToInt16(Session["langId"]);

            #endregion
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == langId, null, "attachment,Faviconattachment").SingleOrDefault();
            XMLReader readXML = new XMLReader(setting.StaticContentDomain);
            try
            {
                var Languages = readXML.ListOfXLanguage();
                ViewBag.LanguageId = new SelectList(Languages, "Id", "Name", XContentType.LanguageId);

                if (ModelState.IsValid)
                {
                    XContentType.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(XContentType.Name);
                    XContentType.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(XContentType.Title);
                    if (!String.IsNullOrEmpty(XContentType.Cover))
                    {
                        Guid id = new Guid(XContentType.Cover);
                        XContentType.Cover = uow.AttachmentRepository.Get(x => x, x => x.Id == id).First().FileName;

                    }
                    if (readXML.ListOfXContentType().Any(x => x.Id == XContentType.Id))
                    {
                        ViewBag.Error = "آیدی وارد شده تکراری است.";
                        return View();
                    }

                    if (readXML.CreateOfXContentType(XContentType))
                    {
                        SearchEngineElementType ste = new SearchEngineElementType();
                        ste.Description = XContentType.Abstract;
                        ste.LanguageId = Convert.ToInt16(XContentType.LanguageId);
                        ste.Priority = 0;
                        ste.SearchEngineElementTypeID = XContentType.Id;
                        ste.Title = XContentType.Name;
                        uow.SearchEngineElementTypeRepository.Insert(ste);
                        uow.Save();

                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(2, "ContentTypes", "Create", false, 200, " ایجاد نوع محتوایِ " + XContentType.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = "خطایی رخ داد.";
                        return View();
                    }
                }


                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ContentTypes", "Create", false, 500, " خطا در ایجاد نوع محتوایِ " + XContentType.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(XContentType);

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ContentTypes", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/ContentTypes/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {
              
                    #region Get Language
                    short langId = 0;
                    if (Session["langId"] == null)
                    {
                        langId = uow.SettingRepository.Get(x => x, null, null, "attachment,Faviconattachment").FirstOrDefault().LanguageId.Value;
                        Session["langId"] = langId;
                    }
                    else
                        langId = Convert.ToInt16(Session["langId"]);

                    #endregion
                    var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == langId, null, "attachment,Faviconattachment").SingleOrDefault();
                    if (ModulePermission.check(User.Identity.GetUserId(), 20, 2))
                    {
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        XContentType oContentType = readXML.DetailOfXContentType(id.Value);
                        if (oContentType == null)
                        {
                            return HttpNotFound();
                        }

                        var Languages = readXML.ListOfXLanguage();
                        ViewBag.LanguageId = new SelectList(Languages, "Id", "Name", oContentType.LanguageId);

                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(1, "ContentTypes", "Edit", true, 200, " نمایشِ صفحه ویرایشِ نوع محتوای " + oContentType.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(oContentType);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت انواع محتوا" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ContentTypes", "Edit", false, 200, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ContentTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit([Bind(Include = "Id,Name,Title,LanguageId,Abstract,InSearch,ShortName,IsVideo,Cover")] XContentType XContentType)
        {
            uow = new UnitOfWorkClass();
            #region Get Language
            short langId = 0;
            if (Session["langId"] == null)
            {
                langId = uow.SettingRepository.Get(x => x, null, null, "attachment,Faviconattachment").FirstOrDefault().LanguageId.Value;
                Session["langId"] = langId;
            }
            else
                langId = Convert.ToInt16(Session["langId"]);

            #endregion
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == langId, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var Languages = readXML.ListOfXLanguage();
                ViewBag.LanguageId = new SelectList(Languages, "Id", "Name", XContentType.LanguageId);

                if (ModelState.IsValid)
                {
                    XContentType.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(XContentType.Name);
                    XContentType.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(XContentType.Title);
                    if (!String.IsNullOrEmpty(XContentType.Cover))
                    {
                        Guid id = new Guid(XContentType.Cover);
                        XContentType.Cover = uow.AttachmentRepository.Get(x => x, x => x.Id == id).First().FileName;

                    }
                    string msg = "";
                    if (readXML.EditXContentType(XContentType,out msg))
                    {
                        SearchEngineElementType ste = uow.SearchEngineElementTypeRepository.GetByID(XContentType.Id);
                        if (ste != null)
                        {
                            ste.Description = XContentType.Abstract;
                            ste.LanguageId = Convert.ToInt16(XContentType.LanguageId);
                            ste.Title = XContentType.Name;
                            uow.Save();
                        }

                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(3, "ContentTypes", "Edit", false, 200, " ویرایش نوع محتوای" + XContentType.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = "خطایی رخ داد."+msg;
                        return View(XContentType);
                    }

                }

                return View(XContentType);

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ContentTypes", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/ContentTypes/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWorkClass();
            #region Get Language
            short langId = 0;
            if (Session["langId"] == null)
            {
                langId = uow.SettingRepository.Get(x => x, null, null, "attachment,Faviconattachment").FirstOrDefault().LanguageId.Value;
                Session["langId"] = langId;
            }
            else
                langId = Convert.ToInt16(Session["langId"]);

            #endregion
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == langId, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                IdentityManager im = new IdentityManager();
                if (!im.IsInRole(User.Identity.GetUserId(), "SuperUser"))
                    return RedirectToAction("Index");
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 20, 3))
                    {
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        XContentType oContentType = readXML.DetailOfXContentType(id.Value);
                        if (oContentType == null)
                        {
                            return HttpNotFound();
                        }

                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(1, "ContentTypes", "Delete", true, 200, " نمایشِ صفحه حذف نوع محتوای " + oContentType.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(oContentType);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت انواع محتوا" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ContentTypes", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ContentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWorkClass();
            #region Get Language
            short langId = 0;
            if (Session["langId"] == null)
            {
                langId = uow.SettingRepository.Get(x => x, null, null, "attachment,Faviconattachment").FirstOrDefault().LanguageId.Value;
                Session["langId"] = langId;
            }
            else
                langId = Convert.ToInt16(Session["langId"]);

            #endregion
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == langId, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                if (id <= 3)
                {
                    ViewBag.Erorr = "مجاز به حذف این رکورد نمی باشید.";
                    return View(readXML.DetailOfXContentType(id));
                }
                if (readXML.RemoveXContentType(id))
                {
                    SearchEngineElementType ste = uow.SearchEngineElementTypeRepository.GetByID(id);
                    if (ste != null)
                    {
                        uow.SearchEngineElementTypeRepository.Delete(ste);
                        uow.Save();
                    }

                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(4, "ContentTypes", "DeleteConfirmed", false, 200, " حذف نوع محتوای" + id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "خطایی رخ داد.";
                    return View();
                }


            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ContentTypes", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
