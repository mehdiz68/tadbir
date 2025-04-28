using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib.Infrastructure
{
    public static class CommonFunctions
    {
        #region Common

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string ToHourClock(double seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);

            //here backslash is must to tell that colon is
            //not the part of format, it just a character that we want in output
            return time.ToString(@"hh\:mm\:ss");
        }

        public static int GetSizeColumn(int SizeId)
        {
            switch (SizeId)
            {
                case 1: return 12;
                case 2: return 10;
                case 3: return 8;
                case 4: return 6;
                case 5: return 4;
                case 6: return 3;
                case 7: return 2;
                default: return 12;
            }
        }

        public static string base64Decode(string sData) //Decode    
        {
            try
            {
                var encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecodeByte = Convert.FromBase64String(sData);
                int charCount = utf8Decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
                char[] decodedChar = new char[charCount];
                utf8Decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
                string result = new String(decodedChar);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Decode" + ex.Message);
            }
        }

        public static string GetOrderCode(long bankOrderId)
        {
            Random r = new Random();
            return bankOrderId.ToString() + r.Next(1000, 9999).ToString();
        }
        public static string CorrectArabianLetter(string Name)
        {
            //remove extra space
            string[] characters = Name.Split(null);
            string temp = "";
            foreach (var item in characters)
            {
                if (!String.IsNullOrEmpty(item))
                    temp += item + " ";
            }
            Name = temp;


            char ve = (char)1654;
            char correctve = (char)1608;
            Name = Name.Replace(ve, correctve);
            ve = (char)1655;
            Name = Name.Replace(ve, correctve);

            char ie = (char)1610;
            char correctie = (char)1740;
            Name = Name.Replace(ie, correctie);
            ie = (char)1609;
            Name = Name.Replace(ie, correctie);

            char ke = (char)1603;
            char correctke = (char)1705;
            Name = Name.Replace(ke, correctke);
            return Name.Trim();
        }
        public static string CorrectArabianLetterOnly(string Name)
        {
            //remove extra space    

            char ve = (char)1654;
            char correctve = (char)1608;
            Name = Name.Replace(ve, correctve);
            ve = (char)1655;
            Name = Name.Replace(ve, correctve);

            char ie = (char)1610;
            char correctie = (char)1740;
            Name = Name.Replace(ie, correctie);
            ie = (char)1609;
            Name = Name.Replace(ie, correctie);

            char ke = (char)1603;
            char correctke = (char)1705;
            Name = Name.Replace(ke, correctke);
            return Name.Trim();
        }
        public static string NormalizeAddress(string address)
        {

            if (!string.IsNullOrEmpty(address))
            {
                address = address.Replace("-", "");
                address = address.Replace("®", "");
                address = address.Replace("™", "");
                address = address.Replace("'", "");
                address = address.Replace("&", "");
                address = address.Replace("(", "");
                address = address.Replace(")", "");
                address = address.Replace("_", "");
                address = address.Replace("%", "");
                address = address.Replace("|", "");
                address = address.Replace(":", "");
                address = address.Replace("*", "");
                address = address.Replace("?", "");
                address = address.Replace("?", "");
                address = address.Replace("!", "");
                address = address.Replace("/", "");
                address = address.Replace("\\", "");
                address = address.Replace("http", "");
                address = address.Replace(".", "-");
                address = address.Replace("#", "");
                address = address.Replace(">", "");
                address = address.Replace("<", "");
                address = address.Replace("+", "");
                address = address.Replace(" ", "-");
                address = address.Replace("\r", "-");
                address = address.Replace("\n", "-");
                address = address.Replace("\"", "");

                while (address.Contains("--"))
                {
                    address = address.Replace("--", "-");
                }

            }
            else
                address = "";

            return address.Trim();
        }


        public static string NormalizeAddressWithSpace(string address)
        {

            if (!string.IsNullOrEmpty(address))
            {
                address = address.Replace("-", " ");
                address = address.Replace("®", " ");
                address = address.Replace("™", " ");
                address = address.Replace("'", " ");
                address = address.Replace("&", " ");
                address = address.Replace("(", " ");
                address = address.Replace(")", " ");
                address = address.Replace("_", " ");
                address = address.Replace("%", " ");
                address = address.Replace("|", " ");
                address = address.Replace(":", " ");
                address = address.Replace("*", " ");
                address = address.Replace("?", " ");
                address = address.Replace("?", " ");
                address = address.Replace("!", " ");
                address = address.Replace("/", " ");
                address = address.Replace("\\", " ");
                address = address.Replace("http", " ");
                address = address.Replace(".", " ");
                address = address.Replace("#", " ");
                address = address.Replace(">", " ");
                address = address.Replace("<", " ");
                address = address.Replace("+", " ");
                address = address.Replace("\r", " ");
                address = address.Replace("\n", " ");
                address = address.Replace("\"", " ");

                while (address.Contains("  "))
                {
                    address = address.Replace("  ", " ");
                }

            }
            else
                address = "";

            return address.Trim();
        }
        public static string GetLanguageName(int LanguageId)
        {
            switch (LanguageId)
            {
                case 1: return "فارسی";
                case 2: return "English";
                case 3: return "العربیه";
                case 4: return "Русский";
                case 5: return "Español";
                case 6: return "Français";
                case 7: return "Deutsch";
                case 8: return "Português";
                case 9: return "Italiano";
                case 10: return "हिंदी";
                case 11: return "한국어";
                case 12: return "日本語";
                case 13: return "Türk";
                case 14: return "Nederlands";
                default: return "سایر زبان ها";
            }
        }
        public static int Getdatatype(int datatype)
        {
            if (datatype == 1 || datatype == 12 || datatype == 26 || datatype == 28 || datatype == 34 || datatype == 35 || datatype == 44 || datatype == 48)
                return 1;
            else if (datatype == 2 || datatype == 43 || datatype == 47 || datatype == 49)
                return 2;
            else if (datatype == 4 || datatype == 31)
                return 4;
            else if (datatype == 5 || datatype == 33 || datatype == 37 || datatype == 38 || datatype == 50)
                return 5;
            else if (datatype == 6 || datatype == 39 || datatype == 40)
                return 6;
            else if (datatype == 7 || datatype == 18 || datatype == 21 || datatype == 25 || datatype == 29 || datatype == 30 || datatype == 42 || datatype == 46)
                return 7;
            else if (datatype == 8 || datatype == 19 || datatype == 24 || datatype == 27 || datatype == 32 || datatype == 52)
                return 8;
            else if (datatype == 36 || datatype == 9)
                return 9;
            else if (datatype == 10)
                return 10;
            else if (datatype == 11)
                return 11;

            else
                return 3;
        }
        public static string GetCastString(int datatype)
        {
            if (datatype == 1)
                return "TRY_CAST(ProductAttributeSelects.Value AS int)";
            else if (datatype == 2)
                return "TRY_CAST(ProductAttributeSelects.Value AS float)";
            else if (datatype == 5)
                return "TRY_CAST(ProductAttributeSelects.Value AS bit)";
            else if (datatype == 6)
                return "TRY_CAST(ProductAttributeSelects.Value AS datetime)";
            else
                return " ProductAttributeSelects.Value";

        }
        public static string GetCastCheckString(int datatype)
        {
            if (datatype == 1)
                return "IsNumeric(case WHEN  Len(value) > 20 then SUBSTRING(value,0,20) else value end)=1 ";
            else if (datatype == 2)
                return "IsNumeric(case WHEN  Len(value) > 20 then SUBSTRING(value,0,20) else value end)=1 ";
            else if (datatype == 5)
                return "IsNumeric(case WHEN  Len(value) > 20 then SUBSTRING(value,0,20) else value end)=1 ";
            else if (datatype == 6)
                return "IsNumeric(case WHEN  Len(value) > 20 then SUBSTRING(value,0,20) else value end)=1 ";
            else
                return "1=1 ";

        }
        public static string GetCurrency(Int16 currency)
        {
            switch (currency)
            {
                case 1: return "تومان";
                case 2: return "ریال";
                case 3: return "یورو";
                case 4: return "دلار";
                case 5: return "درهم";
                case 6: return "پوند";
                case 7: return "ین";
                case 8: return "دلار استرالیا";
                case 9: return "فرانک سوئیس";
                case 10: return "دلار کانادا";
                case 11: return " دلار نیوزیلند";
                case 12: return " یوآن";
                case 13: return " کرون سوئد";
                case 14: return " وون کره جنوبی";
                case 15: return " روپیه هند";
                default: return " تومان ";
            }
        }
        public static int RoundPrice(int price)
        {

            return ((int)Math.Round(price / 10.0)) * 10;
        }
        #endregion

        #region Xml
        public static string GetStateName(int StateId, string StaticUrl)
        {
            CoreLib.ViewModel.Xml.XMLReader xr = new ViewModel.Xml.XMLReader(StaticUrl);
            var data = xr.DetailOfXState(StateId);
            if (data != null)
                return data.Name;
            else
                return "وجود ندارد";
        }
        public static string GetContentTypeName(int ContentTypeId, string StaticUrl)
        {
            CoreLib.ViewModel.Xml.XMLReader xr = new ViewModel.Xml.XMLReader(StaticUrl);
            var data = xr.DetailOfXContentType(ContentTypeId);
            if (data != null)
                return data.Name;
            else
                return "وجود ندارد";
        }
        public static string GetContentTypeShortName(int ContentTypeId, string StaticUrl)
        {
            CoreLib.ViewModel.Xml.XMLReader xr = new ViewModel.Xml.XMLReader(StaticUrl);
            var data = xr.DetailOfXContentType(ContentTypeId);
            if (data != null)
                return data.ShortName;
            else
                return "وجود ندارد";
        }
        public static string GetInternalPageName(int InteralPageId, string StaticUrl)
        {
            CoreLib.ViewModel.Xml.XMLReader xr = new ViewModel.Xml.XMLReader(StaticUrl);
            if (InteralPageId > 0)
            {
                var contenttype = xr.DetailOfXContentType(InteralPageId);
                if (contenttype != null)
                    return contenttype.Name;
                else
                    return "وجود ندارد";
            }
            else
            {
                switch (InteralPageId)
                {
                    case -1: return "دسته بندی";
                    case -2: return "محتوا";
                    case -3: return "برچسبِ انواع محتوا";
                    case -4: return "ادامه جستجو";
                    case -5: return "برچسب";
                    case -6: return "محصول";
                    case -7: return "دسته بندی محصول";
                    case -8: return "لیست محصول";
                    case -9: return "سبد خرید";
                    case -10: return "گالری";
                    case -11: return "دسته بندی گالری";
                    case -12: return "برند";
                    default: return "وجود ندارد";
                }
            }
        }
        public static double CalculatePostPishtazFee(double weight, int UserStateId, string UserCity, double Tax, string StaticUrl)
        {
            CoreLib.ViewModel.Xml.XMLReader xr = new ViewModel.Xml.XMLReader(StaticUrl);
            double PostFee = 0;
            var PostPrice = xr.ListOfXPostPrice().First();
            //Get Current And it`s Neighbor
            int CurrentShoppingStateId = xr.ListOfXBikeDelivery().First().CurrentState;
            string CurrentShoppingCity = xr.ListOfXCurrentCity().First().Name;
            IEnumerable<int> Neighbors = xr.ListOfXCurrentInnerState().Select(x => x.Id);

            //Check PostFee
            //درون شهری
            if (UserStateId == CurrentShoppingStateId && UserCity.Trim().Equals(CurrentShoppingCity))
            {
                if (weight <= 250)
                    PostFee = Convert.ToDouble(PostPrice.Post_Inner_City_250);
                else if (weight <= 500)
                    PostFee = Convert.ToDouble(PostPrice.Post_Inner_City_500);
                else if (weight <= 1000)
                    PostFee = Convert.ToDouble(PostPrice.Post_Inner_City_1000);
                else if (weight <= 2000)
                    PostFee = Convert.ToDouble(PostPrice.Post_Inner_City_2000);
                else if (weight > 2000)
                {
                    int w = Convert.ToInt32(Math.Ceiling(weight / 1000));
                    PostFee = Convert.ToInt32(PostPrice.Post_Inner_City_2000) + (Convert.ToDouble(PostPrice.Post_Inner_City_More_2000) * (w - 2));
                    PostFee = Convert.ToDouble(PostFee + (PostFee * Tax));
                }
            }
            //درون استانی
            else if (UserStateId == CurrentShoppingStateId && !UserCity.Trim().Equals(CurrentShoppingCity))
            {
                if (weight <= 250)
                    PostFee = Convert.ToDouble(PostPrice.Post_Inner_State_250);
                else if (weight <= 500)
                    PostFee = Convert.ToDouble(PostPrice.Post_Inner_State_500);
                else if (weight <= 1000)
                    PostFee = Convert.ToDouble(PostPrice.Post_Inner_State_1000);
                else if (weight <= 2000)
                    PostFee = Convert.ToDouble(PostPrice.Post_Inner_State_2000);
                else if (weight > 2000)
                {
                    int w = Convert.ToInt32(Math.Ceiling(weight / 1000));
                    PostFee = Convert.ToInt32(PostPrice.Post_Inner_State_2000) + (Convert.ToDouble(PostPrice.Post_Inner_State_More_2000) * (w - 2));
                    PostFee = Convert.ToDouble(PostFee + (PostFee * Tax));
                }
            }
            //استان همجوار
            else if (Neighbors.Contains(CurrentShoppingStateId))
            {
                if (weight <= 250)
                    PostFee = Convert.ToDouble(PostPrice.Post_Outer_State_Neighbor_250);
                else if (weight <= 500)
                    PostFee = Convert.ToDouble(PostPrice.Post_Outer_State_Neighbor_500);
                else if (weight <= 1000)
                    PostFee = Convert.ToDouble(PostPrice.Post_Outer_State_Neighbor_1000);
                else if (weight <= 2000)
                    PostFee = Convert.ToDouble(PostPrice.Post_Outer_State_Neighbor_2000);
                else if (weight > 2000)
                {
                    int w = Convert.ToInt32(Math.Ceiling(weight / 1000));
                    PostFee = Convert.ToInt32(PostPrice.Post_Outer_State_Neighbor_2000) + (Convert.ToDouble(PostPrice.Post_Outer_State_Neighbor_More_2000) * (w - 2));
                    PostFee = Convert.ToDouble(PostFee + (PostFee * Tax));
                }
            }
            else
            {
                if (weight <= 250)
                    PostFee = Convert.ToDouble(PostPrice.Post_Outer_State_NoNeighbor_250);
                else if (weight <= 500)
                    PostFee = Convert.ToDouble(PostPrice.Post_Outer_State_NoNeighbor_500);
                else if (weight <= 1000)
                    PostFee = Convert.ToDouble(PostPrice.Post_Outer_State_NoNeighbor_1000);
                else if (weight <= 2000)
                    PostFee = Convert.ToDouble(PostPrice.Post_Outer_State_NoNeighbor_2000);
                else if (weight > 2000)
                {
                    int w = Convert.ToInt32(Math.Ceiling(weight / 1000));
                    PostFee = Convert.ToInt32(PostPrice.Post_Outer_State_NoNeighbor_2000) + (Convert.ToDouble(PostPrice.Post_Outer_State_NoNeighbor_More_2000) * (w - 2));
                    PostFee = Convert.ToDouble(PostFee + (PostFee * Tax));
                }
            }


            return PostFee;
        }
        public static string GetSlideShowTypePartialViewName(Int16 SlideShowType)
        {
            switch (SlideShowType)
            {
                case 1: return "_BootStrapSlideShow";
                case 2: return "_PgwSlideShow";
                case 3: return "_DjSlideShow";
                case 4: return "_SliceboxSlideShow";
                case 5: return "_SliceboxSlideShow2";
                case 6: return "_SliceboxSlideShow3";
                case 7: return "_SliceboxSlideShow4";
                case 8: return "_BootStrapSlideShowCaption";
                default: return "_BootStrapSlideShow";
            }
        }
        public static string GetItemSliderTypePartialViewName(Int16 SlideShowType)
        {
            switch (SlideShowType)
            {
                case 1: return "_olwItemSlider";
                case 2: return "_olwOverItemSlider";
                case 3: return "_LightSliderItemSlider";
                case 4: return "_LightSliderImageItemSlider";
                default: return "_olwItemSlider";
            }
        }
        //public static string GetBankName(int BankId)
        //{
        //    CoreLib.ViewModel.Xml.XMLReader xr = new ViewModel.Xml.XMLReader();
        //    var data = xr.DetailOfXBank(BankId);
        //    if (data != null)
        //        return data.Name;
        //    else
        //        return "وجود ندارد";
        //}
        #endregion
    }
}
