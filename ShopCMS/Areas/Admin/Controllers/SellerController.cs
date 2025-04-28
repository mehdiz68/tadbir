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
using ahmadi.Models;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class SellerController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;
        public SellerController()
        {

            uow = new UnitOfWork.UnitOfWorkClass();
        }

        // GET: Admin/Seller
        [CorrectArabianLetter(new string[] { "nameString", "nameFilter" })]
        public virtual ActionResult Index(string sortOrder, string nameFilter, string nameString, int? page)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 25);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                    #region search
                    if (string.IsNullOrEmpty(nameString))
                        nameString = nameFilter;
                    else
                        page = 1;

                    ViewBag.nameFilter = nameString;

                    var sellers = uow.SellerRepository.GetQueryList().Include("User.CityEntity.Province").Include("ProductPrices").AsNoTracking();
                    if (!String.IsNullOrEmpty(nameString))
                        sellers = sellers.Where(s => s.User.FirstName.Contains(nameString) || s.User.LastName.Contains(nameString));


                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "Name":
                            sellers = sellers.OrderBy(s => s.Id);
                            ViewBag.CurrentSort = "Name";
                            break;
                        case "Name_desc":
                            sellers = sellers.OrderByDescending(s => s.Id);
                            ViewBag.CurrentSort = "Name_desc";
                            break;
                        default:  // Name ascending 
                            sellers = sellers.OrderByDescending(s => s.Id);
                            break;
                    }

                    #endregion

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Seller", "Index", true, 200, " نمایش صفحه مدیریت فروشندگان", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(sellers.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت سفارشات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tags", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }



        // GET: Admin/Seller/Create
        public virtual ActionResult Create()
        {
            try
            {
                if (ModulePermission.check(User.Identity.GetUserId(), 25, 1))
                {


                    ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name");


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Seller", "Create", true, 200, " نمایش صفحه ایجاد فروشنده", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت سفارشات" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Seller", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }

        [HttpPost]
        public virtual ActionResult Create(RegisterViewModel oModel, string Gender, string IsNewLetter)
        {

            ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name");

            try
            {
                Random generator = new Random();
                String ra = generator.Next(0, 1000000).ToString("D6");
                oModel.Password = ra;

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
                au.CityId = oModel.CityId > 0 ? oModel.CityId : 304;
                au.State = oModel.State;
                au.City = oModel.City;
                au.Address = oModel.Address;
                au.AddressNumber = oModel.AddressNumber;
                au.AddressUnit = oModel.AddressUnit;
                if (im.CreateUser(au, oModel.Password))
                {
                    //Add Role
                    im.AddUserToRole(au.Id, "Seller");

                    Seller seller = new Seller() { UserId = au.Id, IsActive = true };
                    uow.SellerRepository.Insert(seller);
                    uow.Save();

                    return RedirectToAction("Index");
                }
                else
                {

                    ViewBag.Error = " خطایی رخ داد،کاربر ایجاد نشد.";

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Seller", "Create", false, 200, " نمایش صفحه ایجاد فروشنده " + au.FirstName + " " + au.LastName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();

                }

            }
            catch (Exception x)
            {

                ViewBag.Error = x.Message;

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Seller", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View();
            }
        }



        // GET: Admin/Seller/Edit/5
        public virtual ActionResult Edit(int id)
        {

            try
            {
                IdentityManager im = new IdentityManager();

                var seller = uow.SellerRepository.GetByID(id);
                if (ModulePermission.check(User.Identity.GetUserId(), 25, 2))
                {
                    ApplicationUser au = uow.UserRepository.Get(x => x, x => x.Id == seller.UserId, null, "CityEntity,Avatarattachment").SingleOrDefault();
                    if (au != null)
                    {
                      
                        EditUserViewModel eu = new EditUserViewModel();
                        eu.Id = seller.UserId;
                        eu.CityId = au.CityId;
                        eu.Disable = au.Disable;
                        eu.Email = au.Email;
                        eu.EmailConfirmed = au.EmailConfirmed;
                        eu.Mobile = au.PhoneNumber;
                        eu.PhoneNumberConfirmed = au.PhoneNumberConfirmed;
                        eu.Gender = au.Gender;
                        eu.FirstName = au.FirstName;
                        eu.LastName = au.LastName;
                        eu.About = au.About;
                        eu.LandlinePhone = au.LandlinePhone;
                        eu.PostalCode = au.PostalCode;
                        eu.AddressNumber = au.AddressNumber;
                        eu.AddressUnit = au.AddressUnit;
                        eu.State = 1;
                        eu.ProvienceId = au.CityEntity.ProvinceId;
                        eu.Address = au.Address;


                        ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", eu.ProvienceId);
                        ViewBag.CityList = new SelectList(uow.CityRepository.Get(x => x), "Id", "Name", eu.CityId);



                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Seller", "Edit", true, 200, " نمایش صفحه ویرایش فروشنده " + eu.FirstName + " " + eu.LastName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(eu);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت سفارشات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Seller", "edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }

        // POST: Admin/Seller/Edit/5
        [HttpPost]
        public virtual ActionResult Edit(EditUserViewModel oModel,string userid)
        {
            ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", oModel.ProvienceId);
            ViewBag.CityList = new SelectList(uow.CityRepository.Get(x => x), "Id", "Name", oModel.CityId);

            oModel.FirstName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(oModel.FirstName);
            oModel.LastName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(oModel.LastName);
            ApplicationUser au = uow.UserRepository.Get(u => u, u => u.Id == userid, null, "AdministratorPermissions").SingleOrDefault();

            try
            {
                if (uow.UserRepository.Get(x => x, x => x.PhoneNumber == oModel.Mobile && x.Id != userid).Any())
                {
                   
                    ViewBag.Error = " تلفن همراه وارد شده تکراری است.";
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
                au.Gender = oModel.Gender;
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Seller", "Edit", false, 200, "   ویرایش فروشنده " + au.FirstName + " " + au.LastName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {
                
                ViewBag.Error = x.Message;
               
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Seller", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(oModel);
            }
        }


        // GET: Admin/Tags/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 2, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Seller Seller = uow.SellerRepository.Get(x=>x,x=>x.Id==id,null, "ProductPrices,User").SingleOrDefault();
                    if (Seller == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Seller", "Delete", true, 200, " نمایش صفحه حذف فروشنده " + Seller.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Seller);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت سفارش ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Seller", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Seller Seller = uow.SellerRepository.GetByID(id);
                uow.SellerRepository.Delete(Seller);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Seller", "DeleteConfirmed", false, 200, "   حذف فروشنده " + Seller.Id, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Seller", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
