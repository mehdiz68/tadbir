using CoreLib.Infrastructure;
using CoreLib.Infrastructure.Captcha;
using CoreLib.ViewModel.Xml;
using Domain;
using ahmadi.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UnitOfWork;
using System.Data.Entity;

namespace ahmadi.Controllers
{
    public class contentController : BaseController
    {
        UnitOfWorkClass uow = null;

        public contentController()
        {

            uow = new UnitOfWorkClass();
        }

        // GET: content
        public ActionResult Index(int? id, string title, int? langid)
        {
            if (String.IsNullOrEmpty(title) || !id.HasValue)
                return Redirect("~/");

            title = title.Replace("-", " ").ToLower();

            if (id.HasValue)
            {
                langid = langid.HasValue ? langid.Value : 1;
                ViewBag.langId = langid;

                #region Get Setting 

                var setting = GetSetting(langid);

                #endregion

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var content = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.LanguageId == langid && (x.Id == id), null, "Category,OtherImages,OtherImages.attachment,attachment,Comments,Tags,Sources,VideoAttachment,ContentRating").FirstOrDefault();
                if (content != null)
                {
                    if (content.PageAddress.ToLower() != title)
                    {
                        return RedirectPermanent(string.Format("~/content/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(content.PageAddress)));
                    }
                    var ContentType = readXML.DetailOfXContentType(content.ContentTypeId);
                    if (ContentType.IsVideo)
                        return RedirectPermanent(string.Format("~/Video/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(content.PageAddress)));

                    #region Meta

                    string pre = langid.HasValue ? "/En" : "";
                    ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
                    oMeta.WebSiteMetaDescription = content.Descr;
                    oMeta.Favicon = setting.FaviconattachmentFileName;
                    oMeta.WebSiteMetakeyword = "";
                    oMeta.WebSiteTitle = content.Title;
                    oMeta.Logo = setting.attachmentFileName;
                    oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + pre + "/content/" + content.Id + "/" + CommonFunctions.NormalizeAddress(content.PageAddress));
                    if (content.attachment != null)
                        oMeta.PageCover = setting.StaticContentDomain + "/Uploadfiles/" + content.attachment.FileName;
                    else
                        oMeta.PageCover = setting.StaticContentDomain + "/images/default-thumbnail.jpg";
                    oMeta.WebSiteName = setting.WebSiteName;
                    oMeta.StaticContentUrl = setting.StaticContentDomain;
                    ViewBag.Meta = oMeta;
                    #endregion

                    if (content.Category != null)
                    {
                        if (content.Category.IsActive == false)
                            return Redirect("~/");
                    }




                    if (content.ContentTypeId == 0)
                        ViewBag.Sliders = uow.SliderRepository.Get(x => x, x => x.IsActive && x.TypeId == 7 && x.LinkId == id, null, "SliderImages.attachment");
                    else
                        ViewBag.Sliders = uow.SliderRepository.Get(x => x, x => x.IsActive && x.TypeId == 1 && x.LinkId == id, null, "SliderImages.attachment");



                    ViewBag.Comments = uow.CommentRepository.Get(x => x, x => x.IsActive && x.Visited, x => x.OrderByDescending(s => s.Id), "", 0, 8);

                    ViewBag.setting = setting;


                    var date = DateTime.Now.Date;
                    if (content.ContentTypeId == 0)
                        ViewBag.Ads = uow.AdverestingRepository.Get(x => x, x => x.TypeId == 7 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null), null, "attachment");
                    else
                        ViewBag.Ads = uow.AdverestingRepository.Get(x => x, x => x.TypeId == 1 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null), null, "attachment");



                    #region Bredcrumb
                    int Plusindex = 2, MinesIndex = 2;
                    string breadcrumbCat = "<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='" + Url.Action("Index", "ContentType", new { id = content.ContentTypeId }) + "'><span itemprop='name'>" + ContentType.Title + "</span></a><meta itemprop='position' content='" + (Plusindex) + "' /></li>";
                    Plusindex++;
                    MinesIndex--;
                    if (content.Category != null)
                    {
                        var currentCat = content.Category;
                        while (currentCat.ParrentId.HasValue)
                        {
                            currentCat = currentCat.ParentCat;
                            breadcrumbCat += "<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='" + Url.Action("Index", "Category", new { id = currentCat.Id, title = CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(currentCat.PageAddress) }) + "'><span itemprop='name'>" + currentCat.Title + "</span></a><meta itemprop='position' content='" + Plusindex + "' /></li>";
                            Plusindex++;
                            MinesIndex--;
                        }
                        breadcrumbCat += "<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item'  href='" + Url.Action("Index", "Category", new { id = currentCat.Id, title = CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(currentCat.PageAddress) }) + "'><span itemprop='name'>" + currentCat.Title + "</span></a><meta itemprop='position' content='" + Plusindex + "' /></li>";
                    }
                    breadcrumbCat += "<li  itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='" + Url.Action("Index", "content", new { id = content.Id, title = CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(content.PageAddress) }) + "'><span itemprop='name'>" + content.Title + "</span></a><meta itemprop='position' content='" + (Plusindex + 1) + "' /></li>";

                    ViewBag.breadcrumbCat = breadcrumbCat;
                    #endregion

                    #region Related Content
                    var contenttypes = readXML.ListOfXContentType().Where(x => x.LanguageId == langid && x.Id == content.ContentTypeId).OrderBy(x => x.Name);
                    List<ViewModels.Content.ContentTypeViewModel> ContentTypeViewModels = new List<ViewModels.Content.ContentTypeViewModel>();
                    foreach (var item in contenttypes)
                    {
                        ViewModels.Content.ContentTypeViewModel newitem = new ViewModels.Content.ContentTypeViewModel();
                        newitem.Abstract = item.Abstract;
                        newitem.Id = item.Id;
                        newitem.LanguageId = item.LanguageId;
                        newitem.IsVideo = item.IsVideo;
                        newitem.Name = item.Name;
                        newitem.Title = item.Title;
                        newitem.Contents = content.CatId.HasValue ? uow.ContentRepository.Get(x => x, x => x.Id != content.Id && x.CatId == content.CatId, x => x.OrderByDescending(a => a.Id), "attachment,Comments", 0, 10).ToList() : uow.ContentRepository.Get(x => x, x => x.Id != content.Id && x.ContentTypeId == content.ContentTypeId, x => x.OrderByDescending(a => a.Id), "attachment,Comments", 0, 10).ToList();
                        ContentTypeViewModels.Add(newitem);
                    }

                    ViewBag.contentTypes = ContentTypeViewModels.AsQueryable();
                    #endregion



                    ViewBag.contentType = ContentType;

                    UpdateVisit(content.Id);
                    if (content.IsContact)
                    {
                        if (langid == 1)
                            return View("_ContactUs", content);
                        else
                            return View("_ContactUs_en", content);
                    }
                    else if (content.IsAbout)
                    {
                        ViewBag.Video = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == setting.LanguageId && x.IsActive && (x.ContentTypeId == 5 || x.ContentTypeId == 10)).Include("attachment").Include("VideoAttachment").OrderBy(s => s.DisplaySort).Skip(() => 0).Take(() => 1).SingleOrDefault();
                        ViewBag.StaticTextCategories = uow.StaticTextCategoryRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == setting.LanguageId && x.IsActive).Include("StaticTextContents").ToList();
                        ViewBag.Comments = uow.CommentRepository.GetQueryList().AsNoTracking().Where(x => x.Content.LanguageId == setting.LanguageId && x.IsActive).OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 10);
                        ViewBag.Teams = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && (x.ContentTypeId == 13 || x.ContentTypeId == 14) && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsRegister == false).Include("attachment").Include("Blogattachment").Include("Comments").OrderByDescending(s => s.Id);
                        if (langid == 1)
                            return View("_AboutUs", content);
                        else
                            return View("_AboutUs_en", content);
                    }
                    else
                    {

                        if (langid == 1)
                            return View("Index", content);
                        else
                            return View("Index_en", content);
                    }
                }
                else
                    return Redirect("~/");
            }
            else
                return Redirect("~/");


        }

        public void UpdateVisit(int id)
        {
            var UpContent = uow.ContentRepository.GetByID(id);
            UpContent.Visits++;
            uow.Save();
        }

        public int GetLineNumber(Exception ex)
        {
            var lineNumber = 0;
            const string lineSearch = ":line ";
            var index = ex.StackTrace.LastIndexOf(lineSearch);
            if (index != -1)
            {
                var lineNumberText = ex.StackTrace.Substring(index + lineSearch.Length);
                if (int.TryParse(lineNumberText, out lineNumber))
                {
                }
            }
            return lineNumber;
        }

        [HttpPost]
        public virtual JsonResult AddRate(int ContentId, double value)
        {
            try
            {
                var content = uow.ContentRepository.GetByID(ContentId);
                if (content != null)
                {

                    HttpCookie RateCookie = HttpContext.Request.Cookies["ContentRate"];
                    if (RateCookie == null)
                        RateCookie = new HttpCookie("ContentRate");
                    if (RateCookie[ContentId.ToString()] != ContentId.ToString())
                    {
                        ContentRating cr = uow.ContentRatingRepository.Get(x => x, x => x.ContentID == ContentId).SingleOrDefault();
                        if (cr != null)
                        {
                            cr.Rating += value;
                            cr.TotalRaters++;
                            cr.AverageRating = Math.Round(Convert.ToDouble(cr.Rating * 1.0 / cr.TotalRaters * 1.0), 2);
                            uow.Save();
                            //Add Cookie 
                            RateCookie[ContentId.ToString()] = ContentId.ToString();
                            RateCookie.Expires = DateTime.Now.AddDays(1);
                            HttpContext.Response.Cookies.Add(RateCookie);

                            return Json(new
                            {
                                TotalRaters = cr.TotalRaters,
                                AverageRating = cr.AverageRating,
                                Message = content.LanguageId == 1 ? "ثبت شد." : " saved...",
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            ContentRating newCr = new ContentRating();

                            newCr.Rating = value;
                            newCr.TotalRaters = 1;
                            newCr.AverageRating = value;
                            newCr.ContentID = ContentId;
                            uow.ContentRatingRepository.Insert(newCr);
                            uow.Save();

                            //Add Cookie 
                            RateCookie[ContentId.ToString()] = ContentId.ToString();
                            RateCookie.Expires = DateTime.Now.AddDays(1);
                            HttpContext.Response.Cookies.Add(RateCookie);

                            return Json(new
                            {
                                TotalRaters = newCr.TotalRaters,
                                AverageRating = newCr.AverageRating,
                                Message = content.LanguageId == 1 ? "ثبت شد." : "saved...",
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Message = content.LanguageId == 1 ? "شما به این مطلب رای داده اید." : "duplicat...",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new
                    {
                        Message = "خطا.",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);

                }

            }
            catch (System.Exception x)
            {

                return Json(new
                {
                    Message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<JsonResult> AddComment(Comment ocomment, int CaptchaValue)
        {
            var content = uow.ContentRepository.GetByID(ocomment.ContentId);
            try
            {
                if (CaptchaValue == Convert.ToInt32(Session["captcha"]))
                {
                    ocomment.FullName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ocomment.FullName);
                    zCaptcha c = new zCaptcha();
                    Session["captcha"] = c.GetCurrentValue();

                    ocomment.InsertDate = DateTime.Now;
                    ocomment.IsActive = false;
                    ocomment.NegativeRating = 0;
                    ocomment.PositiveRating = 0;
                    ocomment.Visited = false;
                    uow.CommentRepository.Insert(ocomment);
                    uow.Save();

                    //Send Mail To Related Admin
                    #region Create Html Body
                    string EmailBodyHtml = "";

                    var oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment").SingleOrDefault();
                    string contentUrl = "http://" + HttpContext.Request.Url.Host + "/content/" + ocomment.ContentId + "/" + CommonFunctions.NormalizeAddress(content.Title);
                    CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "ثبت کامنت جدید در سایت", "مدیر گرامی نظر جدیدی توسط ، " + ocomment.FullName + "، در سایت ثبت شد . شما میتوانید با مراجعه به پنل مدیریت آن را دیده و تایید نمایید .لینک صفحه مربوط به کامنت : <br/> <a href='" + contentUrl + "'>" + content.Title + "</a><br/> نظر : <br/> <p>" + ocomment.Message + "</p>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                    EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                    #endregion

                    #region SendMail
                    var emails = uow.AdministratorPermissionRepository.Get(x => x, x => x.ModuleId == 4 && x.NotificationEmail == true).Select(x => x.User.Email).Distinct();

                    EmailService es = new EmailService();
                    await es.SendMultiDestinationAsync(" ثبت کامنت جدید در سایت ", EmailBodyHtml, emails.ToList());

                    #endregion


                    return Json(new
                    {
                        data = CaptureHelper.RenderViewToString("_Captcha", c, this.ControllerContext),
                        Message = content.LanguageId == 1 ? "ثبت شد." : "saved...",
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    zCaptcha c = new zCaptcha();
                    Session["captcha"] = c.GetCurrentValue();
                    return Json(new
                    {
                        data = CaptureHelper.RenderViewToString("_Captcha", c, this.ControllerContext),
                        Message = content.LanguageId == 1 ? "عدد وارد شده صحیح نیست!" : "Invalid Captcha...",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                zCaptcha c = new zCaptcha();
                Session["captcha"] = c.GetCurrentValue();
                return Json(new
                {
                    data = CaptureHelper.RenderViewToString("_Captcha", c, this.ControllerContext),
                    Message = content.LanguageId == 1? "ثبت شد.":"saved....",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public virtual JsonResult PlusComment(int CommentId)
        {
            try
            {
                var languageId = uow.ContentRepository.Get(x => x.LanguageId, x => x.Comments.Any(s => s.Id == CommentId)).FirstOrDefault();
                if (uow.CommentRepository.GetByID(CommentId) != null)
                {

                    HttpCookie CommentCookie = HttpContext.Request.Cookies["CommentRate"];
                    if (CommentCookie == null)
                        CommentCookie = new HttpCookie("CommentRate");
                    if (CommentCookie[CommentId.ToString()] != CommentId.ToString())
                    {
                        Comment cm = uow.CommentRepository.Get(x => x, x => x.Id == CommentId).SingleOrDefault();
                        if (cm != null)
                        {
                            cm.PositiveRating++;
                            uow.Save();
                            //Add Cookie 
                            CommentCookie[CommentId.ToString()] = CommentId.ToString();
                            CommentCookie.Expires = DateTime.Now.AddDays(1);
                            HttpContext.Response.Cookies.Add(CommentCookie);

                            return Json(new
                            {
                                count = cm.PositiveRating,
                                Message = languageId == 1 ? "ثبت شد." : "saved...",
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                Message = languageId == 1 ? "خطا." : "error...",
                                statusCode = 500
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Message = languageId == 1 ? "شما به این کامنت امتیاز داده اید" : "Duplicate...",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new
                    {
                        Message = "خطا.",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (System.Exception x)
            {
                return Json(new
                {
                    Message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public virtual JsonResult MinusComment(int CommentId)
        {
            try
            {
                var languageId = uow.ContentRepository.Get(x => x.LanguageId, x => x.Comments.Any(s => s.Id == CommentId)).FirstOrDefault();
                if (uow.CommentRepository.GetByID(CommentId) != null)
                {

                    HttpCookie CommentCookie = HttpContext.Request.Cookies["CommentRate"];
                    if (CommentCookie == null)
                        CommentCookie = new HttpCookie("CommentRate");
                    if (CommentCookie[CommentId.ToString()] != CommentId.ToString())
                    {
                        Comment cm = uow.CommentRepository.Get(x => x, x => x.Id == CommentId).SingleOrDefault();
                        if (cm != null)
                        {
                            cm.NegativeRating++;
                            uow.Save();
                            //Add Cookie 
                            CommentCookie[CommentId.ToString()] = CommentId.ToString();
                            CommentCookie.Expires = DateTime.Now.AddDays(1);
                            HttpContext.Response.Cookies.Add(CommentCookie);

                            return Json(new
                            {
                                count = cm.PositiveRating,
                                Message = languageId == 1 ? "ثبت شد." : "saved...",
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                Message = languageId == 1 ? "خطا." : "error",
                                statusCode = 500
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Message = languageId == 1 ? "شما به این کامنت امتیاز داده اید." : "Duplicate...",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    return Json(new
                    {
                        Message = "خطا.",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (System.Exception x)
            {
                return Json(new
                {
                    Message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
