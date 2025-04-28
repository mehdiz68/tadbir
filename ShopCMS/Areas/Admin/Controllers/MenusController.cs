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
using System.Text.RegularExpressions;
using PagedList;
using CoreLib.ViewModel.Xml;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class MenusController : Controller
    {
        UnitOfWork.UnitOfWorkClass uow;

        // GET: Admin/Menus
        public virtual ActionResult Index(string sortOrder, string PlaceShowString, string PlaceShowFilter, string LanguagenameString, string LanguagenameFilter, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 6);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameSelectListItem = new SelectList(readXml.ListOfXLanguage(), "Id", "Name");
                    List<SelectListItem> PlaceShowSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "هدر (بالا) سایت", Value = "1" }, new SelectListItem() { Text = "فوتر (پایین) سایت", Value = "2" }, new SelectListItem() { Text = "هر دو", Value = "3" }, new SelectListItem() { Text = "وبلاگ بالا", Value = "4" }, new SelectListItem() { Text = "وبلاگ چپ", Value = "5" }, new SelectListItem() { Text = "وبلاگ هر دو", Value = "6" }, new SelectListItem() { Text = "ویدئو بالا", Value = "7" }, new SelectListItem() { Text = "ویدئو پایین", Value = "8" }, new SelectListItem() { Text = "ویدئو هر دو", Value = "9" } };
                    ViewBag.PlaceShow = PlaceShowSelectListItem;


                    var menus = uow.MenuRepository.GetByReturnQueryable(x => x, null, null, "ParrentMenu");


                    #region search
                    if (string.IsNullOrEmpty(PlaceShowString))
                        PlaceShowString = PlaceShowFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;
                    else
                        page = 1;



                    ViewBag.PlaceShowFilter = PlaceShowString;
                    ViewBag.LanguagenameFilter = LanguagenameString;


                    if (!String.IsNullOrEmpty(PlaceShowString))
                    {
                        int plcId = Convert.ToInt32(PlaceShowString);
                        menus = menus.Where(s => s.PlaceShow == plcId);
                    }
                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int langId = Convert.ToInt32(LanguagenameString);
                        menus = menus.Where(s => s.LanguageId == langId);
                    }


                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "PlaceShow":
                            menus = menus.OrderBy(s => s.PlaceShow);
                            ViewBag.CurrentSort = "PlaceShow";
                            break;
                        case "PlaceShow_desc":
                            menus = menus.OrderByDescending(s => s.PlaceShow);
                            ViewBag.CurrentSort = "PlaceShow_desc";
                            break;
                        case "Language":
                            menus = menus.OrderBy(s => s.LanguageId);
                            ViewBag.menus = "Language";
                            break;
                        case "Language_desc":
                            menus = menus.OrderByDescending(s => s.LanguageId);
                            ViewBag.CurrentSort = "Language_desc";
                            break;

                        default:  // Name ascending 
                            menus = menus.OrderByDescending(x => x.Id);
                            break;
                    }

                    #endregion

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);
                    ViewBag.RootMenu = menus.Where(x => x.ParrentMenu == null).Include(x => x.ChildMenu).OrderBy(x => x.DisplaySort).ThenBy(x => x.LanguageId).ToList();


                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "مدیریت منو", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Menus", "Index", true, 200, " نمایش صفحه مدیریت منو", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(menus.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت منو" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Menus/Sort
        public virtual ActionResult Sort(int? id, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 6);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    if (id.HasValue)
                        ViewBag.ParrentMenu = uow.MenuRepository.Get(x => x, x => x.Id == id.Value, null, "ParrentMenu").FirstOrDefault();

                    var menus = uow.MenuRepository.Get(x => x, x => x.MenuID == id, o => o.OrderBy(x => x.DisplaySort), "attachment,ParrentMenu");

                    int pageSize = 100;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 8 && x.Name == "مرتب سازی").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Menus", "Sort", true, 200, " نمایش صفحه مرتب سازی دسته بندی منو ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(menus.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت منو" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "Sort", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Menus/Sort/5
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
                    var menu = uow.MenuRepository.GetByID(id);
                    menu.DisplaySort = i;
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "Menus", "Sort", false, 200, "مرتب سازی منو", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Create 

        public virtual JsonResult GetContent(int TypeId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                List<SelectListItem> Contents = new List<SelectListItem>();

                switch (TypeId)
                {
                    case 1:
                        var q = uow.ContentRepository.Get(x => x, x => x.IsActive == true).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 2:
                        var q2 = uow.AttachmentRepository.Get(x => x, x => x.IsActive == true).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q2)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 3:
                        var q3 = uow.CategoryRepository.Get(x => x, x => x.IsActive == true).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q3)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 4:
                        var q4 = uow.TagRepository.GetByReturnQueryable(x => x).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q4)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.TagName;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 5:
                        var q5 = uow.SliderRepository.Get(x => x, x => x.IsActive == true).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q5)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 6:
                        var q6 = uow.SocialRepository.Get(x => x, x => x.IsActive == true).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q6)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 7:
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        var q7 = readXML.ListOfXContentType().Where(x => x.Id != 0);
                        foreach (var item in q7)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Name;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 8:
                    case 13:
                        var q8 = uow.ProductCategoryRepository.Get(x => x, x => x.IsActive == true).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q8)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 11:
                        var q11 = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.ContentTypeId == 0).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q11)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
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
            try
            {
                List<SelectListItem> Contents = new List<SelectListItem>();

                switch (TypeId)
                {
                    case 1:
                        int linkid = Convert.ToInt32(LinkId);
                        var q = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 2:
                        Guid linkidAttachement = new Guid(LinkId);
                        var q2 = uow.AttachmentRepository.Get(x => x, x => x.IsActive == true && x.Id == linkidAttachement).Take(10).OrderByDescending(x => x.Id);
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
                        var q3 = uow.CategoryRepository.Get(x => x, x => x.IsActive == true && x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q3)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 4:
                        linkid = Convert.ToInt32(LinkId);
                        var q4 = uow.TagRepository.Get(x => x, x => x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q4)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.TagName;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 5:
                        linkid = Convert.ToInt32(LinkId);
                        var q5 = uow.SliderRepository.Get(x => x, x => x.IsActive == true && x.LinkId == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q5)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 6:
                        linkid = Convert.ToInt32(LinkId);
                        var q6 = uow.SocialRepository.Get(x => x, x => x.IsActive == true && x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q6)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 7:
                        linkid = Convert.ToInt32(LinkId);
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        var q7 = readXML.ListOfXContentType().Where(x => x.Id == linkid);
                        foreach (var item in q7)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Name;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 8:
                    case 13:
                        linkid = Convert.ToInt32(LinkId);
                        var q8 = uow.ProductCategoryRepository.Get(x => x, x => x.IsActive == true && x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q8)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 11:
                        linkid = Convert.ToInt32(LinkId);
                        var q11 = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.ContentTypeId == 0 && x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q11)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
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
            try
            {
                List<SelectListItem> Contents = new List<SelectListItem>();
                switch (TypeId)
                {
                    case 1:
                        var q = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 2:
                        var q2 = uow.AttachmentRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q2)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 3:
                        var q3 = uow.CategoryRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q3)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 4:
                        var q4 = uow.TagRepository.Get(x => x, x => x.TagName.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q4)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.TagName;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 5:
                        var q5 = uow.SliderRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q5)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 6:
                        var q6 = uow.SocialRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q6)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 7:
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        var q7 = readXML.ListOfXContentType().Where(x => x.Title.Contains(Keyword)).Where(x => x.Id != 0);
                        foreach (var item in q7)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Name;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 8:
                    case 13:
                        var q8 = uow.ProductCategoryRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q8)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 11:
                        var q11 = uow.ContentRepository.Get(x => x, x => x.IsActive == true && x.ContentTypeId == 0 && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q11)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
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

        private static Regex isGuid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);
        internal static bool IsGuid(string candidate, out Guid output)
        {
            bool isValid = false;
            output = Guid.Empty;

            if (candidate != null)
            {

                if (isGuid.IsMatch(candidate))
                {
                    output = new Guid(candidate);
                    isValid = true;
                }
            }

            return isValid;
        }

        // GET: Admin/Menus/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                #region Check License
                IdentityManager im = new IdentityManager();


                #endregion

                if (ModulePermission.check(User.Identity.GetUserId(), 6, 1))
                {
                    XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXml.ListOfXLanguage(), "Id", "Name");


                    List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1" }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "11" }, new SelectListItem() { Text = "فایل ها", Value = "2" }, new SelectListItem() { Text = "دسته بندی ها", Value = "3" }, new SelectListItem() { Text = "برچسب ها", Value = "4" }, new SelectListItem() { Text = "شبکه های اجتماعی", Value = "6" }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "7" }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "8" }, new SelectListItem() { Text = "گالری", Value = "9" }, new SelectListItem() { Text = "جستجوی دسته بندی محصولات", Value = "13" } };
                    ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;



                    List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نشده--", Value = "" } };
                    ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;

                    List<SelectListItem> PlaceShowSelectListItem = new List<SelectListItem>();
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "هدر (بالا) سایت";
                        sli.Value = "1";
                        PlaceShowSelectListItem.Add(sli);
                    }
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "فوتر (پایین) سایت";
                        sli.Value = "2";
                        PlaceShowSelectListItem.Add(sli);
                    }
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "هر دو";
                        sli.Value = "3";
                        PlaceShowSelectListItem.Add(sli);
                    }
                  
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "ویدئو بالا";
                        sli.Value = "7";
                        PlaceShowSelectListItem.Add(sli);
                    }
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "ویدئو پایین";
                        sli.Value = "8";
                        PlaceShowSelectListItem.Add(sli);
                    }
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "ویدئو هردو";
                        sli.Value = "9";
                        PlaceShowSelectListItem.Add(sli);
                    }
                    ViewBag.PlaceShow = PlaceShowSelectListItem;
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 8 && x.Name == "ایجاد منوی اصلی", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Home", "Create", true, 200, " نمایش صفحه ایجاد منو", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت منو" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Menus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create( Menu menu, string LinkIdSelectListItem)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 8 && x.Name == "ایجاد منوی اصلی", null, "HelpModuleSectionFields").FirstOrDefault();
                if (ModelState.IsValid)
                {
                    menu.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(menu.Title);
                    bool isvalid = true;
                    if (!menu.TypeId.HasValue)
                    {
                        if (String.IsNullOrEmpty(menu.OffLink))
                            isvalid = false;
                    }
                    else
                    {
                        if (menu.TypeId.Value != 9 && String.IsNullOrEmpty(LinkIdSelectListItem))
                            isvalid = false;
                    }
                    if (isvalid)
                    {
                        if (!String.IsNullOrEmpty(LinkIdSelectListItem))
                        {
                            Guid GuidId;
                            if (IsGuid(LinkIdSelectListItem, out GuidId))
                                menu.LinkUniqIdentifier = GuidId;
                            else
                                menu.LinkId = Convert.ToInt32(LinkIdSelectListItem);
                        }
                        else if (menu.TypeId.HasValue)
                        {
                            if (menu.TypeId.Value == 9)
                                menu.LinkId = 0;
                        }
                        else
                        {
                            menu.OffLink = menu.OffLink;
                        }

                        if (uow.MenuRepository.Get(x => x).Any())
                            menu.DisplaySort = uow.MenuRepository.Max(x => x.DisplaySort) + 1;
                        uow.MenuRepository.Insert(menu);
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "Menus", "Create", false, 200, "   ایجاد منویِ" + menu.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = " از بین لینک داخلی و لینک خارجی باید یکی را وارد نمایید. ";
                    }
                }

                XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXml.ListOfXLanguage(), "Id", "Name");



                List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1", Selected = (menu.TypeId == 1 ? true : false) }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "11", Selected = (menu.TypeId == 11 ? true : false) }, new SelectListItem() { Text = "فایل ها", Value = "2", Selected = (menu.TypeId == 2 ? true : false) }, new SelectListItem() { Text = "دسته بندی ها", Value = "3", Selected = (menu.TypeId == 3 ? true : false) }, new SelectListItem() { Text = "برچسب ها", Value = "4", Selected = (menu.TypeId == 4 ? true : false) }, new SelectListItem() { Text = "شبکه های اجتماعی", Value = "6", Selected = (menu.TypeId == 6 ? true : false) }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "7", Selected = (menu.TypeId == 7 ? true : false) }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "8", Selected = (menu.TypeId == 8 ? true : false) }, new SelectListItem() { Text = "گالری", Value = "9", Selected = (menu.TypeId == 9 ? true : false) }, new SelectListItem() { Text = "جستجوی دسته بندی محصولات", Value = "13" } };
                ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;


                List<SelectListItem> LinkId = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نشده--", Value = "" } };
                ViewBag.LinkIdSelectListItem = LinkId;


                List<SelectListItem> PlaceShowSelectListItem = new List<SelectListItem>();
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "هدر (بالا) سایت";
                    sli.Value = "1";
                    PlaceShowSelectListItem.Add(sli);
                }
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "فوتر (پایین) سایت";
                    sli.Value = "2";
                    PlaceShowSelectListItem.Add(sli);
                }
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "هر دو";
                    sli.Value = "3";
                    PlaceShowSelectListItem.Add(sli);
                }
               
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "ویدئو بالا";
                    sli.Value = "7";
                    PlaceShowSelectListItem.Add(sli);
                }
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "ویدئو پایین";
                    sli.Value = "8";
                    PlaceShowSelectListItem.Add(sli);
                }
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "ویدئو هردو";
                    sli.Value = "9";
                    PlaceShowSelectListItem.Add(sli);
                }
                ViewBag.PlaceShow = PlaceShowSelectListItem;


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "Create", false, 500, "   خطا در ایجاد منویِ" + menu.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View(menu);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Menus/Create
        public virtual ActionResult SubCreate(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                #region Check License
                IdentityManager im = new IdentityManager();


                #endregion

                if (ModulePermission.check(User.Identity.GetUserId(), 6, 1))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Menu menu = uow.MenuRepository.GetByID(id);
                    if (menu == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.MenuId = menu.Id;
                    ViewBag.MenuTitle = menu.Title;

                    XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXml.ListOfXLanguage(), "Id", "Name");

                    List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1" }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "11" }, new SelectListItem() { Text = "فایل ها", Value = "2" }, new SelectListItem() { Text = "دسته بندی ها", Value = "3" }, new SelectListItem() { Text = "برچسب ها", Value = "4" }, new SelectListItem() { Text = "شبکه های اجتماعی", Value = "6" }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "7" }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "8" }, new SelectListItem() { Text = "گالری", Value = "9" }, new SelectListItem() { Text = "جستجوی دسته بندی محصولات", Value = "13" } };
                    ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;


                    List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نشده--", Value = "" } };
                    ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;


                    List<SelectListItem> PlaceShowSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "هدر (بالا) سایت", Value = "1" }, new SelectListItem() { Text = "فوتر (پایین) سایت", Value = "2" }, new SelectListItem() { Text = "هر دو", Value = "3" }, new SelectListItem() { Text = "وبلاگ بالا", Value = "4" }, new SelectListItem() { Text = "وبلاگ چپ", Value = "5" }, new SelectListItem() { Text = "وبلاگ هر دو", Value = "6" }, new SelectListItem() { Text = "ویدئو بالا", Value = "7" }, new SelectListItem() { Text = "ویدئو پایین", Value = "8" }, new SelectListItem() { Text = "ویدئو هر دو", Value = "9" } };
                    ViewBag.PlaceShow = PlaceShowSelectListItem;

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 8 && x.Name == "ایجاد منوی اصلی", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Home", "SubCreate", true, 200, " نمایش صفحه ایجاد زیر منو", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت منو" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "SubCreate", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Menus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SubCreate( Menu menu, string LinkIdSelectListItem, string MenuId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModelState.IsValid)
                {
                    menu.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(menu.Title);
                    bool isvalid = true;
                    if (!menu.TypeId.HasValue)
                    {
                        if (String.IsNullOrEmpty(menu.OffLink))
                            isvalid = false;
                    }
                    else
                    {
                        if (menu.TypeId.Value != 9 && String.IsNullOrEmpty(LinkIdSelectListItem))
                            isvalid = false;
                    }

                    if (isvalid)
                    {
                        if (!String.IsNullOrEmpty(LinkIdSelectListItem))
                        {
                            Guid GuidId;
                            if (IsGuid(LinkIdSelectListItem, out GuidId))
                                menu.LinkUniqIdentifier = GuidId;
                            else
                                menu.LinkId = Convert.ToInt32(LinkIdSelectListItem);
                        }
                        else if (menu.TypeId.HasValue)
                        {
                            if (menu.TypeId.Value == 9)
                                menu.LinkId = 0;
                        }
                        else
                        {
                            menu.OffLink = menu.OffLink;
                        }

                        if (uow.MenuRepository.Get(x => x).Any())
                            menu.DisplaySort = uow.MenuRepository.Max(x => x.DisplaySort) + 1;
                        menu.ParrentMenu = uow.MenuRepository.GetByID(Convert.ToInt32(MenuId));
                        menu.MenuID = Convert.ToInt32(MenuId);
                        uow.MenuRepository.Insert(menu);
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "Menus", "SubCreate", false, 200, "   ایجاد زیر منویِ" + menu.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = " از بین لینک داخلی و لینک خارجی باید یکی را وارد نمایید. ";
                    }
                }

                XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXml.ListOfXLanguage(), "Id", "Name");



                List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1", Selected = (menu.TypeId == 1 ? true : false) }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "11", Selected = (menu.TypeId == 11 ? true : false) }, new SelectListItem() { Text = "فایل ها", Value = "2", Selected = (menu.TypeId == 2 ? true : false) }, new SelectListItem() { Text = "دسته بندی ها", Value = "3", Selected = (menu.TypeId == 3 ? true : false) }, new SelectListItem() { Text = "برچسب ها", Value = "4", Selected = (menu.TypeId == 4 ? true : false) }, new SelectListItem() { Text = "شبکه های اجتماعی", Value = "6", Selected = (menu.TypeId == 6 ? true : false) }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "7", Selected = (menu.TypeId == 7 ? true : false) }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "8", Selected = (menu.TypeId == 8 ? true : false) }, new SelectListItem() { Text = "گالری", Value = "9", Selected = (menu.TypeId == 9 ? true : false) }, new SelectListItem() { Text = "جستجوی دسته بندی محصولات", Value = "13", Selected = menu.TypeId == 13 ? true : false } };
                ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;

                List<SelectListItem> LinkId = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نشده--", Value = "" } };
                ViewBag.LinkIdSelectListItem = LinkId;


                List<SelectListItem> PlaceShowSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "هدر (بالا) سایت", Value = "1" }, new SelectListItem() { Text = "فوتر (پایین) سایت", Value = "2" }, new SelectListItem() { Text = "هر دو", Value = "3" }, new SelectListItem() { Text = "وبلاگ بالا", Value = "4" }, new SelectListItem() { Text = "وبلاگ چپ", Value = "5" }, new SelectListItem() { Text = "وبلاگ هر دو", Value = "6" }, new SelectListItem() { Text = "ویدئو بالا", Value = "7" }, new SelectListItem() { Text = "ویدئو پایین", Value = "8" }, new SelectListItem() { Text = "ویدئو هر دو", Value = "9" } };
                ViewBag.PlaceShow = PlaceShowSelectListItem;

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 8 && x.Name == "ایجاد منوی اصلی", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "Menus", "SubCreate", false, 500, "   خطا در ایجاد زیر منویِ" + menu.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(menu);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "SubCreate", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        #endregion

        // GET: Admin/Menus/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 8 && x.Name == "ایجاد منوی اصلی", null, "HelpModuleSectionFields").FirstOrDefault();

                if (ModulePermission.check(User.Identity.GetUserId(), 6, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Menu menu = uow.MenuRepository.Get(x=>x,x=>x.Id==id,null, "attachment,Homeattachment").Single();



                    if (menu == null)
                    {
                        return HttpNotFound();
                    }
                    XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXml.ListOfXLanguage(), "Id", "Name");


                    List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1" }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "11", Selected = (menu.TypeId == 11 ? true : false) }, new SelectListItem() { Text = "فایل ها", Value = "2" }, new SelectListItem() { Text = "دسته بندی ها", Value = "3" }, new SelectListItem() { Text = "برچسب ها", Value = "4" }, new SelectListItem() { Text = "شبکه های اجتماعی", Value = "6" }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "7" }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "8" }, new SelectListItem() { Text = "گالری", Value = "9" }, new SelectListItem() { Text = "جستجوی دسته بندی محصولات", Value = "13", Selected = menu.TypeId == 13 ? true : false } };
                    ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;


                    if (menu.MenuID.HasValue)
                    {
                        List<SelectListItem> MenuID = new List<SelectListItem>();
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "--ندارد--";
                        sli.Value = "";
                        MenuID.Add(sli);
                        foreach (Menu item in uow.MenuRepository.Get(x => x, x => x.IsActive && x.Id != menu.Id))
                        {
                            SelectListItem sli2 = new SelectListItem();
                            sli2.Text = item.Title;
                            sli2.Value = item.Id.ToString();
                            if (item.Id == menu.MenuID)
                                sli2.Selected = true;
                            MenuID.Add(sli2);
                        }
                        ViewBag.MenuID = MenuID;
                    }
                    else
                    {
                        List<SelectListItem> MenuID = new List<SelectListItem>();
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "--ندارد--";
                        sli.Value = "";
                        sli.Selected = true;
                        MenuID.Add(sli);
                        foreach (Menu item in uow.MenuRepository.Get(x => x, x => x.IsActive && x.Id != menu.Id))
                        {
                            SelectListItem sli2 = new SelectListItem();
                            sli2.Text = item.Title;
                            sli2.Value = item.Id.ToString();
                            MenuID.Add(sli2);
                        }
                        ViewBag.MenuID = MenuID;
                    }
                    List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نشده--", Value = "" } };
                    ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;

                    List<SelectListItem> PlaceShowSelectListItem = new List<SelectListItem>();
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "هدر (بالا) سایت";
                        sli.Value = "1";
                        sli.Selected = (menu.PlaceShow == 1 ? true : false);
                        PlaceShowSelectListItem.Add(sli);
                    }
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "فوتر (پایین) سایت";
                        sli.Value = "2";
                        sli.Selected = (menu.PlaceShow == 2 ? true : false);
                        PlaceShowSelectListItem.Add(sli);
                    }
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "هر دو";
                        sli.Value = "3";
                        sli.Selected = (menu.PlaceShow == 3 ? true : false);
                        PlaceShowSelectListItem.Add(sli);
                    }
               
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "ویدئو بالا";
                        sli.Value = "7";
                        sli.Selected = (menu.PlaceShow == 7 ? true : false);
                        PlaceShowSelectListItem.Add(sli);
                    }
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "ویدئو پایین";
                        sli.Value = "8";
                        sli.Selected = (menu.PlaceShow == 8 ? true : false);
                        PlaceShowSelectListItem.Add(sli);
                    }
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Text = "ویدئو هردو";
                        sli.Value = "9";
                        sli.Selected = (menu.PlaceShow == 9 ? true : false);
                        PlaceShowSelectListItem.Add(sli);
                    }
                    ViewBag.PlaceShow = PlaceShowSelectListItem;

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Menus", "Edit", true, 200, " نمایش صفحه ویرایش منوی " + menu.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(menu);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت منو" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Menus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit( Menu menu, string LinkIdSelectListItem)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 8 && x.Name == "ایجاد منوی اصلی", null, "HelpModuleSectionFields").FirstOrDefault();
                bool isvalid = true;
                if (!menu.TypeId.HasValue)
                {
                    if (String.IsNullOrEmpty(menu.OffLink))
                        isvalid = false;
                }
                else
                {
                    if (menu.TypeId.Value != 9 && String.IsNullOrEmpty(LinkIdSelectListItem))
                        isvalid = false;
                }

                if (isvalid)
                {
                    if (ModelState.IsValid)
                    {
                        if (!String.IsNullOrEmpty(LinkIdSelectListItem))
                        {
                            Guid GuidId;
                            if (IsGuid(LinkIdSelectListItem, out GuidId))
                                menu.LinkUniqIdentifier = GuidId;
                            else
                                menu.LinkId = Convert.ToInt32(LinkIdSelectListItem);
                        }
                        else if (menu.TypeId.HasValue)
                        {
                            if (menu.TypeId.Value == 9)
                                menu.LinkId = 0;
                        }
                        else
                        {
                            menu.OffLink = menu.OffLink;
                        }
                        menu.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(menu.Title);
                        uow.MenuRepository.Update(menu);
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(3, "Menus", "Edit", false, 200, "   ویرایش منویِ" + menu.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.Error = " از بین لینک داخلی و لینک خارجی باید یکی را وارد نمایید. ";
                }

                XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXml.ListOfXLanguage(), "Id", "Name");

                #region Check License
                IdentityManager im = new IdentityManager();


                #endregion

                List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1", Selected = (menu.TypeId == 1 ? true : false) }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "11", Selected = (menu.TypeId == 11 ? true : false) }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "10", Selected = (menu.TypeId == 10 ? true : false) }, new SelectListItem() { Text = "فایل ها", Value = "2", Selected = (menu.TypeId == 2 ? true : false) }, new SelectListItem() { Text = "دسته بندی ها", Value = "3", Selected = (menu.TypeId == 3 ? true : false) }, new SelectListItem() { Text = "برچسب ها", Value = "4", Selected = (menu.TypeId == 4 ? true : false) }, new SelectListItem() { Text = "شبکه های اجتماعی", Value = "6", Selected = (menu.TypeId == 6 ? true : false) }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "7", Selected = (menu.TypeId == 7 ? true : false) }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "8", Selected = (menu.TypeId == 8 ? true : false) }, new SelectListItem() { Text = "گالری", Value = "9", Selected = (menu.TypeId == 9 ? true : false) }, new SelectListItem() { Text = "جستجوی دسته بندی محصولات", Value = "13", Selected = menu.TypeId == 13 ? true : false } };
                ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;



                if (menu.MenuID.HasValue)
                {
                    List<SelectListItem> MenuID = new List<SelectListItem>();
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "--ندارد--";
                    sli.Value = "";
                    MenuID.Add(sli);
                    foreach (Menu item in uow.MenuRepository.Get(x => x, x => x.IsActive && x.Id != menu.Id))
                    {
                        SelectListItem sli2 = new SelectListItem();
                        sli2.Text = item.Title;
                        sli2.Value = item.Id.ToString();
                        if (item.Id == menu.MenuID)
                            sli2.Selected = true;
                        MenuID.Add(sli2);
                    }
                    ViewBag.MenuID = MenuID;
                }
                else
                {
                    List<SelectListItem> MenuID = new List<SelectListItem>();
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "--ندارد--";
                    sli.Value = "";
                    sli.Selected = true;
                    MenuID.Add(sli);
                    foreach (Menu item in uow.MenuRepository.Get(x => x, x => x.IsActive && x.Id != menu.Id))
                    {
                        SelectListItem sli2 = new SelectListItem();
                        sli2.Text = item.Title;
                        sli2.Value = item.Id.ToString();
                        MenuID.Add(sli2);
                    }
                    ViewBag.MenuID = MenuID;
                }
                List<SelectListItem> LinkId = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نشده--", Value = "" } };
                ViewBag.LinkIdSelectListItem = LinkId;


                List<SelectListItem> PlaceShowSelectListItem = new List<SelectListItem>();
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "هدر (بالا) سایت";
                    sli.Value = "1";
                    sli.Selected = (menu.PlaceShow == 1 ? true : false);
                    PlaceShowSelectListItem.Add(sli);
                }
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "فوتر (پایین) سایت";
                    sli.Value = "2";
                    sli.Selected = (menu.PlaceShow == 2 ? true : false);
                    PlaceShowSelectListItem.Add(sli);
                }
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "هر دو";
                    sli.Value = "3";
                    sli.Selected = (menu.PlaceShow == 3 ? true : false);
                    PlaceShowSelectListItem.Add(sli);
                }
               
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "ویدئو بالا";
                    sli.Value = "7";
                    sli.Selected = (menu.PlaceShow == 7 ? true : false);
                    PlaceShowSelectListItem.Add(sli);
                }
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "ویدئو پایین";
                    sli.Value = "8";
                    sli.Selected = (menu.PlaceShow == 8 ? true : false);
                    PlaceShowSelectListItem.Add(sli);
                }
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Text = "ویدئو هردو";
                    sli.Value = "9";
                    sli.Selected = (menu.PlaceShow == 9 ? true : false);
                    PlaceShowSelectListItem.Add(sli);
                }
                ViewBag.PlaceShow = PlaceShowSelectListItem;

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "Edit", false, 500, "   خطا در ویرایش منویِ" + menu.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(menu);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Menus/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 6, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Menu menu = uow.MenuRepository.GetByID(id);

                    if (menu == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Menus", "Delete", true, 200, " نمایش صفحه حذف منوی " + menu.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(menu);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت منو" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Menus", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                Menu menu = uow.MenuRepository.GetByID(id);
                uow.MenuRepository.Delete(menu);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Menus", "DeleteConfirmed", false, 200, "   حذف منوی " + menu.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Menus", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                Menu menu = uow.MenuRepository.GetByID(id);
                ViewBag.Erorr = "این منو ، دارای زیر منو های دیگری است. ابتدا باید آنها را پاک نمایید.";
                return View(menu);
            }
        }

        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }
    }
}
