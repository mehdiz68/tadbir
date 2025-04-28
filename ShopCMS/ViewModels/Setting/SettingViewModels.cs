using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain;

namespace ahmadi.ViewModels.Setting
{
    [NotMapped]
    public class SettingViewModels
    {
        public SettingViewModels()
        {

        }
        public SettingViewModels(Domain.Setting setting)
        {
            this.Id = setting.Id;
            this.SettingName = setting.SettingName;
            this.WebSiteName = setting.WebSiteName;
            this.WebSiteTitle = setting.WebSiteTitle;
            this.WebSiteMetaDescription = setting.WebSiteMetaDescription;
            this.WebSiteMetakeyword = setting.WebSiteMetakeyword;
            if (setting.Logo.HasValue)
            {
                this.Logo = setting.Logo.Value;
                this.attachment = setting.attachment;
            }
            if (setting.FactorLogo.HasValue)
            {
                this.FactorLogo = setting.FactorLogo.Value;
                this.FactorAttachment = setting.FactorAttachment;
            }
            if (setting.WaterMark.HasValue)
            {
                this.Watermark = setting.WaterMark.Value;
                this.Watermarkattachment = setting.Waterattachment;
            }
            this.StaticContentDomain = setting.StaticContentDomain;
            this.LargeSizeWidth = setting.LargeSizeWidth;
            this.LargeSizeHeight = setting.LargeSizeHeight;
            this.MediumSizeWidth = setting.MediumSizeWidth;
            this.MediumSizeHeight = setting.MediumSizeHeight;
            this.SmallSizeWidth = setting.SmallSizeWidth;
            this.SmallSizeHeight = setting.SmallSizeHeight;
            this.XsmallSizeWidth = setting.XsmallSizeWidth;
            this.XsmallSizeHeight = setting.XsmallSizeHeight;
            this.WebmasterVerification = setting.WebmasterVerification;
            this.AnalyticsVerification = setting.AnalyticsVerification;
            this.DefaultCurrency = setting.DefaultCurrency;
            this.CurrencyConvertionRate = setting.CurrencyConvertionRate;
            this.TaxRate = setting.TaxRate;
            this.LanguageId = setting.LanguageId;
            this.BonPrice = setting.BonPrice;
            this.BonExpireDay = setting.BonExpireDay;
            this.PopUpMessage = setting.PopUpMessage;
            this.PopUpActive = setting.PopUpActive;
            this.PopUpType = setting.PopUpType;
            this.PopUpEditVersion = setting.PopUpEditVersion;
            this.Faviconattachment = setting.Faviconattachment;
            this.Favicon = setting.Favicon;
            this.HelpActiveShowInDefault = setting.HelpActiveShowInDefault;
            this.DisplayRootMenu = setting.DisplayRootMenu;
            this.HasHttps = setting.HasHttps;
            this.yektanet = setting.yektanet;
            this.WaterMarkPosition = setting.WaterMarkPosition;
            this.LargeImageWaremark = setting.LargeImageWaremark;
            this.ShoppingEstelamMinutes = setting.ShoppingEstelamMinutes;
            this.ShoppingPayEstelamMinutes = setting.ShoppingPayEstelamMinutes;
            this.Address = setting.Address;
            this.Address2 = setting.Address2;
            this.Tele = setting.Tele;
            this.Tele2 = setting.Tele2;
            this.Tele3 = setting.Tele3;
            this.Mobile = setting.Mobile;
            this.PostalCode = setting.PostalCode;
            this.TaxNumber = setting.TaxNumber;
            this.FooterGoogleMapLatitude = setting.FooterGoogleMapLatitude;
            this.FooterGoogleMapLongitude = setting.FooterGoogleMapLongitude;
            this.FooterGoogleMapZoom = setting.FooterGoogleMapZoom;
            this.Email = setting.Email;

        }
        public int Id { get; set; }
        [Required(ErrorMessage = "وارد کردن  نام تنظیم ، اجباری است")]
        [Display(Name = "نام تنظیم")]
        [MaxLength(50, ErrorMessage = "حداکثر طول کارکتر ، 50")]
        public string SettingName { get; set; }

        [Required(ErrorMessage = "وارد کردن  نام وب سایت ، اجباری است")]
        [Display(Name = "نام وب سایت")]
        [MaxLength(50, ErrorMessage = "حداکثر طول کارکتر ، 50")]
        public string WebSiteName { get; set; }

        [Required(ErrorMessage = "وارد کردن  عنوان وب سایت، اجباری است")]
        [Display(Name = "عنوان اصلی وب سایت")]
        [MaxLength(64, ErrorMessage = "حداکثر طول کارکتر ، 64")]
        public string WebSiteTitle { get; set; }

        [Required(ErrorMessage = "وارد کردن متای اصلی توضیحات، اجباری است")]
        [Display(Name = "متای اصلی توضیحات وب سایت")]
        [MaxLength(155, ErrorMessage = "حداکثر طول کارکتر ، 155")]
        public string WebSiteMetaDescription { get; set; }

        [Required(ErrorMessage = "وارد کردن متای اصلی کلمات کلیدی، اجباری است")]
        [Display(Name = "متای اصلی کلمات کلیدی وب سایت")]
        [MaxLength(48, ErrorMessage = "حداکثر طول کارکتر ، 48")]
        public string WebSiteMetakeyword { get; set; }

        /*0-پایین چپ
         * 1- پایین مرکز
         * ...
         */
        [Display(Name = "مکان پیش فرض واترمارک")]
        public int WaterMarkPosition { get; set; }


        [Display(Name = "اعمال واترمارک فقط برای عکس اصلی")]
        public bool LargeImageWaremark { get; set; }

        [Required(ErrorMessage = "وارد کردن دامنه ی بدون کوکی، اجباری است")]
        [Display(Name = "دامنه ی بدون کوکی")]
        public string StaticContentDomain { get; set; }

        [Required(ErrorMessage = "وارد کردن لوگوی وب سایت، اجباری است")]
        [Display(Name = "لوگوی وب سایت")]
        public Guid? Logo { get; set; }
        public attachment attachment { get; set; }


        [Display(Name = "لوگو برای فاکتورها")]
        public Guid? FactorLogo {
        [Display(Name = "لوگو برای فاکتورها")] get; set; }
        public attachment FactorAttachment { get; set; }


        [Required(ErrorMessage = "وارد کردن واترمارک وب سایت، اجباری است")]
        [Display(Name = "واترمارک وب سایت")]
        public Guid? Watermark { get; set; }
        public attachment Watermarkattachment { get; set; }



        [Required(ErrorMessage = "وارد کردن آیکن هدر سایت، اجباری است")]
        [Display(Name = "آیکن هدر سایت")]
        public Guid? Favicon { get; set; }
        public attachment Faviconattachment { get; set; }

        [Required(ErrorMessage = "وارد کردن عرض تصاویر بزرگ، اجباری است")]
        [Display(Name = "عرض تصاویر بزرگ")]
        public int LargeSizeWidth { get; set; }

        [Required(ErrorMessage = "وارد کردن طول تصاویر بزرگ، اجباری است")]
        [Display(Name = "طول تصاویر بزرگ")]
        public int LargeSizeHeight { get; set; }

        [Required(ErrorMessage = "وارد کردن عرض تصاویر متوسط، اجباری است")]
        [Display(Name = "عرض تصاویر متوسط")]
        public int MediumSizeWidth { get; set; }

        [Required(ErrorMessage = "وارد کردن طول تصاویر متوسط، اجباری است")]
        [Display(Name = "طول تصاویر متوسط")]
        public int MediumSizeHeight { get; set; }

        [Required(ErrorMessage = "وارد کردن عرض تصاویر کوچک، اجباری است")]
        [Display(Name = "عرض تصاویر کوچک")]
        public int SmallSizeWidth { get; set; }

        [Required(ErrorMessage = "وارد کردن طول تصاویر کوچک، اجباری است")]
        [Display(Name = "طول تصاویر کوچک")]
        public int SmallSizeHeight { get; set; }

        [Required(ErrorMessage = "وارد کردن عرض تصاویر خیلی کوچک، اجباری است")]
        [Display(Name = "عرض تصاویر خیلی کوچک")]
        public int XsmallSizeWidth { get; set; }

        [Required(ErrorMessage = "وارد کردن طول تصاویر خیلی کوچک، اجباری است")]
        [Display(Name = "طول تصاویر خیلی کوچک")]
        public int XsmallSizeHeight { get; set; }

        [Display(Name = "کدِ ثبت وب سایت در وب مستر تولز ")]
        public string WebmasterVerification { get; set; }


        [Display(Name = "کدِ ثبت وب سایت در وب سایت های آماری")]
        public string AnalyticsVerification { get; set; }

        public Int16? LanguageId { get; set; }

        /*
        1- IRT تومان
        2- IRR ریال
        3- EUR یورو
        4- USD دلار
        5- AED درهم
        6- GBP پوند
        7- JPY ین
        8- AUD دلار استرالیا
        9- CHF فرانک سوئیس
        10- CAD دلار کانادا
        11- NZD دلار نیوزیلند
        12- CNY یوآن
        13- SEK کرون سوئد
        14- KRW وون کره جنوبی
        15- INR روپیه هند
            */
        [Display(Name = "ارز انتخابی سایت")]
        [Required(ErrorMessage = "اجباری")]
        public Int16 DefaultCurrency { get; set; }

        [Display(Name = "نرخ تبدیل ارز ")]
        [Required(ErrorMessage = "اجباری")]
        public double CurrencyConvertionRate { get; set; }


        [Display(Name = "نرخ مالیات بر ارزش افزوده")]
        [Required(ErrorMessage = "اجباری")]
        public double TaxRate { get; set; }



        [Display(Name = "ارزش عددی بن تخفیف")]
        [Required(ErrorMessage = "اجباری")]
        public int BonPrice { get; set; }


        [Display(Name = "تعداد روز برای انقضای بن تخفیف")]
        [Required(ErrorMessage = "اجباری")]
        public int BonExpireDay { get; set; }

        [Display(Name = "فعال سازی پنجره پاپ آپ صفحه اصلی")]
        [Required(ErrorMessage = "اجباری")]
        public bool PopUpActive { get; set; }

        /*
         1- content
         2- only image
             */
        [Display(Name = "نوع پاپ آپ")]
        [Required(ErrorMessage = "اجباری")]
        public bool PopUpType { get; set; }

        [Display(Name = "پنجره پاپ آپ صفحه اصلی")]
        [Required(ErrorMessage = "اجباری")]
        public string PopUpMessage { get; set; }

        [Required(ErrorMessage = "اجباری")]
        public int PopUpEditVersion { get; set; }



        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "فعال سازی راهنما در صفحه اصلی ادمین")]
        public bool HelpActiveShowInDefault { get; set; }


        [Required(ErrorMessage = "گروه نمایش منوی محصولات اجباری ست")]
        [Display(Name = "گروه نمایش منوی محصولات")]
        public bool DisplayRootMenu { get; set; }


        [Display(Name = "فعال سازی https( ابتدا باید پروتکل ست شود )")]
        public bool HasHttps { get; set; }


        [Display(Name = "فعال سازی کمپین ریتارگتینگ یکتانت")]
        public bool yektanet { get; set; }

        [Display(Name = "زمان اعلام قیمت سفارش استعلامی(دقیقه)")]
        public int ShoppingEstelamMinutes { get; set; }

        [Display(Name = "مهلت پرداخت سفارش استعلامی(دقیقه)")]
        public int ShoppingPayEstelamMinutes { get; set; }
        public IList<EmailSender> EmailSenders { get; set; }


        [Display(Name = "آدرس")]
        public string Address { get; set; }
        [Display(Name = "2 آدرس")]
        public string Address2 { get; set; }

        [Display(Name = "تلفن")]
        public string Tele { get; set; }

        [Display(Name = "2 تلفن")]
        public string Tele2 { get; set; }

        [Display(Name = "تلفن3 ")]
        public string Tele3 { get; set; }

        [Display(Name = "موبایل")]
        public string Mobile { get; set; }

        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "شماره اقتصادی")]
        public string TaxNumber { get; set; }

        [Display(Name = "عرض جغرافیایی")]
        public string FooterGoogleMapLongitude { get; set; }

        [Display(Name = "طول جغرافیایی")]
        public string FooterGoogleMapLatitude { get; set; }

        [Display(Name = "زوم نقشه")]
        public string FooterGoogleMapZoom { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }
    }
}
