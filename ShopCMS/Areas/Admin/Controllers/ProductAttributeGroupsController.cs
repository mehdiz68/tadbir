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
using ahmadi.Infrastructure.Helper;
using Domain.ViewModels;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class ProductAttributeGroupsController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;

        // GET: Admin/ProductAttributeGroups
        public virtual ActionResult Index()
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

                    List<ProductAttributeGroup> ProductAttributeGroups = uow.ProductAttributeGroupRepository.Get(x => x).ToList();


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributeGroups", "Index", true, 200, " نمایش صفحه مدیریت گروه خصوصیات", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductAttributeGroups.OrderBy(x => x.DisplayOrder));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گروه خصوصیات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeGroups", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public virtual JsonResult Sort(string ids)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (Int16 i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var social = uow.ProductAttributeGroupRepository.GetByID(id);
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

        public virtual ActionResult Create(int? Id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {

                    return View();

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گروه خصوصیت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeGroups", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create([Bind(Include = "Id,Title,DisplayOrder,LanguageId")] ProductAttributeGroup ProductAttributeGroup)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    ProductAttributeGroup.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductAttributeGroup.Title);
                    uow.ProductAttributeGroupRepository.Insert(ProductAttributeGroup);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductAttributeGroups", "Create", false, 200, "   ایجاد گروه خصوصیت " + ProductAttributeGroup.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeGroups", "Create", false, 500, "   خطا در ایجاد گروه خصوصیت " + ProductAttributeGroup.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductAttributeGroup);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "CreateItem", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductAttributeGroup);
            }

        }

        public virtual ActionResult Edit(int? Id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {
                    var ProductAttributeGroup = uow.ProductAttributeGroupRepository.GetByID(Id.Value);

                    if (ProductAttributeGroup != null)
                    {
                        if (ProductAttributeGroup.Primary)
                            return Redirect("~/Admin/ProductAttributeGroups");
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributeGroups", "Edit", true, 200, " نمایش صفحه ویرایش گروه خصوصیت", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(ProductAttributeGroup);
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گروه خصوصیت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeGroups", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit([Bind(Include = "Id,Title,DisplayOrder,LanguageId")] ProductAttributeGroup ProductAttributeGroup)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    ProductAttributeGroup.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(ProductAttributeGroup.Title);
                    uow.ProductAttributeGroupRepository.Update(ProductAttributeGroup);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductAttributes", "Edit", false, 200, "   ویرایش گروه خصوصیت" + ProductAttributeGroup.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeGroups", "Edit", false, 500, "   خطا در ویرایش گروه خصوصیت" + ProductAttributeGroup.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ProductAttributeGroup);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeGroups", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                ViewBag.Error = "خطایی رخ داد";
                return View(ProductAttributeGroup);
            }

        }


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
                    ProductAttributeGroup ProductAttributeGroup = uow.ProductAttributeGroupRepository.Get(x => x, x => x.Id == id).FirstOrDefault();
                    if (ProductAttributeGroup == null)
                    {
                        return HttpNotFound();
                    }
                    else if (ProductAttributeGroup.Primary)
                        return Redirect("~/Admin/ProductAttributeGroups");
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributeGroups", "Delete", true, 200, " نمایش صفحه حذف مقدار خصوصیت " + ProductAttributeGroup.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductAttributeGroup);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گروه خصوصیت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeGroups", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                ProductAttributeGroup ProductAttributeGroup = uow.ProductAttributeGroupRepository.GetByID(id);
                uow.ProductAttributeGroupRepository.Delete(ProductAttributeGroup);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductAttributes", "Delete", false, 200, "   حذف گروه خصوصیت" + ProductAttributeGroup.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributes", "DeleteItem", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        #region ProductAttributeGroupSelect
        // GET: Admin/ProductCategories
        [CorrectArabianLetter(new string[] { "AttributeName,CatTitleFilter,CatTitleString" })]
        public virtual ActionResult Select(string LanguageId)
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

                    var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                    List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "True" }, new SelectListItem() { Text = "غیرفعال", Value = "False" } };
                    ViewBag.IsActive = IsActiveSelectListItem;

                    ViewBag.ContentTypeId = new SelectList(readXML.ListOfXContentType(), "Id", "Name");

                    ViewBag.ProductAttributes = uow.ProductAttributeSelectRepository.Get(x => x, null, null, "ProductAttributeGroupSelect.ProductAttribute,ProductAttributeGroupSelect.ProductAttributeGroup,ProductAttributeGroupSelect.ProductAttributeTab");


                    #region Tree

                    var ContetTypes = readXML.ListOfXContentType();
                    ViewBag.ContentTypeId = new SelectList(ContetTypes, "Id", "Name");

                    if (!String.IsNullOrEmpty(LanguageId))
                    {
                        ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName", LanguageId);
                        int langId = Convert.ToInt32(LanguageId);
                        var TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langId, x => x.OrderBy(o => o.Sort), "attachment,ParentCat");

                        ViewBag.TreeProductCategories = TreeProductCategories.ToList();
                    }
                    else
                    {
                        ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                        var TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null, x => x.OrderBy(o => o.Sort), "attachment,ParentCat");

                        ViewBag.TreeProductCategories = TreeProductCategories.ToList();
                    }
                    #endregion

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "گروه و زیر گروه محصول", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductAttributeGroups", "Select", true, 200, " نمایش صفحه مدیریت انتخاب گروه خصوصیت", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger 
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductAttributeGroups", "Select", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public virtual JsonResult GetCatAttribute()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string HTMLString = "";

                ViewBag.ProductAttributeGroups = new SelectList(uow.ProductAttributeGroupRepository.Get(x => x, x => !x.Primary), "Id", "Title");
                ViewBag.ProductAttributeTabs = new SelectList(uow.ProductAttributeTabRepository.Get(x => x), "Id", "Name");
                CatAttribute CatAttribute = new CatAttribute();
                var a = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => !x.ProductAttributeGroup.Primary && x.ProductAttribute.DataType < 12, null, "ProductAttributeGroup,ProductAttribute");
                var b = a.OrderBy(x => x.DisplayGroupOrder);
                var c = b.Select(x => x.ProductAttributeGroup).Distinct();
                CatAttribute.MainCatGroupAttributes = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => !x.ProductAttributeGroup.Primary && x.ProductAttribute.DataType < 12, null, "ProductAttributeGroup,ProductAttribute").OrderBy(x => x.DisplayGroupOrder).GroupBy(x => x.GroupId).Select(x => x.First()).ToList();



                HTMLString = CaptureHelper.RenderViewToString("_CatAttribute", CatAttribute, this.ControllerContext);


                var jsonResult = Json(new
                {
                    data = HTMLString,
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;



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
        [HttpPost]
        public virtual JsonResult GetGroups()
        {
            uow = new UnitOfWork.UnitOfWorkClass();

            var CatTabs = uow.ProductAttributeTabRepository.Get(x => x);
            var CatGroups = uow.ProductAttributeGroupRepository.Get(x => x, x => !x.Primary);

            return Json(new
            {
                GroupIds = CatGroups.Select(x => x.Id).Distinct().ToList(),
                TabIds = CatTabs.Select(x => x.Id).Distinct().ToList()
            }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        [CorrectArabianLetter(new string[] { "AttributeName" })]
        public virtual JsonResult SearchAttribute(string AttributeName)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                //Get Parrent Id
                if (AttributeName.Length > 1)
                {
                    if (AttributeName.Length == 2)
                    {
                        var Attributes = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.ProductAttribute.Name == AttributeName && x.ProductAttribute.DataType < 12, x => x.OrderBy(s => s.DisplayOrder), "ProductAttribute,ProductAttributeGroup").Select(x => new { x.TabId, x.Id, x.AttributeId, Name = x.ProductAttribute.Name, x.GroupId, GroupName = x.ProductAttributeGroup != null ? x.ProductAttributeGroup.Title : "بدون دسته" });
                        var jsonResult = Json(new
                        {
                            Attributes = Attributes,
                            groups = Attributes.Select(x => new { x.GroupId, x.GroupName }).Distinct(),
                            statusCode = 400
                        }, JsonRequestBehavior.AllowGet);
                        jsonResult.MaxJsonLength = int.MaxValue;
                        return jsonResult;
                    }
                    else
                    {
                        var Attributes = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.ProductAttribute.Name.Contains(AttributeName) && x.ProductAttribute.DataType < 12, x => x.OrderBy(s => s.DisplayOrder), "ProductAttribute,ProductAttributeGroup").Select(x => new { x.TabId, x.Id, x.AttributeId, Name = x.ProductAttribute.Name, x.GroupId, GroupName = x.ProductAttributeGroup != null ? x.ProductAttributeGroup.Title : "بدون دسته" });

                        var jsonResult = Json(new
                        {
                            Attributes = Attributes,
                            groups = Attributes.Select(x => new { x.GroupId, x.GroupName }).Distinct(),
                            statusCode = 400
                        }, JsonRequestBehavior.AllowGet);
                        jsonResult.MaxJsonLength = int.MaxValue;
                        return jsonResult;
                    }

                }
                else
                {
                    return Json(new
                    {
                        Message = "حداقل 2 کارکتر وارد نمایید !",
                        statusCode = 500,
                    }, JsonRequestBehavior.AllowGet);
                }

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


        [HttpPost]
        [CorrectArabianLetter(new string[] { "GroupName" })]
        public virtual JsonResult SearchGroupAttribute(string GroupName)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                //Get Parrent Id
                //AttributeName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(AttributeName);
                if (GroupName.Length > 1)
                {
                    if (GroupName.Length == 2)
                    {
                        var Attributes = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.ProductAttributeGroup.Title == GroupName && x.ProductAttribute.DataType < 12, x => x.OrderBy(s => s.DisplayOrder), "ProductAttribute,ProductAttributeGroup").Select(x => new { x.TabId, x.Id, x.AttributeId, Name = x.ProductAttribute.Name, x.GroupId, GroupName = x.ProductAttributeGroup != null ? x.ProductAttributeGroup.Title : "بدون دسته" });
                        var jsonResult = Json(new
                        {
                            Attributes = Attributes,
                            groups = Attributes.Select(x => new { x.GroupId, x.GroupName }).Distinct(),
                            statusCode = 400
                        }, JsonRequestBehavior.AllowGet);
                        jsonResult.MaxJsonLength = int.MaxValue;
                        return jsonResult;
                    }
                    else
                    {
                        var Attributes = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.ProductAttributeGroup.Title.Contains(GroupName) && x.ProductAttribute.DataType < 12, x => x.OrderBy(s => s.DisplayOrder), "ProductAttribute,ProductAttributeGroup").Select(x => new { x.TabId, x.Id, x.AttributeId, Name = x.ProductAttribute.Name, x.GroupId, GroupName = x.ProductAttributeGroup != null ? x.ProductAttributeGroup.Title : "بدون دسته" });

                        var jsonResult = Json(new
                        {
                            Attributes = Attributes,
                            groups = Attributes.Select(x => new { x.GroupId, x.GroupName }).Distinct(),
                            statusCode = 400
                        }, JsonRequestBehavior.AllowGet);
                        jsonResult.MaxJsonLength = int.MaxValue;
                        return jsonResult;
                    }

                }
                else
                {
                    return Json(new
                    {
                        Message = "حداقل 2 کارکتر وارد نمایید !",
                        statusCode = 500,
                    }, JsonRequestBehavior.AllowGet);
                }

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

        public virtual JsonResult GetGroupAttribute(int? GroupId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                //Get Parrent Id
                var Attributes = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.GroupId == GroupId && x.ProductAttribute.DataType < 12, x => x.OrderBy(s => s.DisplayOrder), "ProductAttribute").Select(x => new { x.TabId, x.Id, x.AttributeId, Name = x.ProductAttribute.Name });
                var jsonResult = Json(new
                {
                    data = Attributes,
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
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

        [HttpPost]
        [CorrectArabianLetter(new string[] { "GroupText" })]
        public virtual JsonResult AddAttribute(AttributeGroup[] attributes, int? GroupId, int? TabId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            int AllInput = 0, CorrectInput = 0, DuplicateInput = 0;
            try
            {

                var ProductAttributeCategorySelects = uow.ProductAttributeGroupSelectRepository.Get(x => x, null, null, "ProductAttribute,ProductAttributeTab,ProductAttributeGroup");
                foreach (var item in attributes)
                {
                    //Add Group
                    if (!item.AttributeId.HasValue)
                    {
                        //Add بدون دسته
                        int? FinalGroupId = item.GroupId;
                        if (item.GroupId == 0)
                        {
                            var SelectedAttributes = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.GroupId == null);
                            if (GroupId.HasValue)
                                FinalGroupId = GroupId.Value;
                            else
                                FinalGroupId = null;

                            foreach (var attribute in SelectedAttributes)
                            {
                                #region Add Attribute If Not Exist In Selecte Category And Group
                                if (!ProductAttributeCategorySelects.Any(x => x.AttributeId == attribute.AttributeId && x.GroupId == FinalGroupId))
                                {
                                    int MaxDisplayOrder = 0, DisplayGroup = 0;
                                    var ItemsOfCategory = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.GroupId == FinalGroupId).OrderByDescending(x => x.Id);
                                    if (ItemsOfCategory.Any())
                                    {
                                        MaxDisplayOrder = ItemsOfCategory.FirstOrDefault().DisplayOrder + 1;
                                        DisplayGroup = ItemsOfCategory.FirstOrDefault().DisplayGroupOrder;
                                    }
                                    else if (uow.ProductAttributeGroupSelectRepository.Get(x => x).Any())
                                        DisplayGroup = uow.ProductAttributeGroupSelectRepository.Get(x => x, null, x => x.OrderByDescending(s => s.Id)).Take(1).FirstOrDefault().DisplayGroupOrder + 1;

                                    ProductAttributeGroupSelect pacs = new ProductAttributeGroupSelect()
                                    {
                                        AttributeId = attribute.AttributeId,
                                        GroupId = FinalGroupId,
                                        TabId = TabId.HasValue ? TabId.Value : attribute.TabId,
                                        DisplayGroupOrder = DisplayGroup,
                                        DisplayOrder = MaxDisplayOrder
                                    };
                                    uow.ProductAttributeGroupSelectRepository.Insert(pacs);
                                    CorrectInput++;
                                }
                                else
                                    DuplicateInput++;
                                AllInput++;
                                #endregion
                            }
                        }
                        else
                        {
                            var SelectedAttributes = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.GroupId == item.GroupId);
                            if (GroupId.HasValue)
                                FinalGroupId = GroupId.Value;

                            foreach (var attribute in SelectedAttributes)
                            {
                                #region Add Attribute If Not Exist In Selecte Category And Group
                                if (!ProductAttributeCategorySelects.Any(x => x.AttributeId == attribute.AttributeId && x.GroupId == FinalGroupId))
                                {
                                    int MaxDisplayOrder = 0, DisplayGroup = 0;
                                    var ItemsOfCategory = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.GroupId == FinalGroupId).OrderByDescending(x => x.Id);
                                    if (ItemsOfCategory.Any())
                                    {
                                        MaxDisplayOrder = ItemsOfCategory.FirstOrDefault().DisplayOrder + 1;
                                        DisplayGroup = ItemsOfCategory.FirstOrDefault().DisplayGroupOrder;
                                    }
                                    else if (uow.ProductAttributeGroupSelectRepository.Get(x => x).Any())
                                        DisplayGroup = uow.ProductAttributeGroupSelectRepository.Get(x => x, null, x => x.OrderByDescending(s => s.Id)).Take(1).FirstOrDefault().DisplayGroupOrder + 1;

                                    ProductAttributeGroupSelect pacs = new ProductAttributeGroupSelect()
                                    {
                                        AttributeId = attribute.AttributeId,
                                        GroupId = FinalGroupId,
                                        TabId = TabId.HasValue ? TabId.Value : attribute.TabId,
                                        DisplayGroupOrder = DisplayGroup,
                                        DisplayOrder = MaxDisplayOrder
                                    };
                                    uow.ProductAttributeGroupSelectRepository.Insert(pacs);
                                    CorrectInput++;
                                }
                                else
                                    DuplicateInput++;
                                AllInput++;
                                #endregion
                            }
                        }
                    }
                    //Add Single
                    else
                    {
                        if (GroupId.HasValue)
                            item.GroupId = GroupId.Value;
                        else
                        {
                            if (item.GroupId == 0)
                                item.GroupId = null;
                        }
                        if (TabId.HasValue)
                            item.TabId = Convert.ToInt16(TabId.Value);
                        else
                        {
                            if (item.TabId == 0)
                                item.TabId = null;
                        }
                        #region Add Attribute If Not Exist In Selecte Category And Group
                        if (!ProductAttributeCategorySelects.Any(x => x.AttributeId == item.AttributeId && x.GroupId == item.GroupId))
                        {
                            int MaxDisplayOrder = 0, DisplayGroup = 0;
                            var ItemsOfCategory = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.GroupId == item.GroupId).OrderByDescending(x => x.Id);
                            if (ItemsOfCategory.Any())
                            {
                                MaxDisplayOrder = ItemsOfCategory.FirstOrDefault().DisplayOrder + 1;
                                DisplayGroup = ItemsOfCategory.FirstOrDefault().DisplayGroupOrder;
                            }
                            else if (uow.ProductAttributeGroupSelectRepository.Get(x => x).Any())
                                DisplayGroup = uow.ProductAttributeGroupSelectRepository.Get(x => x, null, x => x.OrderByDescending(s => s.Id)).Take(1).FirstOrDefault().DisplayGroupOrder + 1;

                            ProductAttributeGroupSelect pacs = new ProductAttributeGroupSelect()
                            {
                                AttributeId = item.AttributeId.Value,
                                GroupId = item.GroupId,
                                TabId = item.TabId,
                                DisplayGroupOrder = DisplayGroup,
                                DisplayOrder = MaxDisplayOrder
                            };
                            uow.ProductAttributeGroupSelectRepository.Insert(pacs);
                            CorrectInput++;
                        }
                        else
                            DuplicateInput++;
                        AllInput++;
                        #endregion
                    }
                    uow.Save();


                }


                return Json(new
                {
                    Message = (CorrectInput > 0 ? " تعداد  " + CorrectInput + " خصوصیت ، از میان " + AllInput + " خصوصیت انتخابی اضافه شد. " : "") + (DuplicateInput > 0 ? " تعداد " + DuplicateInput + " خصوصیت تکراری وجود داشت که اضافه نشد. " : ""),
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

        [HttpPost]
        public virtual JsonResult RemoveAttribute(int[] AttributeId, int? GroupId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                foreach (var Attribute in AttributeId)
                {
                    var ProductAttributeCategorySelect = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.AttributeId == Attribute && x.GroupId == GroupId && x.ProductAttribute.DataType < 12).FirstOrDefault();
                    uow.ProductAttributeGroupSelectRepository.Delete(ProductAttributeCategorySelect);
                }


                uow.Save();

                return Json(new
                {
                    Message = "حذف شد.",
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

        public virtual JsonResult RemoveGroupAttribute(int?[] GroupId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                foreach (var Group in GroupId)
                {
                    var ProductAttributeCategorySelect = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.GroupId == Group && x.ProductAttribute.DataType < 12);
                    uow.ProductAttributeSelectRepository.Delete(ProductAttributeCategorySelect.SelectMany(x => x.ProductAttributeSelects).ToList());
                    uow.ProductAttributeGroupSelectRepository.Delete(ProductAttributeCategorySelect.ToList());

                }

                uow.Save();

                return Json(new
                {
                    Message = "حذف شد.",
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

        [HttpPost]
        public virtual JsonResult EditAttributeGroup(string AttributeId, int? CurrentGroupId, int? GroupId, int? CurrentTabId, short? TabId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                int AllInput = 0, CorrectInput = 0, DuplicateInput = 0;
                var CurrentGroupAttributes = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.GroupId == GroupId);

                string[] attid = AttributeId.Split(',');
                foreach (var item in attid)
                {
                    int id = Convert.ToInt32(item);
                    var ProductAttributeCategorySelect = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.GroupId == CurrentGroupId && x.AttributeId == id).FirstOrDefault();
                    ProductAttributeCategorySelect.TabId = TabId;

                    if (!CurrentGroupAttributes.Any(x => x.AttributeId == id))
                    {
                        ProductAttributeCategorySelect.GroupId = GroupId;
                        CorrectInput++;
                        AllInput++;
                    }
                    else
                        DuplicateInput++;

                }

                uow.Save();

                return Json(new
                {
                    Message = (CorrectInput > 0 ? " ویرایش دسته ی تعداد  " + CorrectInput + " خصوصیت ، از میان " + AllInput + " خصوصیت انتخابی انجام شد. " : "") + (DuplicateInput > 0 ? " تعداد " + DuplicateInput + " خصوصیت تکراری در دسته وجود داشت که انجام نشد. " + " ویرایش تب انجام شد. " : ""),
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

        [HttpPost]
        public virtual JsonResult SortGroup(string ids)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var ProductAttributeCategorySelect = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.GroupId == id);
                    foreach (var item in ProductAttributeCategorySelect)
                    {
                        item.DisplayGroupOrder = Convert.ToInt16(i);
                    }
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductAttributeGroups", "SortGroup", false, 200, " مرتب سازی دسته خصوصیات", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Brands", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual JsonResult SortInGroup(string ids, int? GroupId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var ProductAttributeCategorySelect = uow.ProductAttributeGroupSelectRepository.Get(x => x, x => x.AttributeId == id && x.GroupId == GroupId);
                    foreach (var item in ProductAttributeCategorySelect)
                    {
                        item.DisplayOrder = Convert.ToInt16(i);
                    }
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductCategoryAttribute", "SortInGroup", false, 200, " مرتب سازی خصوصیات یک دسته", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Brands", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }

}
