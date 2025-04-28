using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Domain;
using CoreLib.Infrastructure.DateTime;
using PagedList;
//using CoreLib.ViewModel.Security;
using CoreLib.Infrastructure.ModelBinder;
using UnitOfWork;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Web;
using ahmadi.Infrastructure.Security;
using ahmadi.Models;
using Fasterflect;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Security,SuperUser")]
    public class UserManagementController : Controller
    {
        private ApplicationUserManager _userManager;

        public UserManagementController()
        {
        }
        public UserManagementController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private UnitOfWorkClass uow;

        // GET: Admin/UserManagement
        [CorrectArabianLetter(new string[] { "UsernameString", "FullName" })]
        public virtual ActionResult Index(string sortOrder, string UsernameFilter, string UsernameString, string CreationStartDateInput, string currentStartDateInputFiltering, string CreationEndDateInput, string currentEndDateInputFiltering, string LastActivityStartDateInput, string currentLastActivityStartDateInputFiltering, string LastActivityEndDateInput, string currentLastActivityEndDateInputFiltering, string PhoneNumberString, string PhoneNumberFilter, string RoleId, string CurrentRoleIdFiltering, string EmailConfirmId, string CurrentEmailConfirmId, string PhoneNumberConfirmId, string CurrentPhoneNumberConfirmId, string Disable, string CurrentDisable, string FullName, string FullNameFilter, string Gender, string GenderFilter, int? page)
        {
            uow = new UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 9);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    ViewBag.AddPermission = ModulePermission.check(User.Identity.GetUserId(), 8, null);

                    #region LoadDropdown

                    List<SelectListItem> RoleIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "Admin", Value = "Admin" }, new SelectListItem() { Text = "Security", Value = "Security" }, new SelectListItem() { Text = "Support", Value = "Support" } };
                    ViewBag.RoleId = RoleIdSelectListItem;
                    List<SelectListItem> EmailConfirmSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تایید شده", Value = "True" }, new SelectListItem() { Text = "تایید نشده", Value = "False" } };
                    ViewBag.EmailConfirmSelectListItem = EmailConfirmSelectListItem;
                    List<SelectListItem> PhoneNumberSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تایید شده", Value = "True" }, new SelectListItem() { Text = "تایید نشده", Value = "False" } };
                    ViewBag.PhoneNumberSelectListItem = PhoneNumberSelectListItem;
                    List<SelectListItem> DisableSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "False" }, new SelectListItem() { Text = "غیر فعال", Value = "True" } };
                    ViewBag.DisableSelectListItem = DisableSelectListItem;
                    List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                    ViewBag.GenderSelectListItem = GenderSelectListItem;

                    #endregion

                    #region Search
                    if (string.IsNullOrEmpty(UsernameString))
                        UsernameString = UsernameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(FullName))
                        FullName = FullNameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(Gender))
                        Gender = GenderFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(PhoneNumberString))
                        PhoneNumberString = PhoneNumberFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(RoleId))
                        RoleId = CurrentRoleIdFiltering;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(EmailConfirmId))
                        EmailConfirmId = CurrentEmailConfirmId;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(PhoneNumberConfirmId))
                        PhoneNumberConfirmId = CurrentPhoneNumberConfirmId;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(Disable))
                        Disable = CurrentDisable;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(CreationStartDateInput))
                        CreationStartDateInput = currentStartDateInputFiltering;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(CreationEndDateInput))
                        CreationEndDateInput = currentEndDateInputFiltering;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(LastActivityStartDateInput))
                        LastActivityStartDateInput = currentLastActivityStartDateInputFiltering;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(LastActivityEndDateInput))
                        LastActivityEndDateInput = currentLastActivityEndDateInputFiltering;
                    else
                        page = 1;


                    ViewBag.UsernameFilter = UsernameString;
                    ViewBag.FullNameFilter = FullName;
                    ViewBag.GenderFilter = Gender;
                    ViewBag.PhoneNumberFilter = PhoneNumberString;
                    ViewBag.CurrentRoleIdFiltering = RoleId;
                    ViewBag.CurrentEmailConfirmId = EmailConfirmId;
                    ViewBag.CurrentPhoneNumberConfirmId = PhoneNumberConfirmId;
                    ViewBag.CurrentDisable = Disable;
                    ViewBag.currentStartDateInputFiltering = CreationStartDateInput;
                    ViewBag.currentEndDateInputFiltering = CreationEndDateInput;
                    ViewBag.currentLastActivityStartDateInputFiltering = LastActivityStartDateInput;
                    ViewBag.currentLastActivityEndDateInputFiltering = LastActivityEndDateInput;


                    var user = uow.UserRepository.GetByReturnQueryable(x => x, x => x.Email != "admin@admin.admin", null, "Avatarattachment");

                    //Get All user except Admin and superadmin role , if current Role's user is Security
                    if (UserManager.IsInRole(User.Identity.GetUserId(), "Security"))
                    {
                        var Roleusers = uow.UserRepository.Get(x => x, r => r.Roles.Any(x => x.RoleId != "1" || x.RoleId != "5"));
                        string[] roleIds = Roleusers.Select(r => r.Id).ToArray();
                        user = user.Where(x => roleIds.Contains(x.Id));
                    }

                    if (!String.IsNullOrEmpty(UsernameString))
                        user = user.Where(s => s.UserName.Contains(UsernameString));
                    if (!String.IsNullOrEmpty(Gender))
                    {
                        bool gndr = Convert.ToBoolean(Gender);
                        user = user.Where(s => s.Gender == gndr);
                    }
                    if (!String.IsNullOrEmpty(FullName))
                        user = user.Where(s => s.FirstName.Contains(FullName) || s.LastName.Contains(FullName));
                    if (!String.IsNullOrEmpty(PhoneNumberString))
                        user = user.Where(s => s.PhoneNumber.Contains(PhoneNumberString));
                    if (!String.IsNullOrEmpty(RoleId))
                    {

                        var Roleusers = uow.UserRepository.Get(x => x, r => r.Roles.Any(x => x.RoleId != RoleId));
                        string[] roleIds = Roleusers.Select(r => r.Id).ToArray();
                        user = user.Where(x => roleIds.Contains(x.Id));

                    }
                    if (!String.IsNullOrEmpty(EmailConfirmId))
                    {
                        bool ec = Convert.ToBoolean(EmailConfirmId);
                        user = user.Where(s => s.EmailConfirmed == ec);
                    }
                    if (!String.IsNullOrEmpty(PhoneNumberConfirmId))
                    {
                        bool pc = Convert.ToBoolean(PhoneNumberConfirmId);
                        user = user.Where(s => s.PhoneNumberConfirmed == pc);
                    }
                    if (!String.IsNullOrEmpty(Disable))
                    {
                        bool ds = Convert.ToBoolean(Disable);
                        user = user.Where(s => s.Disable == ds);
                    }
                    #region Creation Date
                    DateTime startCreationDate = DateTime.Now, endCreationDate = DateTime.Now;
                    if (!String.IsNullOrEmpty(CreationStartDateInput))
                        startCreationDate = DateTimeConverter.ChangeShamsiToMiladi(CreationStartDateInput);
                    if (!String.IsNullOrEmpty(CreationEndDateInput))
                        endCreationDate = DateTimeConverter.ChangeShamsiToMiladi(CreationEndDateInput);
                    if (!String.IsNullOrEmpty(CreationStartDateInput) && !String.IsNullOrEmpty(CreationEndDateInput))
                        user = user.Where(s => s.CreationDate >= startCreationDate && s.CreationDate <= endCreationDate);
                    else if (!String.IsNullOrEmpty(CreationStartDateInput))
                        user = user.Where(s => s.CreationDate >= startCreationDate);
                    else if (!String.IsNullOrEmpty(CreationEndDateInput))
                        user = user.Where(s => s.CreationDate <= endCreationDate);
                    #endregion
                    #region Last Activity Date
                    DateTime startLastActivityDate = DateTime.Now, endLastActivityDate = DateTime.Now;
                    if (!String.IsNullOrEmpty(LastActivityStartDateInput))
                        startLastActivityDate = DateTimeConverter.ChangeShamsiToMiladi(LastActivityStartDateInput);
                    if (!String.IsNullOrEmpty(LastActivityEndDateInput))
                        endLastActivityDate = DateTimeConverter.ChangeShamsiToMiladi(LastActivityEndDateInput);
                    if (!String.IsNullOrEmpty(LastActivityStartDateInput) && !String.IsNullOrEmpty(LastActivityEndDateInput))
                        user = user.Where(s => s.LastActivityDate >= startLastActivityDate && s.LastActivityDate <= endLastActivityDate);
                    else if (!String.IsNullOrEmpty(LastActivityStartDateInput))
                        user = user.Where(s => s.LastActivityDate >= startLastActivityDate);
                    else if (!String.IsNullOrEmpty(LastActivityEndDateInput))
                        user = user.Where(s => s.LastActivityDate <= endLastActivityDate);
                    #endregion

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "UserName":
                            user = user.OrderBy(s => s.UserName);
                            ViewBag.CurrentSort = "UserName";
                            break;
                        case "UserName_desc":
                            user = user.OrderByDescending(s => s.UserName);
                            ViewBag.CurrentSort = "UserName_desc";
                            break;
                        case "FullName":
                            user = user.OrderBy(s => s.FirstName + " " + s.LastName);
                            ViewBag.CurrentSort = "FullName";
                            break;
                        case "FullName_desc":
                            user = user.OrderByDescending(s => s.FirstName + " " + s.LastName);
                            ViewBag.CurrentSort = "FullName_desc";
                            break;
                        case "Gender":
                            user = user.OrderBy(s => s.Gender);
                            ViewBag.CurrentSort = "Gender";
                            break;
                        case "Gender_desc":
                            user = user.OrderByDescending(s => s.Gender);
                            ViewBag.CurrentSort = "Gender_desc";
                            break;
                        default:  // Name ascending 
                            user = user.OrderByDescending(s => s.CreationDate);
                            break;
                    }

                    #endregion

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "مدیریت کاربران", x => x.OrderBy(o => o.Id), "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "UserManagement", "Index", true, 200, " نمایش صفحه مدیریت کاربران", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(user.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت کاربران" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public virtual JsonResult AddPermission(string AllowInsertIds, string AllowUpdateIds, string AllowDeleteIds, string UserId)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var ap = uow.AdministratorPermissionRepository.Get(x => x, x => x.UserId == UserId);
                uow.AdministratorPermissionRepository.Delete(ap.ToList());
                uow.Save();

                var AllowInserId = AllowInsertIds.Split(',').Select(x => x.Split('-'));
                var AllowUpdateId = AllowUpdateIds.Split(',').Select(x => x.Split('-'));
                var AllowDeleteId = AllowDeleteIds.Split(',').Select(x => x.Split('-'));

                foreach (var item in AllowInserId)
                {
                    AdministratorPermission apNew = new AdministratorPermission();
                    apNew.ModuleId = Convert.ToInt32(item[0]);
                    apNew.UserId = UserId;
                    apNew.TypeAccess = 1;
                    apNew.NotificationEmail = Convert.ToBoolean(item[1]);

                    uow.AdministratorPermissionRepository.Insert(apNew);
                    uow.Save();
                }
                foreach (var item in AllowUpdateId)
                {
                    AdministratorPermission apNew = new AdministratorPermission();
                    apNew.ModuleId = Convert.ToInt32(item[0]);
                    apNew.UserId = UserId;
                    apNew.TypeAccess = 2;
                    apNew.NotificationEmail = Convert.ToBoolean(item[1]);

                    uow.AdministratorPermissionRepository.Insert(apNew);
                    uow.Save();
                }

                foreach (var item in AllowDeleteId)
                {
                    AdministratorPermission apNew = new AdministratorPermission();
                    apNew.ModuleId = Convert.ToInt32(item[0]);
                    apNew.UserId = UserId;
                    apNew.TypeAccess = 3;
                    apNew.NotificationEmail = Convert.ToBoolean(item[1]);

                    uow.AdministratorPermissionRepository.Insert(apNew);
                    uow.Save();
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "UserManagement", "AddPermission", false, 200, " تعریف مجوز برای کاربران", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 200,
                    status = "انجام شد.",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "AddPermission", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 200,
                    status = x.Message,
                }, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: Admin/UserManagement/Permission/5
        public virtual ActionResult Permission(string id)
        {
            uow = new UnitOfWorkClass();

            try
            {
                IdentityManager im = new IdentityManager();
                if (ModulePermission.check(User.Identity.GetUserId(), 8, null))
                {
                    ApplicationUser au = uow.UserRepository.Get(x => x, x => x.Id == id, null, "AdministratorPermissions").SingleOrDefault();
                    if (au != null)
                    {
                        var adminModules = uow.AdministratorModuleRepository.Get(x => x).AsQueryable();
                        if (im.IsInRole(au.Id, "Support"))
                        {
                            adminModules = adminModules.Where(x => x.Id == 2 || x.Id == 3 || x.Id == 4 || x.Id == 5 || x.Id == 6 || x.Id == 7 || x.Id == 8 || x.Id == 12 || x.Id == 20);
                        }
                        else if (im.IsInRole(au.Id, "Security"))
                        {
                            adminModules = adminModules.Where(x => x.Id == 1 || x.Id == 9 || x.Id == 8);
                        }
                        else if (im.IsInRole(au.Id, "Admin"))
                        {
                            adminModules = adminModules.Where(x => x.Id != 11 && x.Id != 14 && x.Id != 19);
                        }

                        ViewBag.AdministratorModules = adminModules.Where(x => x.Id != 21 && x.Id != 22);
                        ViewBag.UserName = string.Format("{0} ( {1} )", au.FirstName + " " + au.LastName, au.UserName);
                        ViewBag.UserId = au.Id;

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 5 && x.Name == "دسترسی های پنل", null, "HelpModuleSectionFields").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "UserManagement", "Permission", true, 200, "نمایش جزئیات مجوز کاربرِ " + au.FirstName + " " + au.LastName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(au.AdministratorPermissions);
                    }
                    else
                    {

                        return HttpNotFound();
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت کاربران" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "Permission", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }
        // GET: Admin/UserManagement/UserAttachements/5
        public virtual ActionResult UserAttachements(string id, int? page)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 9, null))
                {
                    var Oattachment = uow.AttachmentRepository.Get(x => x, a => a.UserId == id);
                    if (Oattachment != null)
                    {
                        int pageSize = 8;
                        int pageNumber = (page ?? 1);
                        ViewBag.Username = uow.UserRepository.GetByID(id).UserName;

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "UserManagement", "UserAttachements", true, 200, " نمایش فایل های ثبت شده توسط کاربرِ " + ViewBag.Username, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(Oattachment.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));
                    }
                    else
                    {

                        return HttpNotFound();
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت کاربران" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "UserAttachements", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }
        // GET: Admin/UserManagement/Details/5
        public virtual ActionResult Details(string id)
        {
            uow = new UnitOfWorkClass();


            if (ModulePermission.check(User.Identity.GetUserId(), 9, null))
            {
                ApplicationUser au = uow.UserRepository.Get(x => x, x => x.Id == id, null, "Avatarattachment").FirstOrDefault();
                if (au != null)
                {
                    return View(au);
                }
                else
                {

                    return HttpNotFound();
                }
            }
            else
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت کاربران" }));

        }

        // GET: Admin/UserManagement/Create
        public virtual ActionResult Create()
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 9, 1))
                {
                    List<SelectListItem> RoleIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "Admin", Value = "Admin" }, new SelectListItem() { Text = "User", Value = "User" }, new SelectListItem() { Text = "Security", Value = "Security" }, new SelectListItem() { Text = "Support", Value = "Support" } };
                    ViewBag.Rolename = RoleIdSelectListItem;
                    List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                    ViewBag.GenderSelectListItem = GenderSelectListItem;

                    UnitOfWorkClass uow = new UnitOfWorkClass();

                    var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();

                    ViewBag.ProuctLisence = true;
                    CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);

                    ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name");


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "UserManagement", "Create", true, 200, " نمایش صفحه ایجاد کاربر", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت کاربران" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }

        // POST: Admin/UserManagement/Create
        [HttpPost]
        public virtual ActionResult Create(RegisterViewModel oModel, string Rolename, string Gender, string IsNewLetter)
        {
            uow = new UnitOfWorkClass();

            try
            {
                oModel.FirstName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(oModel.FirstName);
                oModel.LastName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(oModel.LastName);
                IdentityManager im = new IdentityManager();
                ApplicationUser au = new ApplicationUser();
                au.AccessFailedCount = 0;
                au.CreationDate = DateTime.Now;
                au.Disable = oModel.Disable;
                au.Email = oModel.Email;
                au.EmailConfirmed = oModel.EmailConfirmed;
                au.LockoutEnabled = true;
                au.PhoneNumber = oModel.Mobile;
                au.PhoneNumberConfirmed = oModel.PhoneNumberConfirmed;
                au.TwoFactorEnabled = false;
                au.UserName = oModel.Mobile;
                au.FirstName = oModel.FirstName;
                au.LastName = oModel.LastName;
                au.Avatar = oModel.Avatar;
                au.About = oModel.About;
                bool gndr = Convert.ToBoolean(Gender);
                au.Gender = gndr;
                au.LandlinePhone = oModel.LandlinePhone;
                au.PostalCode = oModel.PostalCode;
                au.CityId = oModel.CityId;
                au.State = oModel.State;
                au.City = oModel.City;
                au.Address = oModel.Address;
                au.AddressNumber = oModel.AddressNumber;
                au.AddressUnit = oModel.AddressUnit;
                if (im.CreateUser(au, oModel.Password))
                {
                    //Add Role
                    im.AddUserToRole(au.Id, Rolename);
                    //Add Moduele
                    var adminModules = uow.AdministratorModuleRepository.GetByReturnQueryable(x => x);
                    if (Rolename.ToLower() == "support")
                        adminModules = adminModules.Where(x => x.Id == 2 || x.Id == 3 || x.Id == 4 || x.Id == 5 || x.Id == 6 || x.Id == 7 || x.Id == 8 || x.Id == 12);
                    else if (Rolename.ToLower() == "admin")
                        adminModules = adminModules.Where(x => x.Id != 11 && x.Id != 14 && x.Id != 19);
                    else if (Rolename.ToLower() == "security")
                        adminModules = adminModules.Where(x => x.Id == 1 || x.Id == 9 || x.Id == 8);

                    foreach (var item in adminModules)
                    {
                        AdministratorPermission ap = new AdministratorPermission()
                        {
                            ModuleId = item.Id,
                            NotificationEmail = true,
                            TypeAccess = 1,
                            UserId = au.Id
                        };
                        AdministratorPermission ap2 = new AdministratorPermission()
                        {
                            ModuleId = item.Id,
                            NotificationEmail = true,
                            TypeAccess = 2,
                            UserId = au.Id
                        };
                        AdministratorPermission ap3 = new AdministratorPermission()
                        {
                            ModuleId = item.Id,
                            NotificationEmail = true,
                            TypeAccess = 3,
                            UserId = au.Id
                        };

                        uow.AdministratorPermissionRepository.Insert(ap);
                        uow.AdministratorPermissionRepository.Insert(ap2);
                        uow.AdministratorPermissionRepository.Insert(ap3);
                    }
                    uow.Save();


                    //else if (im.IsInRole(au.UserName, "Security"))
                    //{
                    //    adminModules = adminModules.Where(x => x.Id == 1 || x.Id == 9 || x.Id == 10);
                    //}
                    //else if (im.IsInRole(au.UserName, "Admin"))
                    //{
                    //    adminModules = adminModules.Where(x => x.Id != 11 && x.Id != 14 && x.Id != 19);
                    //}
                    if (Convert.ToBoolean(IsNewLetter))
                    {
                        NewsLetterEmail oNewsLetterEmail = new NewsLetterEmail();
                        oNewsLetterEmail.Email = au.Email;
                        oNewsLetterEmail.InsertDate = DateTime.Now;
                        oNewsLetterEmail.LanguageId = 1;
                        oNewsLetterEmail.IsVerified = (au.EmailConfirmed ? true : false);
                        uow.NewsLetterEmailRepository.Insert(oNewsLetterEmail);
                        uow.Save();
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ProuctLisence = true;
                    List<SelectListItem> RoleIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "Admin", Value = "Admin" }, new SelectListItem() { Text = "User", Value = "User" }, new SelectListItem() { Text = "Security", Value = "Security" }, new SelectListItem() { Text = "Support", Value = "Support" } };
                    ViewBag.Rolename = RoleIdSelectListItem;
                    List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                    ViewBag.GenderSelectListItem = GenderSelectListItem;
                    ViewBag.Error = " خطایی رخ داد،کاربر ایجاد نشد.";
                    ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name");

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "UserManagement", "Create", false, 200, " نمایش صفحه ایجاد کاربرِ " + au.FirstName + " " + au.LastName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();

                }

            }
            catch (Exception x)
            {
                List<SelectListItem> RoleIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "Admin", Value = "Admin" }, new SelectListItem() { Text = "User", Value = "User" } };
                ViewBag.Rolename = RoleIdSelectListItem;
                List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                ViewBag.GenderSelectListItem = GenderSelectListItem;
                ViewBag.Error = x.Message;




                var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.ProuctLisence = true;
                CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);
                ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(s => s), "Id", "Name");


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View();
            }
        }

        // GET: Admin/UserManagement/Edit/5
        public virtual ActionResult Edit(string id)
        {
            uow = new UnitOfWorkClass();

            try
            {
                IdentityManager im = new IdentityManager();


                if (ModulePermission.check(User.Identity.GetUserId(), 9, 2))
                {
                    ApplicationUser au = uow.UserRepository.Get(x => x, x => x.Id == id, null, "CityEntity,Avatarattachment").SingleOrDefault();
                    if (au != null)
                    {
                        List<SelectListItem> RoleIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "Admin", Value = "Admin", Selected = (im.IsInRole(au.Id, "Admin") ? true : false) }, new SelectListItem() { Text = "User", Value = "User", Selected = (im.IsInRole(au.Id, "User") ? true : false) }, new SelectListItem() { Text = "Security", Value = "Security", Selected = (im.IsInRole(au.Id, "Security") ? true : false) }, new SelectListItem() { Text = "Support", Value = "Support", Selected = (im.IsInRole(au.Id, "Support") ? true : false) } };
                        ViewBag.Rolename = RoleIdSelectListItem;
                        List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False", Selected = (au.Gender == false ? true : false) }, new SelectListItem() { Text = "مرد", Value = "True", Selected = (au.Gender == true ? true : false) } };
                        ViewBag.GenderSelectListItem = GenderSelectListItem;

                        EditUserViewModel eu = new EditUserViewModel();
                        eu.attachment = au.Avatarattachment;
                        eu.Id = id;
                        eu.CityId = au.CityId;
                        eu.Disable = au.Disable;
                        eu.Email = au.Email;
                        eu.EmailConfirmed = au.EmailConfirmed;
                        eu.Mobile = au.PhoneNumber;
                        eu.PhoneNumberConfirmed = au.PhoneNumberConfirmed;
                        eu.Gender = au.Gender;
                        eu.FirstName = au.FirstName;
                        eu.LastName = au.LastName;
                        eu.Avatar = au.Avatar;
                        eu.About = au.About;
                        eu.LandlinePhone = au.LandlinePhone;
                        eu.PostalCode = au.PostalCode;
                        eu.AddressNumber = au.AddressNumber;
                        eu.AddressUnit = au.AddressUnit;
                        eu.State = 1;
                        eu.ProvienceId = au.CityEntity.ProvinceId;
                        eu.Address = au.Address;
                        Session["CurrentPhoneNumber"] = au.PhoneNumber;
                        Session["CurrentIsNewsLetter"] = (uow.NewsLetterEmailRepository.Get(x => x, x => x.Email == au.Email).Any());
                        ViewBag.CurrentIsNewsLetter = Session["CurrentIsNewsLetter"];


                        ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", eu.ProvienceId);
                        ViewBag.CityList = new SelectList(uow.CityRepository.Get(x => x), "Id", "Name", eu.CityId);


                        var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                        ViewBag.ProuctLisence = true;
                        CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);
                        ViewBag.State = new SelectList(ReadXml.ListOfXState(), "Id", "Name", eu.State);


                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "UserManagement", "Edit", true, 200, " نمایش صفحه ویرایش کاربر " + eu.FirstName + " " + eu.LastName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(eu);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت کاربران" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }

        // POST: Admin/UserManagement/Edit/5
        [HttpPost]
        public virtual ActionResult Edit(EditUserViewModel oModel, string Rolename, string Gender, string IsNewLetter)
        {
            uow = new UnitOfWorkClass();
            oModel.FirstName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(oModel.FirstName);
            oModel.LastName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(oModel.LastName);
            ApplicationUser au = uow.UserRepository.Get(u => u, u => u.Id == oModel.Id, null, "AdministratorPermissions").SingleOrDefault();
            IdentityManager im = new IdentityManager();
            string oldRoleName = "user";
            if (im.IsInRole(oModel.Id, "Admin"))
                oldRoleName = "admin";
            else if (im.IsInRole(oModel.Id, "Support"))
                oldRoleName = "support";
            else if (im.IsInRole(oModel.Id, "Security"))
                oldRoleName = "security";
            try
            {
                if (uow.UserRepository.Get(x => x, x => x.PhoneNumber == oModel.Mobile && x.Id != oModel.Id).Any())
                {
                    List<SelectListItem> RoleIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "Admin", Value = "Admin", Selected = (im.IsInRole(oModel.Id, "Admin") ? true : false) }, new SelectListItem() { Text = "User", Value = "User", Selected = (im.IsInRole(oModel.Id, "User") ? true : false) }, new SelectListItem() { Text = "Security", Value = "Security", Selected = (im.IsInRole(oModel.Id, "Security") ? true : false) }, new SelectListItem() { Text = "Support", Value = "Support", Selected = (im.IsInRole(oModel.Id, "Support") ? true : false) } };
                    ViewBag.Rolename = RoleIdSelectListItem;
                    List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False", Selected = (oModel.Gender == false ? true : false) }, new SelectListItem() { Text = "مرد", Value = "True", Selected = (oModel.Gender == true ? true : false) } };
                    ViewBag.GenderSelectListItem = GenderSelectListItem;
                    ViewBag.Error = " تلفن همراه وارد شده تکراری است.";
                    Session["CurrentIsNewsLetter"] = (uow.NewsLetterEmailRepository.Get(x => x, x => x.Email == au.Email).Any());
                    ViewBag.CurrentIsNewsLetter = Session["CurrentIsNewsLetter"];
                    return View(oModel);
                }
                au.CityId = oModel.CityId;
                au.AccessFailedCount = 0;
                au.CreationDate = DateTime.Now;
                au.Disable = oModel.Disable;
                au.Email = oModel.Email;
                au.EmailConfirmed = oModel.EmailConfirmed;
                au.LockoutEnabled = true;
                au.PhoneNumber = oModel.Mobile;
                au.PhoneNumberConfirmed = oModel.PhoneNumberConfirmed;
                au.TwoFactorEnabled = false;
                au.UserName = oModel.Mobile;
                au.FirstName = oModel.FirstName;
                au.LastName = oModel.LastName;
                au.Avatar = oModel.Avatar;
                au.About = oModel.About;
                au.LandlinePhone = oModel.LandlinePhone;
                au.PostalCode = oModel.PostalCode;
                au.State = oModel.State;
                au.City = oModel.City;
                au.Address = oModel.Address;
                au.AddressNumber = oModel.AddressNumber;
                au.AddressUnit = oModel.AddressUnit;
                if (Session["CurrentPhoneNumber"] != null)
                {
                    if (oModel.Mobile != Session["CurrentPhoneNumber"].ToString())
                        au.PhoneNumberConfirmed = false;
                }
                Session["CurrentPhoneNumber"] = null;
                bool gndr = Convert.ToBoolean(Gender);
                au.Gender = gndr;
                uow.UserRepository.Update(au);
                uow.Save();
                //im.ClearRole(au.Id);
                if (!im.IsInRole(au.Id, Rolename))
                    im.AddUserToRole(au.Id, Rolename);
                //Add Moduele
                if (Rolename.ToLower() != oldRoleName)
                {
                    uow.AdministratorPermissionRepository.Delete(au.AdministratorPermissions);
                    uow.Save();
                    var adminModules = uow.AdministratorModuleRepository.GetByReturnQueryable(x => x);
                    if (Rolename.ToLower() == "support")
                        adminModules = adminModules.Where(x => x.Id == 2 || x.Id == 3 || x.Id == 4 || x.Id == 5 || x.Id == 6 || x.Id == 7 || x.Id == 8 || x.Id == 12);
                    else if (Rolename.ToLower() == "admin")
                        adminModules = adminModules.Where(x => x.Id != 11 && x.Id != 14 && x.Id != 19);
                    else if (Rolename.ToLower() == "security")
                        adminModules = adminModules.Where(x => x.Id == 1 || x.Id == 9 || x.Id == 8);


                    foreach (var item in adminModules)
                    {
                        AdministratorPermission ap = new AdministratorPermission()
                        {
                            ModuleId = item.Id,
                            NotificationEmail = true,
                            TypeAccess = 1,
                            UserId = au.Id
                        };
                        AdministratorPermission ap2 = new AdministratorPermission()
                        {
                            ModuleId = item.Id,
                            NotificationEmail = true,
                            TypeAccess = 2,
                            UserId = au.Id
                        };
                        AdministratorPermission ap3 = new AdministratorPermission()
                        {
                            ModuleId = item.Id,
                            NotificationEmail = true,
                            TypeAccess = 3,
                            UserId = au.Id
                        };

                        uow.AdministratorPermissionRepository.Insert(ap);
                        uow.AdministratorPermissionRepository.Insert(ap2);
                        uow.AdministratorPermissionRepository.Insert(ap3);
                    }
                    uow.Save();
                }

                if (Session["CurrentIsNewsLetter"] != null)
                {
                    if (Convert.ToBoolean(Session["CurrentIsNewsLetter"]) == false && Convert.ToBoolean(IsNewLetter))
                    {
                        NewsLetterEmail oNewsLetterEmail = new NewsLetterEmail();
                        oNewsLetterEmail.Email = au.Email;
                        oNewsLetterEmail.InsertDate = DateTime.Now;
                        oNewsLetterEmail.LanguageId = 1;
                        oNewsLetterEmail.IsVerified = (au.EmailConfirmed ? true : false);
                        uow.NewsLetterEmailRepository.Insert(oNewsLetterEmail);
                        uow.Save();
                    }
                    else if (Convert.ToBoolean(Session["CurrentIsNewsLetter"]) == true && Convert.ToBoolean(IsNewLetter) == false)
                    {

                        NewsLetterEmail oNewsLetterEmail = new NewsLetterEmail();
                        oNewsLetterEmail = uow.NewsLetterEmailRepository.GetByID(au.Email);
                        uow.NewsLetterEmailRepository.Delete(oNewsLetterEmail);
                        uow.Save();
                    }
                    Session["CurrentIsNewsLetter"] = null;
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "UserManagement", "Edit", false, 200, "   ویرایش کاربرِ " + au.FirstName + " " + au.LastName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {
                List<SelectListItem> RoleIdSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "Admin", Value = "Admin" }, new SelectListItem() { Text = "User", Value = "User" }, new SelectListItem() { Text = "Security", Value = "Security" }, new SelectListItem() { Text = "Support", Value = "Support" } };
                ViewBag.Rolename = RoleIdSelectListItem;
                List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                ViewBag.GenderSelectListItem = GenderSelectListItem;
                ViewBag.Error = x.Message;
                Session["CurrentIsNewsLetter"] = (uow.NewsLetterEmailRepository.Get(y => y, y => y.Email == au.Email).Any());
                ViewBag.CurrentIsNewsLetter = Session["CurrentIsNewsLetter"];



                var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.ProuctLisence = true;
                CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);
                ViewBag.State = new SelectList(ReadXml.ListOfXState(), "Id", "Name");


                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(oModel);
            }
        }

        //POST: Admin/UserManagement/Delete/5
        [HttpPost]
        public virtual JsonResult Delete(string Id)
        {
            uow = new UnitOfWorkClass();
            try
            {


                //Remove Tickets Of User
                var Tickets = uow.TicketRepository.Get(x => x.UserId == Id);
                uow.TicketRepository.Delete(Tickets);
                uow.Save();


                //Remove Offers Of User
                var SearchLogs = uow.SearchLogRepository.Get(x => x.UserId == Id);
                uow.SearchLogRepository.Delete(SearchLogs);
                uow.Save();

                //Remove Offers Of User
                var Offers = uow.OfferRepository.Get(x => x.UserId == Id);
                uow.OfferRepository.Delete(Offers);
                uow.Save();

                //Remove Product wallets Of User
                var wallets = uow.WalletRepository.Get(x => x.UserId == Id);
                uow.WalletRepository.Delete(wallets);
                uow.Save();

                //Remove Product Question Of User
                var ProductQuestions = uow.ProductQuestionRepository.Get(x => x, x => x.UserId == Id);
                uow.ProductQuestionRepository.Delete(ProductQuestions);
                uow.Save();

                //Remove Product Comment Of User
                var ProductComments = uow.ProductCommentRepository.Get(x => x, x => x.UserId == Id);
                uow.ProductCommentRepository.Delete(ProductComments);
                uow.Save();

                //Remove Product Comment Of User
                var ProductRankSelects = uow.ProductRankSelectValueRepository.Get(x => x, x => x.UserId == Id);
                uow.ProductRankSelectRepository.Delete(ProductRankSelects);
                uow.Save();

                //Remove Contents Of User
                var Contents = uow.ContentRepository.Get(x => x, x => x.UserId == Id);
                uow.ContentRepository.Delete(Contents);
                uow.Save();

                IdentityManager im = new IdentityManager();
                if (im.DeleteUser(Id))
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(4, "UserManagement", "Delete", false, 200, "   حذف کاربرِ " + Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion

                    return Json(new
                    {
                        statusCode = 200,
                        status = "کاربر مورد نظر حذف شد.",
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "Delete", false, 500, " در حذف کاربر " + Id + " خطایی رخ داد. ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        statusCode = 400,
                        status = "خطایی رخ داد. بعدا تلاش نمایید",
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "Delete", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400,
                    status = x.Message,
                }, JsonRequestBehavior.AllowGet);

            }
        }

        // POST: Admin/UserManagement/Disable/5
        [HttpPost]
        public virtual JsonResult Disable(string Id)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var user = uow.UserRepository.GetByID(Id);
                if (user != null)
                {
                    user.Disable = true;
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "UserManagement", "Disable", false, 200, "   غیرفعال نمودن کاربرِ " + user.FirstName + " " + user.LastName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        statusCode = 200,
                        status = "کاربر مورد نظر ، غیر فعال شد.",
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "Disable", false, 500, "   غیرفعال نمودن کاربرِ " + user.FirstName + " " + user.LastName + " با خطا مواجه شد. ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        statusCode = 400,
                        status = "خطایی رخ داد. بعدا تلاش نمایید",
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "Disable", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400,
                    status = x.Message,
                }, JsonRequestBehavior.AllowGet);

            }
        }

        // POST: Admin/UserManagement/Disable/5
        [HttpPost]
        public virtual JsonResult Enable(string Id)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var user = uow.UserRepository.GetByID(Id);
                if (user != null)
                {
                    user.Disable = false;
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "UserManagement", "Enable", false, 200, "   فعال نمودن کاربرِ " + user.FirstName + " " + user.LastName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        statusCode = 200,
                        status = "کاربر مورد نظر ، فعال شد.",
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "Enable", false, 500, "   فعال نمودن کاربرِ " + user.FirstName + " " + user.LastName + " با خطا مواجه شد. ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        statusCode = 400,
                        status = "خطایی رخ داد. بعدا تلاش نمایید",
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "Enable", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400,
                    status = x.Message,
                }, JsonRequestBehavior.AllowGet);

            }
        }


        // POST: Admin/UserManagement/ChangePassword/5
        [HttpPost]
        public virtual JsonResult ChangePassword(string UserId, string newPass)
        {
            try
            {
                IdentityManager im = new IdentityManager();
                if (im.ChangePassword(UserId, newPass))
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "UserManagement", "ChangePassword", false, 200, "   تغییر رمز عبور کاربرِ " + UserId + " انجام شد. ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        statusCode = 200,
                        status = "رمز عبور تغییر یافت.",
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "ChangePassword", false, 500, "   تغیر رمز عبور کاربرِ " + UserId + " با خطا مواجه شد. ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        statusCode = 400,
                        status = "خطایی رخ داد. بعدا تلاش نمایید",
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "UserManagement", "ChangePassword", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400,
                    status = x.Message,
                }, JsonRequestBehavior.AllowGet);

            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

            }

            base.Dispose(disposing);
        }
    }
}
