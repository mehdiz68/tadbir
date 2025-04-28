using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Brand : Object
    {
        public Brand()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Brand>
        {
            public Configuration()
            {
                HasOptional(Current => Current.attachment).WithMany(Current => Current.Brands).HasForeignKey(Current => Current.AttachementId).WillCascadeOnDelete(false);

                HasOptional(Current => Current.attachmentHomePage).WithMany(Current => Current.Brand2s).HasForeignKey(Current => Current.CoverHomePage).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "نام باید وارد شود")]
        public string Name { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "عنوان باید وارد شود")]
        public string Title { get; set; }


        [Display(Name = "نام فارسی")]
        [Required(ErrorMessage = "نام فارسی باید وارد شود")]
        public string PersianName { get; set; }

        [Display(Name = "تصویر")]
        public Guid? AttachementId { get; set; }
        public  attachment attachment { get; set; }

        [Display(Name = "نمایش کاور(تصویر) صفحه اصلی")]
        public bool IsShowHomePage { get; set; }

        [Display(Name = "کاور(تصویر) صفحه اصلی")]
        public Guid? CoverHomePage { get; set; }
        public attachment attachmentHomePage { get; set; }

        [Required(ErrorMessage = "توضیحات متای گوگل باید وارد شود")]
        public string MeteDescription { get; set; }

        [Display(Name = "توضیحات")]
        public string Data { get; set; }

        [Display(Name = "زبان ( وب سایت )")]
        public Int16? LanguageId { get; set; }

        public  ICollection<Product> Products { get; set; }


        public  ICollection<FreeSendOffer> FreeSendOffers { get; set; }
        public  ICollection<ProductPriceGroupModification> ProductPriceGroupModifications { get; set; }

        #endregion
    }
}
