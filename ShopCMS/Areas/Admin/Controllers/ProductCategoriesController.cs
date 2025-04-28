using CoreLib.Infrastructure.ModelBinder;
using CoreLib.ViewModel.Xml;
using Domain;
using Microsoft.AspNet.Identity;
using PagedList;
using ahmadi.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ahmadi.Infrastructure.Menu;
using System.Net;
using CoreLib.Infrastructure;
using System.Data.SqlClient;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class ProductCategoriesController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow = null;
        public ProductCategoriesController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }
        // GET: Admin/ProductCategories
        [CorrectArabianLetter(new string[] { "CatTitleString", "CatTitleFilter" })]
        public ActionResult Index(string LanguageId, string CatId, string LanguageIdFilter, string CatIdFilter, int? ProductsPage)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;


            var p = ModulePermission.check(User.Identity.GetUserId(), 4);
            if (p.Where(x => x == true).Any())
            {
                ViewBag.AddPermission = p.First();
                ViewBag.EditPermission = p.Skip(1).First();
                ViewBag.DeletePermission = p.Skip(2).First();

                if (!String.IsNullOrEmpty(CatId) || !String.IsNullOrEmpty(CatIdFilter))
                {
                    int catid = 0;

                    if (string.IsNullOrEmpty(CatId))
                    {
                        catid = int.Parse(CatIdFilter);
                        ViewBag.CatId = catid;
                    }
                    else
                    {
                        ProductsPage = 1;
                        catid = int.Parse(CatId);
                        ViewBag.CatId = catid;
                    }


                    ViewBag.Category = uow.ProductCategoryRepository.Get(x => x, x => x.Id == catid, null, "ProductCategoryAttributes,ProductAttributeGroupProductCategorys").FirstOrDefault();

                    ViewBag.AllProductAttributeGroups = uow.ProductAttributeGroupRepository.Get(x => x, x => x.Primary == false);

                    List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", catid)).ToList();

                    List<int> GroupIds = uow.ProductAttributeGroupProductCategorysRepository.Get(x => x.ProductAttributeGroupId, x => CatIds.Contains(x.ProductCategoryId) && x.ProductAttributeGroup.Primary == false).ToList();
                    ViewBag.AllProductAttributes = uow.ProductAttributeGroupSelectRepository.Get(x => x.ProductAttribute, x => (x.ProductAttribute.DataType == 8 || x.ProductAttribute.DataType == 5) && GroupIds.Contains(x.GroupId.Value), null, "ProductAttribute");

                }

                ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "گروه و زیر گروه محصول", null, "HelpModuleSections").FirstOrDefault();

                if (!String.IsNullOrEmpty(LanguageId))
                {
                    int languageId = 0;
                    if (string.IsNullOrEmpty(LanguageId))
                    {
                        languageId = int.Parse(LanguageIdFilter);
                        ViewBag.langId = languageId;
                    }
                    else
                    {
                        languageId = int.Parse(LanguageId);
                        ProductsPage = 1;
                    }

                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName", LanguageId);
                    int langId = Convert.ToInt32(LanguageId);
                    var TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langId, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");

                    return View(TreeProductCategories.ToList());
                }
                else
                {
                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                    var TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");

                    return View(TreeProductCategories.ToList());
                }

            }
            else
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گروه محصولات" }));

        }

        // GET: Admin/ProductCategories/Sort
        public virtual ActionResult Sort(int? id, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 4);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    if (id.HasValue)
                        ViewBag.ParrentCat = uow.ProductCategoryRepository.Get(x => x, x => x.Id == id.Value, null, "ParentCat").FirstOrDefault();

                    var ProductCategories = uow.ProductCategoryRepository.Get(x => x, x => x.ParrentId == id, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");

                    int pageSize = 100;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 16 && x.Name == "مرتب سازی").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductCategories", "Sort", true, 200, " نمایش صفحه مرتب سازی دسته بندی محصولات ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductCategories.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "Sort", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ProductCategories/Sort/5
        [HttpPost]
        public virtual JsonResult Sort(string ids)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                var minMenu = uow.MenuRepository.Get(x => x, x => x.IsActive && x.TypeId != 8).FirstOrDefault();
                int DispalySort = 0;
                if (minMenu != null)
                    DispalySort = minMenu.DisplaySort - 1;
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var ProductCategory = uow.ProductCategoryRepository.GetByID(id);
                    ProductCategory.Sort = i;

                    uow.Save();
                }
                for (int i = idss.Length - 1; i >= 0; i--)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var menu = uow.MenuRepository.Get(x => x, x => x.TypeId == 8 && x.LinkId == id).FirstOrDefault();
                    if (menu != null)
                    {
                        menu.DisplaySort = DispalySort;
                        DispalySort--;
                        uow.Save();
                    }
                }


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductCategories", "Sort", false, 200, "مرتب سازی دسته بندی محصولات", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/ProductCategories/Create
        public virtual ActionResult Create(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 4, 1))
                {
                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                    ViewBag.ParrentId = new SelectList(uow.ProductCategoryRepository.Get(x => x), "Id", "Title");
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title");

                    if (id != null)
                    {
                        ProductCategory category = uow.ProductCategoryRepository.Get(x => x, x => x.Id == id, null, "ParentCat").FirstOrDefault();
                        if (category != null)
                        {
                            ViewBag.ParrentId = new SelectList(uow.ProductCategoryRepository.Get(x => x), "Id", "Title", category.Id);
                            ViewBag.Root = category.Id;
                        }
                    }


                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 16 && x.Name == "ایجاد گروه و زیرگروه", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductCategories", "Create", true, 200, "نمایش صفحه ایجاد دسته بندی", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محصولات" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ProductCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CorrectArabianLetter(new string[] { "Name,Title,Abstract,Descr,PageAddress,Title2,PageAddress2,Descr2" })]
        public virtual ActionResult Create(ProductCategory category)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {
                if (ModelState.IsValid)
                {
                    category.ParentCat = uow.ProductCategoryRepository.Get(x => x, x => x.Id == category.ParrentId).FirstOrDefault();

                    category.PageAddress = CommonFunctions.NormalizeAddressWithSpace(category.PageAddress);
                    category.PageAddress2 = CommonFunctions.NormalizeAddressWithSpace(category.PageAddress2);

                    if (uow.ProductCategoryRepository.Get(x => x).Any())
                        category.Sort = uow.ProductCategoryRepository.Max(x => x.Sort) + 1;
                    uow.ProductCategoryRepository.Insert(category);
                    uow.Save();

                    //var parrent = uow.ProductCategoryRepository.GetByID(category.ParrentId);
                    //string a = MenuManager(category, true, parrent == null ? null : uow.ProductCategoryRepository.GetByID(parrent.ParrentId));
                    //if (a == "")
                    //{
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductCategories", "Create", false, 200, "ایجاد دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index", "ProductCategories");
                    //}
                    //else
                    //{
                    //    return RedirectToAction("Index", "Error", new { error = a });

                    //}
                }

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 16 && x.Name == "ایجاد گروه و زیرگروه", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "Create", false, 500, "خطا در ایجاد دسته بندیِ" + category.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                ViewBag.ParrentId = new SelectList(uow.ProductCategoryRepository.Get(x => x), "Id", "Title", category.ParrentId);
                ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", category.ProductTypeId);
                return View(category);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        /// <summary>
        /// Add Or Delete Category To OR From Menu
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="TypeAction">true is insert,false is delete</param>
        protected string MenuManager(ProductCategory category, bool TypeAction, ProductCategory parrentCategory)
        {
            uow = new UnitOfWork.UnitOfWorkClass();

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            string result = "";
            if (category.ParrentId == null || parrentCategory == null)// Only one and second level
            {
                if (TypeAction)
                {
                    if (!uow.MenuRepository.Get(x => x, x => x.TypeId == 8 && x.LinkId == category.Id).Any())
                    {
                        var cat = uow.ProductCategoryRepository.Get(x => x, x => x.Id == category.Id, null, "attachment,ParentCat").FirstOrDefault();
                        Menu oMenu = new Menu();
                        oMenu.DisplaySort = 0;
                        oMenu.IsActive = true;
                        oMenu.LanguageId = 1;
                        oMenu.LinkId = category.Id;
                        oMenu.PlaceShow = 1;
                        oMenu.Title = category.Title;
                        oMenu.TypeId = 8;
                        oMenu.Cover = cat.MenuCover;
                        if (cat.ParentCat == null)
                            oMenu.ParrentMenu = null;
                        else
                        {
                            var ParentMenu = uow.MenuRepository.Get(x => x, x => x.TypeId == 8 && x.LinkId == cat.ParentCat.Id).FirstOrDefault();
                            if (ParentMenu != null)
                                oMenu.ParrentMenu = ParentMenu;
                            else
                                oMenu.ParrentMenu = null;
                        }
                        uow.MenuRepository.Insert(oMenu);
                        uow.Save();
                        //Add To Xml
                        XMLReader readxml = new XMLReader(setting.StaticContentDomain);
                        result = readxml.CreateOfXMenu(new XMenu()
                        {
                            Cover = oMenu.Cover != null ? oMenu.attachment.FileName : null,
                            DisplayOrder = oMenu.DisplaySort,
                            Link = LinkManager.GenerateLink(oMenu.Id, oMenu.TypeId, oMenu.LinkId, oMenu.LinkUniqIdentifier, oMenu.Title, oMenu.OffLink, oMenu.ParrentMenu, setting.StaticContentDomain),
                            PlaceShow = oMenu.PlaceShow,
                            Title = oMenu.Title,
                            Id = oMenu.Id,
                            MenuId = oMenu.MenuID,
                            LinkId = oMenu.LinkId,
                            TypeId = oMenu.TypeId
                        });
                    }
                    else
                    {
                        var menu = uow.MenuRepository.Get(x => x, x => x.TypeId == 8 && x.LinkId == category.Id).First();
                        var cat = uow.ProductCategoryRepository.Get(x => x, x => x.Id == category.Id, null, "attachment,ParentCat").FirstOrDefault();
                        menu.Title = cat.Title;
                        menu.IsActive = cat.IsActive;
                        menu.Cover = cat.MenuCover;
                        if (cat.ParentCat != null)
                        {
                            var ParentMenu = uow.MenuRepository.Get(x => x, x => x.TypeId == 8 && x.LinkId == cat.ParentCat.Id).FirstOrDefault();
                            if (ParentMenu != null)
                                menu.ParrentMenu = ParentMenu;
                            else
                                menu.ParrentMenu = null;
                        }
                        else
                            menu.ParrentMenu = null;
                        uow.Save();

                        XMLReader readxml = new XMLReader(setting.StaticContentDomain);
                        if (readxml.DetailOfXMenu(menu.Id) != null)
                        {
                            //Edit In Xml
                            result = readxml.EditXMenu(new XMenu()
                            {
                                Cover = menu.Cover != null ? menu.attachment.FileName : null,
                                DisplayOrder = menu.DisplaySort,
                                Link = LinkManager.GenerateLink(menu.Id, menu.TypeId, menu.LinkId, menu.LinkUniqIdentifier, menu.Title, menu.OffLink, menu.ParrentMenu, setting.StaticContentDomain),
                                PlaceShow = menu.PlaceShow,
                                Title = menu.Title,
                                Id = menu.Id,
                                MenuId = menu.MenuID,
                                LinkId = menu.LinkId,
                                TypeId = menu.TypeId
                            });

                        }
                        else
                        {
                            //Add To Xml
                            result = readxml.CreateOfXMenu(new XMenu()
                            {
                                Cover = menu.Cover != null ? menu.attachment.FileName : null,
                                DisplayOrder = menu.DisplaySort,
                                Link = LinkManager.GenerateLink(menu.Id, menu.TypeId, menu.LinkId, menu.LinkUniqIdentifier, menu.Title, menu.OffLink, menu.ParrentMenu, setting.StaticContentDomain),
                                PlaceShow = menu.PlaceShow,
                                Title = menu.Title,
                                Id = menu.Id,
                                MenuId = menu.MenuID,
                                LinkId = menu.LinkId,
                                TypeId = menu.TypeId
                            });
                        }
                    }
                }
                else
                {
                    var ParentMenu = uow.MenuRepository.Get(x => x, x => x.TypeId == 8 && x.LinkId == category.Id, null, "ChildMenu").FirstOrDefault();
                    if (ParentMenu != null)
                    {
                        List<Menu> AllCurrentMenus = new List<Menu>();
                        foreach (var item in ParentMenu.ChildMenu)
                        {
                            AllCurrentMenus.Add(item);
                            foreach (var item2 in item.ChildMenu)
                            {
                                AllCurrentMenus.Add(item2);
                                foreach (var item3 in item2.ChildMenu)
                                {
                                    AllCurrentMenus.Add(item3);
                                    foreach (var item4 in item3.ChildMenu)
                                    {
                                        AllCurrentMenus.Add(item4);

                                    }
                                }
                            }
                        }
                        foreach (var item in AllCurrentMenus.OrderByDescending(x => x.Id))
                        {
                            uow.MenuRepository.Delete(item);
                        }
                        uow.Save();
                        uow.MenuRepository.Delete(ParentMenu);
                        uow.Save();


                        XMLReader readxml = new XMLReader(setting.StaticContentDomain);
                        result = readxml.RemoveXMenus(ParentMenu.Id);
                    }

                }
                Session["submenu"] = null;

            }
            else
            {
                if (!uow.MenuRepository.Get(x => x, x => x.TypeId == 8 && x.LinkId == category.Id).Any())
                {
                    var cat = uow.ProductCategoryRepository.Get(x => x, x => x.Id == category.Id, null, "attachment,ParentCat").FirstOrDefault();
                    Menu oMenu = new Menu();
                    oMenu.DisplaySort = 0;
                    oMenu.IsActive = true;
                    oMenu.LanguageId = 1;
                    oMenu.LinkId = category.Id;
                    oMenu.PlaceShow = 1;
                    oMenu.Title = category.Title;
                    oMenu.TypeId = 8;
                    oMenu.Cover = cat.MenuCover;
                    if (cat.ParentCat == null)
                        oMenu.ParrentMenu = null;
                    else
                    {
                        var ParentMenu = uow.MenuRepository.Get(x => x, x => x.TypeId == 8 && x.LinkId == cat.ParentCat.Id).FirstOrDefault();
                        if (ParentMenu != null)
                            oMenu.ParrentMenu = ParentMenu;
                        else
                            oMenu.ParrentMenu = null;
                    }
                    uow.MenuRepository.Insert(oMenu);
                    uow.Save();
                    //Add To Xml
                    XMLReader readxml = new XMLReader(setting.StaticContentDomain);
                    result = readxml.CreateOfXMenu(new XMenu()
                    {
                        Cover = oMenu.Cover != null ? oMenu.attachment.FileName : null,
                        DisplayOrder = oMenu.DisplaySort,
                        Link = LinkManager.GenerateLink(oMenu.Id, oMenu.TypeId, oMenu.LinkId, oMenu.LinkUniqIdentifier, oMenu.Title, oMenu.OffLink, oMenu.ParrentMenu, setting.StaticContentDomain),
                        PlaceShow = oMenu.PlaceShow,
                        Title = oMenu.Title,
                        Id = oMenu.Id,
                        MenuId = oMenu.MenuID,
                        LinkId = oMenu.LinkId,
                        TypeId = oMenu.TypeId
                    });
                }
                else
                {
                    var menu = uow.MenuRepository.Get(x => x, x => x.TypeId == 8 && x.LinkId == category.Id).First();
                    var cat = uow.ProductCategoryRepository.Get(x => x, x => x.Id == category.Id, null, "attachment,ParentCat").FirstOrDefault();
                    menu.Title = cat.Title;
                    menu.IsActive = cat.IsActive;
                    menu.Cover = cat.MenuCover;
                    if (cat.ParentCat != null)
                    {
                        var ParentMenu = uow.MenuRepository.Get(x => x, x => x.TypeId == 8 && x.LinkId == cat.ParentCat.Id).FirstOrDefault();
                        if (ParentMenu != null)
                            menu.ParrentMenu = ParentMenu;
                        else
                            menu.ParrentMenu = null;
                    }
                    else
                        menu.ParrentMenu = null;
                    uow.Save();


                    XMLReader readxml = new XMLReader(setting.StaticContentDomain);
                    if (readxml.DetailOfXMenu(menu.Id) != null)
                    {
                        //Edit In Xml
                        result = readxml.EditXMenu(new XMenu()
                        {
                            Cover = menu.Cover != null ? menu.attachment.FileName : null,
                            DisplayOrder = menu.DisplaySort,
                            Link = LinkManager.GenerateLink(menu.Id, menu.TypeId, menu.LinkId, menu.LinkUniqIdentifier, menu.Title, menu.OffLink, menu.ParrentMenu, setting.StaticContentDomain),
                            PlaceShow = menu.PlaceShow,
                            Title = menu.Title,
                            Id = menu.Id,
                            MenuId = menu.MenuID,
                            LinkId = menu.LinkId,
                            TypeId = menu.TypeId
                        });

                    }
                    else
                    {
                        //Add To Xml
                        result = readxml.CreateOfXMenu(new XMenu()
                        {
                            Cover = menu.Cover != null ? menu.attachment.FileName : null,
                            DisplayOrder = menu.DisplaySort,
                            Link = LinkManager.GenerateLink(menu.Id, menu.TypeId, menu.LinkId, menu.LinkUniqIdentifier, menu.Title, menu.OffLink, menu.ParrentMenu, setting.StaticContentDomain),
                            PlaceShow = menu.PlaceShow,
                            Title = menu.Title,
                            Id = menu.Id,
                            MenuId = menu.MenuID,
                            LinkId = menu.LinkId,
                            TypeId = menu.TypeId
                        });
                    }
                }
            }
            return result;
        }

        // GET: Admin/ProductCategories/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 4, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    ProductCategory category = uow.ProductCategoryRepository.Get(x => x, x => x.Id == id, null, "ParentCat,attachment,attachmentHomePage,Menuattachment").FirstOrDefault();
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                    ViewBag.ParrentId = new SelectList(uow.ProductCategoryRepository.Get(x => x), "Id", "Title", category.ParrentId);
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", category.ProductTypeId);



                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductCategories", "Edit", true, 200, "نمایش صفحه ویرایش دسته بندیِ   " + category.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 16 && x.Name == "ایجاد گروه و زیرگروه", null, "HelpModuleSectionFields").FirstOrDefault();
                    return View(category);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ProductCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [CorrectArabianLetter(new string[] { "Name,Title,Abstract,Descr,PageAddress,Title2,PageAddress2,Descr2" })]
        public virtual ActionResult Edit(ProductCategory category)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {
                if (ModelState.IsValid)
                {


                    category.PageAddress = CommonFunctions.NormalizeAddressWithSpace(category.PageAddress);
                    category.PageAddress2 = CommonFunctions.NormalizeAddressWithSpace(category.PageAddress2);

                    uow.ProductCategoryRepository.Update(category);
                    uow.Save();


                    var parrent = uow.ProductCategoryRepository.GetByID(category.ParrentId);
                    //MenuManager(category, true, parrent == null ? null : uow.ProductCategoryRepository.GetByID(parrent.ParrentId));


                    //Active/DeActive Content Of Categories
                    if (category.IsActive)
                    {
                        if (uow.ProductRepository.Get(x => x, x => x.ProductCategories.Any(s => s.Id == category.Id)).Any())
                        {
                            var cat = uow.ProductCategoryRepository.Get(x => x, x => x.Id == category.Id, null, "Products").FirstOrDefault();
                            foreach (var item in cat.Products)
                                item.IsActive = true;
                            uow.Save();

                        }
                    }
                    else
                    {
                        var cat = uow.ProductCategoryRepository.Get(x => x, x => x.Id == category.Id, null, "Products").FirstOrDefault();
                        if (cat.Products.Any())
                        {
                            foreach (var item in cat.Products)
                                item.IsActive = false;
                            uow.Save();
                        }
                    }



                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductCategories", "Edit", false, 200, "ویرایش دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index", "ProductCategories");
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "Edit", false, 500, "خطا در ویرایش دسته بندیِ" + category.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName", category.LanguageId);
                ViewBag.ParrentId = new SelectList(uow.ProductCategoryRepository.Get(x => x), "Id", "Title", category.ParrentId);
                ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", category.ProductTypeId);

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 16 && x.Name == "ایجاد گروه و زیرگروه", null, "HelpModuleSectionFields").FirstOrDefault();
                return View(category);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/ProductCategories/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 4, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    ProductCategory category = uow.ProductCategoryRepository.GetByID(id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductCategories", "Delete", true, 200, "نمایش صفحه حذف دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(category);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                ProductCategory category = uow.ProductCategoryRepository.Get(x=>x,x=>x.Id==id,null, "Products").Single();
                //if (category.ChildCategory.Any())
                //{
                //    ViewBag.Erorr = "این گروه، دارای" + category.ChildCategory.Count + " زیرگروه است. ابتدا باید آنها را پاک نمایید.";
                //    return View(category);
                //}
                if (category.Products.Any())
                {
                    ViewBag.Erorr = "این گروه، دارای" + category.Products.Count + " محصول است. ابتدا باید آنها را پاک نمایید.";
                    return View(category);
                }
                List<int> subCatIds = ahmadi.Infrastructure.DbSql.SqlManager.GetAllSubCat(category.Id);
              

                var AllCurrentCategories = uow.ProductCategoryRepository.Get(x => x, x => subCatIds.Contains(x.Id)).ToList();
                uow.ProductCategoryRepository.Delete(AllCurrentCategories);
                uow.Save();

                //MenuManager(category, false, parrent == null ? null : uow.ProductCategoryRepository.GetByID(parrent.ParrentId));

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductCategories", "DeleteConfirmed", false, 200, "حذف دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "ProductCategories");

            }
            catch (Exception x)
            {
                ProductCategory category = uow.ProductCategoryRepository.GetByID(id);
                ViewBag.Erorr = "این دسته بندی ، در حال استفاده می باشد و نمیتوانید آن را حذف کنید.";
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(category);
            }
        }

        [HttpPost]
        public virtual JsonResult UpdateCommision(int CatId, int value)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var category = uow.ProductCategoryRepository.GetByID(CatId);
            try
            {

                category.Commission = value;
                uow.ProductCategoryRepository.Update(category);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductCategories", "UpdateCommision", false, 200, "بروزرسانی کمیسیون گروه " + category.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "UpdateCommision", false, 500, "خطا در بروزرسانی کمیسیون گروه " + category.Name + " " + x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        // POST: Admin/ProductCategories/AddAttribute
        [HttpPost]
        public virtual JsonResult AddAttribute(int[] Attid, string[] chkAttId, int CatId)
        {
            try
            {
                if (Attid == null)
                {
                    //remove all
                    var ProductCategoryAttribute = uow.ProductCategoryAttributeRepository.Get(x => x, x => x.ProductCategoryId == CatId);
                    uow.ProductCategoryAttributeRepository.Delete(ProductCategoryAttribute.ToList());
                    uow.Save();
                }
                else
                {
                    //remove unselects
                    var ProductCategoryAttribute = uow.ProductCategoryAttributeRepository.Get(x => x, x => x.ProductCategoryId == CatId && !Attid.Contains(x.ProductAttributeId));
                    uow.ProductCategoryAttributeRepository.Delete(ProductCategoryAttribute.ToList());

                    //add selects
                    var ProductCategory = uow.ProductCategoryRepository.Get(x => x, x => x.Id == CatId, null, "ProductCategoryAttributes").First();
                    foreach (var item in Attid)
                    {
                        if (ProductCategory.ProductCategoryAttributes != null)
                        {
                            if (!ProductCategory.ProductCategoryAttributes.Any(s => s.ProductAttributeId == item))
                            {
                                ProductCategory.ProductCategoryAttributes.Add(new Domain.ProductCategoryAttribute()
                                {
                                    IsSearchAbe = true,
                                    ProductAttributeId = item,
                                    ProductCategoryId = CatId
                                });
                            }
                        }
                        else
                        {
                            ProductCategory.ProductCategoryAttributes = new List<ProductCategoryAttribute>();
                            ProductCategory.ProductCategoryAttributes.Add(new Domain.ProductCategoryAttribute()
                            {
                                IsSearchAbe = true,
                                ProductAttributeId = item,
                                ProductCategoryId = CatId
                            });
                        }
                    }
                    uow.ProductCategoryRepository.Update(ProductCategory);
                    uow.Save();
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductCategories", "AddAttribute", false, 200, "تغییر خصوصیات قابل سرچ گروه محصولات", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    Message = "تغییرات ثبت شد",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "AddAttribute", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Admin/ProductCategories/AddAttribute
        [HttpPost]
        public virtual JsonResult AddGroup(int[] grpId, string[] chkGroupId, int CatId)
        {
            try
            {
                if (grpId == null)
                {
                    //remove all
                    var ProductAttributeGroupProductCategorys = uow.ProductAttributeGroupProductCategorysRepository.Get(x => x, x => x.ProductCategoryId == CatId);
                    uow.ProductAttributeGroupProductCategorysRepository.Delete(ProductAttributeGroupProductCategorys.ToList());
                    uow.Save();
                }
                else
                {
                    //remove unselects
                    var ProductAttributeGroupProductCategorys = uow.ProductAttributeGroupProductCategorysRepository.Get(x => x, x => x.ProductCategoryId == CatId && !grpId.Contains(x.ProductAttributeGroupId));
                    uow.ProductAttributeGroupProductCategorysRepository.Delete(ProductAttributeGroupProductCategorys.ToList());

                    //add selects
                    var ProductCategory = uow.ProductCategoryRepository.Get(x => x, x => x.Id == CatId, null, "ProductAttributeGroupProductCategorys").First();
                    foreach (var item in grpId)
                    {
                        if (ProductCategory.ProductAttributeGroupProductCategorys != null)
                        {
                            if (!ProductCategory.ProductAttributeGroupProductCategorys.Any(s => s.ProductAttributeGroupId == item))
                            {
                                ProductCategory.ProductAttributeGroupProductCategorys.Add(new Domain.ProductAttributeGroupProductCategory()
                                {
                                    ProductAttributeGroupId = item,
                                    ProductCategoryId = CatId
                                });
                            }
                        }
                        else
                        {
                            ProductCategory.ProductAttributeGroupProductCategorys = new List<ProductAttributeGroupProductCategory>();
                            ProductCategory.ProductAttributeGroupProductCategorys.Add(new Domain.ProductAttributeGroupProductCategory()
                            {
                                ProductAttributeGroupId = item,
                                ProductCategoryId = CatId
                            });
                        }
                    }
                    uow.ProductCategoryRepository.Update(ProductCategory);
                    uow.Save();
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductCategories", "AddGroup", false, 200, "تغییر گروه خصوصیاتِ گروه محصولات", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    Message = "تغییرات ثبت شد",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductCategories", "AddGroup", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}