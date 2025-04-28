using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using System.Collections.Generic;
using CoreLib.ViewModel.Xml;
using ahmadi.Infrastructure.Security;
using Microsoft.AspNet.Identity;
using PagedList;
using CoreLib.Infrastructure.ModelBinder;
using System.Threading.Tasks;
using System.Data.SqlClient;
using OfficeOpenXml;
using System.IO;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Security,SuperUser")]
    public partial class UserGroupController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;
        public UserGroupController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }

        // GET: Admin/usergroup
        [CorrectArabianLetter(new string[] { "NameString", "NameFilter" })]
        public virtual ActionResult Index(string Name, string NameFilter, int? PageSize, int? page)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 9);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    var userGroups = uow.UserGroupRepository.GetByReturnQueryable(f => f, null, null, "UserGroupSelects");

                    if (string.IsNullOrEmpty(Name))
                        Name = NameFilter;
                    else
                        page = 1;

                    ViewBag.NameFilter = Name;

                    if (!String.IsNullOrEmpty(Name))
                        userGroups = userGroups.Where(s => s.Name.Contains(Name));


                    int pageSize = 10;
                    if (PageSize.HasValue)
                    {
                        if (PageSize.Value > 100)
                            pageSize = 100;
                        else if (PageSize < 10)
                            pageSize = 10;
                        else
                            pageSize = PageSize.Value;
                    }
                    ViewBag.pageSize = pageSize;
                    int pageNumber = (page ?? 1);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "UserGroups", "Index", true, 200, " نمایش صفحه مدیریت گروههای کاربران", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(userGroups.OrderBy(x => x.DisplayOrder).ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت کاربران" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserGroups", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Export(int Id)
        {
            var GroupTitle = uow.UserGroupRepository.GetByID(Id).Name;
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.Cells[1, 1].LoadFromCollection(uow.UserGroupSelectRepository.Get(x => new { x.User.FirstName, x.User.LastName, x.User.Email, x.User.PhoneNumber, Province = x.User.CityId.HasValue ? x.User.CityEntity.Province.Name : "--", City = x.User.CityId.HasValue ? x.User.CityEntity.Name : "--", x.User.Address }, x => x.userGroupId == Id, x => x.OrderBy(s => s.User.firstTime).ThenBy(s => s.User.LastName), "User.CityEntity.Province"), true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + GroupTitle + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();

                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        // GET: Admin/usergroup/addMember/5
        public virtual ActionResult addMember(int? id, string NameString, string NameFilter, int? PageSize, int? page, string sortOrder)
        {
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 1);
                if (p.Where(x => x == true).Any())
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    UserGroup UserGroup = uow.UserGroupRepository.Get(x => x, x => x.Id == id, null).FirstOrDefault();
                    if (UserGroup == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();
                    ViewBag.Id = id.Value;

                    if (string.IsNullOrEmpty(NameString))
                        NameString = NameFilter;
                    else
                    {
                        page = 1;
                        NameString = NameString.ToLower();
                    }


                    ViewBag.NameFilter = NameString;

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

                    var UserGroupSelects = uow.UserGroupSelectRepository.GetQueryList().AsNoTracking().Include("User.CityEntity.Province");

                    UserGroupSelects = UserGroupSelects.Where(x => x.userGroupId == id.Value);

                    if (!String.IsNullOrEmpty(NameString))
                        UserGroupSelects = UserGroupSelects.Where(x => x.User.FirstName.Contains(NameString) || x.User.LastName.Contains(NameString));

                    switch (sortOrder)
                    {
                        case "Fullname":
                            UserGroupSelects = UserGroupSelects.OrderBy(s => s.User.firstTime).ThenBy(x => x.User.LastName);
                            ViewBag.CurrentSort = "Fullname";
                            break;
                        case "Fullname_desc":
                            UserGroupSelects = UserGroupSelects.OrderByDescending(s => s.User.firstTime).OrderByDescending(x => x.User.LastName);
                            ViewBag.CurrentSort = "Fullname_desc";
                            break;
                        case "Username":
                            UserGroupSelects = UserGroupSelects.OrderBy(s => s.User.UserName);
                            ViewBag.CurrentSort = "Username";
                            break;
                        case "Username_desc":
                            UserGroupSelects = UserGroupSelects.OrderByDescending(s => s.User.UserName);
                            ViewBag.CurrentSort = "Username_desc";
                            break;
                        case "Province":
                            UserGroupSelects = UserGroupSelects.OrderBy(s => s.User.CityEntity.Province.Name);
                            ViewBag.CurrentSort = "Province";
                            break;
                        case "Province_desc":
                            UserGroupSelects = UserGroupSelects.OrderByDescending(s => s.User.CityEntity.Province.Name);
                            ViewBag.CurrentSort = "Province_desc";
                            break;
                        case "Gender":
                            UserGroupSelects = UserGroupSelects.OrderBy(s => s.User.Gender);
                            ViewBag.CurrentSort = "Gender";
                            break;
                        case "Gender_desc":
                            UserGroupSelects = UserGroupSelects.OrderByDescending(s => s.User.Gender);
                            ViewBag.CurrentSort = "Gender_desc";
                            break;
                        default:
                            UserGroupSelects = UserGroupSelects.OrderByDescending(s => s.Id); break;

                    }

                    ViewBag.UserList = UserGroupSelects.ToPagedList(pageNumber, pageSize);
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "usergroup", "addMember", true, 200, " نمایش صفحه اعضای گروه کاربری " + UserGroup.Descr, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(UserGroup);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "usergroup", "addMember", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/usergroup/Create
        public virtual ActionResult Create()
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 1))
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "usergroup", "Create", true, 200, " نمایش صفحه ایجاد گروه کاربری", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "usergroup", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/usergroup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(UserGroup UserGroup)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                if (!uow.UserGroupRepository.Get(x => x, x => x.Name.ToLower() == UserGroup.Name.ToLower()).Any())
                {
                    if (ModelState.IsValid)
                    {
                        UserGroup.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(UserGroup.Name);
                        UserGroup.InsertDate = DateTime.Now;
                        UserGroup.UpdateDate = DateTime.Now;
                        if (uow.UserGroupRepository.Any(x => x.DisplayOrder))
                            UserGroup.DisplayOrder = uow.UserGroupRepository.Max(x => x.DisplayOrder) + 1;
                        else
                            UserGroup.DisplayOrder = 0;
                        uow.UserGroupRepository.Insert(UserGroup);
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "usergroup", "Create", false, 200, "   ایجاد گروه کاربری ی" + UserGroup.Name, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.Error = " نام وارد شده وجود دارد ";

                }


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "usergroup", "Create", false, 500, "   خطا در ایجاد گروه کاربری ی" + UserGroup.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(UserGroup);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "usergroup", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/usergroup/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    UserGroup UserGroup = uow.UserGroupRepository.GetByID(id);
                    if (UserGroup == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "usergroup", "Edit", true, 200, " نمایش صفحه ویرایش گروه کاربری ی" + UserGroup.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(UserGroup);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "usergroup", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/usergroup/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(UserGroup UserGroup)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                if (!uow.UserGroupRepository.Get(x => x, x => x.Id != UserGroup.Id && x.Name.ToLower() == UserGroup.Name.ToLower()).Any())
                {
                    if (ModelState.IsValid)
                    {
                        UserGroup.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(UserGroup.Name);
                        UserGroup.UpdateDate = DateTime.Now;
                        uow.UserGroupRepository.Update(UserGroup);
                        uow.Save();
                        return RedirectToAction("Index");
                    }
                }
                else
                {

                    ViewBag.Error = " نام وارد شده وجود دارد ";
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "usergroup", "Edit", false, 200, "   ویرایش گروه کاربری ی" + UserGroup.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(UserGroup);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "usergroup", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/usergroup/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    UserGroup UserGroup = uow.UserGroupRepository.GetByID(id);
                    if (UserGroup == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "usergroup", "Delete", true, 200, " نمایش صفحه حذفِ گروه کاربری ی" + UserGroup.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(UserGroup);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "usergroup", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/usergroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            try
            {
                UserGroup UserGroup = uow.UserGroupRepository.Get(x => x, x => x.Id == id, null, "UserGroupSelects").FirstOrDefault();
                uow.UserGroupSelectRepository.Delete(UserGroup.UserGroupSelects.ToList());
                uow.Save();
                uow.UserGroupRepository.Delete(UserGroup);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "usergroup", "DeleteConfirmed", false, 200, "   حذف گروه کاربری ی" + UserGroup.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "usergroup", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/usergroup/DeleteMember/5
        public virtual ActionResult DeleteMember(int? id)
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    UserGroupSelect UserGroupSelect = uow.UserGroupSelectRepository.GetByID(id);
                    if (UserGroupSelect == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "usergroup", "DeleteMember", true, 200, " نمایش صفحه حذفِ  کاربری ی" + UserGroupSelect.UserId, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(UserGroupSelect);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "usergroup", "DeleteMember", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Contents/Delete/5
        [HttpPost]
        public virtual JsonResult DeleteMember(int id)
        {
            try
            {
                UserGroupSelect UserGroupSelect = uow.UserGroupSelectRepository.GetByID(id);
                uow.UserGroupSelectRepository.Delete(UserGroupSelect);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "usergroup", "DeleteMember", false, 200, " حذف کاربر از گروه کاربری " + id, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    Message = "حذف شد",
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "usergroup", "DeleteMember", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    Message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: Admin/usergroup/CreateUserList
        public virtual ActionResult CreateUserList(int id)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 1))
                {
                    var userGroup = uow.UserGroupRepository.GetByID(id);
                    if (userGroup == null)
                        return Redirect("~/Admin/usergroup");

                    ViewBag.userGroup = userGroup;
                    ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", 304);
                    ViewBag.cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == 304), "Id", "Name", 304);

                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);
                    ViewBag.ProductCategories = new SelectList(uow.ProductCategoryRepository.Get(x => x, x => x.ParrentId == null, x => x.OrderBy(s => s.Name)), "Id", "Name");
                    ViewBag.Provinces = new SelectList(uow.ProvinceRepository.Get(x => x, null, x => x.OrderBy(s => s.Name)), "Id", "Name");
                    List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                    ViewBag.GenderSelectListItem = GenderSelectListItem;

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "usergroup", "CreateUserList", true, 200, " نمایش صفحه ایجاد لیست برای گروه کاربری", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(userGroup);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت کاربران" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "usergroup", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public virtual JsonResult GetUsers(int? ProductCatId, int? FromM, int? ToM, int? OrderCount, int? DateCount, int? ProvinceId, int? CityId, int? CodeGiftCount, bool? Gender, int? LastDateBuy)
        {
            var users = uow.UserRepository.GetQueryList().AsNoTracking();
            try
            {
                if (ProductCatId.HasValue && ProductCatId > 0)
                {
                    List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ProductCatId.Value)).ToList();
                    users = users.Where(x => x.Orders.Any(s => s.OrderRows.Any(a => a.Product.ProductCategories.Any(b => CatIds.Contains(b.Id)))));
                }
                if (FromM != null)
                    users = users.Where(x => x.Orders.Any(s => s.OrderWallets.Any(b => b.Wallet.Price >= FromM)));
                if (ToM != null)
                    users = users.Where(x => x.Orders.Any(s => s.OrderWallets.Any(b => b.Wallet.Price <= ToM)));
                if ((OrderCount != null && OrderCount > 0) && (DateCount != null && DateCount > 0))
                {
                    DateTime dateTime = DateTime.Now.AddDays(-DateCount.Value);
                    users = users.Where(x => x.Orders.Count>=OrderCount && x.Orders.Any(s => s.InsertDate >= dateTime));
                }
                if (OrderCount != null)
                    users = users.Where(x => x.Orders.Count >= OrderCount);
                if (DateCount != null)
                {
                    DateTime dateTime = DateTime.Now.AddDays(-DateCount.Value);
                    users = users.Where(x => x.Orders.Any(s => s.InsertDate >= dateTime));
                }
                if (CityId != null)
                    users = users.Where(x => x.CityId == CityId);
                if (ProvinceId != null && CityId == null)
                    users = users.Where(x => x.CityEntity.ProvinceId == ProvinceId);
                if (CodeGiftCount != null)
                    users = users.Where(x => x.UserCodeGifts.Any(s => s.UserCodeGiftLogs.Count >= CodeGiftCount));
                if (Gender != null)
                    users = users.Where(x => x.Gender == Gender.Value);
                if (LastDateBuy != null)
                {
                    DateTime dateTime = DateTime.Now.AddDays(-LastDateBuy.Value);
                    users = users.Where(x => x.Orders.Any(s => s.InsertDate < dateTime));
                }

                return Json(new
                {
                    AllCount = users.Count(),
                    AllDuplicateCount = users.Where(x => x.UserGroupSelects.Any()).Count(),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    AllCount = 0,
                    AllDuplicateCount = 0,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual ActionResult CreateUserList(int GroupId, int? ProductCatId, int? FromM, int? ToM, int? OrderCount, int? DateCount, int? ProvinceId, int? CityId, int? CodeGiftCount, bool? Gender, int? LastDateBuy)
        {
            var users = uow.UserRepository.GetQueryList().AsNoTracking();
            try
            {
                if (ProductCatId.HasValue && ProductCatId > 0)
                {
                    List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ProductCatId.Value)).ToList();
                    users = users.Where(x => x.Orders.Any(s => s.OrderRows.Any(a => a.Product.ProductCategories.Any(b => CatIds.Contains(b.Id)))));
                }
                if (FromM != null)
                    users = users.Where(x => x.Orders.Any(s => s.OrderWallets.Any(b => b.Wallet.Price >= FromM)));
                if (ToM != null)
                    users = users.Where(x => x.Orders.Any(s => s.OrderWallets.Any(b => b.Wallet.Price <= ToM)));
                if ((OrderCount != null && OrderCount > 0) && (DateCount != null && DateCount > 0))
                {
                    DateTime dateTime = DateTime.Now.AddDays(-DateCount.Value);
                    users = users.Where(x => x.Orders.Count >= OrderCount && x.Orders.Any(s => s.InsertDate >= dateTime));
                }
                if (OrderCount != null)
                    users = users.Where(x => x.Orders.Count >= OrderCount);
                if (DateCount != null)
                {
                    DateTime dateTime = DateTime.Now.AddDays(-DateCount.Value);
                    users = users.Where(x => x.Orders.Any(s => s.InsertDate >= dateTime));
                }
                if (CityId != null)
                    users = users.Where(x => x.CityId == CityId);
                if (ProvinceId != null && CityId == null)
                    users = users.Where(x => x.CityEntity.ProvinceId == ProvinceId);
                if (CodeGiftCount != null)
                    users = users.Where(x => x.UserCodeGifts.Any(s => s.UserCodeGiftLogs.Count >= CodeGiftCount));
                if (Gender != null)
                    users = users.Where(x => x.Gender == Gender.Value);
                if (LastDateBuy != null)
                {
                    DateTime dateTime = DateTime.Now.AddDays(-LastDateBuy.Value);
                    users = users.Where(x => x.Orders.Any(s => s.InsertDate < dateTime));
                }

                foreach (var item in users)
                {
                    UserGroupSelect userGroupSelect = new UserGroupSelect();
                    userGroupSelect.InsertDate = DateTime.Now;
                    userGroupSelect.userGroupId = GroupId;
                    userGroupSelect.UserId = item.Id;
                    uow.UserGroupSelectRepository.Insert(userGroupSelect);
                }
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "usergroup", "CreateUserList", true, 200, " ایجاد لیست برای گروه کاربری", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Redirect("~/Admin/UserGroup/addMember/" + GroupId);


            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "usergroup", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Menus/Sort/5
        [HttpPost]
        public virtual JsonResult Sort(string ids)
        {
            try
            {
                string[] idss = ids.Split(',');
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var UserGroup = uow.UserGroupRepository.GetByID(id);
                    UserGroup.DisplayOrder = i;
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "UserGroup", "Sort", false, 200, "مرتب سازی گروه کاربری", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserGroup", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
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
