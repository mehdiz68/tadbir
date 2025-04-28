using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using PagedList;
using CoreLib.Infrastructure.DateTime;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using CoreLib.Infrastructure;
using CoreLib.ViewModel.Xml;
using CoreLib.Infrastructure.ModelBinder;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class CommentsController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;

        // GET: Admin/Comments
        [CorrectArabianLetter(new string[] { "FullNameFilter", "FullNameString" })]
        public virtual ActionResult Index(string sortOrder, string FullNameString, string FullNameFilter, string LanguagenameString, string LanguagenameFilter, string InsertDateStart, string InsertDateStartFilter, string InsertDateEnd, string InsertDateEndFilter, string IsActive, string IsActiveFilter, string IsVisit, string IsVisitFilter, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                #region Check License
                
                
                #endregion

                    var p = ModulePermission.check(User.Identity.GetUserId(), 17);
                    if (p.Where(x => x == true).Any())
                    {
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                        List<SelectListItem> IsDeleteSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "دیده شده ها", Value = "True" }, new SelectListItem() { Text = "دیده نشده ها", Value = "False" } };
                        ViewBag.IsVisit = IsDeleteSelectListItem;

                        List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "True" }, new SelectListItem() { Text = "غیرفعال", Value = "False" } };
                        ViewBag.IsActive = IsActiveSelectListItem;

                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();

                        #region search
                        if (string.IsNullOrEmpty(FullNameString))
                            FullNameString = FullNameFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(LanguagenameString))
                            LanguagenameString = LanguagenameFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(InsertDateStart))
                            InsertDateStart = InsertDateStartFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(InsertDateEnd))
                            InsertDateEnd = InsertDateEndFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(IsActive))
                            IsActive = IsActiveFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(IsVisit))
                            IsVisit = IsVisitFilter;
                        else
                            page = 1;

                        ViewBag.FullNameFilter = FullNameString;
                        ViewBag.LanguagenameFilter = LanguagenameString;
                        ViewBag.InsertDateStartFilter = InsertDateStart;
                        ViewBag.InsertDateEndFilter = InsertDateEnd;
                        ViewBag.IsActiveFilter = IsActive;
                        ViewBag.IsVisitFilter = IsVisit;

                        var comments = uow.CommentRepository.GetByReturnQueryable(x=>x,null,null, "Content,ParentComment");

                        if (!String.IsNullOrEmpty(FullNameString))
                            comments = comments.Where(s => s.FullName.Contains(FullNameString));
                        if (!String.IsNullOrEmpty(LanguagenameString))
                        {
                            int langId = Convert.ToInt32(LanguagenameString);
                            comments = comments.Where(s => s.Content.LanguageId == langId);
                        }
                        if (!String.IsNullOrEmpty(IsActive))
                        {
                            bool isactive = Convert.ToBoolean(IsActive);
                            comments = comments.Where(s => s.IsActive == isactive);
                        }
                        if (!String.IsNullOrEmpty(IsVisit))
                        {
                            bool isvisit = Convert.ToBoolean(IsVisit);
                            comments = comments.Where(s => s.Visited == isvisit);
                        }

                        DateTime dtInsertDateStart = DateTime.Now.Date, dtInsertDateEnd = DateTime.Now.Date;
                        if (!String.IsNullOrEmpty(InsertDateStart))
                            dtInsertDateStart = DateTimeConverter.ChangeShamsiToMiladi(InsertDateStart);
                        if (!String.IsNullOrEmpty(InsertDateEnd))
                            dtInsertDateEnd = DateTimeConverter.ChangeShamsiToMiladi(InsertDateEnd);


                        if (!String.IsNullOrEmpty(InsertDateStart) && !String.IsNullOrEmpty(InsertDateEnd))
                            comments = comments.Where(s => s.InsertDate >= dtInsertDateStart && s.InsertDate <= dtInsertDateEnd);
                        else if (!String.IsNullOrEmpty(InsertDateStart))
                            comments = comments.Where(s => s.InsertDate >= dtInsertDateStart);
                        else if (!String.IsNullOrEmpty(InsertDateEnd))
                            comments = comments.Where(s => s.InsertDate <= dtInsertDateEnd);

                        #endregion

                        #region Sort
                        switch (sortOrder)
                        {
                            case "FullName":
                                comments = comments.OrderBy(s => s.FullName);
                                ViewBag.CurrentSort = "FullName";
                                break;
                            case "FullName_desc":
                                comments = comments.OrderByDescending(s => s.FullName);
                                ViewBag.CurrentSort = "FullName_desc";
                                break;
                            case "IsVisit":
                                comments = comments.OrderBy(s => s.Visited);
                                ViewBag.CurrentSort = "IsVisit";
                                break;
                            case "IsVisit_desc":
                                comments = comments.OrderByDescending(s => s.Visited);
                                ViewBag.CurrentSort = "IsVisit_desc";
                                break;
                            case "IsActive":
                                comments = comments.OrderBy(s => s.IsActive);
                                ViewBag.CurrentSort = "IsActive";
                                break;
                            case "IsActive_desc":
                                comments = comments.OrderByDescending(s => s.IsActive);
                                ViewBag.CurrentSort = "IsActive_desc";
                                break;
                            case "InsertDate":
                                comments = comments.OrderBy(s => s.InsertDate);
                                ViewBag.CurrentSort = "InsertDate";
                                break;
                            case "InsertDate_desc":
                                comments = comments.OrderByDescending(s => s.InsertDate);
                                ViewBag.CurrentSort = "InsertDate_desc";
                                break;
                            default:  // Name ascending 
                                comments = comments.OrderBy(x => x.Visited).ThenBy(x => x.Id);
                                break;
                        }

                        #endregion

                        int pageSize = 8;
                        int pageNumber = (page ?? 1);


                        ViewBag.HelpModule = uow.HelpModuleRepository.Get(x=>x,x => x.Name == "مدیریت نظرات ",null, "HelpModuleSections").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Comments", "Index", true, 200, " نمایش صفحه نظرات", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(comments.ToPagedList(pageNumber, pageSize));
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Comments", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Comments/Details/5
        public virtual ActionResult Details(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                #region Check License
                
                
                #endregion

             
                    var p = ModulePermission.check(User.Identity.GetUserId(), 17);
                    if (p.Where(x => x == true).Any())
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        Comment comment = uow.CommentRepository.Get(x=>x,x=>x.Id==id,null, "Content.User").SingleOrDefault();
                        if (comment == null)
                        {
                            return HttpNotFound();
                        }
                        if (User.Identity.GetUserId() == comment.Content.User.UserName)
                            comment.IsAuthorSeen = true;
                        comment.Visited = true;
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Comments", "Details", true, 200, " نمایش جزئیات نظرِ " + comment.FullName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(comment);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Comments", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Comments/Active/5
        [HttpPost]
        public virtual async Task<JsonResult> Active(string id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                int Id = Convert.ToInt32(id);
                Comment oComment = uow.CommentRepository.Get(x=>x,x=>x.Id==Id,null, "Content").Single();
                oComment.IsActive = true;
                uow.Save();


                #region Create Html Body
                string EmailBodyHtml = "";

                var oSetting = uow.SettingRepository.Get(x=>x,x => x.LanguageId == 1,null, "attachment").SingleOrDefault();
                string contentUrl = "http://" + HttpContext.Request.Url.Host + "/content/" + oComment.ContentId + "/" + CommonFunctions.NormalizeAddress(oComment.Content.Title);
                CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید نظر شما", "کاربر گرامی ، " + oComment.FullName + "، نظر شما تایید شد.لینک صفحه : <br/> <a href='"+ contentUrl + "'>"+oComment.Content.Title+"</a>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                #endregion

                #region SendMail
                EmailService es = new EmailService();
                IdentityMessage imessage = new IdentityMessage();
                imessage.Body = EmailBodyHtml;
                imessage.Destination = oComment.Email;
                imessage.Subject = " تایید نظر شما ";
                await es.SendAsync(imessage);

                #endregion

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Comments", "Active", false, 200, " فعال سازی نظرِ " + oComment.FullName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 200,
                    Message = "فعال شد"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Comments", "Active", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500,
                    Message = x.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Admin/Comments/DeActive/5
        [HttpPost]
        public virtual JsonResult DeActive(string id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                int Id = Convert.ToInt32(id);
                Comment oComment = uow.CommentRepository.GetByID(Id);
                oComment.IsActive = false;
                uow.Save();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Comments", "DeActive", false, 200, " غیرفعال سازی نظرِ " + oComment.FullName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 200,
                    Message = "غیر فعال شد"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Comments", "DeActive", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500,
                    Message = x.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual async Task<ActionResult> Replay(string ReplayMessage, int Id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            Comment comment = uow.CommentRepository.Get(x => x, x => x.Id == Id, null, "Content").Single();
            if (comment == null)
            {
                return HttpNotFound();
            }
            try
            {
                #region Add Comment
                IdentityManager im = new IdentityManager();
                ApplicationUser au = im.GetUser(User.Identity.GetUserId());

                Comment oComment = new Comment();
                oComment.Content = comment.Content;
                oComment.ContentId = comment.ContentId;
                oComment.Email = (!String.IsNullOrEmpty(au.Email)? au.Email:"test@test.com");
                oComment.FullName = au.FirstName+" " +au.LastName;
                oComment.InsertDate = DateTime.Now;
                oComment.IsActive = true;
                oComment.Message = ReplayMessage;
                oComment.NegativeRating = 0;
                oComment.ParentComment = comment;
                oComment.ParrentId = comment.Id;
                oComment.PositiveRating = 0;
                oComment.Visited = true;
                uow.CommentRepository.Insert(oComment);
                uow.Save();
                #endregion

                #region Edit Parent Comment
                ViewBag.message = "پاسخ شما ثبت شد و در سایت نمایش پیدا کرد.";
                if (comment.IsActive == false)
                {
                    comment.IsActive = true;
                    uow.Save();
                }
                #endregion

                #region Create Html Body
                string EmailBodyHtml = "";

                var oSetting = uow.SettingRepository.Get(x=>x,x => x.LanguageId == 1,null, "attachment").SingleOrDefault();
                string contentUrl = "http://" + HttpContext.Request.Url.Host + "/content/" + oComment.ContentId + "/" + CommonFunctions.NormalizeAddress(oComment.Content.Title);
                CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید و پاسخ به نظر شما", "کاربر گرامی ، " + comment.FullName + "، توسط مدیر سایت برای نظر شما پاسخی ثبت شد :<br/><div>" + ReplayMessage + "</div><br/> لینک صفحه :  <a href='" + contentUrl + "'>" + oComment.Content.Title + "</a> ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                #endregion

                #region SendMail
                EmailService es = new EmailService();
                IdentityMessage imessage = new IdentityMessage();
                imessage.Body = EmailBodyHtml;
                imessage.Destination = comment.Email;
                imessage.Subject = " تایید و پاسخ به نظر شما ";
                await es.SendAsync(imessage);

                #endregion

                ViewBag.ContentId = new SelectList(uow.ContentRepository.Get(x=>x), "Id", "Title", comment.ContentId);
                ViewBag.ParrentId = new SelectList(uow.CommentRepository.Get(x=>x), "Id", "Email", comment.ParrentId);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Comments", "Replay", false, 200, " پاسخ دهی به نظرِ " + comment.FullName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View("Details", comment);
            }
            catch (Exception x)
            {
                ViewBag.message = x.Message;
                ViewBag.ContentId = new SelectList(uow.ContentRepository.Get(sx => sx), "Id", "Title", comment.ContentId);
                ViewBag.ParrentId = new SelectList(uow.CommentRepository.Get(sx => sx), "Id", "Email", comment.ParrentId);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Comments", "Replay", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View("Details", comment);
            }
        }

        //GET: Admin/Comments/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                #region Check License
                
                
                #endregion

               
                    if (ModulePermission.check(User.Identity.GetUserId(), 17, 2))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        Comment comment = uow.CommentRepository.Get(x=>x,x=>x.Id==id, null, "Content.User").SingleOrDefault();
                        ViewBag.InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(comment.InsertDate) + " " + DateTimeConverter.ChangeMiladiToShamsiTime(comment.InsertDate);
                        if (comment == null)
                        {
                            return HttpNotFound();
                        }
                        if (User.Identity.GetUserId() == comment.Content.User.UserName)
                            comment.IsAuthorSeen = true;
                        comment.Visited = true;
                        uow.Save();
                        ViewBag.ContentId = new SelectList(uow.ContentRepository.Get(x=>x), "Id", "Title", comment.ContentId);
                        ViewBag.ParrentId = new SelectList(uow.CommentRepository.Get(x=>x), "Id", "Email", comment.ParrentId);

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Comments", "Edit", true, 200, " نمایش صفحه ویرایشِ نظرِ " + comment.FullName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(comment);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Comments", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        //POST: Admin/Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Edit([Bind(Include = "Id,Message,Email,ContentId,FullName,Visited,IsActive")] Comment comment, string OldIsActive,string InsertDate)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    comment.InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladiDateTime(InsertDate);
                    uow.CommentRepository.Update(comment);
                    uow.Save();
                    if (Convert.ToBoolean(OldIsActive) == false && comment.IsActive == true)
                    {
                        #region Create Html Body
                        string EmailBodyHtml = "";

                        string contentTitle = uow.ContentRepository.Get(x=>x,x => x.Id == comment.ContentId).SingleOrDefault().Title;
                        string contentUrl = "http://" + HttpContext.Request.Url.Host + "/content/" + comment.ContentId + "/" + CommonFunctions.NormalizeAddress(contentTitle);

                        var oSetting = uow.SettingRepository.Get(x=>x,x => x.LanguageId == 1,null, "attachment").SingleOrDefault();

                        CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید نظر شما", "کاربر گرامی ، " + comment.FullName + "، نظر شما تایید شد.<br/> لینک صفحه :  <a href='" + contentUrl + "'>" + contentTitle + "</a>", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                        EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                        #endregion

                        #region SendMail
                        EmailService es = new EmailService();
                        IdentityMessage imessage = new IdentityMessage();
                        imessage.Body = EmailBodyHtml;
                        imessage.Destination = comment.Email;
                        imessage.Subject = " تایید نظر شما ";
                        await es.SendAsync(imessage);

                        #endregion
                    }
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "Comments", "Edit", false, 200, "ویرایش نظرِ " + comment.FullName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                ViewBag.ContentId = new SelectList(uow.ContentRepository.Get(x=>x), "Id", "Title", comment.ContentId);
                ViewBag.ParrentId = new SelectList(uow.CommentRepository.Get(x=>x), "Id", "Email", comment.ParrentId);
                return View(comment);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Comments", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        //GET: Admin/Comments/Edit/5
        public virtual ActionResult Create(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                #region Check License
                
                
                #endregion

               
                    if (ModulePermission.check(User.Identity.GetUserId(), 17, 3))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        Content Content = uow.ContentRepository.GetByID(id);
                        if (Content == null)
                        {
                            return HttpNotFound();
                        }
                        ViewBag.Content = Content;
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Comments", "Create", true, 200, " نمایش صفحه ایجاد نظرِ " , DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Comments", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual  ActionResult Create( Comment comment)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    uow.CommentRepository.Insert(comment);
                    uow.Save();

                    ViewBag.Error = "نظر شما ثبت شد.";
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "Comments", "Create", false, 200, "ثبت نظر جدید ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                Content Content = uow.ContentRepository.GetByID(comment.ContentId);
                ViewBag.Content = Content;
                ViewBag.ContentId = new SelectList(uow.ContentRepository.Get(x => x), "Id", "Title", comment.ContentId);
                ViewBag.ParrentId = new SelectList(uow.CommentRepository.Get(x => x), "Id", "Email", comment.ParrentId);
                return View(comment);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Comments", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        //GET: Admin/Comments/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                #region Check License
                
                
                #endregion

                
                    if (ModulePermission.check(User.Identity.GetUserId(), 17, 3))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        Comment comment = uow.CommentRepository.GetByID(id);
                        if (comment == null)
                        {
                            return HttpNotFound();
                        }
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Comments", "Delete", true, 200, " نمایش صفحه حذف نظرِ " + comment.FullName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(comment);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Comments", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                List<int> Ids = new List<int>();
                Comment comment = uow.CommentRepository.GetByID(id);
                if (comment.ChildComment.Any())
                {
                    foreach (Comment item in comment.ChildComment)
                    {
                        foreach (Comment item2 in item.ChildComment)
                        {
                            foreach (Comment item3 in item2.ChildComment)
                            {
                                foreach (Comment item4 in item3.ChildComment)
                                {
                                    Ids.Add(item4.Id);

                                }
                                Ids.Add(item3.Id);
                            }
                            Ids.Add(item2.Id);
                        }
                        Ids.Add(item.Id);
                    }
                }
                Ids.Add(id);

                foreach (int item in Ids)
                {
                    uow.CommentRepository.Delete(uow.CommentRepository.GetByID(item));
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Comments", "DeleteConfirmed", false, 200, "  حذف نظرِ " + comment.FullName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Comments", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
