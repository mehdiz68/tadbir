using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ahmadi.Controllers;

namespace ahmadi.Areas.Admin.Controllers
{
    public class UserMessagesController : BaseController
    {
        private UnitOfWork.UnitOfWorkClass uow = null;
        public UserMessagesController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }
        // GET: Admin/UserMessages
        public ActionResult Index(int? page)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            #endregion

            try
            {

                string userid = User.Identity.GetUserId();
                var UserMessages = uow.UserMessageRepository.GetByReturnQueryable(x => x, x => x.UserIdTo == userid || x.UserIdTo == null || x.UserId == "");

                int pageSize = 10;
                int pageNumber = (page ?? 1);


                return View(UserMessages.ToPagedList(pageNumber, pageSize));

            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error");
            }
        }
    }
}