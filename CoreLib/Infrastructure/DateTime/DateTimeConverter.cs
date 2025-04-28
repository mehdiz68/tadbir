using System;

namespace CoreLib.Infrastructure.DateTime
{
    public static class DateTimeConverter
    {
        public static System.DateTime ChangeShamsiToMiladiDateTime(string Shamsi)
        {
            System.DateTime miladi = default(System.DateTime);
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            miladi = pc.ToDateTime(Convert.ToInt32(Shamsi.Substring(0, 4)), Convert.ToInt32(Shamsi.Substring(5, 2)), Convert.ToInt32(Shamsi.Substring(8, 2)), Convert.ToInt32(Shamsi.Substring(11, 2)), Convert.ToInt32(Shamsi.Substring(14, 2)), 0, 0, System.Globalization.Calendar.CurrentEra);
            return miladi;
        }

        public static System.DateTime ChangeShamsiToMiladi(string Shamsi)
        {
            System.DateTime miladi = default(System.DateTime);
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            miladi = pc.ToDateTime(Convert.ToInt32(Shamsi.Substring(0, 4)), Convert.ToInt32(Shamsi.Substring(5, 2)), Convert.ToInt32(Shamsi.Substring(8, 2)), 10, 10, 10, 10, System.Globalization.Calendar.CurrentEra);
            miladi = miladi.Date;
            return miladi;
        }
        public static string ChangeMiladiToShamsi(System.DateTime Miladi)
        {
            string Shamsi = null;
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            string Year = null;
            string Month = null;
            string Day = null;

            Year = PC.GetYear(Miladi).ToString();
            Month = PC.GetMonth(Miladi).ToString();
            if (Month.Length < 2)
                Month = "0" + Month;
            Day = PC.GetDayOfMonth(Miladi).ToString();
            if (Day.Length < 2)
                Day = "0" + Day;

            Shamsi = Year + "/" + Month + "/" + Day;

            return Shamsi;
        }
        public static string ChangeMiladiToShamsiTime(System.DateTime Miladi)
        {
            string Shamsi = null;
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            string Year = null;
            string Month = null;
            string Day = null;

            Year = PC.GetHour(Miladi).ToString();
            Month = PC.GetMinute(Miladi).ToString();
            if (Month.Length < 2)
                Month = "0" + Month;
            Day = PC.GetSecond(Miladi).ToString();
            if (Day.Length < 2)
                Day = "0" + Day;

            Shamsi = Year + ":" + Month + ":" + Day;

            return Shamsi;
        }
        public static string ChangeMiladiToLongShamsi(System.DateTime Miladi)
        {
            string Shamsi = null;
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            string Year = null;
            string Month = null;
            string Day = null;
            var daysofweek = new string[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنج شنبه", "جمعه", "شنبه" };

            Year = PC.GetYear(Miladi).ToString();
            Month = PC.GetMonth(Miladi).ToString();
            if (Month.Length < 2)
                Month = "0" + Month;
            Day = PC.GetDayOfMonth(Miladi).ToString();
            if (Day.Length < 2)
                Day = "0" + Day;
            string m = "";
            switch (Month)
            {
                case "01": m = "فروردین"; break;
                case "02": m = "اردیبشت"; break;
                case "03": m = "خرداد"; break;
                case "04": m = "تیر"; break;
                case "05": m = "مرداد"; break;
                case "06": m = "شهریور"; break;
                case "07": m = "مهر"; break;
                case "08": m = "آبان"; break;
                case "09": m = "آذر"; break;
                case "10": m = "دی"; break;
                case "11": m = "بهمن"; break;
                case "12": m = "اسفند"; break;
            }
            Shamsi = daysofweek[(int)PC.GetDayOfWeek(Miladi)] + " " + Day + " " + m + " " + Year;

            return Shamsi;
        }

        public static string ChangeMiladiToLongShamsiWithoutYear(System.DateTime Miladi)
        {
            string Shamsi = null;
            System.Globalization.PersianCalendar PC = new System.Globalization.PersianCalendar();
            string Year = null;
            string Month = null;
            string Day = null;
            var daysofweek = new string[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنج شنبه", "جمعه", "شنبه" };

            Year = PC.GetYear(Miladi).ToString();
            Month = PC.GetMonth(Miladi).ToString();
            if (Month.Length < 2)
                Month = "0" + Month;
            Day = PC.GetDayOfMonth(Miladi).ToString();
            if (Day.Length < 2)
                Day = "0" + Day;
            string m = "";
            switch (Month)
            {
                case "01": m = "فروردین"; break;
                case "02": m = "اردیبشت"; break;
                case "03": m = "خرداد"; break;
                case "04": m = "تیر"; break;
                case "05": m = "مرداد"; break;
                case "06": m = "شهریور"; break;
                case "07": m = "مهر"; break;
                case "08": m = "آبان"; break;
                case "09": m = "آذر"; break;
                case "10": m = "دی"; break;
                case "11": m = "بهمن"; break;
                case "12": m = "اسفند"; break;
            }
            Shamsi = daysofweek[(int)PC.GetDayOfWeek(Miladi)] + " " + Day + " " + m ;

            return Shamsi;
        }

        public static string GetDayOfWeek(int day)
        {
            switch (day)
            {
                case 0: return "شنبه";
                case 1: return "یکشنبه"; 
                case 2: return "دوشنبه"; 
                case 3: return "سه شنبه"; 
                case 4: return "چهارشنبه"; 
                case 5: return "پنجشنبه"; 
                case 6: return "جمعه";
                default: return "نامشخص";
            }
        }
    }
}
