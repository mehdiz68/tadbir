using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace Domain
{
    public class Setting
    {
        #region Ctor
        public Setting()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Setting>
        {
            public Configuration()
            {
                Property(current => current.SettingName).IsUnicode(true).HasMaxLength(50).IsVariableLength().IsRequired();
                Property(current => current.WebSiteName).IsUnicode(true).HasMaxLength(50).IsVariableLength().IsRequired();
                Property(current => current.WebSiteTitle).IsUnicode(true).HasMaxLength(64).IsVariableLength().IsRequired();
                Property(current => current.WebSiteMetaDescription).IsUnicode(true).HasMaxLength(155).IsVariableLength().IsRequired();
                Property(current => current.WebSiteMetakeyword).IsUnicode(true).HasMaxLength(48).IsVariableLength().IsRequired();
                Property(current => current.Logo).IsOptional();
                HasOptional(Current => Current.attachment).WithMany(Current => Current.Settings).HasForeignKey(Current => Current.Logo).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Waterattachment).WithMany(Current => Current.WaterMarkSettings).HasForeignKey(Current => Current.WaterMark).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Faviconattachment).WithMany(Current => Current.FaviconSettings).HasForeignKey(Current => Current.Favicon).WillCascadeOnDelete(false);
                HasOptional(Current => Current.FactorAttachment).WithMany(Current => Current.FactorSettings).HasForeignKey(Current => Current.FactorLogo).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Province).WithMany(Current => Current.Settings).HasForeignKey(Current => Current.ProvinceId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.City).WithMany(Current => Current.Settings).HasForeignKey(Current => Current.CityId).WillCascadeOnDelete(false);
                Property(current => current.LargeSizeWidth).IsRequired();
                Property(current => current.LargeSizeHeight).IsRequired();
                Property(current => current.MediumSizeWidth).IsRequired();
                Property(current => current.MediumSizeHeight).IsRequired();
                Property(current => current.SmallSizeWidth).IsRequired();
                Property(current => current.SmallSizeHeight).IsRequired();
                Property(current => current.XsmallSizeWidth).IsRequired();
                Property(current => current.XsmallSizeHeight).IsRequired();
                Property(current => current.LanguageId).IsOptional();

            }
        }
        #endregion

        #region Properties
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

        [Display(Name = "دامنه ی بدون کوکی")]
        public string StaticContentDomain { get; set; }

        [Display(Name = "لوگوی وب سایت")]
        public Guid? Logo { get; set; }
        public attachment attachment { get; set; }


        [Display(Name = "واترمارک وب سایت")]
        public Guid? WaterMark { get; set; }
        public attachment Waterattachment { get; set; }


        [Display(Name = "لوگو برای فاکتورها")]
        public Guid? FactorLogo { get; set; }
        public attachment FactorAttachment { get; set; }

        /*0-پایین چپ
         * 1- پایین مرکز
         * ...
         */
        [Display(Name = "مکان پیش فرض واترمارک")]
        public int WaterMarkPosition { get; set; }

        [Display(Name = "اعمال واترمارک فقط برای عکس اصلی")]
        public bool LargeImageWaremark { get; set; }


        [Display(Name = "آیکن هدر سایت")]
        public Guid? Favicon { get; set; }
        public attachment Faviconattachment { get; set; }

        [Required(ErrorMessage = "وارد کردن عرض تصاویر بزرگ، اجباری است")]
        [Display(Name = "عرض تصاویر بزرگ (پیکسل)")]
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


        [Display(Name = "کدِ ثبت وب سایت در وب سایت های آماری ( اسکریپت )")]
        public string AnalyticsVerification { get; set; }

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

        [Display(Name = "محتوای پنجره پاپ آپ صفحه اصلی")]
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

        [Required]
        [Display(Name = "زبان ( وب سایت)")]
        [Index("IX_LanguageId_Setting", IsClustered = false, IsUnique = true)]
        public Int16? LanguageId { get; set; }



        [Display(Name = "استان کنونی فروشگاه")]
        public int? ProvinceId { get; set; }
        public Province Province { get; set; }



        [Display(Name = "شهر کنونی فروشگاه")]
        public int? CityId { get; set; }
        public City City { get; set; }

        [Display(Name = "آدرس")]
        public string Address { get; set; }

        [Display(Name = "آدرس 2")]
        public string Address2 { get; set; }

        [Display(Name = "تلفن")]
        public string Tele { get; set; }

        [Display(Name = "تلفن 2")]
        public string Tele2 { get; set; }
        [Display(Name = "تلفن 3")]
        public string Tele3 { get; set; }

        [Display(Name = "موبایل")]
        public string Mobile { get; set; }

        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "شماره اقتصادی")]
        public string TaxNumber { get; set; }

        [Display(Name = "عرض جغرافیایی")]
        public string FooterGoogleMapLongitude { get; set; }

        [Display(Name = "طول جغرافیایی")]
        public string FooterGoogleMapLatitude { get; set; }

        [Display(Name = "زوم نقشه")]
        public string FooterGoogleMapZoom { get; set; }


        public IList<EmailSender> EmailSenders { get; set; }

        public IList<SmsSender> SmsSenders { get; set; }
        public ICollection<ShoppingWorkTime> ShoppingWorkTimes { get; set; }
        public ICollection<SettingState> settingStates { get; set; }
        public ICollection<ProductRandomSetting> ProductRandomSettings { get; set; }


        #endregion

    }
}
