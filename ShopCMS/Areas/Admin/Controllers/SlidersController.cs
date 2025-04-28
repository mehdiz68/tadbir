using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using ahmadi.ViewModels.Slider;
using System.Collections.Generic;
using CoreLib.ViewModel.Xml;
using CoreLib.Infrastructure.ModelBinder;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class SlidersController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uof = null;

        // GET: Admin/Sliders
        [CorrectArabianLetter(new string[] { "TitleString", "TitleFilter" })]
        public virtual ActionResult Index(string sortOrder, string TitleFilter, string TitleString, string LanguagenameFilter, string LanguagenameString)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 6);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                    #region search
                    if (string.IsNullOrEmpty(TitleString))
                        TitleString = TitleFilter;
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;

                    ViewBag.TitleFilter = TitleString;
                    ViewBag.LanguagenameFilter = LanguagenameString;

                    var sliders = uof.SliderRepository.GetByReturnQueryable(x => x);
                    if (!String.IsNullOrEmpty(TitleString))
                        sliders = sliders.Where(s => s.Title.Contains(TitleString));
                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int langId = Convert.ToInt32(LanguagenameString);
                        sliders = sliders.Where(s => s.LanguageId == langId);
                    }

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "Title":
                            sliders = sliders.OrderBy(s => s.Title);
                            ViewBag.CurrentSort = "Title";
                            break;
                        case "Title_desc":
                            sliders = sliders.OrderByDescending(s => s.Title);
                            ViewBag.CurrentSort = "Title_desc";
                            break;
                        case "Languagename":
                            sliders = sliders.OrderBy(s => s.LanguageId);
                            ViewBag.CurrentSort = "Languagename";
                            break;
                        case "Languagename_desc":
                            sliders = sliders.OrderByDescending(s => s.LanguageId);
                            ViewBag.CurrentSort = "Languagename_desc";
                            break;
                        case "DisplaySort":
                            sliders = sliders.OrderBy(s => s.DisplaySort);
                            ViewBag.CurrentSort = "DisplaySort";
                            break;
                        case "DisplaySort_desc":
                            sliders = sliders.OrderByDescending(s => s.DisplaySort);
                            ViewBag.CurrentSort = "DisplaySort_desc";
                            break;
                        case "IsActive":
                            sliders = sliders.OrderBy(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive";
                            break;
                        case "IsActive_desc":
                            sliders = sliders.OrderByDescending(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive_desc";
                            break;

                        default:  // Name ascending 
                            sliders = sliders.OrderBy(s => s.DisplaySort);
                            break;
                    }
                    #endregion


                    ViewBag.HelpModule = uof.HelpModuleRepository.Get(x => x, x => x.Name == "اسلایدر", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Sliders", "Index", true, 200, " نمایش صفحه مدیریت اسلایدر", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(sliders.ToList());
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت اسلایدرها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Sliders", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Categories/Sort/5
        [HttpPost]
        public virtual JsonResult Sort(string ids)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var slider = uof.SliderRepository.GetByID(id);
                    slider.DisplaySort = i;
                    uof.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "Sliders", "Sort", true, 200, " نمایش صفحه مرتب سازی اسلایدر", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Sliders", "Sort", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public virtual JsonResult GetContent(int TypeId)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                List<SelectListItem> Contents = new List<SelectListItem>();

                switch (TypeId)
                {
                    case 1:
                        var q = uof.ContentRepository.Get(x => x, x => x.IsActive == true && x.ContentTypeId > 0).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 2:
                        var q2 = uof.CategoryRepository.Get(x => x, x => x.IsActive == true).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q2)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 3:
                        var q3 = uof.TagRepository.Get(x => x).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q3)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.TagName;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 4:
                        var q4 = uof.ProductCategoryRepository.Get(x => x, x => x.IsActive == true).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q4)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 5:
                        var q5 = uof.GalleryCategoryRepository.Get(x => x, x => x.IsActive == true).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q5)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 6:
                        XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                        var q6 = readXml.ListOfXContentType().OrderByDescending(x => x.Id);
                        foreach (var item in q6)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Name;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 7:
                        var q7 = uof.ContentRepository.Get(x => x, x => x.IsActive == true && (x.ContentTypeId == 0 || x.ContentTypeId == 9)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q7)
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
            uof = new UnitOfWork.UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                List<SelectListItem> Contents = new List<SelectListItem>();

                switch (TypeId)
                {
                    case 1:
                        int linkid = Convert.ToInt32(LinkId);
                        var q = uof.ContentRepository.Get(x => x, x => x.IsActive == true && x.Id == linkid && x.ContentTypeId > 0).Take(10).OrderByDescending(x => x.Id);
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
                        var q2 = uof.CategoryRepository.Get(x => x, x => x.IsActive == true && x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
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
                        var q3 = uof.TagRepository.Get(x => x, x => x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q3)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.TagName;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 4:
                        linkid = Convert.ToInt32(LinkId);
                        var q4 = uof.ProductCategoryRepository.Get(x => x, x => x.IsActive == true && x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
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
                        var q5 = uof.GalleryCategoryRepository.Get(x => x, x => x.IsActive == true && x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
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
                        XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                        var q6 = readXml.ListOfXContentType().Where(x => x.Id == linkid).OrderByDescending(x => x.Id);
                        foreach (var item in q6)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Name;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 7:
                        linkid = Convert.ToInt32(LinkId);
                        var q7 = uof.ContentRepository.Get(x => x, x => x.IsActive == true && (x.ContentTypeId == 0 || x.ContentTypeId == 9) && x.Id == linkid).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q7)
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
            uof = new UnitOfWork.UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                List<SelectListItem> Contents = new List<SelectListItem>();
                switch (TypeId)
                {
                    case 1:
                        var q = uof.ContentRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword) && x.ContentTypeId > 0).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 2:
                        var q2 = uof.CategoryRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q2)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;

                    case 3:
                        var q3 = uof.TagRepository.Get(x => x, x => x.TagName.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q3)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.TagName;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 4:
                        var q4 = uof.ProductCategoryRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q4)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 5:
                        var q5 = uof.GalleryCategoryRepository.Get(x => x, x => x.IsActive == true && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q5)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Title;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 6:
                        XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                        var q6 = readXml.ListOfXContentType().Where(x => x.Title.Contains(Keyword)).OrderByDescending(x => x.Id);
                        foreach (var item in q6)
                        {
                            SelectListItem ListItemContents = new SelectListItem();
                            ListItemContents.Text = item.Name;
                            ListItemContents.Value = item.Id.ToString();
                            Contents.Add(ListItemContents);
                        }
                        break;
                    case 7:
                        var q7 = uof.ContentRepository.Get(x => x, x => x.IsActive == true && (x.ContentTypeId == 0 || x.ContentTypeId==9) && x.Title.Contains(Keyword)).Take(10).OrderByDescending(x => x.Id);
                        foreach (var item in q7)
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

        // GET: Admin/Sliders/Create
        public virtual ActionResult Create()
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 6, 1))
                {
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                    XMasterTheme master = readXML.ListOfXMasterTheme().First();

                    List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = (master.FooterSliderVisibility ? "--فوتر تمامی صفحات--" : "--انتخاب نشده--"), Value = "0" }, new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1" }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "7" }, new SelectListItem() { Text = "دسته بندی ها", Value = "2" }, new SelectListItem() { Text = "برچسب ها", Value = "3" }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "4" }, new SelectListItem() { Text = "گالری", Value = "5" }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "6" } };
                    ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;


                    List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = (master.FooterSliderVisibility ? "--فوتر تمامی صفحات--" : "--انتخاب نشده--"), Value = "0" }, };
                    ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;
                    Session["JanebiList"] = null;
                    Session["SliderImageLink"] = null;

                    ViewBag.HelpModuleSection = uof.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 9 && x.Name == "ایجاد اسلایدر جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Sliders", "Create", true, 200, " نمایش صفحه ایجاد اسلایدر", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت اسلایدرها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Sliders", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }

        // POST: Admin/Sliders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create([Bind(Include = "Id,Title,DisplaySort,IsActive,LanguageId,TypeId,LinkId")] Slider slider, string[] JanebiId, string[] JanebiTitle, string[] JanebiLink)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    slider.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(slider.Title);
                    if (uof.SliderRepository.Get(x => x).Any())
                        slider.DisplaySort = uof.SliderRepository.Max(x => x.DisplaySort) + 1;
                    #region Add OtherImage
                    List<OtherImageViewModel> JanebiList = new List<OtherImageViewModel>();
                    if (JanebiId != null)
                    {
                        slider.SliderImages = new List<SliderImage>();
                        for (int i = 0; i < JanebiId.Length; i++)
                        {
                            slider.SliderImages.Add(new SliderImage() { Title = JanebiTitle[i], DisplaySort = i, Cover = new Guid(JanebiId[i]), Link = JanebiLink[i] });
                        }
                    }
                    #endregion
                    uof.SliderRepository.Insert(slider);
                    uof.Save();



                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Sliders", "Create", false, 200, "   ایجاد اسلایدرِ " + slider.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }


                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", slider.LanguageId);

                #region Check License


                #endregion
                XMasterTheme master = readXML.ListOfXMasterTheme().First();

                List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = (master.FooterSliderVisibility ? "--فوتر تمامی صفحات--" : "--انتخاب نشده--"), Value = "0" }, new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1" }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "7" }, new SelectListItem() { Text = "دسته بندی ها", Value = "2" }, new SelectListItem() { Text = "برچسب ها", Value = "3" }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "4" }, new SelectListItem() { Text = "گالری", Value = "5" }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "6" } };
                ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;

                List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = (master.FooterSliderVisibility ? "--فوتر تمامی صفحات--" : "--انتخاب نشده--"), Value = "0" }, };
                ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;


                ViewBag.HelpModuleSection = uof.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 9 && x.Name == "ایجاد اسلایدر جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Sliders", "Create", false, 500, "   خطا در ایجاد اسلایدرِ " + slider.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(slider);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Sliders", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Sliders/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();

            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 6, 2))
                {

                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Slider slider = uof.SliderRepository.Get(x => x, x => x.Id == id, null, "SliderImages.attachment").SingleOrDefault();
                    if (slider == null)
                    {
                        return HttpNotFound();
                    }

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", slider.LanguageId);

                    XMasterTheme master = readXML.ListOfXMasterTheme().First();

                    List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = (master.FooterSliderVisibility ? "--فوتر تمامی صفحات--" : "--انتخاب نشده--"), Value = "0", Selected = (slider.TypeId == 0 ? true : false) }, new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1", Selected = (slider.TypeId == 1 ? true : false) }, new SelectListItem() { Text = "صفحات پایه ای سایت ( صفحه اصلی ، درباره ما ، تماس با ما و...)", Value = "7", Selected = (slider.TypeId == 7 ? true : false) }, new SelectListItem() { Text = "دسته بندی ها", Value = "2", Selected = (slider.TypeId == 2 ? true : false) }, new SelectListItem() { Text = "برچسب ها", Value = "3", Selected = (slider.TypeId == 3 ? true : false) }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "4", Selected = (slider.TypeId == 4 ? true : false) }, new SelectListItem() { Text = "گالری", Value = "5", Selected = (slider.TypeId == 5 ? true : false) }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "6", Selected = (slider.TypeId == 6 ? true : false) } };
                    ViewBag.TypeIdSelectListItem = TypeIdSelectListItem;

                    List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = (master.FooterSliderVisibility ? "--فوتر تمامی صفحات--" : "--انتخاب نشده--"), Value = "0", Selected = (slider.LinkId == 0 ? true : false) } };
                    ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;

                    Session["JanebiList"] = null;
                    Session["SliderImageLink"] = null;
                    #region Load Other Images
                    List<OtherImageViewModel> JanebiList = new List<OtherImageViewModel>();
                    foreach (SliderImage item in slider.SliderImages)
                    {
                        OtherImageViewModel oOtherImage = new OtherImageViewModel();
                        oOtherImage.Cover = item.Cover;
                        oOtherImage.DisplaySort = item.DisplaySort;
                        oOtherImage.Title = item.Title;
                        oOtherImage.Src = item.attachment.FileName;
                        oOtherImage.Link = item.Link;
                        JanebiList.Add(oOtherImage);
                    }
                    Session["JanebiList"] = JanebiList.OrderBy(x => x.Cover).ToList();

                    List<SliderImageViewModel> LinkList = new List<SliderImageViewModel>();
                    foreach (SliderImage item in slider.SliderImages)
                    {
                        SliderImageViewModel oOtherImageLink = new SliderImageViewModel();
                        oOtherImageLink.Cover = item.Cover;
                        oOtherImageLink.Link = item.Link;
                        LinkList.Add(oOtherImageLink);
                    }
                    Session["SliderImageLink"] = LinkList.OrderBy(x => x.Cover).ToList();
                    #endregion


                    ViewBag.HelpModuleSection = uof.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 9 && x.Name == "ایجاد اسلایدر جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Sliders", "Edit", true, 200, " نمایش صفحه ویرایش اسلایدرِ " + slider.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(slider);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت اسلایدر ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Sliders", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Sliders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit([Bind(Include = "Id,Cover,Title,DisplaySort,IsActive,LanguageId,TypeId,LinkId")] Slider slider, string[] JanebiId, string[] JanebiTitle, string[] JanebiLink)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                if (ModelState.IsValid)
                {

                    slider.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(slider.Title);
                    uof.SliderRepository.Update(slider);
                    uof.Save();

                    #region Delete Old Other Image
                    List<SliderImage> lstOtherImage = new List<SliderImage>();
                    if (JanebiId != null)
                    {
                        for (int i = 0; i < JanebiId.Length; i++)
                        {
                            lstOtherImage.Add(new SliderImage { Title = JanebiTitle[i], DisplaySort = i, Cover = new Guid(JanebiId[i]), Link = JanebiLink[i] });
                        }

                        var SliderlstOtherImages = uof.SliderImageRepository.Get(x => x, x => x.SliderId == slider.Id);
                        List<SliderImage> DeleteOtherImages = new List<SliderImage>();
                        foreach (SliderImage item in SliderlstOtherImages)
                        {
                            var current = lstOtherImage.Where(x => x.Cover == item.Cover).FirstOrDefault();
                            if (current != null)
                                lstOtherImage.Remove(current);
                            else
                                DeleteOtherImages.Add(item);
                        }
                        foreach (SliderImage item in DeleteOtherImages)
                        {
                            uof.SliderImageRepository.Delete(item);
                            uof.Save();
                        }

                    }
                    else
                    {
                        var SliderlstOtherImages = uof.SliderImageRepository.Get(x => x, x => x.SliderId == slider.Id);
                        List<SliderImage> DeleteOtherImages = new List<SliderImage>();
                        foreach (SliderImage item in SliderlstOtherImages)
                        {
                            DeleteOtherImages.Add(item);
                        }
                        foreach (SliderImage item in DeleteOtherImages)
                        {
                            uof.SliderImageRepository.Delete(item);
                            uof.Save();
                        }

                    }
                    #endregion

                    if (JanebiId != null)
                    {
                        foreach (SliderImage item in lstOtherImage)
                        {
                            item.SliderId = slider.Id;
                            uof.SliderImageRepository.Insert(item);
                            uof.Save();
                        }
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "Sliders", "Edit", false, 200, "   ویرایش اسلایدرِ " + slider.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", slider.LanguageId);

                #region Check License


                #endregion
                XMasterTheme master = readXML.ListOfXMasterTheme().First();
                List<SelectListItem> TypeIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = (master.FooterSliderVisibility ? "--فوتر تمامی صفحات--" : "--انتخاب نشده--"), Value = "0", Selected = (slider.TypeId == 0 ? true : false) }, new SelectListItem() { Text = "محتواها ( صفحه داخلی خبر،مقاله،بلاگ و... )", Value = "1", Selected = (slider.TypeId == 1 ? true : false) }, new SelectListItem() { Text = "دسته بندی ها", Value = "2", Selected = (slider.TypeId == 2 ? true : false) }, new SelectListItem() { Text = "برچسب ها", Value = "3", Selected = (slider.TypeId == 3 ? true : false) }, new SelectListItem() { Text = "دسته بندی محصولات", Value = "4", Selected = (slider.TypeId == 4 ? true : false) }, new SelectListItem() { Text = "گالری", Value = "5", Selected = (slider.TypeId == 5 ? true : false) }, new SelectListItem() { Text = "انواع محتوا ( صفحه لیست اخبار،مقالات،وبلاگ و ...)", Value = "6", Selected = (slider.TypeId == 6 ? true : false) } };

                List<SelectListItem> LinkIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = (master.FooterSliderVisibility ? "--فوتر تمامی صفحات--" : "--انتخاب نشده--"), Value = "0", Selected = (slider.LinkId == 0 ? true : false) } };
                ViewBag.LinkIdSelectListItem = LinkIdSelectListItem;


                ViewBag.HelpModuleSection = uof.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 9 && x.Name == "ایجاد اسلایدر جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Sliders", "Edit", false, 500, "   خطا در ویرایش اسلایدرِ " + slider.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(slider);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Sliders", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Sliders/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {


                if (ModulePermission.check(User.Identity.GetUserId(), 6, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Slider slider = uof.SliderRepository.GetByID(id);
                    if (slider == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Sliders", "Delete", true, 200, " نمایش صفحه حذف اسلایدرِ " + slider.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(slider);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت اسلایدر ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Sliders", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {
                Slider slider = uof.SliderRepository.GetByID(id);
                uof.SliderRepository.Delete(slider);
                uof.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Sliders", "DeleteConfirmed", false, 200, "   حذف اسلایدرِ " + slider.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Sliders", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        public virtual JsonResult AddImageLink(string ID, string Link)
        {
            try
            {
                List<SliderImageViewModel> LinkList = new List<SliderImageViewModel>();
                if (Session["SliderImageLink"] != null)
                    LinkList = (List<SliderImageViewModel>)Session["SliderImageLink"];
                SliderImageViewModel newLink = new SliderImageViewModel();
                newLink.Cover = new Guid(ID);
                newLink.Link = Link;
                LinkList.Add(newLink);
                Session["SliderImageLink"] = LinkList;
                return Json(new
                {
                    message = "انجام شد",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
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
