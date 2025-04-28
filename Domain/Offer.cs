using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Offer : Object
    {
        #region Ctor
        public Offer()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Offer>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.Offers).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.attachment).WithMany(Current => Current.Offers).HasForeignKey(Current => Current.Cover).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "عنوان")]
        public string Title { get; set; }


        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }
        public  attachment attachment { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "تاریخ و زمان شروع")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "تاریخ و زمان انقضا")]
        public DateTime? ExpireDate { get; set; }



        /*
         * 1- شگفت انگیز
         * 2- ویژه
         * 0- ارسال رایگان
         * 4- کد تخفیف
         * 5- کد تخفیف عمومی
         */
        [Display(Name = "نوع تخفیف")]
        public Int16? CodeTypeValueCode { get; set; }

        [Display(Name = "تعداد روز برای انقضای پیش فرض")]
        public short? DefaultDayExpire { get; set; }

        [Display(Name = "کد تخفیف پیش فرض")]
        public string DefalutCode { get; set; }

        [Required]
        [Display(Name = "ارزش کد تخفیف پیش فرض")]
        public int DeflautValue { get; set; }


        /*
         * 1- ثابت
         * 2- درصدی
         */
        [Required]
        [Display(Name = "نوع ارزش کد تخفیف پیش فرض")]
        public Int16 DefaultCodeType { get; set; }


        [Display(Name = "سقف مبلغ استفاده از کد تخفیف در هر سفارش پیش فرض")]
        public int DefalutMaxValue { get; set; }

        [Display(Name = "سقف تعداد استفاده از کد تخفیف پیش فرض")]
        public int DefalutCountUse { get; set; }

        [Display(Name = "نوع ثبت کد تخفیف")]
        public CodeUseType codeUseType { get; set; }


        [Required]
        [Display(Name = "وضعیت")]
        public bool state { get; set; }


        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }


        [Required]
        [Display(Name = "نمایش در صفحه تخفیف ها")]
        public bool ShowinSuerdeal { get; set; }


        [Display(Name = "نویسنده")]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }


        [Display(Name = "زبان ( وب سایت )")]
        public Int16 LanguageId { get; set; }

        public bool IsDeleted { get; set; }

        public int Sort { get; set; }
        public  ICollection<UserCodeGift> UserCodeGifts { get; set; }
        public  ICollection<ProductOffer> ProductOffers { get; set; }
        public  ICollection<FreeSendOffer> FreeSendOffers { get; set; }
        public  ICollection<Order> Orders { get; set; }
        public ICollection<OfferProductCategory> offerProductCategories { get; set; }
        public ICollection<OfferUserGroup> offerUserGroups { get; set; }
        public ICollection<GeneralCodeGift> generalCodeGifts { get; set; }
        public ICollection<UserOfferMessage> userOfferMessages { get; set; }
        #endregion
    }

    public enum CodeUseType
    {

        [Display(Name = "ثبت در پنل مدیریت")]
        ثبت_در_پنل_مدیریت,
        [Display(Name = "ثبت پس از ثبت نظر محصول")]
        ثبت_پس_از_ثبت_نظر_محصول,
        [Display(Name = "ثبت پس از ثبت نظر سفارش")]
        ثبت_پس_از_ثبت_نظر_سفارش,
    }
}
