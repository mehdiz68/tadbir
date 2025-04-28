using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.ViewModels.Setting;
using ahmadi.Infrastructure.Security;
using System.Collections.Generic;
using CoreLib.ViewModel.Xml;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Security,SuperUser")]
    public partial class SettingController : Controller
    {
        UnitOfWork.UnitOfWorkClass uow = null;

        public SettingController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();

        }
        // GET: Admin/Setting
        public virtual ActionResult Index()
        {
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

                    var Settings = uow.SettingRepository.Get(x => x, null, null, "attachment");

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "تنظیمات سایت", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Settings", "Index", true, 200, " نمایش صفحه تنظیمات", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Settings.ToList());
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Settings", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: attachments/Create
        public virtual ActionResult Create()
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 1))
                {
                    List<short?> CurrentLanguageIds = uow.SettingRepository.Get(x => x).Select(x => x.LanguageId).ToList();
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage().Where(x => !CurrentLanguageIds.Contains(x.Id)), "Id", "Name");


                    var ContentTypes = readXML.ListOfXContentType();
                    ViewBag.ContentTypes = ContentTypes;

                    if (readXML.DetailOfXMasterTheme(1) != null)
                        ViewBag.HeaderMainMenuVisibility = readXML.DetailOfXMasterTheme(1).HeaderSearchVisibility;
                    else
                        ViewBag.HeaderMainMenuVisibility = false;


                    List<SelectListItem> DefaultCurrencySelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تومان-IRT", Value = "1" }, new SelectListItem() { Text = "ریال-IRR", Value = "2" }, new SelectListItem() { Text = "EUR-یورو", Value = "3" }, new SelectListItem() { Text = "USD-دلار", Value = "4" }, new SelectListItem() { Text = "AED-درهم", Value = "5" }, new SelectListItem() { Text = "GBP-پوند", Value = "6" }, new SelectListItem() { Text = "JPY-ین", Value = "7" }, new SelectListItem() { Text = "AUD-دلار استرالیا", Value = "8" }, new SelectListItem() { Text = "CHF-فرانک سوئیس", Value = "9" }, new SelectListItem() { Text = "CAD-دلار کانادا", Value = "10" }, new SelectListItem() { Text = "NZD-دلار نیوزیلند", Value = "11" }, new SelectListItem() { Text = "CNY-یوآن", Value = "12" }, new SelectListItem() { Text = "SEK-کرون سوئد", Value = "13" }, new SelectListItem() { Text = "KRW-وون کره جنوبی", Value = "14" }, new SelectListItem() { Text = "INR-روپیه هند", Value = "15" } };
                    ViewBag.DefaultCurrency = DefaultCurrencySelectListItem;
                    ViewBag.ProuctLisence = true;



                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "ایجاد تنظیم جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Settings", "Create", true, 200, " نمایش صفحه ایجاد تنظیمات", DateTime.Now, User.Identity.GetUserId());
                    #endregion

                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Settings", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: attachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Create(Setting Setting)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "ایجاد تنظیم جدید", null, "HelpModuleSectionFields").FirstOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    uow.SettingRepository.Insert(Setting);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Settings", "Create", false, 200, "   ایجاد تنظیم سایتِ " + Setting.SettingName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                #region Check License


                #endregion

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");


                var ContentTypes = readXML.ListOfXContentType();
                ViewBag.ContentTypes = ContentTypes;

                if (readXML.DetailOfXMasterTheme(1) != null)
                    ViewBag.HeaderMainMenuVisibility = readXML.DetailOfXMasterTheme(1).HeaderSearchVisibility;
                else
                    ViewBag.HeaderMainMenuVisibility = false;


                List<SelectListItem> DefaultCurrencySelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تومان-IRT", Value = "1" }, new SelectListItem() { Text = "ریال-IRR", Value = "2" }, new SelectListItem() { Text = "EUR-یورو", Value = "3" }, new SelectListItem() { Text = "USD-دلار", Value = "4" }, new SelectListItem() { Text = "AED-درهم", Value = "5" }, new SelectListItem() { Text = "GBP-پوند", Value = "6" }, new SelectListItem() { Text = "JPY-ین", Value = "7" }, new SelectListItem() { Text = "AUD-دلار استرالیا", Value = "8" }, new SelectListItem() { Text = "CHF-فرانک سوئیس", Value = "9" }, new SelectListItem() { Text = "CAD-دلار کانادا", Value = "10" }, new SelectListItem() { Text = "NZD-دلار نیوزیلند", Value = "11" }, new SelectListItem() { Text = "CNY-یوآن", Value = "12" }, new SelectListItem() { Text = "SEK-کرون سوئد", Value = "13" }, new SelectListItem() { Text = "KRW-وون کره جنوبی", Value = "14" }, new SelectListItem() { Text = "INR-روپیه هند", Value = "15" } };
                ViewBag.DefaultCurrency = DefaultCurrencySelectListItem;
                ViewBag.ProuctLisence = true;



                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Settings", "Create", false, 500, "   خطا در ایجاد تنظیم سایتِ " + Setting.SettingName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View();
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Settings", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = x.Message;



                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");


                var ContentTypes = readXML.ListOfXContentType();
                ViewBag.ContentTypes = ContentTypes;

                if (readXML.DetailOfXMasterTheme(1) != null)
                    ViewBag.HeaderMainMenuVisibility = readXML.DetailOfXMasterTheme(1).HeaderSearchVisibility;
                else
                    ViewBag.HeaderMainMenuVisibility = false;


                List<SelectListItem> DefaultCurrencySelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تومان-IRT", Value = "1" }, new SelectListItem() { Text = "ریال-IRR", Value = "2" }, new SelectListItem() { Text = "EUR-یورو", Value = "3" }, new SelectListItem() { Text = "USD-دلار", Value = "4" }, new SelectListItem() { Text = "AED-درهم", Value = "5" }, new SelectListItem() { Text = "GBP-پوند", Value = "6" }, new SelectListItem() { Text = "JPY-ین", Value = "7" }, new SelectListItem() { Text = "AUD-دلار استرالیا", Value = "8" }, new SelectListItem() { Text = "CHF-فرانک سوئیس", Value = "9" }, new SelectListItem() { Text = "CAD-دلار کانادا", Value = "10" }, new SelectListItem() { Text = "NZD-دلار نیوزیلند", Value = "11" }, new SelectListItem() { Text = "CNY-یوآن", Value = "12" }, new SelectListItem() { Text = "SEK-کرون سوئد", Value = "13" }, new SelectListItem() { Text = "KRW-وون کره جنوبی", Value = "14" }, new SelectListItem() { Text = "INR-روپیه هند", Value = "15" } };
                ViewBag.DefaultCurrency = DefaultCurrencySelectListItem;
                ViewBag.ProuctLisence = true;



                return View();
            }
        }

        // GET: Admin/Setting/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var setting = uow.SettingRepository.Get(x => x, x => x.Id == id, null, "FactorAttachment,attachment,Faviconattachment,Waterattachment").SingleOrDefault();
                    SettingViewModels Settings = new SettingViewModels(setting);
                    if (Settings == null)
                    {
                        return HttpNotFound();
                    }

                    XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                    var ContentTypes = readXml.ListOfXContentType().Where(x => x.LanguageId == setting.LanguageId).ToList();
                    ViewBag.ContentTypes = ContentTypes;

                    ViewBag.HeaderMainMenuVisibility = readXml.DetailOfXMasterTheme(Convert.ToInt32(setting.LanguageId)).HeaderSearchVisibility;


                    List<SelectListItem> DefaultCurrencySelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تومان-IRT", Value = "1", Selected = (setting.DefaultCurrency == 1 ? true : false) }, new SelectListItem() { Text = "ریال-IRR", Value = "2", Selected = (setting.DefaultCurrency == 2 ? true : false) }, new SelectListItem() { Text = "EUR-یورو", Value = "3", Selected = (setting.DefaultCurrency == 3 ? true : false) }, new SelectListItem() { Text = "USD-دلار", Value = "4", Selected = (setting.DefaultCurrency == 4 ? true : false) }, new SelectListItem() { Text = "AED-درهم", Value = "5", Selected = (setting.DefaultCurrency == 5 ? true : false) }, new SelectListItem() { Text = "GBP-پوند", Value = "6", Selected = (setting.DefaultCurrency == 6 ? true : false) }, new SelectListItem() { Text = "JPY-ین", Value = "7", Selected = (setting.DefaultCurrency == 7 ? true : false) }, new SelectListItem() { Text = "AUD-دلار استرالیا", Value = "8", Selected = (setting.DefaultCurrency == 8 ? true : false) }, new SelectListItem() { Text = "CHF-فرانک سوئیس", Value = "9", Selected = (setting.DefaultCurrency == 9 ? true : false) }, new SelectListItem() { Text = "CAD-دلار کانادا", Value = "10", Selected = (setting.DefaultCurrency == 10 ? true : false) }, new SelectListItem() { Text = "NZD-دلار نیوزیلند", Value = "11", Selected = (setting.DefaultCurrency == 11 ? true : false) }, new SelectListItem() { Text = "CNY-یوآن", Value = "12", Selected = (setting.DefaultCurrency == 12 ? true : false) }, new SelectListItem() { Text = "SEK-کرون سوئد", Value = "13", Selected = (setting.DefaultCurrency == 13 ? true : false) }, new SelectListItem() { Text = "KRW-وون کره جنوبی", Value = "14", Selected = (setting.DefaultCurrency == 14 ? true : false) }, new SelectListItem() { Text = "INR-روپیه هند", Value = "15", Selected = (setting.DefaultCurrency == 15 ? true : false) } };
                    ViewBag.DefaultCurrency = DefaultCurrencySelectListItem;
                    ViewBag.ProuctLisence = true;


                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "ایجاد تنظیم جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Settings", "Edit", true, 200, " نمایش صفحه ویرایش تنظیم سایتِ " + Settings.SettingName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Settings);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Settings", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Setting/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit(SettingViewModels Setting)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Setting Osetting = uow.SettingRepository.GetByID(Setting.Id);
                    Osetting.SettingName = Setting.SettingName;
                    Osetting.WebSiteTitle = Setting.WebSiteTitle;
                    Osetting.WebSiteMetaDescription = Setting.WebSiteMetaDescription;
                    Osetting.WebSiteMetakeyword = Setting.WebSiteMetakeyword;
                    Osetting.Logo = Setting.Logo;
                    Osetting.FactorLogo = Setting.FactorLogo;
                    Osetting.WaterMark = Setting.Watermark;
                    Osetting.LargeSizeHeight = Setting.LargeSizeHeight;
                    Osetting.LargeSizeWidth = Setting.LargeSizeWidth;
                    Osetting.MediumSizeWidth = Setting.MediumSizeWidth;
                    Osetting.MediumSizeHeight = Setting.MediumSizeHeight;
                    Osetting.SmallSizeWidth = Setting.SmallSizeWidth;
                    Osetting.SmallSizeHeight = Setting.SmallSizeHeight;
                    Osetting.XsmallSizeWidth = Setting.XsmallSizeWidth;
                    Osetting.XsmallSizeHeight = Setting.XsmallSizeHeight;
                    Osetting.StaticContentDomain = Setting.StaticContentDomain;
                    Osetting.WebSiteName = Setting.WebSiteName;
                    Osetting.WebmasterVerification = Setting.WebmasterVerification;
                    Osetting.AnalyticsVerification = Setting.AnalyticsVerification;
                    Osetting.DefaultCurrency = Setting.DefaultCurrency;
                    Osetting.CurrencyConvertionRate = Setting.CurrencyConvertionRate;
                    Osetting.TaxRate = Setting.TaxRate;
                    Osetting.BonPrice = Setting.BonPrice;
                    Osetting.BonExpireDay = Setting.BonExpireDay;
                    Osetting.PopUpMessage = Setting.PopUpMessage;
                    Osetting.PopUpActive = Setting.PopUpActive;
                    Osetting.PopUpEditVersion = Osetting.PopUpEditVersion;
                    Osetting.PopUpType = Osetting.PopUpType;
                    Osetting.Favicon = Setting.Favicon;
                    Osetting.HelpActiveShowInDefault = Setting.HelpActiveShowInDefault;
                    Osetting.WaterMarkPosition = Setting.WaterMarkPosition;
                    Osetting.LargeImageWaremark = Setting.LargeImageWaremark;
                    Osetting.HasHttps = Setting.HasHttps;
                    Osetting.yektanet = Setting.yektanet;
                    Osetting.Tele2 = Setting.Tele2;
                    Osetting.Tele3 = Setting.Tele3;
                    Osetting.ShoppingPayEstelamMinutes = Setting.ShoppingPayEstelamMinutes;
                    Osetting.ShoppingEstelamMinutes = Setting.ShoppingEstelamMinutes;
                    Osetting.Address = Setting.Address;
                    Osetting.Address2 = Setting.Address2;
                    Osetting.Tele = Setting.Tele;
                    Osetting.Mobile = Setting.Mobile;
                    Osetting.PostalCode = Setting.PostalCode;
                    Osetting.TaxNumber = Setting.TaxNumber;
                    Osetting.FooterGoogleMapZoom = Setting.FooterGoogleMapZoom;
                    Osetting.FooterGoogleMapLatitude = Setting.FooterGoogleMapLatitude;
                    Osetting.FooterGoogleMapLongitude = Setting.FooterGoogleMapLongitude;
                    Osetting.Email = Setting.Email;
                    uow.SettingRepository.Update(Osetting);
                    uow.Save();

                    HttpContext.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                    HttpContext.Response.AddHeader("Pragma", "no-cache");
                    HttpContext.Response.AddHeader("Expires", "0");
                    Session["settingPersian"] = null;
                    Session["Languages"] = null;
                    Session["HomePage"] = null;

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "Settings", "Edit", false, 200, "   ویرایش تنظیم سایتِ " + Setting.SettingName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "ایجاد تنظیم جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Settings", "Edit", false, 500, "   ویرایش تنظیم سایتِ " + Setting.SettingName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(Setting);

            }
            catch (Exception x)
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(y => y, y => y.Name == "ایجاد تنظیم جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Settings", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Setting/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 3))
                {
                    Setting setting = uow.SettingRepository.Get(x => x, x => x.Id == id, null, "attachment,Faviconattachment,Waterattachment").SingleOrDefault();
                    try
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        if (setting == null)
                        {
                            return HttpNotFound();
                        }

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Settings", "Delete", true, 200, " نمایش صفحه حذف تنظیم سایتِ " + setting.SettingName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(setting);
                    }
                    catch (Exception x)
                    {
                        ViewBag.Error = x.Message;
                        return View(setting);
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Settings", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Setting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Delete(int id, FormCollection Collection)
        {
            try
            {
                Setting setting = uow.SettingRepository.Get(x => x, x => x.Id == id, null, "attachment").SingleOrDefault();
                if (setting == null)
                {
                    return HttpNotFound();
                }
                if (setting.LanguageId == 1)
                {
                    ViewBag.Erorr = " شما مجاز به حذف تنها تنظیمِ فارسیِ سایت نمی باشید. ";
                    return View(setting);
                }
                else
                {
                    uow.SettingRepository.Delete(setting);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(4, "Settings", "DeleteConfirmed", false, 200, "   حذف تنظیمات سایتِ " + setting.SettingName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Settings", "Index", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.Error = x.Message;
                return View();
            }
        }


        [HttpPost]
        public virtual JsonResult EditContentTypes(string Ids, int langid)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                string msg = "";
                XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                var ContentTypes = readXml.ListOfXContentType().Where(x => x.LanguageId == langid);
                foreach (var item in ContentTypes)
                {
                    item.InSearch = false;
                    readXml.EditXContentType(item, out msg);
                }
                if (Ids != "")
                {
                    string[] TagIds = Ids.Split(',');
                    for (int i = 0; i < TagIds.Length; i++)
                    {
                        XContentType contentType = readXml.DetailOfXContentType(Convert.ToInt32(TagIds[i]));
                        contentType.InSearch = true;
                        readXml.EditXContentType(contentType, out msg);
                    }
                }
                return Json(new
                {
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult ProductRandom(int? settingId)
        {
            if (ModulePermission.check(User.Identity.GetUserId(), 1, 2))
            {
                if (settingId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var setting = uow.SettingRepository.Get(x => x, x => x.Id == settingId, null, "ProductRandomSettings.productCategory").SingleOrDefault();
                if (setting == null)
                {
                    return HttpNotFound();
                }
                ViewBag.settingId = settingId;
                return View(setting.ProductRandomSettings);

            }

            else
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));
        }


        public virtual ActionResult CreateProductRandom(int settingId)
        {
            try
            {
                if (ModulePermission.check(User.Identity.GetUserId(), 1, 1))
                {
                    ViewBag.SettingId = settingId;
                    ViewBag.ProductCats = new SelectList(uow.ProductCategoryRepository.Get(x => x, x => x.ParrentId == null && x.IsActive), "Id", "Name");
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));



            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "EmailSenders", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateProductRandom(int ProductCategoryId, int SettingId)
        {
            try
            {
                ProductRandomSetting productRandomSetting = new ProductRandomSetting()
                {
                    ProductCatId = ProductCategoryId,
                    SettingId = SettingId
                };
                uow.ProductRandomSettingRepository.Insert(productRandomSetting);
                uow.Save();

                return RedirectToAction("ProductRandom", new { settingId = SettingId });

            }
            catch (Exception x)
            {
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Setting/Delete/5
        public virtual ActionResult DeleteProductRandom(int? id)
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 3))
                {
                    ProductRandomSetting ProductRandomSetting = uow.ProductRandomSettingRepository.Get(x => x, x => x.Id == id, null, "productCategory").SingleOrDefault();
                    try
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        if (ProductRandomSetting == null)
                        {
                            return HttpNotFound();
                        }


                        return View(ProductRandomSetting);
                    }
                    catch (Exception x)
                    {
                        ViewBag.Error = x.Message;
                        return View(ProductRandomSetting);
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

            }
            catch (Exception x)
            {

                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteProductRandom(ProductRandomSetting productRandomSetting)
        {
            try
            {
                int settingId = productRandomSetting.SettingId;
                uow.ProductRandomSettingRepository.Delete(productRandomSetting);
                uow.Save();


                return RedirectToAction("ProductRandom", new { settingId = settingId });

            }
            catch (Exception x)
            {

                ViewBag.Error = x.Message;
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
