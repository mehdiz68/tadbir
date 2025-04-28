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

namespace ahmadi.Controllers
{
    public class VideoController : BaseController
    {
        UnitOfWorkClass uow = null;
        // GET: content
        public ActionResult Index(int? id, string title, int? langid)
        {
            uow = new UnitOfWorkClass();



            langid = langid.HasValue ? langid.Value : 1;
            ViewBag.langId = langid;

            var setting = GetSetting(langid);

            if (id.HasValue)
            {
                #region Get Setting 


                #endregion

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var content = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.LanguageId == langid && (x.Id == id), null, "Category,OtherImages,OtherImages.attachment,attachment,Comments,Tags,Sources,VideoAttachment").FirstOrDefault();
                if (content != null)
                {
                    var ContentType = readXML.DetailOfXContentType(content.ContentTypeId);
                    ViewBag.ContentType = ContentType;
                    if (!ContentType.IsVideo)
                        return RedirectPermanent(string.Format("~/content/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(content.PageAddress)));


                    #region Meta
                    string pre = langid.HasValue ? "/En" : "";
                    ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
                    oMeta.WebSiteMetaDescription = content.Descr;
                    oMeta.Favicon = setting.FaviconattachmentFileName;
                    oMeta.WebSiteMetakeyword = "";
                    oMeta.WebSiteTitle = content.Title;
                    oMeta.Logo = setting.attachmentFileName;
                    oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + pre + "/Video/" + content.Id + "/" + CommonFunctions.NormalizeAddress(content.PageAddress));
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
                            return Redirect("~/En");
                    }




                    if (content.ContentTypeId == 0)
                        ViewBag.Sliders = uow.SliderRepository.Get(x => x, x => x.LanguageId == langid && x.IsActive && x.TypeId == 7 && x.LinkId == id, null, "SliderImages.attachment");
                    else
                        ViewBag.Sliders = uow.SliderRepository.Get(x => x, x => x.LanguageId == langid && x.IsActive && x.TypeId == 1 && x.LinkId == id, null, "SliderImages.attachment");


                    var date = DateTime.Now.Date;
                    if (content.ContentTypeId == 0)
                        ViewBag.Ads = uow.AdverestingRepository.Get(x => x, x => x.LanguageId == langid && x.TypeId == 7 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null), null, "attachment");
                    else
                        ViewBag.Ads = uow.AdverestingRepository.Get(x => x, x => x.LanguageId == langid && x.TypeId == 1 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null), null, "attachment");


                    #region Bredcrumb
                    int Plusindex = 2, MinesIndex = 2;
                    string breadcrumbCat = "<li style='order:" + (Plusindex) + "' itemprop='itemListElement' class='breadcrumb-item'><a href='/videos' class='breadcrumb-item'>" + ContentType.Title + "</a><meta itemprop='position' content='" + (Plusindex) + "' /></li>";
                    Plusindex++;
                    MinesIndex--;
                    if (content.Category != null)
                    {
                        var currentCat = content.Category;
                        while (currentCat.ParrentId.HasValue)
                        {
                            currentCat = currentCat.ParentCat;
                            breadcrumbCat += "<li style='order:" + MinesIndex + "'  itemprop='itemListElement' class='breadcrumb-item'><a href='" + Url.Action("Index", "videoCategory", new { id = currentCat.Id, title = CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(currentCat.PageAddress) }) + "'>" + currentCat.Title + "</a><meta itemprop='position' content='" + Plusindex + "' /></li>";
                            Plusindex++;
                            MinesIndex--;
                        }
                        breadcrumbCat += "<li style='order:" + Plusindex + "' itemprop='itemListElement' class='breadcrumb-item'><a href='" + Url.Action("Index", "videoCategory", new { id = content.Category.Id, title = CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(content.Category.PageAddress) }) + "'>" + content.Category.Title + "</a><meta itemprop='position' content='" + Plusindex + "' /></li>";
                    }

                    ViewBag.breadcrumbCat = breadcrumbCat;
                    #endregion

                    #region Related Content
                    var contenttypes = readXML.ListOfXContentType().Where(x => x.LanguageId == 1 && x.Id != ContentType.Id).OrderBy(x => x.Name);
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
                        newitem.Contents = uow.ContentRepository.Sql(x => x, "exec GetRelatedContent @ContentId,@contentTypeId,@ResultCount,@Lang", new SqlParameter("@ContentId", content.Id), new SqlParameter("@contentTypeId", item.Id), new SqlParameter("@ResultCount", 3), new SqlParameter("@Lang", 1)).ToList();
                        ContentTypeViewModels.Add(newitem);
                    }

                    ViewBag.contentTypes = ContentTypeViewModels.AsQueryable();
                    #endregion


                    if (content.CatId.HasValue)
                    {
                        List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetContentSubCats @CatId", new SqlParameter("@CatId", content.CatId)).ToList();
                        ViewBag.RecentContentCategory = uow.ContentRepository.Get(x => x, x => CatIds.Contains(x.CatId.Value) && x.IsActive && x.LanguageId == langid, x => x.OrderByDescending(s => s.Id), "attachment", 0, 8);

                        var TopContentsByRandom2 = uow.ContentRepository.Get(x => x, x => x.LanguageId == langid && CatIds.Contains(x.CatId.Value) && x.ContentTypeId == content.ContentTypeId && x.IsActive && x.IsAbout == false && x.IsContact == false && x.IsDefault == false && x.IsRegister == false, o => o.OrderBy(x => new Guid()), "attachment", 0, 8, true);
                        List<int> Random2 = new List<int>();
                        if (TopContentsByRandom2.Any())
                            Random2 = TopContentsByRandom2.Select(x => x.Id).ToList();
                        ViewBag.TopContentsByRandom2 = TopContentsByRandom2;
                        var TopContentsByRandom3 = uow.ContentRepository.Get(x => x, x => x.LanguageId == langid && CatIds.Contains(x.CatId.Value) && !Random2.Contains(x.Id) && x.ContentTypeId == content.ContentTypeId && x.IsActive && x.IsAbout == false && x.IsContact == false && x.IsDefault == false && x.IsRegister == false, o => o.OrderBy(x => new Guid()), "attachment", 0, 8, true);
                        List<int> Random3 = new List<int>();
                        if (TopContentsByRandom3.Any())
                            Random3 = TopContentsByRandom3.Select(x => x.Id).ToList();
                        ViewBag.TopContentsByRandom3 = TopContentsByRandom3;
                        ViewBag.TopContentsByRandom4 = uow.ContentRepository.Get(x => x, x => x.LanguageId == langid && CatIds.Contains(x.CatId.Value) && !Random3.Contains(x.Id) && x.ContentTypeId == content.ContentTypeId && x.IsActive && x.IsAbout == false && x.IsContact == false && x.IsDefault == false && x.IsRegister == false, o => o.OrderBy(x => new Guid()), "attachment", 0, 8, true);
                    }


                    UpdateVisit(content.Id);
                    if (content.IsContact)
                    {
                        ViewBag.masterTheme = readXML.DetailOfXMasterTheme(Convert.ToInt16(content.LanguageId));
                        return View("_ContactUs", content);
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
            uow = new UnitOfWorkClass();
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
            uow = new UnitOfWorkClass();
            try
            {
                if (uow.ContentRepository.GetByID(ContentId) != null)
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
                                Message = "ثبت شد.",
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
                                Message = "ثبت شد.",
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Message = "شما به این مطلب رای داده اید.",
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
            uow = new UnitOfWorkClass();
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

                    var content = uow.ContentRepository.GetByID(ocomment.ContentId);
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
                        Message = "ثبت شد.",
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
                        Message = "عدد وارد شده صحیح نیست!",
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
                    Message = "ثبت شد.",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual JsonResult PlusComment(int CommentId)
        {
            uow = new UnitOfWorkClass();
            try
            {
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
                                Message = "ثبت شد.",
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
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
                    else
                    {
                        return Json(new
                        {
                            Message = "شما به این کامنت امتیاز داده اید.",
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
            uow = new UnitOfWorkClass();
            try
            {
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
                                Message = "ثبت شد.",
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
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
                    else
                    {
                        return Json(new
                        {
                            Message = "شما به این کامنت امتیاز داده اید.",
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
        public virtual JsonResult Like(int Id)
        {
            uow = new UnitOfWorkClass();
            try
            {
                if (uow.ContentRepository.GetByID(Id) != null)
                {

                    HttpCookie CommentCookie = HttpContext.Request.Cookies["ContentLike"];
                    if (CommentCookie == null)
                        CommentCookie = new HttpCookie("ContentLike");
                    if (CommentCookie[Id.ToString()] != Id.ToString())
                    {
                        Content cm = uow.ContentRepository.Get(x => x, x => x.Id == Id).SingleOrDefault();
                        if (cm != null)
                        {
                            cm.Link++;
                            uow.Save();
                            //Add Cookie 
                            CommentCookie[Id.ToString()] = Id.ToString();
                            CommentCookie.Expires = DateTime.Now.AddDays(1);
                            HttpContext.Response.Cookies.Add(CommentCookie);

                            return Json(new
                            {
                                count = cm.Link,
                                Message = "ثبت شد.",
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
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
                    else
                    {
                        return Json(new
                        {
                            Message = "شما این ویدئو را لایک یا دیسلایک کرده اید!.",
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
        public virtual JsonResult DisLike(int Id)
        {
            uow = new UnitOfWorkClass();
            try
            {
                if (uow.ContentRepository.GetByID(Id) != null)
                {

                    HttpCookie CommentCookie = HttpContext.Request.Cookies["ContentLike"];
                    if (CommentCookie == null)
                        CommentCookie = new HttpCookie("ContentLike");
                    if (CommentCookie[Id.ToString()] != Id.ToString())
                    {
                        Content cm = uow.ContentRepository.Get(x => x, x => x.Id == Id).SingleOrDefault();
                        if (cm != null)
                        {
                            cm.Link--;
                            uow.Save();
                            //Add Cookie 
                            CommentCookie[Id.ToString()] = Id.ToString();
                            CommentCookie.Expires = DateTime.Now.AddDays(1);
                            HttpContext.Response.Cookies.Add(CommentCookie);

                            return Json(new
                            {
                                count = cm.Link,
                                Message = "ثبت شد.",
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
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
                    else
                    {
                        return Json(new
                        {
                            Message = "شما این ویدئو را لایک یا دیسلایک کرده اید!.",
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
