using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using System.Collections.Generic;
using CoreLib.ViewModel.Xml;
using ahmadi.Infrastructure.Security;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
   [Authorize(Roles = "Admin,Security,SuperUser")]
    public partial class FoldersController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;

        // GET: Admin/Folders
        public virtual ActionResult Index(string LanguageId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
               
                    var p = ModulePermission.check(User.Identity.GetUserId(), 1);
                    if (p.Where(x => x == true).Any())
                    {
                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();

                        var folders = uow.FolderRepository.Get(f=>f,null,null,"ParrentFolder");

                        #region Tree 

                        if (!String.IsNullOrEmpty(LanguageId))
                        {
                            ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x=>x), "LanguageId", "SettingName", LanguageId);
                            int langId = Convert.ToInt32(LanguageId);
                            var Folders = uow.FolderRepository.Get(c=>c,c => c.FolderID == null && c.LanguageId == langId, x=>x.OrderByDescending(o => o.Id), "attachments,ParrentFolder");
                            ViewBag.Folders = Folders.ToList();
                        }
                        else
                        {
                            ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x=>x), "LanguageId", "SettingName");
                            var Folders = uow.FolderRepository.Get(c=>c,c => c.FolderID == null, c=>c.OrderByDescending(x => x.Id),"attachments,ParrentFolder");
                            ViewBag.Folders = Folders.ToList();
                        }

                        #endregion

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Folders", "Index", true, 200, " نمایش صفحه مدیریت پوشه های فایل ها", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x=>x,x => x.ModuleId == 1 && x.Name == "مدیریت پوشه ها",null, "HelpModuleSectionFields").FirstOrDefault();
                        return View(folders.ToList());
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));
              
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Folders", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Folders/Details/5
        public virtual ActionResult Details(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
              
                    if (ModulePermission.check(User.Identity.GetUserId(), 1, null))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        Folder folder = uow.FolderRepository.GetByID(id);
                        if (folder == null)
                        {
                            return HttpNotFound();
                        }

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Folders", "Details", true, 200, " نمایش صفحه جزئیات پوشه ی" + folder.FolderName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(folder);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Folders", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Folders/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 1, 1))
                    {
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                        ViewBag.FolderID = new SelectList(uow.FolderRepository.Get(x=>x), "Id", "FolderName");

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Folders", "Create", true, 200, " نمایش صفحه ایجاد پوشه", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Folders", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Folders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create([Bind(Include = "Id,FolderName,FolderID,LanguageId")] Folder folder)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                if (!uow.FolderRepository.Get(x=>x,x => x.FolderName.ToLower() == folder.FolderName.ToLower()).Any())
                {
                    if (ModelState.IsValid)
                    {
                        folder.FolderName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(folder.FolderName);
                        uow.FolderRepository.Insert(folder);
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "Folders", "Create", false, 200, "   ایجاد پوشه ی" + folder.FolderName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.Error = " نام وارد شده وجود دارد ";
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", folder.LanguageId);
                    ViewBag.FolderID = new SelectList(uow.FolderRepository.Get(x=>x), "Id", "FolderName", folder.FolderID);

                }

                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", folder.LanguageId);
                ViewBag.FolderID = new SelectList(uow.FolderRepository.Get(x => x), "Id", "FolderName", folder.FolderID);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "Folders", "Create", false, 500, "   خطا در ایجاد پوشه ی" + folder.FolderName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(folder);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Folders", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Folders/Create
        public virtual ActionResult SubCreate(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 1, 1))
                    {
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                        ViewBag.FolderID = id;

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Folders", "SubCreate", true, 200, " نمایش صفحه ایجاد پوشه", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(uow.FolderRepository.GetByID(id.Value));
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Folders", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Folders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SubCreate([Bind(Include = "FolderName,FolderID,LanguageId")] Folder folder)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                if (!uow.FolderRepository.Get(x=>x,x => x.FolderName.ToLower() == folder.FolderName.ToLower()).Any())
                {
                    if (ModelState.IsValid)
                    {
                        folder.FolderName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(folder.FolderName);
                        uow.FolderRepository.Insert(folder);
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "Folders", "SubCreate", false, 200, "   ایجاد پوشه ی" + folder.FolderName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.Error = " نام وارد شده وجود دارد ";
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", folder.LanguageId);
                    ViewBag.FolderID = new SelectList(uow.FolderRepository.Get(x=>x), "Id", "FolderName", folder.FolderID);

                }
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", folder.LanguageId);
                ViewBag.FolderID = new SelectList(uow.FolderRepository.Get(x=>x), "Id", "FolderName", folder.FolderID);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "Folders", "Create", false, 500, "   خطا در ایجاد پوشه ی" + folder.FolderName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(folder);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Folders", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Folders/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 1, 2))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        Folder folder = uow.FolderRepository.GetByID(id);
                        if (folder == null)
                        {
                            return HttpNotFound();
                        }
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", folder.LanguageId);
                        ViewBag.FolderID = new SelectList(uow.FolderRepository.Get(x=>x), "Id", "FolderName", folder.FolderID);

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Folders", "Edit", true, 200, " نمایش صفحه ویرایش پوشه ی" + folder.FolderName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(folder);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Folders", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Folders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit([Bind(Include = "Id,FolderName,FolderID,LanguageId")] Folder folder)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                if (!uow.FolderRepository.Get(x=>x,x =>x.Id!=folder.Id && x.FolderName.ToLower() == folder.FolderName.ToLower()).Any())
                {
                    if (ModelState.IsValid)
                    {
                        folder.FolderName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(folder.FolderName);
                        uow.FolderRepository.Update(folder);
                        uow.Save();
                        return RedirectToAction("Index");
                    }
                }
                else
                {

                    ViewBag.Error = " نام وارد شده وجود دارد ";
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", folder.LanguageId);
                    ViewBag.FolderID = new SelectList(uow.FolderRepository.Get(x=>x), "Id", "FolderName", folder.FolderID);
                }
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", folder.LanguageId);
                ViewBag.FolderID = new SelectList(uow.FolderRepository.Get(x=>x), "Id", "FolderName", folder.FolderID);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Folders", "Edit", false, 200, "   ویرایش پوشه ی" + folder.FolderName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(folder);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Folders", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Folders/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 1, 3))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        Folder folder = uow.FolderRepository.GetByID(id);
                        if (folder == null)
                        {
                            return HttpNotFound();
                        }

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Folders", "Delete", true, 200, " نمایش صفحه حذفِ پوشه ی" + folder.FolderName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(folder);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Folders", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Folders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                Folder folder = uow.FolderRepository.Get(x=>x,x=>x.Id==id,null, "ChildFolder").FirstOrDefault();
                if (!folder.ChildFolder.Any())
                {
                    uow.FolderRepository.Delete(folder);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(4, "Folders", "DeleteConfirmed", false, 200, "   حذف پوشه ی" + folder.FolderName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Erorr = " این  پوشه دارای زیر پوشه است. ابتدا همه زیرپوشه های این پوشه را حذف نمایید. ";
                    return View(folder);
                }
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Folders", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
