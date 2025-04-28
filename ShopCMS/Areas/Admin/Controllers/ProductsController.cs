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
using CoreLib.Infrastructure;
using Microsoft.Ajax.Utilities;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class ProductsController : Controller
    {
        private UnitOfWorkClass uow;

        // GET: Admin/Products
        public ActionResult Index(int? PageSize, string sortOrder, string ProductCatId, string ProductCatIdFilter, string NameString, string NameFilter, string IsActive, string IsActiveFilter, string BrandId, string BrandFilter, string ProductTypeId, string ProductTypeFilter, string ProductCodeString, string ProductCodeFilter, string ProductStateId, string ProductStateFilter, string ProductInsertStateId, string ProductInsertStateFilter, string LanguagenameString, string LanguagenameFilter, string InsertDateStart, string InsertDateStartFilter, string InsertDateEnd, string InsertDateEndFilter, string SellerId, string SellerFilter, int? page)
        {

            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
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

                    List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "True" }, new SelectListItem() { Text = "غیرفعال", Value = "False" } };
                    ViewBag.IsActive = IsActiveSelectListItem;
                    List<SelectListItem> stateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در حال بررسی", Value = "1" }, new SelectListItem() { Text = "بررسی مجدد", Value = "2" }, new SelectListItem() { Text = "عدم تایید", Value = "3" }, new SelectListItem() { Text = "تایید", Value = "4" } };
                    ViewBag.ProductInsertStateId = stateSelectListItem;

                    ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langid, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
                    ViewBag.BrandId = new SelectList(uow.BrandRepository.Get(x => x, x => x.LanguageId == langid, x => x.OrderBy(s => s.Name)), "Id", "Name");
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);
                    ViewBag.ProductStateId = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title");
                    ViewBag.SellerId = new SelectList(uow.SellerRepository.Get(x => x).Select(x => x.User), "Id", "LastName");


                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameString = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                    #endregion

                    #region search
                    if (string.IsNullOrEmpty(ProductCatId))
                        ProductCatId = ProductCatIdFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(NameString))
                        NameString = NameFilter;
                    else
                    {
                        page = 1;

                        NameString = NameString.ToLower();
                    }

                    if (string.IsNullOrEmpty(IsActive))
                        IsActive = IsActiveFilter;
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
                    if (string.IsNullOrEmpty(ProductCodeString))
                        ProductCodeString = ProductCodeFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ProductStateId))
                        ProductStateId = ProductStateFilter;
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
                    if (string.IsNullOrEmpty(SellerId))
                        SellerId = SellerFilter;
                    else
                        page = 1;

                    ViewBag.ProductCatIdFilter = ProductCatId;
                    ViewBag.NameFilter = NameString;
                    ViewBag.IsActiveFilter = IsActive;
                    ViewBag.BrandFilter = BrandId;
                    ViewBag.ProductTypeFilter = ProductTypeId;
                    ViewBag.ProductCodeFilter = ProductCodeString;
                    ViewBag.ProductStateFilter = ProductStateId;
                    ViewBag.ProductInsertStateFilter = ProductInsertStateId;
                    ViewBag.LanguagenameFilter = LanguagenameString;
                    ViewBag.InsertDateEndFilter = InsertDateEnd;
                    ViewBag.InsertDateStartFilter = InsertDateStart;
                    ViewBag.SellerFilter = SellerId;

                    var Products = uow.ProductRepository.GetQueryList().AsNoTracking().Include("ProductPrices").Include("ProductImages.Image").Include("ProductCategories").Include("Brand").Include("ProductPrices.ProductState");


                    if (!String.IsNullOrEmpty(NameString))
                    {
                        var color = uow.ProductAttributeItemColorRepository.Get(x => x, x => x.Color.Contains(NameString)).FirstOrDefault();
                        string colorId = "0";
                        if (color != null)
                            colorId = color.Id.ToString();
                        var Garanty = uow.ProductAttributeItemRepository.Get(x => x, x => x.Value.Contains(NameString)).FirstOrDefault();
                        string GarantyId = "0";
                        if (Garanty != null)
                            GarantyId = Garanty.Id.ToString();
                        Products = from s in Products
                                   where
                                        (s.LatinName != null && s.LatinName.ToLower().Contains(NameString)) ||
                                        (s.Name != null && s.Name.ToLower().Contains(NameString)) ||
                                        (s.Code != null && s.Code.ToLower().Contains(NameString)) ||
                                        (s.Code != null && s.Code.ToLower().Contains(NameString)) ||
                                        (s.ProductPrices.Any(a => a.ProductAttributeSelectModelId != null && a.ProductAttributeSelectModel.Value.ToLower().Contains(NameString))) ||
                                        (s.ProductPrices.Any(a => a.ProductAttributeSelectSizeId != null && a.ProductAttributeSelectSize.Value.ToLower().Contains(NameString))) ||
                                        (s.ProductPrices.Any(a => a.ProductAttributeSelectWeightId != null && a.ProductAttributeSelectweight.Value.ToLower().Contains(NameString))) ||
                                        (s.ProductPrices.Any(a => a.ProductAttributeSelectColorId != null && a.ProductAttributeSelectColor.Value.ToLower().Contains(NameString))) ||
                                        (s.ProductPrices.Any(a => a.ProductAttributeSelectGarantyId != null && a.ProductAttributeSelectGaranty.Value.ToLower().Contains(NameString)))
                                   select s;
                    }
                    if (!String.IsNullOrEmpty(LanguagenameString))
                        Products = Products.Where(s => s.LanguageId == langid);
                    if (!String.IsNullOrEmpty(ProductCatId))
                    {
                        int ctId = Convert.ToInt32(ProductCatId);
                        if (ctId > 0)
                        {

                            List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ctId)).ToList();
                            Products = Products.Where(s => s.ProductCategories.Any(x => CatIds.Contains(x.Id)));
                        }
                    }
                    if (!String.IsNullOrEmpty(IsActive))
                    {
                        bool isActive = Convert.ToBoolean(IsActive);
                        Products = Products.Where(s => s.IsActive == isActive);
                    }
                    if (!String.IsNullOrEmpty(BrandId))
                    {
                        int value = int.Parse(BrandId);
                        Products = Products.Where(s => s.BrandId == value);
                    }
                    if (!String.IsNullOrEmpty(ProductTypeId))
                    {
                        int value = int.Parse(ProductTypeId);
                        Products = Products.Where(s => s.ProductTypeId == value);
                    }
                    if (!String.IsNullOrEmpty(ProductCodeString))
                    {
                        Products = Products.Where(s => s.Code == ProductCodeString || s.ProductPrices.Any(x => x.code == ProductCodeString));
                    }
                    if (!String.IsNullOrEmpty(ProductStateId))
                    {
                        int value = int.Parse(ProductStateId);
                        Products = Products.Where(s => s.ProductPrices.Any(x => x.ProductStateId == value && x.IsDefault));
                    }
                    if (!String.IsNullOrEmpty(ProductInsertStateId))
                    {

                        int value = int.Parse(ProductInsertStateId);
                        Products = Products.Where(s => s.state == value);
                    }
                    if (!String.IsNullOrEmpty(SellerId))
                    {
                        int value = int.Parse(SellerId);
                        Products = Products.Where(s => s.ProductSellers.Any(x => x.SellerId == value));
                    }
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

                    #region Sort
                    switch (sortOrder)
                    {
                        case "Code":
                            Products = Products.OrderBy(s => s.Code);
                            ViewBag.CurrentSort = "Code";
                            break;
                        case "Code_desc":
                            Products = Products.OrderByDescending(s => s.Code);
                            ViewBag.CurrentSort = "Code_desc";
                            break;
                        case "Name":
                            Products = Products.OrderBy(s => s.Name);
                            ViewBag.CurrentSort = "Name";
                            break;
                        case "Name_desc":
                            Products = Products.OrderByDescending(s => s.Name);
                            ViewBag.CurrentSort = "Name_desc";
                            break;
                        case "Brand":
                            Products = Products.OrderBy(s => s.Brand.Name);
                            ViewBag.CurrentSort = "Brand";
                            break;
                        case "Brand_desc":
                            Products = Products.OrderByDescending(s => s.Brand.Name);
                            ViewBag.CurrentSort = "Brand_desc";
                            break;
                        case "ProductType":
                            Products = Products.OrderBy(s => s.ProductType.Title);
                            ViewBag.CurrentSort = "ProductType";
                            break;
                        case "ProductType_desc":
                            Products = Products.OrderByDescending(s => s.ProductType.Title);
                            ViewBag.CurrentSort = "ProductType_desc";
                            break;
                        case "ProductState":
                            Products = Products.OrderBy(s => s.ProductPrices.Select(a=>a.ProductState.Title));
                            ViewBag.CurrentSort = "ProductState";
                            break;
                        case "ProductState_desc":
                            Products = Products.OrderByDescending(s => s.ProductPrices.Select(a => a.ProductState.Title));
                            ViewBag.CurrentSort = "ProductState_desc";
                            break;
                        case "state":
                            Products = Products.OrderBy(s => s.state);
                            ViewBag.CurrentSort = "state";
                            break;
                        case "state_desc":
                            Products = Products.OrderByDescending(s => s.state);
                            ViewBag.CurrentSort = "state_desc";
                            break;
                        case "Visits":
                            Products = Products.OrderBy(s => s.Visits);
                            ViewBag.CurrentSort = "Visits";
                            break;
                        case "Visits_desc":
                            Products = Products.OrderByDescending(s => s.Visits);
                            ViewBag.CurrentSort = "Visits_desc";
                            break;
                        case "InsertDate":
                            Products = Products.OrderBy(s => s.UpdateDate);
                            ViewBag.CurrentSort = "InsertDate";
                            break;
                        case "InsertDate_desc":
                            Products = Products.OrderByDescending(s => s.UpdateDate);
                            ViewBag.CurrentSort = "InsertDate_desc";
                            break;
                        case "IsActive":
                            Products = Products.OrderBy(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive";
                            break;
                        case "IsActive_desc":
                            Products = Products.OrderByDescending(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive_desc";
                            break;
                        case "Language":
                            Products = Products.OrderBy(s => s.LanguageId);
                            ViewBag.CurrentSort = "Language";
                            break;
                        case "Language_desc":
                            Products = Products.OrderByDescending(s => s.LanguageId);
                            ViewBag.CurrentSort = "Language_desc";
                            break;
                        default:  // Name ascending 
                            Products = Products.OrderByDescending(s => s.UpdateDate).ThenByDescending(x => x.Id);
                            break;
                    }

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

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "مدیریت محصولات", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Products", "Index", true, 200, " نمایش صفحه مدیریت محصولات", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Products.ToPagedList(pageNumber, pageSize));

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Products/Create
        public virtual ActionResult Create(int? Id)
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                #region Check License


                #endregion


                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 18 && x.Name == "تعریف محصول جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                    if (Id.HasValue)
                    {
                        var Product = uow.ProductRepository.Get(x => x, x => x.Id == Id, null, "VideoAttachment,ProductAdvantages,ProductDisAdvantages,ProductRankSelects.ProductRankGroupSelect.ProductRank,ProductSellers,ProductCategories,Brand,ProductType,ProductPrices.ProductState,ProductIcon,Tax,ProductImages,ProductImages.Image,ProductSendWaySelects,ProductCourses,Tags,ProductAttributeSelects").FirstOrDefault();
                        if (Product != null)
                        {
                            //ViewBag.ProductAttributeGroups = uow.ProductAttributeGroupSelectRepository.Get(x => x, null,null, "ProductAttributeGroup").OrderBy(x => x.DisplayGroupOrder).GroupBy(x => x.GroupId).Select(x => x.First());
                            int catid = Product.ProductCategories.First().Id;
                            ViewBag.ProductAttributeGroups = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.ProductAttributeGroup.Primary || x.ProductAttributeGroup.ProductAttributeGroupProductCategorys.Any(s => s.ProductCategoryId == catid), x => x.OrderBy(o => o.DisplayGroupOrder), "ProductAttributeGroup,ProductAttribute,ProductAttribute.ProductAttributeItemColors,ProductAttribute.ProductAttributeItems");
                            ViewBag.stateList = new List<SelectListItem>() { new SelectListItem() { Text = "در حال بررسی", Value = "1", Selected = Product.state == 1 }, new SelectListItem() { Text = "بررسی مجدد", Value = "2", Selected = Product.state == 2 }, new SelectListItem() { Text = "عدم تایید", Value = "3", Selected = Product.state == 3 }, new SelectListItem() { Text = "تایید", Value = "4", Selected = Product.state == 4 } };
                            ViewBag.LanguageList = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", Product.LanguageId);
                            ViewBag.BrandList = new SelectList(uow.BrandRepository.Get(x => x), "Id", "Name", Product.BrandId);
                            ViewBag.ProductTypeList = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", Product.ProductTypeId);
                            //ViewBag.ProductStateList = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title", Product.ProductStateId);
                            ViewBag.ProductIconList = new SelectList(uow.ProductIconRepository.Get(x => x), "Id", "Title", Product.ProductIconId);
                            ViewBag.SellerList = new MultiSelectList(uow.SellerRepository.Get(x => x, x => x.IsActive, null, "User"), "Id", "User.LastName", Product.ProductSellers.Select(x => x.SellerId));
                            ViewBag.TaxList = new SelectList(uow.TaxRepository.Get(x => x), "Id", "Title", Product.TaxId);
                            ViewBag.ProductSendWayList = new MultiSelectList(uow.ProductSendWayRepository.Get(x => x, x => x.FreeOff == false), "Id", "Title", Product.ProductSendWaySelects.Select(x => x.ProductSendWayId));
                            ViewBag.ProductCourses = new SelectList(uow.ProductCourseRepository.Get(x => x, x => x.ProductId == Product.Id), "Id", "Title");
                            ViewBag.ProductRankGroupSelect = uow.ProductRankGroupSelectRepository.Get(x => x, null, x => x.OrderBy(s => s.DisplayGroupOrder), "ProductRankGroup,ProductRank");
                            ViewBag.ProductRankSelectValue = uow.ProductRankSelectValueRepository.Get(x => x, x => x.ProductRankSelect.ProductId == Product.Id && x.IsPrimary);
                            ViewBag.CurrentCatId = catid;
                            ViewBag.TagId = uow.TagRepository.GetByReturnQueryable(x => x).OrderByDescending(x => x.Id);
                            #region Load Tags
                            List<string> CurrentTags = new List<string>();
                            List<Tag> CurrentTagItems = new List<Tag>();
                            if (Product.Tags.Any())
                            {
                                foreach (Tag item in Product.Tags)
                                {
                                    CurrentTags.Add(item.Id.ToString());
                                    Tag oTag = new Tag();
                                    oTag.Id = item.Id;
                                    oTag.TagName = item.TagName;
                                    CurrentTagItems.Add(oTag);
                                }
                                Session["CurrentTags"] = CurrentTags;
                                ViewBag.CurrentTagItems = CurrentTagItems;
                            }
                            #endregion

                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(1, "Products", "Create", true, 200, " نمایش صفحه ایجاد محصول", DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return View(Product);


                        }
                        else
                            return Redirect("~/Admin/Products/Create");
                    }
                    ViewBag.stateList = new List<SelectListItem>() { new SelectListItem() { Text = "در حال بررسی", Value = "1" }, new SelectListItem() { Text = "بررسی مجدد", Value = "2" }, new SelectListItem() { Text = "عدم تایید", Value = "3" }, new SelectListItem() { Text = "تایید", Value = "4" } };
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                    ViewBag.BrandId = new SelectList(uow.BrandRepository.Get(x => x), "Id", "Name");
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title");
                    ViewBag.ProductStateId = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title");
                    ViewBag.ProductIconList = new SelectList(uow.ProductIconRepository.Get(x => x), "Id", "Title");
                    ViewBag.SellerList = new MultiSelectList(uow.SellerRepository.Get(x => x, x => x.IsActive, null, "User"), "Id", "User.LastName");
                    ViewBag.TaxId = new SelectList(uow.TaxRepository.Get(x => x), "Id", "Title");
                    ViewBag.ProductSendWayList = new MultiSelectList(uow.ProductSendWayRepository.Get(x => x, x => x.FreeOff == false), "Id", "Title");
                    ViewBag.CurrentCatId = null;

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Products", "Create", true, 200, " نمایش صفحه ایجاد محصول", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // PoST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Create(Product Product, int? ProductCatId, int[] SellerId, string[] janebi, int[] ProductSendWayId, string mainJanebi)
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    Product.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Product.Title);
                    Product.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Product.Name);
                    if (!String.IsNullOrEmpty(Product.LatinName))
                        Product.LatinName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Product.LatinName);
                    Product.Descr = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Product.Descr);

                    if (Product.Id > 0)//edit
                    {

                        Product.UserId = User.Identity.GetUserId();
                        if (SellerId == null)
                            ViewBag.Error = " فروشنده انتخاب نشده است ";
                        else if (ProductSendWayId == null)
                            ViewBag.Error = " روش ارسال انتخاب نشده است ";
                        else if (!String.IsNullOrEmpty(Product.ScratchCode) && (uow.ProductRepository.Get(x => x, x => x.ScratchCode == Product.ScratchCode && x.Id != Product.Id).Any()))
                            ViewBag.Error = " کد اسکرچ تکراری ست ";
                        //else if (uow.ProductRepository.Any(x => x.Id, x => x.PageAddress == Product.PageAddress && x.Id != Product.Id))
                        //    ViewBag.Error = "عنوان محصول وارد شده تکراری است";


                        else
                        {
                            //#region ProductImage
                            //var primage = uow.ProductImageRepository.Get(x => x, x => x.IsMain && x.ProductId == Product.Id).FirstOrDefault();
                            //primage.AttachementId = new Guid(Cover);
                            //uow.ProductImageRepository.Update(primage);
                            //#endregion

                            #region Janebi
                            var currentJanebi = uow.ProductImageRepository.Get(x => x, x => x.ProductId == Product.Id && !x.ProductPriceId.HasValue);
                            if (janebi == null)
                                uow.ProductImageRepository.Delete(currentJanebi.ToList());
                            else
                            {
                                foreach (var item in currentJanebi)
                                {
                                    item.DisplaySort = janebi.ToList().FindIndex(x => new Guid(x) == item.AttachementId);
                                    if (item.AttachementId.ToString() == mainJanebi)
                                        item.IsMain = true;
                                    else
                                        item.IsMain = false;
                                }
                                int i = 0;
                                bool isdefaultset = false;
                                IEnumerable<string> AddedJanebi = janebi.Where(x => !currentJanebi.Select(s => s.AttachementId).ToList().Contains(new Guid(x)));
                                foreach (var item in AddedJanebi)
                                {
                                    if (isdefaultset == false && item == mainJanebi)
                                    {
                                        uow.ProductImageRepository.Insert(new ProductImage() { ProductId = Product.Id, AttachementId = new Guid(item), Data = Product.Descr, IsImage = true, IsMain = true, DisplaySort = i });
                                        isdefaultset = true;
                                    }
                                    else
                                        uow.ProductImageRepository.Insert(new ProductImage() { ProductId = Product.Id, AttachementId = new Guid(item), Data = Product.Descr, IsImage = true, IsMain = false, DisplaySort = i });
                                    i++;
                                }
                                IEnumerable<ProductImage> NotExistJanebi = currentJanebi.Where(x => !janebi.Contains(x.AttachementId.ToString()));
                                uow.ProductImageRepository.Delete(NotExistJanebi.ToList());
                            }
                            #endregion

                            #region Seller
                            var currentSellers = uow.ProductSellerRepository.Get(x => x, x => x.ProductId == Product.Id);
                            if (currentSellers == null)
                                uow.ProductSellerRepository.Delete(currentSellers.ToList());
                            else
                            {
                                IEnumerable<int> AddedSellers = SellerId.Where(x => !currentSellers.Select(s => s.SellerId).ToList().Contains(x));
                                foreach (var item in AddedSellers)
                                    uow.ProductSellerRepository.Insert(new ProductSeller() { ProductId = Product.Id, SellerId = item });

                                IEnumerable<ProductSeller> NotExistSellers = currentSellers.Where(x => !SellerId.Contains(x.SellerId));
                                uow.ProductSellerRepository.Delete(NotExistSellers.ToList());
                            }
                            #endregion

                            #region SendWay
                            var SendWaySelects = uow.ProductSendWaySelectRepository.Get(x => x, x => x.ProductId == Product.Id);
                            if (SendWaySelects == null)
                                uow.ProductSendWaySelectRepository.Delete(SendWaySelects.ToList());
                            else
                            {
                                IEnumerable<int> AddedSendWays = ProductSendWayId.Where(x => !SendWaySelects.Select(s => s.ProductSendWayId).ToList().Contains(x));
                                foreach (var item in AddedSendWays)
                                    uow.ProductSendWaySelectRepository.Insert(new ProductSendWaySelect() { ProductId = Product.Id, ProductSendWayId = item });

                                IEnumerable<ProductSendWaySelect> NotExistSendWays = SendWaySelects.Where(x => !ProductSendWayId.Contains(x.ProductSendWayId));
                                uow.ProductSendWaySelectRepository.Delete(NotExistSendWays.ToList());
                            }

                            #endregion


                            #region Title & pageAddress
                            var productPrice = uow.ProductPriceRepository.Get(x=>x,x => x.ProductId == Product.Id && x.IsDefault && x.IsActive,null, "ProductAttributeSelectModel,ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").FirstOrDefault();
                            if (productPrice != null)
                            {
                                string title = Product.Name;
                                string pageaddress = Product.Name;
                                if (productPrice.ProductAttributeSelectModelId.HasValue)
                                {
                                    title += " مدل " + productPrice.ProductAttributeSelectModel.Value;
                                    pageaddress += " " + productPrice.ProductAttributeSelectModel.Value;
                                }
                                if (productPrice.ProductAttributeSelectSizeId.HasValue)
                                    title += " سایز " + productPrice.ProductAttributeSelectSize.Value + productPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit;
                                if (productPrice.ProductAttributeSelectColorId.HasValue)
                                    title += " " + uow.ProductAttributeItemColorRepository.GetByID(int.Parse(uow.ProductAttributeSelectRepository.GetByID(productPrice.ProductAttributeSelectColorId).Value)).Color;

                                Product.PageAddress = CommonFunctions.NormalizeAddressWithSpace(pageaddress);
                                Product.Title = title;
                            }
                            #endregion

                            Product.UpdateDate = DateTime.Now;
                            uow.ProductRepository.Update(Product);
                            uow.Save();

                            //if (uow.ProductLetmeknowRepository.CheckLetmeKnowsOfProduct(Product.Id).Result == true && Product.ProductStateId < 3)
                            //{
                            //    SmsService sms = new SmsService();
                            //    IdentityMessage iPhonemessage = new IdentityMessage();
                            //    foreach (var item in uow.ProductLetmeknowRepository.GetLetmeKnowsOfProduct(Product.Id).Result)
                            //    {
                            //        await sms.SendSMSAsync(iPhonemessage, "NewOrderAdmin", isestalam ? "استعلامی" : "عادی", null, null, string.Format("{0:n0}", wallet.Price.ToString()), user.CityId.HasValue ? user.CityEntity.Name : order.OrderDeliveries.First().UserAddress.CityEntity.Name);

                            //    }
                            //}

                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(2, "Products", "Create", false, 200, " ویرایش محصول " + Product.Name, DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return RedirectToAction("Create", new { Id = Product.Id });
                        }
                    }
                    else//add
                    {
                        Product.PageAddress = Product.Title;
                        Product.IsActive = true;
                        Product.UserId = User.Identity.GetUserId();
                        Product.state = 4;
                        if (!ProductCatId.HasValue)
                            ViewBag.Error = "دسته بندی انتخاب نشده است";
                        else if (SellerId == null)
                            ViewBag.Error = " فروشنده انتخاب نشده است ";
                        else if (ProductSendWayId == null && Product.ProductTypeId < 3)
                            ViewBag.Error = " روش ارسال انتخاب نشده است ";
                        //else if (uow.ProductRepository.Get(x => x, x => x.Code == Product.Code).Any())
                        //    ViewBag.Error = " کد مرجع کالا تکراری ست ";
                        else if (!String.IsNullOrEmpty(Product.ScratchCode) && (uow.ProductRepository.Get(x => x, x => x.ScratchCode == Product.ScratchCode).Any()))
                            ViewBag.Error = " کد اسکرچ تکراری ست ";
                        //else if (uow.ProductRepository.Any(x => x.Id, x => x.PageAddress == Product.PageAddress))
                        //    ViewBag.Error = "عنوان محصول وارد شده تکراری است";

                        else
                        {
                            Product.ProductImages = new List<ProductImage>();
                            //Product.ProductImages.Add(new ProductImage() { AttachementId = new Guid(Cover), Data = Product.Descr, IsImage = true, IsMain = true });
                            if (janebi != null)
                            {
                                int i = 0;
                                bool isdefaultset = false;
                                foreach (var item in janebi)
                                {
                                    if (isdefaultset == false && item == mainJanebi)
                                    {
                                        uow.ProductImageRepository.Insert(new ProductImage() { ProductId = Product.Id, AttachementId = new Guid(item), Data = Product.Descr, IsImage = true, IsMain = true, DisplaySort = i });
                                        isdefaultset = true;
                                    }
                                    else
                                        uow.ProductImageRepository.Insert(new ProductImage() { ProductId = Product.Id, AttachementId = new Guid(item), Data = Product.Descr, IsImage = true, IsMain = false, DisplaySort = i });
                                    i++;
                                }
                            }
                            Product.ProductCategories = new List<ProductCategory>();
                            Product.ProductCategories.Add(uow.ProductCategoryRepository.GetByID(ProductCatId.Value));
                            Product.ProductSellers = new List<ProductSeller>();
                            foreach (int item in SellerId)
                                Product.ProductSellers.Add(new ProductSeller { SellerId = item });
                            Product.ProductSendWaySelects = new List<ProductSendWaySelect>();
                            if (ProductSendWayId != null)
                            {
                                foreach (int item in ProductSendWayId)
                                    Product.ProductSendWaySelects.Add(new ProductSendWaySelect { ProductSendWayId = item });
                            }
                            uow.ProductRepository.Insert(Product);
                            uow.Save();


                            #region add productvideo to videocontent
                            if (Product.Video.HasValue)
                            {
                                Content content = new Content()
                                {
                                    Abstract = Product.Abstract,
                                    ContentTypeId = 5,
                                    Descr = Product.Descr,
                                    InsertDate = DateTime.Now,
                                    IsActive = true,
                                    LanguageId = 1,
                                    PageAddress = Product.Title,
                                    Title = Product.Title,
                                    UserId = User.Identity.GetUserId(),
                                    Video = Product.Video.Value,
                                    CatId = uow.CategoryRepository.Get(x => x.Id, x => x.IsVideo && x.IsActive).First(),
                                    Cover = setting.Logo
                                };
                                uow.ContentRepository.Insert(content);
                                uow.Save();
                            }
                            #endregion


                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(2, "Products", "Create", false, 200, " ایجاد محصول " + Product.Name, DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return RedirectToAction("Create", new { Id = Product.Id });
                        }

                    }

                }
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 18 && x.Name == "تعریف محصول جدید", null, "HelpModuleSectionFields").FirstOrDefault();

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.stateList = new List<SelectListItem>() { new SelectListItem() { Text = "در حال بررسی", Value = "1", Selected = Product.state == 1 }, new SelectListItem() { Text = "بررسی مجدد", Value = "2", Selected = Product.state == 2 }, new SelectListItem() { Text = "عدم تایید", Value = "3", Selected = Product.state == 3 }, new SelectListItem() { Text = "تایید", Value = "4", Selected = Product.state == 4 } };
                ViewBag.LanguageList = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", Product.LanguageId);
                ViewBag.BrandList = new SelectList(uow.BrandRepository.Get(x => x), "Id", "Name", Product.BrandId);
                ViewBag.ProductTypeList = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", Product.ProductTypeId);
                //ViewBag.ProductStateList = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title", Product.ProductStateId);
                ViewBag.ProductIconList = new SelectList(uow.ProductIconRepository.Get(x => x), "Id", "Title", Product.ProductIconId);
                ViewBag.SellerList = new MultiSelectList(uow.SellerRepository.Get(x => x, x => x.IsActive, null, "User"), "Id", "User.LastName");
                ViewBag.TaxList = new SelectList(uow.TaxRepository.Get(x => x), "Id", "Title", Product.TaxId);
                ViewBag.ProductSendWayList = new MultiSelectList(uow.ProductSendWayRepository.Get(x => x, x => x.FreeOff == false), "Id", "Title");
                ViewBag.ProductCourses = new SelectList(uow.ProductCourseRepository.Get(x => x, x => x.ProductId == Product.Id), "Id", "Title");
                ViewBag.ProductRankGroupSelect = uow.ProductRankGroupSelectRepository.Get(x => x, null, x => x.OrderBy(s => s.DisplayGroupOrder), "ProductRankGroup,ProductRank");
                ViewBag.ProductRankSelectValue = uow.ProductRankSelectValueRepository.Get(x => x, x => x.ProductRankSelect.ProductId == Product.Id && x.IsPrimary);
                ViewBag.TagId = uow.TagRepository.GetByReturnQueryable(x => x).OrderByDescending(x => x.Id);
                return View();

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "Create", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 18 && x.Name == "تعریف محصول جدید", null, "HelpModuleSectionFields").FirstOrDefault();

                var pr2 = uow.ProductRepository.Get(x => x, x => x.Id == Product.Id, null, "VideoAttachment,ProductAdvantages, ProductDisAdvantages, ProductRankSelects.ProductRankGroupSelect.ProductRank, ProductSellers, ProductCategories, Brand, ProductType, ProductState, ProductIcon, Tax, ProductImages, ProductImages.Image, ProductSendWaySelects, ProductCourses, Tags, ProductAttributeSelects").FirstOrDefault();
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.stateList = new List<SelectListItem>() { new SelectListItem() { Text = "در حال بررسی", Value = "1", Selected = Product.state == 1 }, new SelectListItem() { Text = "بررسی مجدد", Value = "2", Selected = Product.state == 2 }, new SelectListItem() { Text = "عدم تایید", Value = "3", Selected = Product.state == 3 }, new SelectListItem() { Text = "تایید", Value = "4", Selected = Product.state == 4 } };
                ViewBag.LanguageList = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", Product.LanguageId);
                ViewBag.BrandList = new SelectList(uow.BrandRepository.Get(x => x), "Id", "Name", Product.BrandId);
                ViewBag.ProductTypeList = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", Product.ProductTypeId);
                //ViewBag.ProductStateList = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title", Product.ProductStateId);
                ViewBag.ProductIconList = new SelectList(uow.ProductIconRepository.Get(x => x), "Id", "Title", Product.ProductIconId);
                ViewBag.SellerList = new MultiSelectList(uow.SellerRepository.Get(x => x, x => x.IsActive, null, "User"), "Id", "User.LastName", pr2.ProductSellers.Select(x => x.SellerId));
                ViewBag.TaxList = new SelectList(uow.TaxRepository.Get(x => x), "Id", "Title", Product.TaxId);
                ViewBag.ProductSendWayList = new MultiSelectList(uow.ProductSendWayRepository.Get(x => x, x => x.FreeOff == false), "Id", "Title", pr2.ProductSendWaySelects.Select(x => x.ProductSendWayId));
                ViewBag.ProductCourses = new SelectList(uow.ProductCourseRepository.Get(x => x, x => x.ProductId == Product.Id), "Id", "Title");
                ViewBag.ProductRankGroupSelect = uow.ProductRankGroupSelectRepository.Get(x => x, null, x => x.OrderBy(a => a.DisplayGroupOrder), "ProductRankGroup,ProductRank");
                ViewBag.ProductRankSelectValue = uow.ProductRankSelectValueRepository.Get(x => x, x => x.ProductRankSelect.ProductId == Product.Id && x.IsPrimary);
                ViewBag.TagId = uow.TagRepository.GetByReturnQueryable(x => x).OrderByDescending(x => x.Id);

                ViewBag.Error = s.Message;
                return View();
            }
        }


        // GET: Admin/Products/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Product Product = uow.ProductRepository.Get(x => x, x => x.Id == id, null, "OrderRows").SingleOrDefault();
                    if (Product == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Products", "Delete", true, 200, " نمایش صفحه حذف محصولِ" + Product.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Product);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var Product = uow.ProductRepository.Get(x => x, x => x.Id == id, null, "VideoAttachment,ProductComments,OrderRows,ProductAdvantages,ProductDisAdvantages,ProductPrices,ProductCourses,ProductRankSelects.ProductRankGroupSelect.ProductRank,ProductFavorates,ProductFileInfos,ProductSellers,ProductLetmeknows,ProductMainAccessory,ProductMultipeMainItems,ProductOffers,ProductQuestions,ProductCategories,Brand,ProductType,ProductState,ProductIcon,Tax,ProductImages,ProductImages.Image,ProductSendWaySelects,ProductCourses,Tags,ProductAttributeSelects").FirstOrDefault();
                if (!Product.OrderRows.Any())
                {
                    foreach (var child in Product.ProductImages.ToList())
                        uow.ProductImageRepository.Delete(child);
                    foreach (var child in Product.ProductPrices.ToList())
                        uow.ProductPriceRepository.Delete(child);
                    foreach (var child in Product.ProductAttributeSelects.ToList())
                        uow.ProductAttributeSelectRepository.Delete(child);
                    foreach (var child in Product.ProductAdvantages.ToList())
                        uow.ProductAdvantageRepository.Delete(child);
                    foreach (var child in Product.ProductComments.ToList())
                        uow.ProductCommentRepository.Delete(child);
                    foreach (var child in Product.ProductCourses.ToList())
                        uow.ProductCourseRepository.Delete(child);
                    foreach (var child in Product.ProductDisAdvantages.ToList())
                        uow.ProductDisAdvantageRepository.Delete(child);
                    foreach (var child in Product.ProductFavorates.ToList())
                        uow.ProductFavorateRepository.Delete(child);
                    foreach (var child in Product.ProductFileInfos.ToList())
                        uow.ProductFileInfoRepository.Delete(child);
                    foreach (var child in Product.ProductMainAccessory.ToList())
                        uow.ProductAccessoryRepository.Delete(child);
                    foreach (var child in Product.ProductMultipeMainItems.ToList())
                        uow.ProductMultipeItemRepository.Delete(child);
                    //foreach (var child in Product.ProductOffers.ToList())
                    //    uow.ProductOfferRepository.Delete(child);
                    foreach (var child in Product.ProductQuestions.ToList())
                        uow.ProductQuestionRepository.Delete(child);
                    foreach (var child in Product.ProductRankSelects.ToList())
                        uow.ProductRankSelectRepository.Delete(child);
                    foreach (var child in Product.ProductSendWaySelects.ToList())
                        uow.ProductSendWaySelectRepository.Delete(child);
                    foreach (var child in Product.ProductSellers.ToList())
                        uow.ProductSellerRepository.Delete(child);
                    //uow.ProductPriceRepository.Delete(uow.ProductPriceRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductAttributeSelectRepository.Delete(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductAdvantageRepository.Delete(uow.ProductAdvantageRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductCommentRepository.Delete(uow.ProductCommentRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductCourseLessonRepository.Delete(uow.ProductCourseLessonRepository.Get(x => x, x => x.ProductCourse.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductCourseRepository.Delete(uow.ProductCourseRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductDisAdvantageRepository.Delete(uow.ProductDisAdvantageRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductFavorateRepository.Delete(uow.ProductFavorateRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductFileItemRepository.Delete(uow.ProductFileItemRepository.Get(x => x, x => x.ProductFileInfo.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductFileInfoRepository.Delete(uow.ProductFileInfoRepository.Get(x => x, x => x.ProductId == id).ToList().ToList());
                    //uow.Save();
                    //uow.ProductLetmeknowRepository.Delete(uow.ProductLetmeknowRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductAccessoryRepository.Delete(uow.ProductAccessoryRepository.Get(x => x, x => x.MainProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductMultipeItemRepository.Delete(uow.ProductMultipeItemRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductOfferRepository.Delete(uow.ProductOfferRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductQuestionRepository.Delete(uow.ProductQuestionRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductRankSelectValueRepository.Delete(uow.ProductRankSelectValueRepository.Get(x => x, x => x.ProductRankSelect.Product.Id == id).ToList());
                    //uow.Save();
                    //uow.ProductRankSelectRepository.Delete(uow.ProductRankSelectRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductSellerRepository.Delete(uow.ProductSellerRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    //uow.ProductSendWaySelectRepository.Delete(uow.ProductSendWaySelectRepository.Get(x => x, x => x.ProductId == id).ToList());
                    //uow.Save();
                    Product.Tags.Clear();
                    Product.ProductCategories.Clear();
                    uow.ProductRepository.Update(Product);
                    uow.Save();
                    uow.ProductRepository.Delete(Product);
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(4, "Products", "DeleteConfirmed", false, 200, "   حذف محصولِ" + Product.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Products", "DeleteConfirmed", false, 200, "   عدم حذف محصولِ" + Product.Id, DateTime.Now, User.Identity.GetUserId());
                return View(Product);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Products/EditCategory/5
        public virtual ActionResult EditCategory(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Product Product = uow.ProductRepository.Get(x => x, x => x.Id == id.Value, null, "ProductCategories").SingleOrDefault();
                    if (Product == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.Product = Product;
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 18 && x.Name == "ویرایش گروه و زیرگروه", null, "HelpModuleSectionFields").FirstOrDefault();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Products", "EditCategory", true, 200, " نمایش صفحه ویرایش ویرایش گروه و زیرگروه " + Product.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    ViewBag.ProductId = Product.Id;
                    return View(Product.ProductCategories.First());

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "EditCategory", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // POST: Admin/Products/EditCategory/5
        [HttpPost]
        public virtual ActionResult EditCategory(int CatId, int ProductId)
        {
            uow = new UnitOfWorkClass();
            try
            {
                Product Product = uow.ProductRepository.Get(x => x, x => x.Id == ProductId, null, "ProductCategories").SingleOrDefault();
                Product.ProductCategories.Clear();
                uow.Save();

                var ProductCategory = uow.ProductCategoryRepository.Get(x => x, x => x.Id == CatId).FirstOrDefault();
                Product.ProductCategories.Add(ProductCategory);
                uow.Save();


                return Json(new
                {
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {

                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult FAQ(int id, int? page)
        {
            uow = new UnitOfWorkClass();

            int pageSize = 8;
            int pageNumber = (page ?? 1);
            var product = uow.ProductRepository.GetByID(id);
            if (product == null)
                return Redirect("~/Admin");

            ViewBag.Name = product.Name;
            ViewBag.id = product.Id;
            ViewBag.Title = product.Title;

            #region EventLogger
            ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductQuestions", "FAQ", true, 200, " نمایش صفحه مدیریت پرسش های محصول " + product.Name, DateTime.Now, User.Identity.GetUserId());
            #endregion
            return View(uow.ProductQuestionRepository.Get(x => x, x => x.ProductId == id, x => x.OrderByDescending(s => s.Id)).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult FAQCreate(int id)
        {
            uow = new UnitOfWorkClass();
            var product = uow.ProductRepository.GetByID(id);
            if (product == null)
                return Redirect("~/Admin");

            var randomUser = uow.UserRepository.GetQueryList().AsNoTracking().OrderBy(s => Guid.NewGuid()).FirstOrDefault();
            if (randomUser != null)
                ViewBag.userFake = randomUser.FirstName;
            else
                ViewBag.userFake = "کاربر";

            ViewBag.product = product;

            return View();
        }

        [HttpPost]
        public ActionResult FAQCreate(ProductQuestion productQuestion, string question, string Answer, string FakeUserFullName)
        {
            uow = new UnitOfWorkClass();
            try
            {

                #region Add Question
                ProductQuestion prq = new ProductQuestion()
                {
                    InsertDate = DateTime.Now,
                    IsActive = true,
                    Like = 0,
                    Message = question,
                    ProductId = productQuestion.ProductId,
                    UnLike = 0,
                    UserId = User.Identity.GetUserId(),
                    Visited = true,
                    AdminAnswer = false,
                    FakeUserFullName = FakeUserFullName
                };
                uow.ProductQuestionRepository.Insert(prq);
                #endregion

                uow.Save();

                #region Add Answer
                ProductQuestion pra = new ProductQuestion()
                {
                    InsertDate = DateTime.Now,
                    IsActive = true,
                    Like = 0,
                    Message = Answer,
                    ProductId = productQuestion.ProductId,
                    ParrentId = prq.Id,
                    UnLike = 0,
                    UserId = User.Identity.GetUserId(),
                    Visited = true,
                    AdminAnswer = true
                };
                uow.ProductQuestionRepository.Insert(pra);
                #endregion

                uow.Save();


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductQuestions", "Create", false, 200, "افزودن پرسش در مورد محصول " + productQuestion.ProductId, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return RedirectToAction("FAQ", new { id = productQuestion.ProductId });
            }
            catch (Exception ex)
            {
                var randomUser = uow.UserRepository.GetQueryList().AsNoTracking().OrderBy(s => Guid.NewGuid()).FirstOrDefault();
                if (randomUser != null)
                    ViewBag.userFake = randomUser.FirstName;
                else
                    ViewBag.userFake = "کاربر";

                ViewBag.product = uow.ProductRepository.GetByID(productQuestion.ProductId);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "Create", false, 500, ex.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(productQuestion);
            }
        }



        public ActionResult FAQEdit(int id)
        {
            uow = new UnitOfWorkClass();
            var ProductQuestion = uow.ProductQuestionRepository.Get(x => x, x => x.Id == id, null, "Product").FirstOrDefault(); ;
            if (ProductQuestion == null)
                return Redirect("~/Admin");


            ViewBag.product = ProductQuestion.Product;

            return View(ProductQuestion);
        }

        [HttpPost]
        public ActionResult FAQEdit(ProductQuestion productQuestion, string question)
        {
            uow = new UnitOfWorkClass();
            try
            {

                #region Update Question Or Answer
                productQuestion.Message = question;
                uow.ProductQuestionRepository.Update(productQuestion);
                #endregion

                uow.Save();


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Products", "FAQEdit", false, 200, "افزودن پرسش در مورد محصول " + productQuestion.ProductId, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return RedirectToAction("FAQ", new { id = productQuestion.ProductId });
            }
            catch (Exception ex)
            {

                ViewBag.product = uow.ProductRepository.GetByID(productQuestion.ProductId);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "FAQEdit", false, 500, ex.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(productQuestion);
            }
        }

        public ActionResult FAQDelete(int id)
        {
            uow = new UnitOfWorkClass();
            var ProductQuestion = uow.ProductQuestionRepository.Get(x => x, x => x.Id == id, null, "Product").FirstOrDefault();
            if (ProductQuestion == null)
                return Redirect("~/Admin");


            ViewBag.product = ProductQuestion.Product;

            return View(ProductQuestion);
        }


        [HttpPost]
        public ActionResult FAQDelete(ProductQuestion productQuestion)
        {
            uow = new UnitOfWorkClass();
            try
            {
                int productid = productQuestion.ProductId;
                var ProductQuestion = uow.ProductQuestionRepository.Get(x => x, x => x.Id == productQuestion.Id, null, "Product,ChildComment").FirstOrDefault();

                #region delete Question Or Answer
                uow.ProductQuestionRepository.Delete(ProductQuestion.ChildComment.ToList());
                uow.Save();
                uow.ProductQuestionRepository.Delete(ProductQuestion);
                uow.Save();
                #endregion



                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Products", "FAQDelete", false, 200, "حذف پرسش در مورد محصول " + productQuestion.ProductId, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return RedirectToAction("FAQ", new { id = productid });
            }
            catch (Exception ex)
            {

                ViewBag.product = uow.ProductRepository.GetByID(productQuestion.ProductId);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "FAQDelete", false, 500, ex.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(productQuestion);
            }
        }


        #region Controls

        #region Category Manager
        public virtual JsonResult GetCategoryProducts(int CatId)
        {
            uow = new UnitOfWorkClass();
            var products = uow.ProductRepository.Get(x => x, x => x.IsActive && x.ProductCategories.Any(s => s.Id == CatId)).OrderBy(x => x.Title);
            return Json(new
            {
                data = products.Select(x => new { Id = x.Id, Title = x.Title, LanguageId = x.LanguageId }).ToList(),
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult GetCategory(int ProductTypeId)
        {
            uow = new UnitOfWorkClass();
            var categories = uow.ProductCategoryRepository.Get(x => x, x => x.IsActive && x.ProductTypeId == ProductTypeId && x.ParrentId == null).OrderBy(x => x.Title);
            return Json(new
            {
                data = categories.Select(x => new { Id = x.Id, Title = x.Title, LanguageId = x.LanguageId }).ToList(),
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult GetOfferCategory(int ProductTypeId, int OfferId)
        {
            uow = new UnitOfWorkClass();
            List<int> catIds = uow.OfferProductCategoryRepository.Get(x => x.CatId, x => x.OfferId == OfferId).ToList();
            if (catIds.Any())
            {
                var categories = uow.ProductCategoryRepository.Get(x => x, x => x.IsActive && x.ProductTypeId == ProductTypeId && catIds.Contains(x.Id)).OrderBy(x => x.Title);
                return Json(new
                {
                    data = categories.Select(x => new { Id = x.Id, Title = x.Title, LanguageId = x.LanguageId }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var categories = uow.ProductCategoryRepository.Get(x => x, x => x.IsActive && x.ProductTypeId == ProductTypeId).OrderBy(x => x.Title);
                return Json(new
                {
                    data = categories.Select(x => new { Id = x.Id, Title = x.Title, LanguageId = x.LanguageId }).ToList(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public virtual PartialViewResult OpenProductCategories(int? CurrentCatId, int? ProductTypeId)
        {
            uow = new UnitOfWorkClass();
            var prcats = uow.ProductCategoryRepository.Get(x => x, x => x.IsActive && x.ParrentId == null && x.ProductType.DataType == ProductTypeId.Value).OrderBy(x => x.Title);

            if (CurrentCatId.HasValue)
                ViewBag.CurrentCatId = CurrentCatId.Value;
            //else if (prcats.Any())
            //    ViewBag.CurrentCatId = prcats.First().Id;
            else
                ViewBag.CurrentCatId = null;

            return PartialView("_ProductCategories", prcats);
        }
        public virtual PartialViewResult OpenOfferProductCategories(int? CurrentCatId, List<int> catIds = null)
        {
            if (CurrentCatId.HasValue)
                ViewBag.CurrentCatId = CurrentCatId.Value;
            else
                ViewBag.CurrentCatId = null;

            uow = new UnitOfWorkClass();
            if (catIds.Any())
            {
                var prcats = uow.ProductCategoryRepository.Get(x => x, x => catIds.Contains(x.Id) && x.IsActive).OrderBy(x => x.Title);
                return PartialView("_ProductCategories", prcats);
            }
            else
            {
                var prcats = uow.ProductCategoryRepository.Get(x => x, x => x.IsActive && x.ParrentId == null).OrderBy(x => x.Title);
                return PartialView("_ProductCategories", prcats);
            }
        }
        public virtual JsonResult GetChildCategory(int? CategoryId, int? ProductTypeId)
        {
            uow = new UnitOfWorkClass();
            var categories = uow.ProductCategoryRepository.Get(x => x, x => x.IsActive && x.ParrentId == CategoryId && x.ProductTypeId == ProductTypeId.Value).OrderBy(x => x.Title);
            return Json(new
            {
                data = categories.Select(x => new { Id = x.Id, Title = x.Title, LanguageId = x.LanguageId }).ToList(),
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);
        }
        public virtual JsonResult GetParentCategory(int CategoryId)
        {
            uow = new UnitOfWorkClass();

            var cat = uow.ProductCategoryRepository.GetByID(CategoryId);
            if (cat.ParrentId.HasValue)
            {
                int? ParrentId = cat.ParrentId;
                List<ProductCategory> categories = new List<ProductCategory>();
                while (ParrentId.HasValue)
                {
                    categories.Add(uow.ProductCategoryRepository.GetByID(ParrentId));
                    ParrentId = categories.Last().ParrentId;
                }
                categories.Add(cat);
                return Json(new
                {
                    data = categories.OrderBy(x => x.ParrentId).Select(x => new { Id = x.Id, Title = x.Title, LanguageId = x.LanguageId, ParrentId = x.ParrentId }).ToList(),
                    statusCode = 201
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    data = cat.Id,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Course Manager
        [HttpPost]
        public virtual JsonResult AddCourse(int ProductId, string Title)
        {
            try
            {
                uow = new UnitOfWorkClass();
                int displaysort = 0;
                var pr = uow.ProductRepository.Get(x => x, x => x.Id == ProductId, null, "ProductCourses").FirstOrDefault();
                if (pr.ProductCourses == null)
                    pr.ProductCourses = new List<ProductCourse>();
                else if (pr.ProductCourses.Any())
                    displaysort = pr.ProductCourses.Max(x => x.DisplaySort + 1);
                pr.ProductCourses.Add(new ProductCourse { ProductId = ProductId, Title = Title, DisplaySort = displaysort });
                uow.ProductRepository.Update(pr);
                uow.Save();

                return Json(new
                {
                    Id = pr.ProductCourses.Last().Id,
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

        [HttpPost]
        public virtual JsonResult RemoveCourse(int Id)
        {
            try
            {
                uow = new UnitOfWorkClass();
                var prc = uow.ProductCourseRepository.GetByID(Id);
                uow.ProductCourseRepository.Delete(prc);
                uow.Save();

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
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual JsonResult UpdateCourse(int Id, string Title)
        {
            try
            {
                uow = new UnitOfWorkClass();
                var prc = uow.ProductCourseRepository.GetByID(Id);
                prc.Title = Title;
                uow.ProductCourseRepository.Update(prc);
                uow.Save();

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
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual JsonResult AddLesson(int CourseId, string Title, string description, double capacity, int duration, int price, string video)
        {
            try
            {
                uow = new UnitOfWorkClass();
                int displaysort = 0;
                var prc = uow.ProductCourseRepository.Get(x => x, x => x.Id == CourseId, null, "ProductCourseLessons").FirstOrDefault();
                if (prc.ProductCourseLessons == null)
                    prc.ProductCourseLessons = new List<ProductCourseLesson>();
                else if (prc.ProductCourseLessons.Any())
                    displaysort = prc.ProductCourseLessons.Max(x => x.DisplaySort + 1);
                prc.ProductCourseLessons.Add(new ProductCourseLesson { ProductCourseId = CourseId, Title = Title, Duration = duration, Description = description, Capacity = capacity, price = price, AttachementId = new Guid(video), DisplaySort = displaysort });
                uow.ProductCourseRepository.Update(prc);
                uow.Save();

                return Json(new
                {
                    Id = prc.ProductCourseLessons.Last().Id,
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

        [HttpPost]
        public virtual JsonResult RemoveCourseLesson(int Id)
        {
            try
            {
                uow = new UnitOfWorkClass();
                var prcl = uow.ProductCourseLessonRepository.GetByID(Id);
                uow.ProductCourseLessonRepository.Delete(prcl);
                uow.Save();

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
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public virtual JsonResult GetCourseLesson(int Id)
        {
            try
            {
                uow = new UnitOfWorkClass();
                var prcl = uow.ProductCourseLessonRepository.GetByID(Id);

                return Json(new
                {
                    data = new { ProductCourseId = prcl.ProductCourseId, Title = prcl.Title, Description = prcl.Description, Capacity = prcl.Capacity, Duration = prcl.Duration, price = prcl.price, video = prcl.Attachement.FileName, AttachementId = prcl.AttachementId },
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


        [HttpPost]
        public virtual JsonResult UpdateCourseLesson(int Id, int CourseId, string Title, string description, double capacity, int duration, int price, string video)
        {
            try
            {
                uow = new UnitOfWorkClass();
                var prcl = uow.ProductCourseLessonRepository.GetByID(Id);
                prcl.AttachementId = new Guid(video);
                prcl.ProductCourseId = CourseId;
                prcl.Title = Title;
                prcl.Description = description;
                prcl.Capacity = capacity;
                prcl.Duration = duration;
                prcl.price = price;
                uow.ProductCourseLessonRepository.Update(prcl);
                uow.Save();

                return Json(new
                {
                    data = new { ProductCourseId = prcl.ProductCourseId, Title = prcl.Title, Description = prcl.Description, Capacity = prcl.Capacity, Duration = prcl.Duration, price = prcl.price, video = prcl.Attachement.FileName, AttachementId = prcl.AttachementId },
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
        #endregion

        #region Rank Manager

        [HttpPost]
        public virtual JsonResult AddProductRankSelect(int ProductId, int? Id, int? GroupId)
        {
            try
            {
                uow = new UnitOfWorkClass();
                if (Id.HasValue)
                {
                    if (uow.ProductRankSelectRepository.Get(x => x, x => x.ProductId == ProductId && x.ProductRankGroupId == Id.Value).Any())
                    {
                        return Json(new
                        {
                            message = "آیتم انتخاب شده تکراری است !",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }
                    ProductRankSelect prs = new ProductRankSelect() { ProductId = ProductId, ProductRankGroupId = Id.Value };
                    uow.ProductRankSelectRepository.Insert(prs);
                    uow.Save();
                }
                else
                {
                    foreach (var item in uow.ProductRankGroupSelectRepository.Get(x => x, x => x.GroupId == GroupId.Value))
                    {
                        ProductRankSelect prs = new ProductRankSelect() { ProductId = ProductId, ProductRankGroupId = item.Id };
                        uow.ProductRankSelectRepository.Insert(prs);
                    }
                    uow.Save();
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
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public virtual JsonResult RemoveProductRankSelect(int ProductId, int Id)
        {
            try
            {
                uow = new UnitOfWorkClass();
                var prc = uow.ProductRankSelectRepository.Get(x => x, x => x.ProductRankGroupId == Id && x.ProductId == ProductId).First();
                var prcs = uow.ProductRankSelectValueRepository.Delete(prc.ProductRankSelectValues);
                uow.ProductRankSelectRepository.Delete(prcs);
                uow.Save();
                uow.ProductRankSelectRepository.Delete(prc);
                uow.Save();

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
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public virtual JsonResult ProductRankSelectValue(int ProductId, string ids, string values)
        {
            try
            {
                string[] Ids = ids.Split(',');
                string[] Values = values.Split(',');
                uow = new UnitOfWorkClass();

                for (int i = 0; i < Ids.Length; i++)
                {
                    int id = int.Parse(Ids[i]);
                    int value = int.Parse(Values[i]);
                    string userid = User.Identity.GetUserId();

                    var prs = uow.ProductRankSelectRepository.Get(x => x, x => x.ProductRankGroupId == id && x.ProductId == ProductId).First();
                    var prsvc = uow.ProductRankSelectValueRepository.Get(x => x, x => x.IsPrimary == true && x.ProductRankSelectId == prs.Id && x.UserId == userid).FirstOrDefault();
                    if (prsvc == null)
                    {
                        ProductRankSelectValue prsv = new Domain.ProductRankSelectValue() { IsPrimary = true, ProductRankSelectId = prs.Id, UserId = userid, Value = value };
                        uow.ProductRankSelectValueRepository.Insert(prsv);
                    }
                    else
                        prsvc.Value = value;
                }
                uow.Save();

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
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Tag Manager

        public virtual JsonResult AddTag(int ProductId, string ids)
        {
            try
            {
                string[] Ids = ids.Split(',');
                uow = new UnitOfWorkClass();
                var pr = uow.ProductRepository.Get(x => x, x => x.Id == ProductId, null, "Tags").FirstOrDefault();
                if (pr.Tags == null)
                    pr.Tags = new List<Tag>();

                for (int i = 0; i < Ids.Length; i++)
                {
                    int id = int.Parse(Ids[i]);
                    if (!pr.Tags.Any(x => x.Id == id))
                        pr.Tags.Add(uow.TagRepository.GetByID(id));

                }

                uow.ProductRepository.Update(pr);
                uow.Save();

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
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public virtual JsonResult RemoveTag(int ProductId, int Id)
        {
            try
            {
                uow = new UnitOfWorkClass();
                var pr = uow.ProductRepository.Get(x => x, x => x.Id == ProductId, null, "Tags").FirstOrDefault();
                var tag = uow.TagRepository.GetByID(Id);
                pr.Tags.Remove(tag);
                uow.ProductRepository.Update(pr);
                uow.Save();

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
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Link To Content
        public virtual ActionResult OpenContents(string sortOrder, string TitleString, string TitleFilter, string LanguagenameString, string LanguagenameFilter, string CatId, string CatFilter, string ContentTypeId, string ContentTypeIdFilter, int? page)
        {
            if (Request.IsAjaxRequest())
            {

                uow = new UnitOfWorkClass();
                var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var contentTypes = new SelectList(readXML.ListOfXContentType(), "Id", "Name");
                ViewBag.ContentType = contentTypes;
                ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                int contenttypeid = 0;
                if (ContentTypeId != null)
                    contenttypeid = Convert.ToInt32(ContentTypeId);
                else if (!String.IsNullOrEmpty(ContentTypeIdFilter))
                    contenttypeid = Convert.ToInt32(ContentTypeIdFilter);
                else
                    contenttypeid = Convert.ToInt32(contentTypes.First().Value);

                ViewBag.ContentTypeName = CoreLib.Infrastructure.CommonFunctions.GetContentTypeName(contenttypeid, setting.StaticContentDomain);
                ViewBag.ContentTypeId = contenttypeid;

                #region Check License


                #endregion


                var p = ModulePermission.check(User.Identity.GetUserId(), 4);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    #region Load DropDown List

                    ViewBag.Categories = uow.CategoryRepository.Get(x => x, x => x.ParentCat == null && x.IsVideo == false && x.ContentTypeId.Value == contenttypeid).OrderBy(x => x.Sort).ToList();

                    ViewBag.ContentTypes = readXML.ListOfXContentType();

                    #endregion

                    #region search
                    if (string.IsNullOrEmpty(TitleString))
                        TitleString = TitleFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(CatId))
                        CatId = CatFilter;
                    else
                        page = 1;
                    ViewBag.TitleFilter = TitleString;
                    ViewBag.LanguagenameFilter = LanguagenameString;
                    ViewBag.CatFilter = CatId;
                    ViewBag.ContentTypeIdFilter = contenttypeid.ToString();

                    var contents = uow.ContentRepository.Get(x => x, x => x.IsActive, null, "attachment,Category,User,attachment");

                    if (!String.IsNullOrEmpty(TitleString))
                        contents = contents.Where(s => s.Title.Contains(TitleString));
                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int langId = Convert.ToInt32(LanguagenameString);
                        contents = contents.Where(s => s.LanguageId == langId);
                    }
                    if (!String.IsNullOrEmpty(CatId))
                    {
                        int ctId = Convert.ToInt32(CatId);
                        if (ctId == 0)
                            contents = contents.Where(s => s.CatId == null);
                        else
                            contents = contents.Where(s => s.CatId == ctId || s.Category.ParentCat.Id == ctId);
                    }
                    if (!String.IsNullOrEmpty(contenttypeid.ToString()))
                    {
                        contents = contents.Where(s => s.ContentTypeId == contenttypeid);
                    }

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "Title":
                            contents = contents.OrderBy(s => s.Title);
                            ViewBag.CurrentSort = "Title";
                            break;
                        case "Title_desc":
                            contents = contents.OrderByDescending(s => s.Title);
                            ViewBag.CurrentSort = "Title_desc";
                            break;
                        case "CatTitle":
                            contents = contents.OrderBy(s => s.Category.Title);
                            ViewBag.CurrentSort = "CatTitle";
                            break;
                        case "CatTitle_desc":
                            contents = contents.OrderByDescending(s => s.Category.Title);
                            ViewBag.CurrentSort = "CatTitle_desc";
                            break;
                        case "ContentType":
                            contents = contents.OrderBy(s => s.ContentTypeId);
                            ViewBag.CurrentSort = "ContentType";
                            break;
                        case "ContentType_desc":
                            contents = contents.OrderByDescending(s => s.ContentTypeId);
                            ViewBag.CurrentSort = "ContentType_desc";
                            break;
                        case "Visits":
                            contents = contents.OrderBy(s => s.Visits);
                            ViewBag.CurrentSort = "Visits";
                            break;
                        case "Visits_desc":
                            contents = contents.OrderByDescending(s => s.Visits);
                            ViewBag.CurrentSort = "Visits_desc";
                            break;
                        case "InsertDate":
                            contents = contents.OrderBy(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate";
                            break;
                        case "InsertDate_desc":
                            contents = contents.OrderByDescending(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate_desc";
                            break;
                        case "Language":
                            contents = contents.OrderBy(s => s.LanguageId);
                            ViewBag.CurrentSort = "Language";
                            break;
                        case "Language_desc":
                            contents = contents.OrderByDescending(s => s.LanguageId);
                            ViewBag.CurrentSort = "Language_desc";
                            break;
                        default:  // Name ascending 
                            contents = contents.OrderByDescending(s => s.Id);
                            break;
                    }

                    #endregion

                    int pageSize = 4;
                    int pageNumber = (page ?? 1);

                    return PartialView("_ContentManager", contents.ToPagedList(pageNumber, pageSize));

                }
                else
                    return new HttpNotFoundResult();

            }
            else
                return new HttpNotFoundResult();

        }

        #endregion

        #region Link To Product
        public virtual ActionResult OpenProducts(int? ProductId, string sortOrder, string ProductCatId, string CatFilter, string NameString, string NameFilter, string IsActive, string IsActiveFilter, string BrandId, string BrandFilter, string ProductTypeId2, string ProductTypeFilter, string ProductCodeString, string ProductCodeFilter, string ProductStateId, string ProductStateFilter, string ProductInsertStateId, string ProductInsertStateFilter, string LanguagenameString, string LanguagenameFilter, string InsertDateStart, string InsertDateStartFilter, string InsertDateEnd, string InsertDateEndFilter, string SellerId, string SellerFilter, int? page)
        {
            if (Request.IsAjaxRequest())
            {

                uow = new UnitOfWorkClass();
                var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;


                var p = ModulePermission.check(User.Identity.GetUserId(), 4);
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

                    List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "True" }, new SelectListItem() { Text = "غیرفعال", Value = "False" } };
                    ViewBag.IsActive = IsActiveSelectListItem;
                    List<SelectListItem> stateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در حال بررسی", Value = "1" }, new SelectListItem() { Text = "بررسی مجدد", Value = "2" }, new SelectListItem() { Text = "عدم تایید", Value = "3" }, new SelectListItem() { Text = "تایید", Value = "4" } };
                    ViewBag.ProductInsertStateId = stateSelectListItem;

                    ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langid, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
                    ViewBag.BrandList = new SelectList(uow.BrandRepository.Get(x => x, x => x.LanguageId == langid, x => x.OrderBy(s => s.Name)), "Id", "Name");
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title");
                    ViewBag.ProductStateId = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title");
                    ViewBag.SellerId = new SelectList(uow.SellerRepository.Get(x => x).Select(x => x.User), "Id", "LastName");


                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameString = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                    #endregion

                    #region search
                    if (string.IsNullOrEmpty(ProductCatId))
                        ProductCatId = CatFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(NameString))
                        NameString = NameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(IsActive))
                        IsActive = IsActiveFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(BrandId))
                        BrandId = BrandFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ProductTypeId2))
                        ProductTypeId2 = ProductTypeFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ProductCodeString))
                        ProductCodeString = ProductCodeFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ProductStateId))
                        ProductStateId = ProductStateFilter;
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
                    if (string.IsNullOrEmpty(SellerId))
                        SellerId = SellerFilter;
                    else
                        page = 1;

                    ViewBag.CatFilter = ProductCatId;
                    ViewBag.NameFilter = NameString;
                    ViewBag.IsActiveFilter = IsActive;
                    ViewBag.BrandFilter = BrandId;
                    ViewBag.ProductTypeFilter = ProductTypeId2;
                    ViewBag.ProductCodeFilter = ProductCodeString;
                    ViewBag.ProductStateFilter = ProductStateId;
                    ViewBag.ProductInsertStateFilter = ProductInsertStateId;
                    ViewBag.LanguagenameFilter = LanguagenameString;
                    ViewBag.InsertDateEndFilter = InsertDateEnd;
                    ViewBag.InsertDateStartFilter = InsertDateStart;
                    ViewBag.SellerFilter = SellerId;

                    var Products = uow.ProductRepository.Get(x => x, null, x => x.OrderByDescending(s => s.Id), "ProductPrices,ProductPrices.ProductImages.Image");

                    if (!String.IsNullOrEmpty(NameString))
                        Products = Products.Where(s => s.Name.Contains(NameString) || s.Title.Contains(NameString));
                    if (!String.IsNullOrEmpty(LanguagenameString))
                        Products = Products.Where(s => s.LanguageId == langid);
                    if (!String.IsNullOrEmpty(ProductCatId))
                    {
                        int ctId = Convert.ToInt32(ProductCatId);
                        if (ctId > 0)
                            Products = Products.Where(s => s.ProductCategories.Any(x => x.Id == ctId));
                    }
                    if (!String.IsNullOrEmpty(IsActive))
                    {
                        bool isActive = Convert.ToBoolean(IsActive);
                        Products = Products.Where(s => s.IsActive == isActive);
                    }
                    if (!String.IsNullOrEmpty(BrandId))
                        Products = Products.Where(s => s.BrandId == int.Parse(BrandId));
                    if (!String.IsNullOrEmpty(ProductTypeId2))
                        Products = Products.Where(s => s.ProductTypeId == int.Parse(ProductTypeId2));
                    if (!String.IsNullOrEmpty(ProductCodeString))
                        Products = Products.Where(s => s.Code == ProductCodeString);
                    if (!String.IsNullOrEmpty(ProductStateId))
                        Products = Products.Where(s => s.ProductPrices.Any(a=>a.ProductStateId == int.Parse(ProductStateId)));
                    if (!String.IsNullOrEmpty(ProductInsertStateId))
                        Products = Products.Where(s => s.state == int.Parse(ProductInsertStateId));
                    if (!String.IsNullOrEmpty(SellerId))
                        Products = Products.Where(s => s.ProductSellers.Any(x => x.SellerId == int.Parse(SellerId)));

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

                    #region Sort
                    switch (sortOrder)
                    {
                        case "Name":
                            Products = Products.OrderBy(s => s.Name);
                            ViewBag.CurrentSort = "Name";
                            break;
                        case "Name_desc":
                            Products = Products.OrderByDescending(s => s.Name);
                            ViewBag.CurrentSort = "Name_desc";
                            break;
                        case "Brand":
                            Products = Products.OrderBy(s => s.Brand.Name);
                            ViewBag.CurrentSort = "Brand";
                            break;
                        case "Brand_desc":
                            Products = Products.OrderByDescending(s => s.Brand.Name);
                            ViewBag.CurrentSort = "Brand_desc";
                            break;
                        case "ProductType":
                            Products = Products.OrderBy(s => s.ProductType.Title);
                            ViewBag.CurrentSort = "ProductType";
                            break;
                        case "ProductType_desc":
                            Products = Products.OrderByDescending(s => s.ProductType.Title);
                            ViewBag.CurrentSort = "ProductType_desc";
                            break;
                        case "ProductState":
                            Products = Products.OrderBy(s => s.ProductPrices.Select(a => a.ProductState.Title));
                            ViewBag.CurrentSort = "ProductState";
                            break;
                        case "ProductState_desc":
                            Products = Products.OrderByDescending(s => s.ProductPrices.Select(a => a.ProductState.Title));
                            ViewBag.CurrentSort = "ProductState_desc";
                            break;
                        case "state":
                            Products = Products.OrderBy(s => s.state);
                            ViewBag.CurrentSort = "state";
                            break;
                        case "state_desc":
                            Products = Products.OrderByDescending(s => s.state);
                            ViewBag.CurrentSort = "state_desc";
                            break;
                        case "Visits":
                            Products = Products.OrderBy(s => s.Visits);
                            ViewBag.CurrentSort = "Visits";
                            break;
                        case "Visits_desc":
                            Products = Products.OrderByDescending(s => s.Visits);
                            ViewBag.CurrentSort = "Visits_desc";
                            break;
                        case "InsertDate":
                            Products = Products.OrderBy(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate";
                            break;
                        case "InsertDate_desc":
                            Products = Products.OrderByDescending(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate_desc";
                            break;
                        case "IsActive":
                            Products = Products.OrderBy(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive";
                            break;
                        case "IsActive_desc":
                            Products = Products.OrderByDescending(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive_desc";
                            break;
                        case "Language":
                            Products = Products.OrderBy(s => s.LanguageId);
                            ViewBag.CurrentSort = "Language";
                            break;
                        case "Language_desc":
                            Products = Products.OrderByDescending(s => s.LanguageId);
                            ViewBag.CurrentSort = "Language_desc";
                            break;
                        default:  // Name ascending 
                            Products = Products.OrderByDescending(s => s.Id);
                            break;
                    }

                    #endregion

                    int pageSize = 3;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "مدیریت محصولات", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Products", "Index", true, 200, " نمایش صفحه مدیریت محصولات", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return PartialView("_ProductManager", Products.ToPagedList(pageNumber, pageSize));

                }
                else
                    return new HttpNotFoundResult();

            }
            else
                return new HttpNotFoundResult();
        }
        #endregion

        #region ProductAttributeSelects


        [HttpPost]
        public virtual JsonResult AddProductAttributeSelects(int productid, IEnumerable<ProductAttributeSelectViewModel> AttributeSelects)
        {
            DataLayer.ahmadiDbContext db = new DataLayer.ahmadiDbContext();
            try
            {

                if (!AttributeSelects.Any())
                {
                    return Json(new
                    {
                        message = "خطایی رخ داد",
                        statusCode = 500,
                    }, JsonRequestBehavior.AllowGet);
                }


                var pr = db.Products.Include("ProductAttributeSelects").Where(x => x.Id == productid).FirstOrDefault();

                #region Add not Relational Attribute
                try
                {
                    var CurretAttributeSelects = pr.ProductAttributeSelects.ToList();

                    //if Not Any Input,Delete All Attribute
                    if (AttributeSelects == null)
                    {
                        foreach (var item in CurretAttributeSelects)
                        {
                            db.ProductAttributeSelects.Remove(item);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        foreach (var item in AttributeSelects)
                        {
                            string value = CheckValue(item.AttributeId, item.Value);
                            AttributeSelects.Where(x => x.AttributeId == item.AttributeId && x.Value == item.Value).First().Value = CommonFunctions.CorrectArabianLetterOnly(value);

                            if (pr.ProductAttributeSelects == null)
                                pr.ProductAttributeSelects = new List<ProductAttributeSelect>();

                            if (db.ProductAttributeGroupSelects.Include("ProductAttribute").Where(x => x.Id == item.GroupCatAttributeId).Single().ProductAttribute.HasMultipleValue == true)
                            {
                                if (!db.ProductAttributeSelects.Any(x => x.ProductId == productid && x.ProductAttributeCategorySelectId == item.GroupCatAttributeId && x.Value.Equals(value)))
                                {
                                    ProductAttributeSelect pas = new ProductAttributeSelect() { DisplayOrder = 0, ProductAttributeGroupSelect = db.ProductAttributeGroupSelects.Find(item.GroupCatAttributeId), ProductId = productid, Value = CommonFunctions.CorrectArabianLetterOnly(value) };
                                    pr.ProductAttributeSelects.Add(pas);
                                }
                                else
                                    pr.ProductAttributeSelects.Where(x => x.ProductAttributeCategorySelectId == item.GroupCatAttributeId && x.Value.Equals(value)).FirstOrDefault().Value = CommonFunctions.CorrectArabianLetterOnly(value);
                            }
                            else
                            {
                                if (!db.ProductAttributeSelects.Any(x => x.ProductId == productid && x.ProductAttributeCategorySelectId == item.GroupCatAttributeId))
                                {
                                    ProductAttributeSelect pas = new ProductAttributeSelect() { DisplayOrder = 0, ProductAttributeGroupSelect = db.ProductAttributeGroupSelects.Find(item.GroupCatAttributeId), ProductId = productid, Value = CommonFunctions.CorrectArabianLetterOnly(value) };
                                    pr.ProductAttributeSelects.Add(pas);
                                }
                                else
                                    pr.ProductAttributeSelects.Where(x => x.ProductAttributeCategorySelectId == item.GroupCatAttributeId).FirstOrDefault().Value = CommonFunctions.CorrectArabianLetterOnly(value);
                            }


                        }
                        db.SaveChanges();
                    }

                    #region Remove
                    List<int> ids = new List<int>();
                    foreach (var item in CurretAttributeSelects)
                    {
                        if (!AttributeSelects.Any(x => x.GroupCatAttributeId == item.ProductAttributeCategorySelectId && x.Value.Equals(item.Value)))
                            ids.Add(item.Id);

                    }

                    foreach (var item in ids)
                    {
                        db.ProductAttributeSelects.Remove(db.ProductAttributeSelects.Find(item));
                    }
                    db.SaveChanges();
                    #endregion

                }
                catch (Exception x)
                {

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "AddProductRankSelect", false, 500, x.Message, DateTime.Now, User.Identity.Name);
                    #endregion
                    return Json(new
                    {
                        statusCode = 500,
                    }, JsonRequestBehavior.AllowGet);
                }

                #endregion



                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "Products", "AddProductRankSelect", false, 200, "ثبت مشخصات محصول با ID   " + productid, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return Json(new
                {
                    statusCode = 200,
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "AddProductRankSelect", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return Json(new
                {
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
        }

        #endregion

        #endregion


        public string CheckValue(int AttributeId, string value)
        {
            uow = new UnitOfWorkClass();
            int DataType = uow.ProductAttributeRepository.GetByID(AttributeId).DataType;
            switch (DataType)
            {
                case 6:
                case 39:
                case 40:
                    return DateTimeConverter.ChangeShamsiToMiladiDateTime(value).ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                default:
                    return value;
            }
        }


        [HttpPost]
        public JsonResult AddPoint(string[] advantages, string[] Disadvantages, int ProductId)
        {
            uow = new UnitOfWorkClass();
            try
            {
                //add
                if (advantages != null)
                {
                    for (int i = 0; i < advantages.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(advantages[i]))
                        {
                            advantages[i] = CommonFunctions.CorrectArabianLetter(advantages[i]).Trim();
                            string text = advantages[i];
                            if (!uow.ProductAdvantageRepository.Get(x => x, x => x.ProductId == ProductId && x.Title == text).Any())
                            {
                                ProductAdvantage pa = new ProductAdvantage() { ProductId = ProductId, Title = text };
                                uow.ProductAdvantageRepository.Insert(pa);
                                uow.Save();
                            }
                        }
                    }
                }
                if (Disadvantages != null)
                {
                    for (int i = 0; i < Disadvantages.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(Disadvantages[i]))
                        {
                            Disadvantages[i] = CommonFunctions.CorrectArabianLetter(Disadvantages[i]).Trim();
                            string text = Disadvantages[i];
                            if (!uow.ProductDisAdvantageRepository.Get(x => x, x => x.ProductId == ProductId && x.Title == text).Any())

                            {
                                ProductDisAdvantage pda = new ProductDisAdvantage() { ProductId = ProductId, Title = text };
                                uow.ProductDisAdvantageRepository.Insert(pda);
                                uow.Save();
                            }
                        }
                    }
                }


                //Delete
                if (advantages != null)
                {
                    var Deletepa = uow.ProductAdvantageRepository.Get(x => x, x => x.ProductId == ProductId && !advantages.Contains(x.Title));
                    uow.ProductAdvantageRepository.Delete(Deletepa.ToList());
                }
                if (Disadvantages != null)
                {
                    var Deletepda = uow.ProductDisAdvantageRepository.Get(x => x, x => x.ProductId == ProductId && !Disadvantages.Contains(x.Title));
                    uow.ProductDisAdvantageRepository.Delete(Deletepda.ToList());
                }
                uow.Save();

                return Json(new
                {
                    Message = "ثبت شد.",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    Message = "خطایی رخ داد",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }


}


public class ProductAttributeSelectViewModel
{
    public ProductAttributeSelectViewModel()
    {

    }
    public ProductAttributeSelectViewModel(int groupCatAttributeId, int attributeId, string value)
    {
        GroupCatAttributeId = groupCatAttributeId;
        AttributeId = attributeId;
        Value = value;
    }
    public ProductAttributeSelectViewModel(ProductAttributeSelectViewModel model)
    {
        GroupCatAttributeId = model.GroupCatAttributeId;
        AttributeId = model.AttributeId;
        Value = model.Value;
    }
    public int GroupCatAttributeId { get; set; }
    public int AttributeId { get; set; }
    public string Value { get; set; }
}