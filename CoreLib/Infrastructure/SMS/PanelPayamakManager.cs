namespace CoreLib.Infrastructure.SMS
{
    public class PanelPayamakManager
    {
        public static string SingleSms(string message, string mobile, string Username, string Password, string SmsSender)
        {
            //com.payamakpanel.api.SendSmsRequest s = new com.payamakpanel.api.SendSmsRequest();
            //return s.SendSimpleSMS2(Username, Password, mobile, SmsSender, message, false);
            return "";
        }
        public static string GroupSms(string message, string[] mobile, string Username, string Password, string SmsSender)
        {
            //for (int i = 0; i < mobile.Length; i++)
            //{
            //    com.payamakpanel.api. s = new com.payamakpanel.api.Send();
            //    s.SendSimpleSMS2(Username, Password, mobile[i], SmsSender, message, false);
            //}
            return "";
        }
    }
}
