using CoreLib.WebReference;
using System;
using NikSms.Library.Net.WebApi;
using System.Threading.Tasks;
using System.Linq;

namespace CoreLib.Infrastructure.SMS
{
    public static class NikSmsManager
    {
        #region Method

        public static string GetCredit(string Username, string Password)
        {
            try
            {
                var niksms = new NiksmsWebservice();
                niksms.Timeout = 30000;
                AuthenticationModel am = new AuthenticationModel();
                am.Username = Username;
                am.Password = Password;
                return string.Format("وضعیت اعتبار  : {0}", string.Format("{0:n0}", niksms.GetCredit(am)) + " ریال ");
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }
        public static string GetDiscountCredit(string Username, string Password)
        {
            try
            {
                var niksms = new NiksmsWebservice();
                niksms.Timeout = 30000;
                AuthenticationModel am = new AuthenticationModel();
                am.Username = Username;
                am.Password = Password;
                return string.Format("اعتبار تخفیفی شما  : {0 }", string.Format("{0:n0}", niksms.GetDiscountCredit(am)) + " ریال ");

            }
            catch (Exception x)
            {
                return x.Message;
            }

        }
        public static string GetPanelExpireDate(string Username, string Password)
        {
            try
            {
                var niksms = new NiksmsWebservice();
                niksms.Timeout = 30000;
                AuthenticationModel am = new AuthenticationModel();
                am.Username = Username;
                am.Password = Password;
                if (niksms.GetPanelExpireDate(am).HasValue)
                {
                    System.DateTime ExpireDate = niksms.GetPanelExpireDate(am).Value;
                    return string.Format(" تاریخ انقضای پنل  : {0}", CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(ExpireDate)) + " - " + ExpireDate.ToShortTimeString();
                }
                else
                    return " نامشخص ";
            }
            catch (Exception x)
            {
                return x.Message;
            }

        }
        public async static Task<string> GroupSms(string message, string[] mobiles, string Username, string Password, string SmsSender)
        {

            PublicApiV1 publicApiV1 = new PublicApiV1(Username, Password);
            await publicApiV1.GroupSms(SmsSender, mobiles.ToList(), message, System.DateTime.Now, 1, new System.Collections.Generic.List<string>() { "100001" });



            string ret = "";
            //var niksms = new NiksmsWebservice();
            //niksms.Timeout = 30000;
            //AuthenticationModel am = new AuthenticationModel();
            //am.Username = Username;
            //am.Password = Password;
            //var result = niksms.GroupSms(am, new GroupSmsModel()
            //{
            //    Message = message,
            //    SenderNumber = SmsSender,
            //    Numbers = mobiles,
            //    SendType = OperatorSmsSendType.Normal,
            //    SendOn = System.DateTime.Now
            //});
            //if (result.Status == SmsReturn.Successful)
            //{
            //    ret = string.Format(" پیام با موفقیت برای ارسال به مخاطبین ، فرستاده شد. کد پیگیری : {0} ", result.Id);

            //}
            //else
            //{
            //    ret = string.Format("ErrorCode : {0} {1} Error Description : {2}", result.Status, Environment.NewLine, SendSmsStatusHandling(result.Status));
            //}
            return "";
        }

        public async static Task<string> SingleSms(string message, string mobiles, string Username, string Password, string SmsSender)
        {
            PublicApiV1 publicApiV1 = new PublicApiV1(Username, Password);
            var a = await publicApiV1.PtpSms(SmsSender, new System.Collections.Generic.List<string>() { mobiles }, new System.Collections.Generic.List<string>() { message }, System.DateTime.Now, 1, new System.Collections.Generic.List<string>() { "100001" });



            //string ret = "";
            //var niksms = new NiksmsWebservice();
            //niksms.Timeout = 30000;
            //AuthenticationModel am = new AuthenticationModel();
            //am.Username = Username;
            //am.Password = Password;
            //niksms.PtpSmsAsync(am, new PtpSmsModel()
            //{
            //    Message = new string[] { message },
            //    SenderNumber = SmsSender,
            //    Numbers = new string[] { mobiles },
            //    SendType = OperatorSmsSendType.Normal,
            //    SendOn = System.DateTime.Now,
            //    YourMessageId = new long[] { 100001 },
            //});
            return "";
        }

        private static string SendSmsStatusHandling(SmsReturn smsReturn)
        {
            switch (smsReturn)
            {
                case SmsReturn.ArgumentIsNullOrIncorrect:
                    return "پارامترهای هایی که برای ارسال پیام خود به سیستم فرستاده اید، اشتباه است.";

                case SmsReturn.Filtered:
                    return "پیام شما از نظر متنی مشکلی داشته که باعث فیلتر شدن پنل شما شده است.";

                case SmsReturn.ForbiddenHours:
                    return "شما مجاز به ارسال در این ساعت نمی باشید";

                case SmsReturn.InsufficientCredit:
                    return "موجودی یا اعتبار شما برای انجام عملیات کافی نیست.";

                case SmsReturn.MessageBodyIsNullOrEmpty:
                    return "پیام ارسالی شما دارای متن نبوده است، متن پیام را باید حتما وارد نمایید.";

                case SmsReturn.NoFilters:
                    return "پیام شما از نظر متنی مشکلی داشته که باعث فیلتر شدن پیام شما شده است.";

                case SmsReturn.PanelIsBlocked:
                    return "پنل کاربری شما مسدود شده است و باید با پشتیبانی تماس بگیرید.";

                case SmsReturn.PrivateNumberIsDisable:
                    return "شماره اختصاصی که برای ارسال پیام خود انتخاب کرده اید، غیر فعال شده است.";

                case SmsReturn.PrivateNumberIsIncorrect:
                    return "شماره اختصاصی وارد شده اشتباه است و یا به شما تعلق ندارد.";

                case SmsReturn.ReceptionNumberIsIncorrect:
                    return "شماره موبایل های ارسالی اشتباه است.";

                case SmsReturn.SentTypeIsIncorrect:
                    return "نوع ارسالی که انتخاب کرده اید با محتوای ارسالی شما مطابقت نداشته و اشتباه است";

                case SmsReturn.Successful:
                    return "پیام شما با موفقیت ارسال شده است";
                case SmsReturn.SiteUpdating:
                    return "سایت در حال بروزرسانی می باشد لطفا دقایقی دیگر مجددا درخواست خود را ارسال نمایید";
                case SmsReturn.UnknownError:
                    return
                        "خطای نامشخصی رخ داده است که پیش بینی نشده بوده و باید با پشتیبانی فنی تماس بگیرید. (احتمال رخ دادن این خطا نزدیک به صفر بوده ولی جهت اطمینان، در مستندات ارائه می شود) ";
                case SmsReturn.Warning:
                    return "ارسال شما با موفقیت انجام شد ولی برای متن انتخابی شما هشداری به ثبت رسید";
                default:
                    return "وضعیت تعریف نشده لطفا به پشتیبانی فنی نیک اس ام اس اطلاع دهید";
            }
        }

        static bool ResetReceiveSmsVisitedStatus(string Username, string Password)
        {
            var niksms = new NiksmsWebservice();
            niksms.Timeout = 30000;
            AuthenticationModel am = new AuthenticationModel();
            am.Username = Username;
            am.Password = Password;
            return niksms.ResetReceiveSmsVisitedStatus(am, null, null);

        }

        #endregion
    }
}
