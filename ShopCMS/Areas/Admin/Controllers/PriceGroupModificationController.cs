using CoreLib.Infrastructure.DateTime;
using CoreLib.ViewModel.Xml;
using Domain;
using Microsoft.AspNet.Identity;
using PagedList;
using ahmadi.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UnitOfWork;
using System.Data.SqlClient;
using System.Net.Http;
using Microsoft.Ajax.Utilities;
using System.Threading.Tasks;

namespace ahmadi.Areas.Admin.Controllers
{
    public class PriceGroupModificationController : Controller
    {
        private UnitOfWorkClass uow;
        // GET: Admin/PriceGroupModification
        public ActionResult Index(int? PageSize, string ProductCatId, string ProductCatIdFilter, string BrandId, string BrandFilter, string ProductTypeId, string ProductTypeFilter, string ProductInsertStateId, string ProductInsertStateFilter, string LanguagenameString, string LanguagenameFilter, string InsertDateStart, string InsertDateStartFilter, string InsertDateEnd, string InsertDateEndFilter, int? page)
        {

            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 26);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    #region Load DropDown List
                    int langid = 1;
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;
                    else
                        page = 1;
                    if (LanguagenameString != null)
                        langid = int.Parse(LanguagenameString);
                    ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langid, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
                    ViewBag.BrandId = new SelectList(uow.BrandRepository.Get(x => x, x => x.LanguageId == langid, x => x.OrderBy(s => s.Name)), "Id", "Name");
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameString = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                    #endregion

                    #region search
                    if (string.IsNullOrEmpty(ProductCatId))
                        ProductCatId = ProductCatIdFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(BrandId))
                        BrandId = BrandFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ProductTypeId))
                        ProductTypeId = ProductTypeFilter;
                    else
                        page = 1;

                    if (string.IsNullOrEmpty(ProductInsertStateId))
                        ProductInsertStateId = ProductInsertStateFilter;
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

                    ViewBag.ProductCatIdFilter = ProductCatId;
                    ViewBag.BrandFilter = BrandId;
                    ViewBag.ProductTypeFilter = ProductTypeId;
                    ViewBag.ProductInsertStateFilter = ProductInsertStateId;
                    ViewBag.LanguagenameFilter = LanguagenameString;
                    ViewBag.InsertDateEndFilter = InsertDateEnd;
                    ViewBag.InsertDateStartFilter = InsertDateStart;

                    var Products = uow.ProductPriceGroupModificationRepository.GetQueryList().AsNoTracking().Include("Brand").Include("ProductCategory").Include("User");

                    if (!String.IsNullOrEmpty(ProductCatId))
                    {
                        int ctId = Convert.ToInt32(ProductCatId);
                        if (ctId > 0)
                        {

                            Products = Products.Where(s => s.CatId == ctId);
                        }
                    }

                    if (!String.IsNullOrEmpty(BrandId))
                        Products = Products.Where(s => s.BrandId == int.Parse(BrandId));


                    DateTime dtInsertDateStart = DateTime.Now.Date, dtInsertDateEnd = DateTime.Now.Date, dtUpdateDateStart = DateTime.Now.Date, dtUpdateDateEnd = DateTime.Now.Date, dtDeleteDateStart = DateTime.Now.Date, dtDeleteDateEnd = DateTime.Now.Date;
                    if (!String.IsNullOrEmpty(InsertDateStart))
                        dtInsertDateStart = DateTimeConverter.ChangeShamsiToMiladi(InsertDateStart);
                    if (!String.IsNullOrEmpty(InsertDateEnd))
                        dtInsertDateEnd = DateTimeConverter.ChangeShamsiToMiladi(InsertDateEnd);

                    if (!String.IsNullOrEmpty(InsertDateStart) && !String.IsNullOrEmpty(InsertDateEnd))
                        Products = Products.Where(s => s.InsertDate >= dtInsertDateStart && s.InsertDate <= dtInsertDateEnd);
                    else if (!String.IsNullOrEmpty(InsertDateStart))
                        Products = Products.Where(s => s.InsertDate >= dtInsertDateStart);
                    else if (!String.IsNullOrEmpty(InsertDateEnd))
                        Products = Products.Where(s => s.InsertDate <= dtInsertDateEnd);

                    #endregion

                    int pageSize = 10;
                    if (PageSize.HasValue)
                    {
                        if (PageSize.Value > 100)
                            pageSize = 100;
                        else if (pageSize < 10)
                            pageSize = 10;
                        else
                            pageSize = PageSize.Value;
                    }
                    ViewBag.PageSize = pageSize;
                    int pageNumber = (page ?? 1);


                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "مدیریت اصلاح گروهی قیمت ها", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "PriceGroupModification", "Index", true, 200, " نمایش صفحه مدیریت تخفیف گروهی قیمت ها", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Products.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));

                }
                else
                    return RedirectToAction("Index", "Home");

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PriceGroupModification", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }

        // GET: Admin/Products/Create
        public virtual ActionResult Create(bool? BrandOrCat)
        {
            if (!BrandOrCat.HasValue)
                return Redirect("/Admin/PriceGroupModification/");

            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 26, 1))
                {
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 18 && x.Name == "تعریف اصلاح گروهی قیمت جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                    #region Load DropDown List

                    ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == setting.LanguageId, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
                    if (BrandOrCat.Value == true)
                    {
                        var ProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.LanguageId == setting.LanguageId, x => x.OrderBy(s => s.ParrentId)).ToList();
                        ProductCategories.Add(new ProductCategory { Id = -1, Name = "--هیچکدام--", Title = "--هیچکدام--", LanguageId = 1, Descr = "--هیچکدام--" });
                        ViewBag.ProductCategories = new SelectList(ProductCategories.OrderBy(x => x.Id), "Id", "Name");
                    }
                    var brands = uow.BrandRepository.Get(x => x, x => x.LanguageId == setting.LanguageId, x => x.OrderBy(s => s.Name)).ToList();
                    brands.Add(new Brand { Id = -1, Name = "--هیچکدام--", Title = "--هیچکدام--", LanguageId = 1, MeteDescription = "--هیچکدام--" });
                    if (BrandOrCat.Value == false)
                        brands.Add(new Brand { Id = 0, Name = "--همه--", Title = "--همه--", LanguageId = 1, MeteDescription = "--همه--" });

                    ViewBag.BrandId = new SelectList(brands.OrderBy(x => x.Id), "Id", "Name");


                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);
                    ViewBag.LanguagenameString = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                    ViewBag.BrandOrCat = BrandOrCat;
                    #endregion

                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "PriceGroupModification", "Create", true, 200, " نمایش صفحه تعریف اصلاح قیمت جدید", DateTime.Now, User.Identity.GetUserId());

                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {


                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PriceGroupModification", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId()); throw;
            }
        }

        [HttpPost]
        public virtual ActionResult Create(Domain.ProductPriceGroupModification model, int ProductCatId)
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            XMLReader readXML = new XMLReader(setting.StaticContentDomain);

            #region Load DropDown List

            ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == setting.LanguageId, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
            if (model.BrandOrCat == true)
            {
                var ProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.LanguageId == setting.LanguageId, x => x.OrderBy(s => s.ParrentId)).ToList();
                ProductCategories.Add(new ProductCategory { Id = -1, Name = "--هیچکدام--", Title = "--هیچکدام--", LanguageId = 1, Descr = "--هیچکدام--" });
                ViewBag.ProductCategories = new SelectList(ProductCategories.OrderBy(x => x.Id), "Id", "Name");
            }
            var brands = uow.BrandRepository.Get(x => x, x => x.LanguageId == setting.LanguageId, x => x.OrderBy(s => s.Name)).ToList();
            brands.Add(new Brand { Id = -1, Name = "--هیچکدام--", Title = "--هیچکدام--", LanguageId = 1, MeteDescription = "--هیچکدام--" });
            if (model.BrandOrCat == false)
                brands.Add(new Brand { Id = 0, Name = "--همه--", Title = "--همه--", LanguageId = 1, MeteDescription = "--همه--" });

            ViewBag.BrandId = new SelectList(brands.OrderBy(x => x.Id), "Id", "Name");


            ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);
            ViewBag.LanguagenameString = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
            ViewBag.BrandOrCat = model.BrandOrCat;
            #endregion


            try
            {
                if (ModelState.IsValid)
                {
                    if (model.BrandOrCat == false)
                    {
                        if (ProductCatId == 0)
                        {
                            ViewBag.Error = "دسته بندی انتخاب نشده است !";
                            return View(model);

                        }
                        //برند انتخاب شده است
                        if (model.BrandId.HasValue && model.BrandId != -1)
                        {
                            //همه برندها
                            if (model.BrandId.Value == 0)
                            {
                                List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ProductCatId)).ToList();
                                foreach (var item in uow.ProductPriceRepository.Get(x => x, x => x.Product.ProductCategories.Any(s => CatIds.Contains(s.Id))))
                                {

                                    item.Price = ChangePrice(item.Price, model.ValueType, model.IncreaseType, model.Value);
                                    uow.ProductPriceRepository.Update(item);
                                }
                            }
                            // برند انتخابی
                            else
                            {
                                List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ProductCatId)).ToList();
                                foreach (var item in uow.ProductPriceRepository.Get(x => x, x => x.Product.BrandId == model.BrandId && x.Product.ProductCategories.Any(s => CatIds.Contains(s.Id))))
                                {

                                    item.Price = ChangePrice(item.Price, model.ValueType, model.IncreaseType, model.Value);
                                    uow.ProductPriceRepository.Update(item);
                                }
                            }
                        }
                        //گروه انتخاب شده است
                        else
                        {
                            List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ProductCatId)).ToList();
                            foreach (var item in uow.ProductPriceRepository.Get(x => x, x => x.Product.ProductCategories.Any(s => CatIds.Contains(s.Id))))
                            {

                                item.Price = ChangePrice(item.Price, model.ValueType, model.IncreaseType, model.Value);
                                uow.ProductPriceRepository.Update(item);
                            }
                        }
                        uow.Save();
                        model.InsertDate = DateTime.Now;
                        model.UserId = User.Identity.GetUserId();
                        if (model.BrandId == 0 || model.BrandId == -1)
                            model.BrandId = null;
                        model.CatId = ProductCatId;
                        uow.ProductPriceGroupModificationRepository.Insert(model);
                        uow.Save();

                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "PriceGroupModification", "Create", true, 200, "   تعریف اصلاح قیمت جدید", DateTime.Now, User.Identity.GetUserId());

                        return RedirectToAction("Index");


                    }
                    else
                    {
                        if (model.BrandId == -1)
                        {
                            ViewBag.Error = "برند انتخاب نشده است !";
                            return View(model);

                        }
                        //همه برندها
                        if (ProductCatId == -1)
                        {
                            foreach (var item in uow.ProductPriceRepository.Get(x => x, x => x.Product.BrandId == model.BrandId))
                            {

                                item.Price = ChangePrice(item.Price, model.ValueType, model.IncreaseType, model.Value);
                                uow.ProductPriceRepository.Update(item);
                            }
                        }
                        // برند انتخابی
                        else
                        {
                            List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ProductCatId)).ToList();
                            foreach (var item in uow.ProductPriceRepository.Get(x => x, x => x.Product.BrandId == model.BrandId && x.Product.ProductCategories.Any(s => CatIds.Contains(s.Id))))
                            {

                                item.Price = ChangePrice(item.Price, model.ValueType, model.IncreaseType, model.Value);
                                uow.ProductPriceRepository.Update(item);
                            }
                        }

                        uow.Save();
                        model.InsertDate = DateTime.Now;
                        model.UserId = User.Identity.GetUserId();
                        if (ProductCatId == 0 || ProductCatId == -1)
                            model.CatId = null;
                        uow.ProductPriceGroupModificationRepository.Insert(model);
                        uow.Save();

                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "PriceGroupModification", "Create", true, 200, "   تعریف اصلاح قیمت جدید", DateTime.Now, User.Identity.GetUserId());

                        return RedirectToAction("Index");

                    }
                }

                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PriceGroupModification", "Create", true, 500, "خطا در تعریف اصلاح قیمت جدید", DateTime.Now, User.Identity.GetUserId());

                return View(model);
            }
            catch (Exception x)
            {
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PriceGroupModification", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId()); throw;

                return View(model);
            }
        }
        public virtual JsonResult GetBrands(int CatId)
        {
            uow = new UnitOfWorkClass();
            List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", CatId)).ToList();
            var brands = uow.BrandRepository.Get(x => x, x => x.Products.SelectMany(a => a.ProductCategories.Where(s => CatIds.Contains(s.Id))).Any()).OrderBy(x => x.Title).Select(x => x.Id);
            return Json(new
            {
                data = brands,
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult GetCats(int BrandId)
        {
            uow = new UnitOfWorkClass();
            var cats = uow.ProductCategoryRepository.Get(x => x, x => x.Products.Any(a => a.BrandId == BrandId)).OrderBy(x => x.Title).Select(x => x.Id);
            return Json(new
            {
                data = cats,
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);
        }
        private long ChangePrice(long price, bool ValueType, bool IncreaseType, long Value)
        {
            long cprice = 0;
            if (ValueType)
            {
                if (IncreaseType)
                    cprice = price + Value;
                else
                    cprice = price - Value;
            }
            else
            {
                if (IncreaseType)
                    cprice = price + (Convert.ToInt64(price * (Value * 0.01)));
                else
                    cprice = price - (Convert.ToInt64(price * (Value * 0.01)));
            }
            return Convert.ToInt64(Math.Ceiling(cprice * 0.001) * 1000);
        }

        public virtual JsonResult Undo(int id)
        {
            uow = new UnitOfWorkClass();
            var prp = uow.ProductPriceGroupModificationRepository.GetByID(id);

            if (prp.BrandId.HasValue && prp.CatId.HasValue)
            {
                List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", prp.CatId)).ToList();
                foreach (var item in uow.ProductPriceRepository.Get(x => x, x => x.Product.BrandId == prp.BrandId && x.Product.ProductCategories.Any(s => CatIds.Contains(s.Id))))
                {

                    item.Price = Convert.ToInt64(item.Price / (1 + ((prp.Value * 0.01))));
                    item.Price = Convert.ToInt64(Math.Ceiling(item.Price * 0.001) * 1000);
                    uow.ProductPriceRepository.Update(item);
                }
            }
            else if (prp.BrandId.HasValue)
            {
                foreach (var item in uow.ProductPriceRepository.Get(x => x, x => x.Product.BrandId == prp.BrandId))
                {

                    item.Price = Convert.ToInt64(item.Price / (1 + ((prp.Value * 0.01))));
                    item.Price = Convert.ToInt64(Math.Ceiling(item.Price * 0.001) * 1000);
                    uow.ProductPriceRepository.Update(item);
                }
            }
            else if (prp.CatId.HasValue)
            {
                List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", prp.CatId)).ToList();
                foreach (var item in uow.ProductPriceRepository.Get(x => x, x => x.Product.ProductCategories.Any(s => CatIds.Contains(s.Id))))
                {

                    item.Price = Convert.ToInt64(item.Price / (1 + ((prp.Value * 0.01))));
                    item.Price = Convert.ToInt64(Math.Ceiling(item.Price * 0.001) * 1000);
                    uow.ProductPriceRepository.Update(item);
                }
            }
            uow.ProductPriceGroupModificationRepository.Delete(prp);
            uow.Save();

            return Json(new
            {
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);
        }
    }
}