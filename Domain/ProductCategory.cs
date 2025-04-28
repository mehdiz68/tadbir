using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class ProductCategory : Object
    {
        #region Ctor
        public ProductCategory()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductCategory>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductType).WithMany(Current => Current.ProductCategories).HasForeignKey(Current => Current.ProductTypeId);
                HasOptional(Current => Current.ParentCat).WithMany(Current => Current.ChildCategory).HasForeignKey(Current => Current.ParrentId);
                HasOptional(Current => Current.attachment).WithMany(Current => Current.ProductCategories).HasForeignKey(Current => Current.Cover).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Menuattachment).WithMany(Current => Current.ProductCategorie2s).HasForeignKey(Current => Current.MenuCover).WillCascadeOnDelete(false);
                HasOptional(Current => Current.attachmentHomePage).WithMany(Current => Current.ProductCategorie3s).HasForeignKey(Current => Current.CoverHomePage).WillCascadeOnDelete(false);

                HasMany(u => u.Products).WithMany(m => m.ProductCategories).Map(m =>
                {
                    m.ToTable("ProductCategorySelect");
                    m.MapLeftKey("CatId");  // because it is the "left" column, isn't it?
                    m.MapRightKey("ProductId"); // because it is the "right" column, isn't it?
                });
                //HasMany(u => u.ProductAttributes).WithMany(m => m.ProductCategories).Map(m =>
                //{
                //    m.ToTable("ProductAttributeCategory");
                //    m.MapLeftKey("CatId");  // because it is the "left" column, isn't it?
                //    m.MapRightKey("AttId"); // because it is the "right" column, isn't it?
                //});

            }
        }
        #endregion

        #region Properties
        [Key]
        [Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نام")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Name { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان سئو")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 80")]
        public string Title { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان لینک (URL)")]
        [MaxLength(80, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string PageAddress { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان سئو برای صفحه جست و جو")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 80")]
        public string Title2 { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان لینک (URL) برای صفحه جستجو")]
        [MaxLength(80, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string PageAddress2 { get; set; }


        [Display(Name = "دسته بندی اصلی")]
        public int? ParrentId { get; set; }
        public virtual ProductCategory ParentCat { get; set; }
        public  virtual ICollection<ProductCategory> ChildCategory { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "توضیحات متای گوگل")]
        [MaxLength(150, ErrorMessage = "حداکثر طول کارکتر ، 150")]
        public string Descr { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "توضیحات متای گوگل برای صفحه جستجو")]
        [MaxLength(150, ErrorMessage = "حداکثر طول کارکتر ، 150")]
        public string Descr2 { get; set; }

        //[Required(ErrorMessage = "اجباری")]
        [Display(Name = "چکیده")]
        public string Abstract { get; set; }

        [Display(Name = "محتوا(متن،عکس،فایل و ... )")]
        public string Data { get; set; }

        [Display(Name = "زبان ( وب سایت )")]
        public Int16? LanguageId { get; set; }

        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }
        public virtual attachment attachment { get; set; }

        [Display(Name = "کاور برای منو (تصویر)")]
        public Guid? MenuCover { get; set; }
        public attachment Menuattachment { get; set; }



        [Display(Name = "نمایش کاور(تصویر) صفحه اصلی")]
        public bool IsShowHomePage { get; set; }

        [Display(Name = "کاور(تصویر) صفحه اصلی")]
        public Guid? CoverHomePage { get; set; }
        public attachment attachmentHomePage { get; set; }

        [Display(Name = "نوع گروه محصول")]
        public int ProductTypeId{ get; set; }
        public ProductType ProductType { get; set; }

        [Display(Name = "درصد کمیسیون( در صورت نیاز)")]
        public double? Commission { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int Sort { get; set; }
        
        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        public string OldName { get; set; }

        public  ICollection<Product> Products { get; set; }

        public  ICollection<ProductCategoryAttribute> ProductCategoryAttributes { get; set; }

        public  ICollection<FreeSendOffer> FreeSendOffers { get; set; }

        public  ICollection<Tag> Tags { get; set; }

        public  ICollection<ProductPriceGroupModification> ProductPriceGroupModifications { get; set; }
        public  ICollection<ProductAttributeGroupProductCategory> ProductAttributeGroupProductCategorys { get; set; }
        public ICollection<ProductRandomSetting> ProductRandomSettings { get; set; }
        public ICollection<OfferProductCategory> offerProductCategories { get; set; }

        #endregion
    }
}
