using UnitOfWork;
using System;
using System.Web;

namespace ahmadi.Infrastructure.EventLog
{
    public static class Logger
    {
        public static void Add(Int16 logType, string controllerName, string actionName, bool requestType, int statusCode, string description, System.DateTime logDateTime, string userid)
        {
            string ipAddress = "127.0.0.1";
            if (HttpContext.Current != null)
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            UnitOfWorkClass UOW = new UnitOfWorkClass();
            UOW.EventLogRepository.Insert(new Domain.EventLog() { IP= ipAddress, LogType = logType, ControllerName = controllerName, ActionName = actionName, RequestType = requestType, StatusCode = statusCode, Description = description, LogDateTime = logDateTime, UserId = userid });
            UOW.Save();
        }
    }
}
