using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using System.Xml.Linq;

namespace Domain
{
    public class Product : BaseEntityFullAutoId
    {
        #region Ctor
        public Product()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Product>
        {
            public Configuration()
            {
                //Property(current => current.UserId).IsOptional();
                HasOptional(Current => Current.VideoAttachment).WithMany(Current => Current.Videos).HasForeignKey(Current => Current.Video);
                HasOptional(Current => Current.CatalogAttachment).WithMany(Current => Current.Catalogs).HasForeignKey(Current => Current.Catalog);
                HasRequired(Current => Current.User).WithMany(Current => Current.Products).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Brand).WithMany(Current => Current.Products).HasForeignKey(Current => Current.BrandId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.ProductType).WithMany(Current => Current.Products).HasForeignKey(Current => Current.ProductTypeId).WillCascadeOnDelete(false);
                //HasRequired(Current => Current.ProductState).WithMany(Current => Current.Products).HasForeignKey(Current => Current.ProductStateId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductIcon).WithMany(Current => Current.Products).HasForeignKey(Current => Current.ProductIconId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Tax).WithMany(Current => Current.Products).HasForeignKey(Current => Current.TaxId).WillCascadeOnDelete(false);

                HasMany(u => u.Tags).WithMany(m => m.Product).Map(m =>
                {
                    m.ToTable("ProductTags");
                    m.MapLeftKey("ProductId");  // because it is the "left" column, isn't it?
                    m.MapRightKey("TagId"); // because it is the "right" column, isn't it?
                });
            }
        }
        #endregion

        #region Properties


        [Required(ErrorMessage = "نام کالا باید وارد شود")]
        [Display(Name = "نام کالا")]
        public string Name { get; set; }

        [Display(Name = "نام لاتین کالا")]
        public string LatinName { get; set; }

        [Required(ErrorMessage = "عنوان کالا باید وارد شود")]
        [Display(Name = "عنوان کالا")]
        public string Title { get; set; }

        [Display(Name = "آدرس صفحه کالا")]
        public string PageAddress { get; set; }

        [Required(ErrorMessage = "توضیحات متای گوگل باید وارد شود")]
        [Display(Name = "توضیحات متای گوگل")]
        [MaxLength(160, ErrorMessage = "حداکثر طول کارکتر ، 160")]
        public string Descr { get; set; }

        [Required(ErrorMessage = "معرفی اجمالی کالا باید وارد شود")]
        [Display(Name = "معرفی اجمالی کالا")]
        public string Abstract { get; set; }

        [Display(Name = "مشخصات کلی")]
        public string Data { get; set; }


        [Display(Name = "مشخصات فنی")]
        public string Data2 { get; set; }

        [Display(Name = "برند")]
        public int? BrandId { get; set; }
        public  Brand Brand { get; set; }

        [Display(Name = "نوع محصول")]
        public int ProductTypeId { get; set; }
        public  ProductType ProductType { get; set; }

        [Display(Name = "کد کالا")]
        public string Code { get; set; }
        
        [Display(Name = "کد اسکرچ")]
        public string ScratchCode { get; set; }

        //[Display(Name = "وضعیت موجودی محصول")]
        //public int ProductStateId { get; set; }
        //public  ProductState ProductState { get; set; }

        [Display(Name = "وضعیت محصول")]
        public int? ProductIconId { get; set; }
        public  ProductIcon ProductIcon { get; set; }

        [Required(ErrorMessage = "ارتفاع بسته باید وارد شود")]
        [Display(Name = "ارتفاع بسته (سانتی نتر)")]
        public int Height { get; set; }

        [Required(ErrorMessage = "عرض بسته باید وارد شود")]
        [Display(Name = "عرض بسته (سانتی متر)")]
        public int Width { get; set; }

        [Required(ErrorMessage = "طول بسته باید وارد شود")]
        [Display(Name = "طول بسته (سانتی متر)")]
        public int Lenght { get; set; }

        [Required(ErrorMessage = "وزن بسته باید وارد شود")]
        [Display(Name = "وزن بسته (گرم)")]
        public int ProductWeight { get; set; }
        
        [Display(Name = "‎هزینه اضافی حمل (برای یک قلم کالا به تومان)")]
        public int ExtraSendPrice { get; set; }

        [Display(Name = "مالیات")]
        public int? TaxId { get; set; }
        public  Tax Tax { get; set; }

        [Display(Name = "‎بن تخفیف")]
        public int? bon { get; set; }

        [Display(Name = "‎حداکثر استفاده از بن تخفیف در سبد")]
        public int? bonMaxBasket { get; set; }

        /*
         * 1- در حال بررسی
         * 2- بررسی مجدد
         * 3- عدم تایید
         * 4- تایید
         */
        [Required]
        [Display(Name = "وضعیت")]
        public Int16 state { get; set; }

        [Display(Name = "زبان ( وب سایت )")]
        public Int16? LanguageId { get; set; }

        [Required]
        [Display(Name = "تعداد بازدید")]
        public int Visits { get; set; }

        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        public int FavCount { get; set; }
        public int SellCount { get; set; }

        [Column(TypeName = "xml")]
        public string LogPrice { get; set; }


        [Display(Name = "ویدئو")]
        public Guid? Video { get; set; }
        public  attachment VideoAttachment { get; set; }

        [Display(Name= "عدم ارسال رایگان در مرسوله")]
        public bool CancelFreeSend { get; set; }


        [Display(Name = "کاتالوگ")]
        public Guid? Catalog { get; set; }
        public  attachment CatalogAttachment { get; set; }

        public virtual  ICollection<ProductCategory> ProductCategories { get; set; }

        public  ICollection<ProductQuestion> ProductQuestions { get; set; }

        public  ICollection<ProductComment> ProductComments { get; set; }

        public  ICollection<ProductRankSelect> ProductRankSelects { get; set; }

        public  ICollection<ProductAttributeSelect> ProductAttributeSelects { get; set; }

        public  ICollection<ProductLetmeknow> ProductLetmeknows { get; set; }

        public  ICollection<ProductFavorate> ProductFavorates { get; set; }

        public  ICollection<OrderRow> OrderRows { get; set; }

        public  ICollection<ProductAdvantage> ProductAdvantages { get; set; }

        public  ICollection<ProductDisAdvantage> ProductDisAdvantages { get; set; }

        public  ICollection<ProductImage> ProductImages { get; set; }

        public  ICollection<ProductSeller> ProductSellers { get; set; }

        public  ICollection<ProductMultipeItem> ProductMultipeMainItems { get; set; }

        public  ICollection<ProductMultipeItem> ProductMultipeSelectItems { get; set; }

        public  ICollection<ProductFileInfo> ProductFileInfos { get; set; }

        public  ICollection<ProductCourse> ProductCourses { get; set; }

        public  ICollection<Tag> Tags { get; set; }

        public  ICollection<ProductAccessory> ProductMainAccessory { get; set; }

        //public  ICollection<ProductAccessory> ProductAccessory { get; set; }

        public ICollection<ProductSendWaySelect> ProductSendWaySelects { get; set; }

        public  ICollection<ProductPrice> ProductPrices { get; set; }

        public ICollection<FreeSendOffer> FreeSendOffers { get; set; }

        public  ICollection<CourseRating> CourseRatings { get; set; }

      
        #endregion
    }
}
