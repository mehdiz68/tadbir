using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using System.Collections.Generic;
using CoreLib.ViewModel.Xml;
using PagedList;
using CoreLib.Infrastructure.ModelBinder;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class adverestingController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow = null;

        // GET: Admin/Adverestings

        [CorrectArabianLetter(new string[] { "TitleString", "TitleFilter" })]
        public virtual ActionResult Index(int? page, string sortOrder, string TitleFilter, string TitleString, string LanguagenameFilter, string LanguagenameString, string TypeId, string TypeIdFilter, string PositionId, string PositionIdFilter)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {
                var p = ModulePermission.check(User.Identity.GetUserId(), 23);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");


                    //User Has Access to Product And Gallery Module
                    List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--محل نمایش تبلیغ را انتخاب نمایید--", Value = "0" }, new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1" }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "7" }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "8" }, new SelectListItem() { Text = "دسته بندی ها", Value = "2" }, new SelectListItem() { Text = "برچسب ها", Value = "3" }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "4" }, new SelectListItem() { Text = "گالری", Value = "5" }, new SelectListItem() { Text = "جست و جوی محصولات", Value = "6" } };
                    ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;


                    List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--همه صفحات--", Value = "0" }, };
                    ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;

                    List<SelectListItem> PositionIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "بالا", Value = "1" }, new SelectListItem() { Text = "راست", Value = "2" }, new SelectListItem() { Text = "پایین", Value = "3" }, new SelectListItem() { Text = "چپ", Value = "4" } };
                    ViewBag.PositionIdSelectListItem = PositionIdSelectListItem;


                    #region search
                    if (string.IsNullOrEmpty(TitleString))
                        TitleString = TitleFilter;
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;
                    if (string.IsNullOrEmpty(TypeId))
                        TypeId = TypeIdFilter;
                    if (string.IsNullOrEmpty(PositionId))
                        PositionId = PositionIdFilter;

                    ViewBag.PositionIdFilter = PositionId;
                    ViewBag.TitleFilter = TitleString;
                    ViewBag.LanguagenameFilter = LanguagenameString;
                    ViewBag.TypeIdFilter = TypeId;

                    var Adverestings = uow.AdverestingRepository.GetQueryList().AsNoTracking().Include("AdverestingLogs").Include("attachment");
                    if (!String.IsNullOrEmpty(TitleString))
                        Adverestings = Adverestings.Where(s => s.Title.Contains(TitleString));
                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int langId = Convert.ToInt32(LanguagenameString);
                        Adverestings = Adverestings.Where(s => s.LanguageId == langId);
                    }
                    if (TypeId != "0" && !String.IsNullOrEmpty(TypeId))
                    {
                        int typeid = Convert.ToInt32(TypeId);
                        Adverestings = Adverestings.Where(s => s.TypeId == typeid);
                    }
                    if (!String.IsNullOrEmpty(PositionId))
                    {
                        int PosId = Convert.ToInt32(PositionId);
                        Adverestings = Adverestings.Where(s => s.Position == PosId);
                    }
                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "Title":
                            Adverestings = Adverestings.OrderBy(s => s.Title);
                            ViewBag.CurrentSort = "Title";
                            break;
                        case "Title_desc":
                            Adverestings = Adverestings.OrderByDescending(s => s.Title);
                            ViewBag.CurrentSort = "Title_desc";
                            break;
                        case "Languagename":
                            Adverestings = Adverestings.OrderBy(s => s.LanguageId);
                            ViewBag.CurrentSort = "Languagename";
                            break;
                        case "Languagename_desc":
                            Adverestings = Adverestings.OrderByDescending(s => s.LanguageId);
                            ViewBag.CurrentSort = "Languagename_desc";
                            break;
                        case "DisplaySort":
                            Adverestings = Adverestings.OrderBy(s => s.DisplaySort);
                            ViewBag.CurrentSort = "DisplaySort";
                            break;
                        case "DisplaySort_desc":
                            Adverestings = Adverestings.OrderByDescending(s => s.DisplaySort);
                            ViewBag.CurrentSort = "DisplaySort_desc";
                            break;
                        case "IsActive":
                            Adverestings = Adverestings.OrderBy(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive";
                            break;
                        case "IsActive_desc":
                            Adverestings = Adverestings.OrderByDescending(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive_desc";
                            break;
                        case "Position":
                            Adverestings = Adverestings.OrderBy(s => s.Position);
                            ViewBag.CurrentSort = "Position";
                            break;
                        case "Position_desc":
                            Adverestings = Adverestings.OrderByDescending(s => s.Position);
                            ViewBag.CurrentSort = "Position_desc";
                            break;

                        default:  // Name ascending 
                            Adverestings = Adverestings.OrderByDescending(s => s.InsertDate);
                            break;
                    }
                    #endregion

                    ViewBag.Adverestings = Adverestings;
                    int pageSize = 100;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "سیستم تبلیغات", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Adverestings", "Index", true, 200, " نمایش صفحه مدیریت تبلیغات", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Adverestings.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تبلیغات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Adverestings", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public virtual ActionResult Sort(int? TypeId, int? LinkId, int? PositionId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {


                var p = ModulePermission.check(User.Identity.GetUserId(), 23);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");



                    List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--محل نمایش تبلیغ را انتخاب نمایید--", Value = "0" }, new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1" }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "7" }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "8" }, new SelectListItem() { Text = "دسته بندی ها", Value = "2" }, new SelectListItem() { Text = "برچسب ها", Value = "3" }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "4" }, new SelectListItem() { Text = "گالری", Value = "5" }, new SelectListItem() { Text = "جست و جوی محصولات", Value = "6" } };
                    ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;



                    //List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--همه صفحات--", Value = "" }, };
                    //ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;

                    var date = DateTime.Now.Date;
                    var Adverestings = uow.AdverestingRepository.Get(x => x, x => x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null), x => x.OrderBy(s => s.DisplaySort), "attachment").AsQueryable();
                    if (TypeId.HasValue)
                    {
                        Adverestings = Adverestings.Where(x => x.TypeId == TypeId.Value);
                        int[] linkids = Adverestings.Select(x => x.LinkId).ToArray();
                        switch (TypeId.Value)
                        {
                            case 1:
                                ViewBag.LinkIdIdSelectListItem = new SelectList(uow.ContentRepository.Get(x => x, x => linkids.Contains(x.Id)), "Id", "Title", LinkId); break;
                            case 2:
                                ViewBag.LinkIdIdSelectListItem = new SelectList(uow.CategoryRepository.Get(x => x, x => linkids.Contains(x.Id)), "Id", "Title", LinkId); break;
                            case 3:
                                ViewBag.LinkIdIdSelectListItem = new SelectList(uow.TagRepository.Get(x => x, x => linkids.Contains(x.Id)), "Id", "TagName", LinkId); break;
                            case 4:
                            case 6:
                                ViewBag.LinkIdIdSelectListItem = new SelectList(uow.ProductCategoryRepository.Get(x => x, x => linkids.Contains(x.Id)), "Id", "Title", LinkId); break;
                            case 5:
                                ViewBag.LinkIdIdSelectListItem = new SelectList(uow.GalleryCategoryRepository.Get(x => x, x => linkids.Contains(x.Id)), "Id", "Title", LinkId); break;
                            default: break;
                        }
                    }
                    if (LinkId.HasValue)
                    {
                        Adverestings = Adverestings.Where(x => x.LinkId == LinkId.Value);
                    }
                    if (PositionId.HasValue)
                    {
                        Adverestings = Adverestings.Where(x => x.Position == PositionId.Value);
                    }
                    List<SelectListItem> PositionIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "بالا", Value = "1", Selected = (PositionId.HasValue ? PositionId.Value == 1 ? true : false : false) }, new SelectListItem() { Text = "راست", Value = "2", Selected = (PositionId.HasValue ? PositionId.Value == 2 ? true : false : false) }, new SelectListItem() { Text = "پایین", Value = "3", Selected = (PositionId.HasValue ? PositionId.Value == 3 ? true : false : false) }, new SelectListItem() { Text = "چپ", Value = "4", Selected = (PositionId.HasValue ? PositionId.Value == 4 ? true : false : false) } };
                    ViewBag.PositionIdSelectListItem = PositionIdSelectListItem;

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 22 && x.Name == "مرتب سازی", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Adverestings", "Sort", true, 200, " نمایش صفحه مرتب سازی تبلیغات", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Adverestings);

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تبلیغات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Adverestings", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Categories/Sort/5
        [HttpPost]
        public virtual JsonResult Sort(string ids)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var ads = uow.AdverestingRepository.GetByID(id);
                    ads.DisplaySort = i;
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "adveresting", "Sort", true, 200, " نمایش صفحه مرتب سازی تبلیغات", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "adveresting", "Sort", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public virtual JsonResult GetContent(int TypeId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                List<SelectListItem> Contents = new List<SelectListItem>();

                switch (TypeId)
                {
                    case 1:
                        var q = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.ContentTypeId > 0).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 2:
                        var q2 = uow.CategoryRepository.Get(x => x, x => x.IsActive == true).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q2)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 3:
                        var q3 = uow.TagRepository.Get(x => x).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q3)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.TagName;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 4:
                    case 6:
                        var q4 = uow.ProductCategoryRepository.Get(x => x).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q4)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 5:
                        var q5 = uow.GalleryCategoryRepository.Get(x => x).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q5)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 7:
                        var q7 = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.ContentTypeId == 0).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q7)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 8:
                        XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                        var q8 = readXml.ListOfXContentType().Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q8)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Name;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                }
                return Json(new
                {
                    data = Contents,
                    statusCode = 400,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new
                {
                    Message = x.Message,
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public virtual JsonResult GetContentByLinkId(int TypeId, string LinkId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                List<SelectListItem> Contents = new List<SelectListItem>();

                switch (TypeId)
                {
                    case 1:
                        int linkid = Convert.ToInt32(LinkId);
                        var q = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.Id == linkid && x.ContentTypeId > 0).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 2:
                        linkid = Convert.ToInt32(LinkId);
                        var q2 = uow.CategoryRepository.Get(x => x, x => x.IsActive == true && x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q2)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 3:
                        linkid = Convert.ToInt32(LinkId);
                        var q3 = uow.TagRepository.Get(x => x, x => x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q3)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.TagName;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 4:
                    case 6:
                        linkid = Convert.ToInt32(LinkId);
                        var q4 = uow.ProductCategoryRepository.Get(x => x, x => x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q4)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 5:
                        linkid = Convert.ToInt32(LinkId);
                        var q5 = uow.GalleryCategoryRepository.Get(x => x, x => x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q5)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 7:
                        linkid = Convert.ToInt32(LinkId);
                        var q7 = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.ContentTypeId == 0 && x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q7)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 8:
                        linkid = Convert.ToInt32(LinkId);
                        XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                        var q8 = readXml.ListOfXContentType().Where(x => x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q8)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Name;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                }
                return Json(new
                {
                    data = Contents,
                    statusCode = 400,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new
                {
                    Message = x.Message,
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public virtual JsonResult SearchContent(int TypeId, string Keyword)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                List<SelectListItem> Contents = new List<SelectListItem>();
                switch (TypeId)
                {
                    case 1:
                        var q = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword) && x.ContentTypeId > 0).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 2:
                        var q2 = uow.CategoryRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q2)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 3:
                        var q3 = uow.TagRepository.Get(x => x, x => x.TagName.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q3)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.TagName;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 4:
                    case 6:
                        var q4 = uow.ProductCategoryRepository.Get(x => x, x => x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q4)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 5:
                        var q5 = uow.GalleryCategoryRepository.Get(x => x, x => x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q5)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 7:
                        var q7 = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.ContentTypeId == 0 && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q7)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 8:
                        XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                        var q8 = readXml.ListOfXContentType().Where(x => x.Name.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q8)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Name;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                }
                return Json(new
                {
                    data = Contents,
                    statusCode = 400,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new
                {
                    Message = x.Message,
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Adverestings/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {


                if (ModulePermission.check(User.Identity.GetUserId(), 23, 1))
                {
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");


                    List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نمایید--", Value = "0" }, new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1" }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "7" }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "8" }, new SelectListItem() { Text = "دسته بندی ها", Value = "2" }, new SelectListItem() { Text = "برچسب ها", Value = "3" }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "4" }, new SelectListItem() { Text = "گالری", Value = "5" }, new SelectListItem() { Text = "جست و جوی محصولات", Value = "6" } };
                    ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;


                    List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--همه صفحات--", Value = "0" }, };
                    ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;

                    List<SelectListItem> SizeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تمام صفحه", Value = "1" }, new SelectListItem() { Text = "75% صفحه", Value = "2" }, new SelectListItem() { Text = "66% صفحه", Value = "3" }, new SelectListItem() { Text = "50% صفحه", Value = "4" }, new SelectListItem() { Text = "33% صفحه", Value = "5" }, new SelectListItem() { Text = "25% صفحه", Value = "6" }, new SelectListItem() { Text = "20% صفحه", Value = "7" } };
                    ViewBag.AdverestingSizeId = SizeIdSelectListItem;
                    ViewBag.Position = new List<SelectListItem>() { new SelectListItem() { Text = "بالای صفحه", Value = "1" }, new SelectListItem() { Text = "راست صفحه", Value = "2" }, new SelectListItem() { Text = "پایین صفحه", Value = "3" }, new SelectListItem() { Text = "چپ صفحه", Value = "4" } };

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 22 && x.Name == "ایجاد تبلیغ جدید", null, "HelpModuleSectionFields").FirstOrDefault();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "adveresting", "Create", true, 200, " نمایش صفحه ایجاد تبلیغ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تبلیغات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "adveresting", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }

        // POST: Admin/Adverestings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create([Bind(Include = "Id,Title,DisplaySort,IsActive,LanguageId,TypeId,LinkId,ExpireDatestr,StartDate,Cover,AdverestingSizeId,Position,Link,TypeLink")] Adveresting ads)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                ads.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ads.Title);
                if (ads.ExpireDatestr != null)
                    ads.ExpireDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladi(ads.ExpireDatestr);
                if (ads.StartDate != null)
                    ads.StartDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladi(ads.StartDate.Value.ToString("yyyy-MM-dd"));
                ads.InsertDate = DateTime.Now;
                string msg = "";
                if (ads.TypeId == 0)
                    msg += " محل نمایش تبلیغ انتخاب نشده است. ";
                if (ads.Cover == Guid.Empty)
                    msg += "تصویر انتخاب نشده است.";
                if (msg == "" && ModelState.IsValid)
                {

                    if (uow.AdverestingRepository.Get(x => x).Any())
                        ads.DisplaySort = uow.AdverestingRepository.Max(x => x.DisplaySort) + 1;
                    uow.AdverestingRepository.Insert(ads);
                    uow.Save();


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "adveresting", "Create", false, 200, "   ایجاد تبلیغ " + ads.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");

                }

                ViewBag.Error = msg;


                if (ModulePermission.check(User.Identity.GetUserId(), 23, 1))
                {
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");


                    List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نمایید--", Value = "0" }, new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1" }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "7" }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "8" }, new SelectListItem() { Text = "دسته بندی ها", Value = "2" }, new SelectListItem() { Text = "برچسب ها", Value = "3" }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "4" }, new SelectListItem() { Text = "گالری", Value = "5" }, new SelectListItem() { Text = "جست و جوی محصولات", Value = "6" } };
                    ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;


                    List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--همه صفحات--", Value = "0" }, };
                    ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;


                    List<SelectListItem> SizeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تمام صفحه", Value = "1", Selected = ads.AdverestingSizeId == 1 ? true : false }, new SelectListItem() { Text = "75% صفحه", Value = "2", Selected = ads.AdverestingSizeId == 2 ? true : false }, new SelectListItem() { Text = "66% صفحه", Value = "3", Selected = ads.AdverestingSizeId == 3 ? true : false }, new SelectListItem() { Text = "50% صفحه", Value = "4", Selected = ads.AdverestingSizeId == 4 ? true : false }, new SelectListItem() { Text = "33% صفحه", Value = "5", Selected = ads.AdverestingSizeId == 5 ? true : false }, new SelectListItem() { Text = "25% صفحه", Value = "6", Selected = ads.AdverestingSizeId == 6 ? true : false }, new SelectListItem() { Text = "20% صفحه", Value = "7", Selected = ads.AdverestingSizeId == 7 ? true : false } };
                    ViewBag.AdverestingSizeId = SizeIdSelectListItem;
                    ViewBag.Position = new List<SelectListItem>() { new SelectListItem() { Text = "بالای صفحه", Value = "1", Selected = (ads.Position == 1 ? true : false) }, new SelectListItem() { Text = "راست صفحه", Value = "2", Selected = (ads.Position == 2 ? true : false) }, new SelectListItem() { Text = "پایین صفحه", Value = "3", Selected = (ads.Position == 3 ? true : false) }, new SelectListItem() { Text = "چپ صفحه", Value = "4", Selected = (ads.Position == 4 ? true : false) } };

                }

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 22 && x.Name == "ایجاد تبلیغ جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "adveresting", "Create", false, 500, "   خطا در ایجاد تبلیغ " + ads.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ads);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "adveresting", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Adverestings/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 23, 2))
                {

                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Adveresting Adveresting = uow.AdverestingRepository.Get(x => x, x => x.Id == id, null, "attachment").FirstOrDefault();
                    if (Adveresting == null)
                    {
                        return HttpNotFound();
                    }

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", Adveresting.LanguageId);


                    List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نمایید--", Value = "0", Selected = (Adveresting.TypeId == 0 ? true : false) }, new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1", Selected = (Adveresting.TypeId == 1 ? true : false) }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "7", Selected = (Adveresting.TypeId == 7 ? true : false) }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "8", Selected = (Adveresting.TypeId == 8 ? true : false) }, new SelectListItem() { Text = "دسته بندی ها", Value = "2", Selected = (Adveresting.TypeId == 2 ? true : false) }, new SelectListItem() { Text = "برچسب ها", Value = "3", Selected = (Adveresting.TypeId == 3 ? true : false) }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "4", Selected = (Adveresting.TypeId == 4 ? true : false) }, new SelectListItem() { Text = "گالری", Value = "5", Selected = (Adveresting.TypeId == 5 ? true : false) }, new SelectListItem() { Text = "جست و جوی محصولات", Value = "6", Selected = (Adveresting.TypeId == 6 ? true : false) } };
                    ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;

                    List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--همه صفحات--", Value = "0", Selected = (Adveresting.LinkId == 0 ? true : false) } };
                    ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;

                    List<SelectListItem> SizeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تمام صفحه", Value = "1", Selected = Adveresting.AdverestingSizeId == 1 ? true : false }, new SelectListItem() { Text = "75% صفحه", Value = "2", Selected = Adveresting.AdverestingSizeId == 2 ? true : false }, new SelectListItem() { Text = "66% صفحه", Value = "3", Selected = Adveresting.AdverestingSizeId == 3 ? true : false }, new SelectListItem() { Text = "50% صفحه", Value = "4", Selected = Adveresting.AdverestingSizeId == 4 ? true : false }, new SelectListItem() { Text = "33% صفحه", Value = "5", Selected = Adveresting.AdverestingSizeId == 5 ? true : false }, new SelectListItem() { Text = "25% صفحه", Value = "6", Selected = Adveresting.AdverestingSizeId == 6 ? true : false }, new SelectListItem() { Text = "20% صفحه", Value = "7", Selected = Adveresting.AdverestingSizeId == 7 ? true : false } };
                    ViewBag.AdverestingSizeId = SizeIdSelectListItem;
                    ViewBag.Position = new List<SelectListItem>() { new SelectListItem() { Text = "بالای صفحه", Value = "1", Selected = (Adveresting.Position == 1 ? true : false) }, new SelectListItem() { Text = "راست صفحه", Value = "2", Selected = (Adveresting.Position == 2 ? true : false) }, new SelectListItem() { Text = "پایین صفحه", Value = "3", Selected = (Adveresting.Position == 3 ? true : false) }, new SelectListItem() { Text = "چپ صفحه", Value = "4", Selected = (Adveresting.Position == 4 ? true : false) } };


                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 22 && x.Name == "ایجاد تبلیغ جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Adveresting", "Edit", true, 200, " نمایش صفحه ویرایش تبلیغ " + Adveresting.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Adveresting);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تبلیغات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Adveresting", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Adverestings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit([Bind(Include = "Id,Title,InsertDate,DisplaySort,IsActive,LanguageId,TypeId,LinkId,ExpireDatestr,StartDate,Cover,AdverestingSizeId,Position,Link,TypeLink")] Adveresting ads)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                ads.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ads.Title);
                if (ads.ExpireDatestr != null)
                    ads.ExpireDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladi(ads.ExpireDatestr);
                if (ads.StartDate != null)
                    ads.StartDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladi(ads.StartDate.Value.ToString("yyyy-MM-dd"));
                if (ModelState.IsValid)
                {

                    uow.AdverestingRepository.Update(ads);
                    uow.Save();


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "adveresting", "Edit", false, 200, "   ویرایش تبلیغ " + ads.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", ads.LanguageId);



                List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نمایید--", Value = "0", Selected = (ads.TypeId == 0 ? true : false) }, new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1", Selected = (ads.TypeId == 1 ? true : false) }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "7", Selected = (ads.TypeId == 7 ? true : false) }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "8", Selected = (ads.TypeId == 8 ? true : false) }, new SelectListItem() { Text = "دسته بندی ها", Value = "2", Selected = (ads.TypeId == 2 ? true : false) }, new SelectListItem() { Text = "برچسب ها", Value = "3", Selected = (ads.TypeId == 3 ? true : false) }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "4", Selected = (ads.TypeId == 4 ? true : false) }, new SelectListItem() { Text = "گالری", Value = "5", Selected = (ads.TypeId == 5 ? true : false) }, new SelectListItem() { Text = "جست و جوی محصولات", Value = "6", Selected = (ads.TypeId == 6 ? true : false) } };
                ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;

                List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--همه صفحات--", Value = "0", Selected = (ads.LinkId == 0 ? true : false) } };
                ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;

                List<SelectListItem> SizeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تمام صفحه", Value = "1", Selected = ads.AdverestingSizeId == 1 ? true : false }, new SelectListItem() { Text = "75% صفحه", Value = "2", Selected = ads.AdverestingSizeId == 2 ? true : false }, new SelectListItem() { Text = "66% صفحه", Value = "3", Selected = ads.AdverestingSizeId == 3 ? true : false }, new SelectListItem() { Text = "50% صفحه", Value = "4", Selected = ads.AdverestingSizeId == 4 ? true : false }, new SelectListItem() { Text = "33% صفحه", Value = "5", Selected = ads.AdverestingSizeId == 5 ? true : false }, new SelectListItem() { Text = "25% صفحه", Value = "6", Selected = ads.AdverestingSizeId == 6 ? true : false }, new SelectListItem() { Text = "20% صفحه", Value = "7", Selected = ads.AdverestingSizeId == 7 ? true : false } };
                ViewBag.AdverestingSizeId = SizeIdSelectListItem;
                ViewBag.Position = new List<SelectListItem>() { new SelectListItem() { Text = "بالای صفحه", Value = "1", Selected = (ads.Position == 1 ? true : false) }, new SelectListItem() { Text = "راست صفحه", Value = "2", Selected = (ads.Position == 2 ? true : false) }, new SelectListItem() { Text = "پایین صفحه", Value = "3", Selected = (ads.Position == 3 ? true : false) }, new SelectListItem() { Text = "چپ صفحه", Value = "4", Selected = (ads.Position == 4 ? true : false) } };



                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 22 && x.Name == "ایجاد تبلیغ جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "adveresting", "Edit", false, 500, "   خطا در ویرایش تبلیغِ " + ads.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ads);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "adveresting", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Adverestings/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModulePermission.check(User.Identity.GetUserId(), 23, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Adveresting Adveresting = uow.AdverestingRepository.GetByID(id);
                    if (Adveresting == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Adveresting", "Delete", true, 200, " نمایش صفحه حذف تبلیغِ " + Adveresting.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Adveresting);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تبلیغات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Adveresting", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Adverestings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                Adveresting Adveresting = uow.AdverestingRepository.GetByID(id);
                uow.AdverestingRepository.Delete(Adveresting);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Adveresting", "DeleteConfirmed", false, 200, "   حذف تبلیغِ " + Adveresting.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Adveresting", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult AdverestingLog(int? id, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            if (id.HasValue)
            {
                ViewBag.ads = uow.AdverestingRepository.Get(x => x, x => x.Id == id.Value, null, "AdverestingLogs").SingleOrDefault();
                if (ViewBag.ads != null)
                {

                    var p = ModulePermission.check(User.Identity.GetUserId(), 23);
                    if (p.Where(x => x == true).Any())
                    {
                        var adsLog = uow.AdverestingLogRepository.Get(x => x, x => x.AdId == id.Value);


                        int pageSize = 20;
                        int pageNumber = (page ?? 1);
                        return View(adsLog.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
                    }
                    else
                        return Redirect("~/");
                    ;
                }
                else
                    return Redirect("~/");
            }
            else
                return Redirect("~/");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
