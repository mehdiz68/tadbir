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
using CoreLib.Infrastructure;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class ProductAttributesController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;

        // GET: Admin/ProductAttributes
        [CorrectArabianLetter(new string[] { "Name", "Unit" })]
        public virtual ActionResult Index(string sortOrder, string NameFilter, string NameString, string LanguagenameFilter, string LanguagenameString, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                    #region search
                    if (string.IsNullOrEmpty(NameString))
                        NameString = NameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;
                    else
                        page = 1;

                    ViewBag.NameFilter = NameString;
                    ViewBag.LanguagenameFilter = LanguagenameString;

                    var Attributes = uow.ProductAttributeRepository.GetByReturnQueryable(x => x);
                    if (!String.IsNullOrEmpty(NameString))
                        Attributes = Attributes.Where(s => s.Name.Contains(NameString));
                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int langId = Convert.ToInt32(LanguagenameString);
                        Attributes = Attributes.Where(s => s.LanguageId == langId);
                    }

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "Name":
                            Attributes = Attributes.OrderBy(s => s.Name);
                            ViewBag.CurrentSort = "TagName";
                            break;
                        case "Name_desc":
                            Attributes = Attributes.OrderByDescending(s => s.Name);
                            ViewBag.CurrentSort = "TagName_desc";
                            break;
                        case "Languagename":
                            Attributes = Attributes.OrderBy(s => s.LanguageId);
                            ViewBag.CurrentSort = "Languagename";
                            break;
                        case "Languagename_desc":
                            Attributes = Attributes.OrderByDescending(s => s.LanguageId);
                            ViewBag.CurrentSort = "Languagename_desc";
                            break;
                        case "DataType":
                            Attributes = Attributes.OrderBy(s => s.DataType);
                            ViewBag.CurrentSort = "DataType";
                            break;
                        case "DataType_desc":
                            Attributes = Attributes.OrderByDescending(s => s.DataType);
                            ViewBag.CurrentSort = "DataType_desc";
                            break;
                        case "Unit":
                            Attributes = Attributes.OrderBy(s => s.Unit);
                            ViewBag.CurrentSort = "Unit";
                            break;
                        case "Unit_desc":
                            Attributes = Attributes.OrderByDescending(s => s.Unit);
                            ViewBag.CurrentSort = "Unit_desc";
                            break;
                        case "PriceEffect":
                            Attributes = Attributes.OrderBy(s => s.PriceEffect);
                            ViewBag.CurrentSort = "PriceEffect";
                            break;
                        case "PriceEffect_desc":
                            Attributes = Attributes.OrderByDescending(s => s.PriceEffect);
                            ViewBag.CurrentSort = "PriceEffect_desc";
                            break;
                        case "HasMultipleValue":
                            Attributes = Attributes.OrderBy(s => s.HasMultipleValue);
                            ViewBag.CurrentSort = "HasMultipleValue";
                            break;
                        case "HasMultipleValue_desc":
                            Attributes = Attributes.OrderByDescending(s => s.HasMultipleValue);
                            ViewBag.CurrentSort = "HasMultipleValue_desc";
                            break;
                        case "DefaultValue":
                            Attributes = Attributes.OrderBy(s => s.DefaultValue);
                            ViewBag.CurrentSort = "DefaultValue";
                            break;
                        case "DefaultValue_desc":
                            Attributes = Attributes.OrderByDescending(s => s.DefaultValue);
                            ViewBag.CurrentSort = "DefaultValue_desc";
                            break;
                        default:  // Name ascending 
                            Attributes = Attributes.OrderByDescending(s => s.Id);
                            break;
                    }

                    #endregion

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);


                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "خصوصیات محصول", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "Index", true, 200, " نمایش صفحه مدیریت خصوصیات", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Attributes.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/ProductAttributes/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWork.UnitOfWorkClass();

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            Session["Items"] = null;
            Session["ItemColors"] = null;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");



                    LoadDropDown(false, new ProductAttribute());
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "Create", true, 200, " نمایش صفحه ایجاد خصوصیت", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ProductAttributes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create( ProductAttribute ProductAttribute)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {
                if (ModelState.IsValid)
                {
                    ProductAttribute.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductAttribute.Name);
                    if (Session["Items"] != null)
                    {
                        ProductAttribute.ProductAttributeItems = new List<ProductAttributeItem>();
                        foreach (var item in Session["Items"] as List<string>)
                        {
                            ProductAttribute.ProductAttributeItems.Add(new ProductAttributeItem() { Value = item, DisplayOrder = 0 });
                        }
                    }
                    if (Session["ItemColors"] != null)
                    {
                        ProductAttribute.ProductAttributeItemColors = new List<ProductAttributeItemColor>();
                        foreach (var item in Session["ItemColors"] as List<ItemColors>)
                        {
                            ProductAttribute.ProductAttributeItemColors.Add(new ProductAttributeItemColor() { Value = item.Value, Color = item.Color });
                        }
                    }
                    uow.ProductAttributeRepository.Insert(ProductAttribute);
                    uow.Save();

                    uow.ProductAttributeGroupSelectRepository.Insert(new ProductAttributeGroupSelect { AttributeId = ProductAttribute.Id, DisplayGroupOrder = 0, DisplayOrder = 0, GroupId = null, TabId = null });
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductAttributes", "Create", false, 200, "   ایجاد خصوصیتِ " + ProductAttribute.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                LoadDropDown(false, ProductAttribute);
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", ProductAttribute.LanguageId);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "Create", false, 500, "   خطا در ایجاد خصوصیتِ " + ProductAttribute.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductAttribute);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                LoadDropDown(false, ProductAttribute);
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", ProductAttribute.LanguageId);
                ViewBag.Error = "خصوصیت وارد شده تکراری است.";
                return View(ProductAttribute);
            }

        }

        // GET: Admin/ProductAttributes/Edit/5
        public virtual ActionResult Edit(int? id)
        {

            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    ProductAttribute ProductAttribute = uow.ProductAttributeRepository.Get(x => x, x => x.Id == id, null, "ProductAttributeItemColors,ProductAttributeItems").SingleOrDefault();
                    if (ProductAttribute == null)
                    {
                        return HttpNotFound();
                    }

                    List<ItemColors> itemcolors = new List<ItemColors>(); ;
                    foreach (var item in ProductAttribute.ProductAttributeItemColors.OrderBy(x => x.Color))
                    {
                        itemcolors.Add(new ItemColors() { Color = item.Color, Value = item.Value });
                    }
                    //Session["Items"] = ProductAttribute.ProductAttributeItems.Select(x => x.Value).ToList();
                    //Session["ItemColors"] = itemcolors;
                    LoadDropDown(true, ProductAttribute);
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", ProductAttribute.LanguageId);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "Edit", true, 200, " نمایش صفحه ویرایش خصوصیتِ " + ProductAttribute.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductAttribute);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ProductAttributes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit( ProductAttribute ProductAttribute)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                if (ModelState.IsValid)
                {
                    if (!uow.ProductAttributeRepository.Get(x => x, x => x.Name == ProductAttribute.Name.Trim() && x.Id != ProductAttribute.Id, null).Any())
                    {
                        ProductAttribute.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductAttribute.Name);
                        //if (Session["Items"] != null)
                        //{

                        //    var items = Session["Items"] as List<string>;
                        //    //add
                        //    foreach (var item in items)
                        //    {
                        //        if (!uow.ProductAttributeItemRepository.Any(x=>x,x => x.Value == item && x.AttributeId==ProductAttribute.Id))
                        //        {
                        //            uow.ProductAttributeItemRepository.Insert(new ProductAttributeItem() { Value = item, DisplayOrder = 0, AttributeId = ProductAttribute.Id });
                        //            uow.Save();
                        //        }
                        //    }
                        //    //remove
                        //    List<int> ids = new List<int>();
                        //    foreach (var item in uow.ProductAttributeItemRepository.Get(x=>x,x=>x.AttributeId==ProductAttribute.Id))
                        //    {
                        //        if (!items.Any(x => x == item.Value))
                        //            ids.Add(item.Id);
                        //    }
                        //    uow.ProductAttributeItemRepository.Delete(uow.ProductAttributeItemRepository.Get(x=>x,x=> ids.Contains(x.Id)).ToList());
                        //    uow.Save();
                        //}
                        //if (Session["ItemColors"] != null)
                        //{

                        //    var itemColors = Session["ItemColors"] as List<ItemColors>;
                        //    //add
                        //    foreach (var item in itemColors)
                        //    {
                        //        if (!uow.ProductAttributeItemColorRepository.Any(x => x, x => x.Value == item.Value && x.AttributeId == ProductAttribute.Id && x.Color==item.Color))
                        //        {
                        //            uow.ProductAttributeItemColorRepository.Insert(new ProductAttributeItemColor() { Value = item.Value, Color = item.Color, AttributeId = ProductAttribute.Id });
                        //            uow.Save();
                        //        }
                        //    }
                        //    //remove
                        //    List<int> ids = new List<int>();
                        //    foreach (var item in uow.ProductAttributeItemColorRepository.Get(x => x, x => x.AttributeId == ProductAttribute.Id))
                        //    {
                        //        if (!itemColors.Any(x => x.Value == item.Value && x.Color==item.Color))
                        //            ids.Add(item.Id);
                        //    }
                        //    uow.ProductAttributeItemColorRepository.Delete(uow.ProductAttributeItemColorRepository.Get(x => x, x => ids.Contains(x.Id)).ToList());
                        //    uow.Save();
                        //}
                        uow.ProductAttributeRepository.Update(ProductAttribute);
                        uow.Save();

                        if (!uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.AttributeId == ProductAttribute.Id).Any())
                        {
                            uow.ProductAttributeGroupSelectRepository.Insert(new ProductAttributeGroupSelect { AttributeId = ProductAttribute.Id, DisplayGroupOrder = 0, DisplayOrder = 0, GroupId = null, TabId = null });
                            uow.Save();
                        }

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductAttributes", "Edit", false, 200, "   ویرایش خصوصیتِ " + ProductAttribute.Name, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        LoadDropDown(true, ProductAttribute);

                        ViewBag.Error = "خصوصیت وارد شده تکراری است.";
                        ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", ProductAttribute.LanguageId);
                    }
                }

                LoadDropDown(true, ProductAttribute);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", ProductAttribute.LanguageId);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "Edit", false, 500, "   خطا در ویرایش خصوصیتِ " + ProductAttribute.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductAttribute);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/ProductAttributes/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    ProductAttribute ProductAttribute = uow.ProductAttributeRepository.Get(x => x, x => x.Id == id).FirstOrDefault();
                    if (ProductAttribute == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "Delete", true, 200, " نمایش صفحه حذف خصوصیتِ " + ProductAttribute.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductAttribute);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ProductAttributes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                ProductAttribute ProductAttribute = uow.ProductAttributeRepository.GetByID(id);
                if (ProductAttribute.DataType < 11)
                {
                    uow.ProductAttributeRepository.Delete(ProductAttribute);
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductAttributes", "DeleteConfirmed", false, 200, "   حذف خصوصیتِ " + ProductAttribute.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        #region ProductAttributeItems
        // GET: Admin/ProductAttributes/Edit/5
        public virtual ActionResult Items(int? AttributeId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();
                    if (AttributeId == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    List<ProductAttributeItem> ProductAttributeItems = uow.ProductAttributeItemRepository.Get(x => x, x => x.AttributeId == AttributeId, null, "ProductAttribute").ToList();


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "Items", true, 200, " نمایش صفحه مقادیر خصوصیتِ ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductAttributeItems.OrderBy(x => x.DisplayOrder));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "Items", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        public virtual JsonResult SortItems(string ids)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (Int16 i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var social = uow.ProductAttributeItemRepository.GetByID(id);
                    social.DisplayOrder = i;
                    uow.Save();
                }
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "SortItems", false, 200, "   مرتب سازی مقادیر خصوصیت", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "SortItems", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public virtual ActionResult CreateItem(int? Id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {
                    var ProductAttribute = uow.ProductAttributeRepository.GetByID(Id.Value);
                    if (ProductAttribute != null)
                    {
                        ViewBag.ProductAttribute = ProductAttribute;
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "CreateItem", true, 200, " نمایش صفحه ایجاد مقدار برای خصوصیت", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "CreateItem", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult CreateItem( ProductAttributeItem ProductAttributeItem)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (!uow.ProductAttributeItemRepository.Any(x => x, x => x.AttributeId == ProductAttributeItem.AttributeId && x.Value == ProductAttributeItem.Value))
                {
                    if (ModelState.IsValid)
                    {
                        ProductAttributeItem.Value = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(CommonFunctions.CorrectArabianLetter(ProductAttributeItem.Value));
                        uow.ProductAttributeItemRepository.Insert(ProductAttributeItem);
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductAttributes", "CreateItem", false, 200, "   ایجاد مقدار " + ProductAttributeItem.Value, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Items", new { AttributeId = ProductAttributeItem.AttributeId });
                    }
                }
                else
                {
                    ViewBag.Error = "رکورد وارد شده تکراری است !";
                }


                var ProductAttribute = uow.ProductAttributeRepository.GetByID(ProductAttributeItem.AttributeId);
                ViewBag.ProductAttribute = ProductAttribute;
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "CreateItem", false, 500, "   خطا در ایجاد مقدار " + ProductAttributeItem.Value, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductAttributeItem);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "CreateItem", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductAttributeItem);
            }

        }

        public virtual ActionResult EditItem(int? Id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {
                    var ProductAttributeItem = uow.ProductAttributeItemRepository.Get(x => x, x => x.Id == Id.Value, null, "ProductAttribute").SingleOrDefault();
                    if (ProductAttributeItem != null)
                    {
                        ViewBag.ProductAttribute = ProductAttributeItem.ProductAttribute;
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "EditItem", true, 200, " نمایش صفحه ویرایش مقدار خصوصیت", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(ProductAttributeItem);
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "EditItem", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult EditItem( ProductAttributeItem ProductAttributeItem)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (!uow.ProductAttributeItemRepository.Any(x => x, x => x.AttributeId == ProductAttributeItem.AttributeId && x.Value == ProductAttributeItem.Value && x.Id != ProductAttributeItem.Id))
                {
                    if (ModelState.IsValid)
                    {
                        ProductAttributeItem.Value = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductAttributeItem.Value);
                        uow.ProductAttributeItemRepository.Update(ProductAttributeItem);
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductAttributes", "EditItem", false, 200, "   ویرایش مقدار خصوصیت " + ProductAttributeItem.Value, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Items", new { AttributeId = ProductAttributeItem.AttributeId });
                    }
                }
                else
                {
                    ViewBag.Error = "رکورد وارد شده تکراری است !";
                }


                var ProductAttribute = uow.ProductAttributeRepository.GetByID(ProductAttributeItem.AttributeId);
                ViewBag.ProductAttribute = ProductAttribute;
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "EditItem", false, 500, "   خطا در ویرایش مقدار خصوصیت " + ProductAttributeItem.Value, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductAttributeItem);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "EditItem", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductAttributeItem);
            }

        }


        public virtual ActionResult DeleteItem(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModulePermission.check(User.Identity.GetUserId(), 22, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    ProductAttributeItem ProductAttributeItem = uow.ProductAttributeItemRepository.Get(x => x, x => x.Id == id).FirstOrDefault();
                    if (ProductAttributeItem == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.ProductAttribute = ProductAttributeItem.ProductAttribute;

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "DeleteItem", true, 200, " نمایش صفحه حذف مقدار خصوصیت " + ProductAttributeItem.Value, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductAttributeItem);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "DeleteItem", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteItem(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                ProductAttributeItem ProductAttributeItem = uow.ProductAttributeItemRepository.GetByID(id);

                uow.ProductAttributeItemRepository.Delete(ProductAttributeItem);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductAttributes", "DeleteItem", false, 200, "   حذف مقدار خصوصیت " + ProductAttributeItem.Value, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Items", new { AttributeId = ProductAttributeItem.AttributeId });
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "DeleteItem", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        #endregion

        #region AttributeItemAJAX
        [HttpPost]
        public virtual JsonResult AddAttributeItem(string value)
        {
            List<string> Items = new List<string>();
            try
            {
                if (Session["Items"] != null)
                    Items = (List<string>)Session["Items"];

                if (!Items.Any(x => x == CommonFunctions.CorrectArabianLetter(value)))
                {
                    Items.Add(value);
                    Session["Items"] = Items;
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 501
                    }, JsonRequestBehavior.AllowGet);

                }

                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual JsonResult RemoveAttributeItem(string value)
        {
            try
            {
                if (Session["Items"] != null)
                {
                    List<string> Items = (List<string>)Session["Items"];

                    Items.Remove(value);
                    Session["Items"] = Items;
                }

                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual JsonResult EditAttributeItem(string value, string OldValue)
        {
            try
            {
                if (Session["Items"] != null)
                {
                    List<string> Items = (List<string>)Session["Items"];

                    if (!Items.Any(x => x == value))
                    {
                        Items[Items.FindIndex(ind => ind.Equals(CommonFunctions.CorrectArabianLetter(OldValue)))] = CommonFunctions.CorrectArabianLetter(value);
                        Session["Items"] = Items;
                    }
                    else
                    {
                        return Json(new
                        {
                            statusCode = 501
                        }, JsonRequestBehavior.AllowGet);

                    }

                }

                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ProductAttributeItemColors
        // GET: Admin/ProductAttributes/Edit/5
        public virtual ActionResult ItemColors(int? AttributeId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();
                    if (AttributeId == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    List<ProductAttributeItemColor> ProductAttributeItemColor = uow.ProductAttributeItemColorRepository.Get(x => x, x => x.AttributeId == AttributeId, null, "ProductAttribute").ToList();



                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "ItemColors", true, 200, " نمایش صفحه رنگهای خصوصیتِ ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductAttributeItemColor.OrderBy(x => x.Color));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "ItemColors", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }



        public virtual ActionResult CreateItemColor(int? Id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {
                    var ProductAttribute = uow.ProductAttributeRepository.GetByID(Id.Value);
                    if (ProductAttribute != null)
                    {
                        ViewBag.ProductAttribute = ProductAttribute;
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "CreateItemColor", true, 200, " نمایش صفحه ایجاد مقدار برای رنگ", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "CreateItemColor", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateItemColor([Bind(Include = "Id,AttributeId,Value,Color")] ProductAttributeItemColor ProductAttributeItemColor)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (!uow.ProductAttributeItemColorRepository.Any(x => x, x => x.AttributeId == ProductAttributeItemColor.AttributeId && (x.Color == ProductAttributeItemColor.Color || x.Value == ProductAttributeItemColor.Value)))
                {
                    if (ModelState.IsValid)
                    {

                        ProductAttributeItemColor.Color = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductAttributeItemColor.Color);
                        uow.ProductAttributeItemColorRepository.Insert(ProductAttributeItemColor);
                        uow.Save();



                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductAttributes", "CreateItemColor", false, 200, "   ایجاد رنگ " + ProductAttributeItemColor.Color, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("ItemColors", new { AttributeId = ProductAttributeItemColor.AttributeId });
                    }
                }
                else
                {
                    ViewBag.Error = "رکورد وارد شده تکراری است !";
                }


                var ProductAttribute = uow.ProductAttributeRepository.GetByID(ProductAttributeItemColor.AttributeId);
                ViewBag.ProductAttribute = ProductAttribute;
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "CreateItemColor", false, 500, "   خطا در ایجاد مقدار " + ProductAttributeItemColor.Color, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductAttributeItemColor);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "CreateItemColor", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductAttributeItemColor);
            }

        }

        public virtual ActionResult EditItemColor(int? Id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {
                    var ProductAttributeItemColor = uow.ProductAttributeItemColorRepository.GetByID(Id.Value);
                    if (ProductAttributeItemColor != null)
                    {
                        ViewBag.ProductAttribute = ProductAttributeItemColor.ProductAttribute;
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "EditItemColor", true, 200, " نمایش صفحه ویرایش رنگ خصوصیت", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(ProductAttributeItemColor);
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "EditItemColor", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult EditItemColor([Bind(Include = "Id,AttributeId,Value,Color")] ProductAttributeItemColor ProductAttributeItemColor)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (!uow.ProductAttributeItemColorRepository.Any(x => x, x => x.AttributeId == ProductAttributeItemColor.AttributeId && (x.Color == ProductAttributeItemColor.Color || x.Value == ProductAttributeItemColor.Value) && x.Id != ProductAttributeItemColor.Id))
                {
                    if (ModelState.IsValid)
                    {
                        ProductAttributeItemColor.Color = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductAttributeItemColor.Color);
                        uow.ProductAttributeItemColorRepository.Update(ProductAttributeItemColor);
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductAttributes", "EditItem", false, 200, "   ویرایش رنگ خصوصیت " + ProductAttributeItemColor.Color, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("ItemColors", new { AttributeId = ProductAttributeItemColor.AttributeId });
                    }
                }
                else
                {
                    ViewBag.Error = "رکورد وارد شده تکراری است !";
                }


                var ProductAttribute = uow.ProductAttributeRepository.GetByID(ProductAttributeItemColor.AttributeId);
                ViewBag.ProductAttribute = ProductAttribute;
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "EditItemColor", false, 500, "   خطا در ویرایش رنگ خصوصیت " + ProductAttributeItemColor.Color, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductAttributeItemColor);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "EditItemColor", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductAttributeItemColor);
            }

        }


        public virtual ActionResult DeleteItemColor(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    ProductAttributeItemColor ProductAttributeItemColor = uow.ProductAttributeItemColorRepository.Get(x => x, x => x.Id == id).FirstOrDefault();
                    if (ProductAttributeItemColor == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.ProductAttribute = ProductAttributeItemColor.ProductAttribute;

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributes", "DeleteItemColor", true, 200, " نمایش صفحه حذف رنگ خصوصیت " + ProductAttributeItemColor.Value, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductAttributeItemColor);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "DeleteItemColor", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteItemColor(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                ProductAttributeItemColor ProductAttributeItemColor = uow.ProductAttributeItemColorRepository.GetByID(id);

                uow.ProductAttributeItemColorRepository.Delete(ProductAttributeItemColor);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductAttributes", "DeleteItemColor", false, 200, "   حذف مقدار رنگ " + ProductAttributeItemColor.Color, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("ItemColors", new { AttributeId = ProductAttributeItemColor.AttributeId });
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "DeleteItemColor", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        #endregion

        #region AttributeItemColorAJAX
        [HttpPost]
        public virtual JsonResult AddAttributeItemColor(string Value, string Color)
        {
            List<ItemColors> Items = new List<ItemColors>();
            try
            {
                if (Session["ItemColors"] != null)
                    Items = (List<ItemColors>)Session["ItemColors"];
                if (!Items.Any(x => x.Color == Color || x.Value == Value))
                {
                    Items.Add(new ItemColors() { Value = Value, Color = Color });
                    Session["ItemColors"] = Items;
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 501
                    }, JsonRequestBehavior.AllowGet);

                }
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual JsonResult RemoveAttributeItemColor(string Value, string Color)
        {
            try
            {
                if (Session["ItemColors"] != null)
                {
                    List<ItemColors> Items = (List<ItemColors>)Session["ItemColors"];

                    Items.Remove(Items.Find(x => x.Color == Color));
                    Session["ItemColors"] = Items;
                }

                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual JsonResult EditAttributeItemColor(string Color, string OldColor, string Value, string OldValue)
        {
            try
            {
                if (Session["ItemColors"] != null)
                {
                    List<ItemColors> Items = (List<ItemColors>)Session["ItemColors"];
                    if (!Items.Any(x => x.Color == Color || x.Value == Value))
                    {
                        Items.Find(x => x.Color == OldColor).Color = Color;
                        Items.Find(x => x.Value == OldValue).Value = Value;
                        Session["ItemColors"] = Items;
                    }
                    else
                    {
                        return Json(new
                        {
                            statusCode = 501
                        }, JsonRequestBehavior.AllowGet);

                    }

                }

                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception z)
            {
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        private void LoadDropDown(bool edit, ProductAttribute ProductAttribute)
        {
            IdentityManager im = new IdentityManager();
            if (im.IsInRole(User.Identity.GetUserId(), "SuperUser"))
                ViewBag.SuperUserPermission = true;

            if (edit)
            {
                if (ViewBag.SuperUserPermission == true)
                {
                    List<SelectListItem> DataTypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "عدد صحیح", Value = "1", Selected = ProductAttribute.DataType == 1 ? true : false }, new SelectListItem() { Text = "عدد اعشاری", Value = "2", Selected = ProductAttribute.DataType == 2 ? true : false }, new SelectListItem() { Text = "متن", Value = "3", Selected = ProductAttribute.DataType == 3 ? true : false }, new SelectListItem() { Text = "HTML Editor", Value = "4", Selected = ProductAttribute.DataType == 4 ? true : false }, new SelectListItem() { Text = "منطقی", Value = "5", Selected = ProductAttribute.DataType == 5 ? true : false }, new SelectListItem() { Text = "تاریخ و زمان", Value = "6", Selected = ProductAttribute.DataType == 6 ? true : false }, new SelectListItem() { Text = "فایل", Value = "7", Selected = ProductAttribute.DataType == 7 ? true : false }, new SelectListItem() { Text = "انتخابی", Value = "8", Selected = ProductAttribute.DataType == 8 ? true : false }, new SelectListItem() { Text = "لینک به محتوا", Value = "9", Selected = ProductAttribute.DataType == 9 ? true : false }, new SelectListItem() { Text = "لینک به محصول", Value = "10", Selected = ProductAttribute.DataType == 10 ? true : false }, new SelectListItem() { Text = "لینک به رنگ", Value = "11", Selected = ProductAttribute.DataType == 11 ? true : false }, new SelectListItem() { Text = "رنگ", Value = "12", Selected = ProductAttribute.DataType == 12 ? true : false }, new SelectListItem() { Text = "سایز", Value = "13", Selected = ProductAttribute.DataType == 13 ? true : false }, new SelectListItem() { Text = "مدل", Value = "14", Selected = ProductAttribute.DataType == 14 ? true : false }, new SelectListItem() { Text = "گارانتی", Value = "15", Selected = ProductAttribute.DataType == 15 ? true : false }, new SelectListItem() { Text = "وزن محصول", Value = "16", Selected = ProductAttribute.DataType == 16 ? true : false }, new SelectListItem() { Text = "بن", Value = "17", Selected = ProductAttribute.DataType == 17 ? true : false }, new SelectListItem() { Text = "حداکثر استفاده از بن", Value = "18", Selected = ProductAttribute.DataType == 18 ? true : false } };
                    ViewBag.DataType = DataTypeSelectListItem;
                }
                else
                {
                    List<SelectListItem> DataTypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "عدد صحیح", Value = "1", Selected = ProductAttribute.DataType == 1 ? true : false }, new SelectListItem() { Text = "عدد اعشاری", Value = "2", Selected = ProductAttribute.DataType == 2 ? true : false }, new SelectListItem() { Text = "متن", Value = "3", Selected = ProductAttribute.DataType == 3 ? true : false }, new SelectListItem() { Text = "HTML Editor", Value = "4", Selected = ProductAttribute.DataType == 4 ? true : false }, new SelectListItem() { Text = "منطقی", Value = "5", Selected = ProductAttribute.DataType == 5 ? true : false }, new SelectListItem() { Text = "تاریخ و زمان", Value = "6", Selected = ProductAttribute.DataType == 6 ? true : false }, new SelectListItem() { Text = "فایل", Value = "7", Selected = ProductAttribute.DataType == 7 ? true : false }, new SelectListItem() { Text = "انتخابی", Value = "8", Selected = ProductAttribute.DataType == 8 ? true : false }, new SelectListItem() { Text = "لینک به محتوا", Value = "9", Selected = ProductAttribute.DataType == 9 ? true : false }, new SelectListItem() { Text = "لینک به محصول", Value = "10", Selected = ProductAttribute.DataType == 10 ? true : false }, new SelectListItem() { Text = "لینک به رنگ", Value = "11", Selected = ProductAttribute.DataType == 11 ? true : false } };
                    ViewBag.DataType = DataTypeSelectListItem;

                }

            }
            else
            {
                if (ViewBag.SuperUserPermission == true)
                {
                    List<SelectListItem> DataTypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "عدد صحیح", Value = "1" }, new SelectListItem() { Text = "عدد اعشاری", Value = "2" }, new SelectListItem() { Text = "متن", Value = "3" }, new SelectListItem() { Text = "HTML Editor", Value = "4" }, new SelectListItem() { Text = "منطقی", Value = "5" }, new SelectListItem() { Text = "تاریخ و زمان", Value = "6" }, new SelectListItem() { Text = "فایل", Value = "7" }, new SelectListItem() { Text = "انتخابی", Value = "8" }, new SelectListItem() { Text = "لینک به محتوا", Value = "9" }, new SelectListItem() { Text = "لینک به محصول", Value = "10" }, new SelectListItem() { Text = "لینک به رنگ", Value = "11" }, new SelectListItem() { Text = "رنگ", Value = "12" }, new SelectListItem() { Text = "سایز", Value = "13" }, new SelectListItem() { Text = "مدل", Value = "14" }, new SelectListItem() { Text = "گارانتی", Value = "15" }, new SelectListItem() { Text = "وزن محصول", Value = "16" }, new SelectListItem() { Text = "بن", Value = "17" }, new SelectListItem() { Text = "حداکثر استفاده از بن", Value = "18" } };
                    ViewBag.DataType = DataTypeSelectListItem;
                }
                else
                {
                    List<SelectListItem> DataTypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "عدد صحیح", Value = "1" }, new SelectListItem() { Text = "عدد اعشاری", Value = "2" }, new SelectListItem() { Text = "متن", Value = "3" }, new SelectListItem() { Text = "HTML Editor", Value = "4" }, new SelectListItem() { Text = "منطقی", Value = "5" }, new SelectListItem() { Text = "تاریخ و زمان", Value = "6" }, new SelectListItem() { Text = "فایل", Value = "7" }, new SelectListItem() { Text = "انتخابی", Value = "8" }, new SelectListItem() { Text = "لینک به محتوا", Value = "9" }, new SelectListItem() { Text = "لینک به محصول", Value = "10" }, new SelectListItem() { Text = "لینک به رنگ", Value = "11" } };
                    ViewBag.DataType = DataTypeSelectListItem;

                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }

}

public class ItemColors
{
    public string Value { get; set; }
    public string Color { get; set; }
}
