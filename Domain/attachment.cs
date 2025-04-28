using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class attachment : BaseEntityFullUniqueidentifier
    {
        #region Ctor
        public attachment()
        {
        }
        public attachment(string title, string fileName, bool hasMultiSize, bool hasWarermark, int capacity, int useCount, int fileTypeId)
        {
            this.Title = title;
            this.FileName = fileName;
            this.HasMultiSize = hasMultiSize;
            this.HasWatermark = hasWarermark;
            this.Capacity = capacity;
            this.UseCount = useCount;
            this.FileTypeId = fileTypeId;
        }
        public attachment(string title, string fileName, bool hasMultiSize, bool hasWarermark, int capacity, int useCount, int fileTypeId, int folderId)
        {
            this.Title = title;
            this.FileName = fileName;
            this.HasMultiSize = hasMultiSize;
            this.HasWatermark = hasWarermark;
            this.Capacity = capacity;
            this.UseCount = useCount;
            this.FileTypeId = fileTypeId;
            this.FolderId = folderId;
        }

        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<attachment>
        {
            public Configuration()
            {
                //Property(current => current.Title).IsUnicode(true).HasMaxLength(50).IsVariableLength().IsRequired();
                //Property(current => current.FileName).IsUnicode(true).HasMaxLength(255).IsVariableLength().IsRequired();
                Property(current => current.HasMultiSize).IsRequired();
                Property(current => current.HasWatermark).IsRequired();
                Property(current => current.Capacity).IsRequired();
                Property(current => current.UseCount).IsRequired();
                Property(current => current.FileTypeId).IsRequired();
                HasRequired(Current => Current.FileType).WithMany(Current => Current.attachments).HasForeignKey(Current => Current.FileTypeId);
                Property(current => current.FolderId).IsOptional();
                HasOptional(Current => Current.Folder).WithMany(Current => Current.attachments).HasForeignKey(Current => Current.FolderId);
                Property(current => current.LanguageId).IsOptional();
                Property(current => current.UserId).IsOptional();
                HasOptional(Current => Current.User).WithMany(Current => Current.Attachements).HasForeignKey(Current => Current.UserId);
            }
        }

        #endregion

        #region Properties
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نام فایل")]
        public string Title { get; set; }

        [Display(Name = "فایل فایل")]
        public string FileName { get; set; }



        public bool HasMultiSize { get; set; }

        public bool HasWatermark { get; set; }

        public int Capacity { get; set; }

        public int UseCount { get; set; }

        public int FileTypeId { get; set; }

        public  FileType FileType { get; set; }

        public int? FolderId { get; set; }
        public  Folder Folder { get; set; }
        public Int16? LanguageId { get; set; }

        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }


        public  ICollection<Setting> Settings { get; set; }
        public  ICollection<Setting> WaterMarkSettings { get; set; }
        public  ICollection<Setting> FaviconSettings { get; set; }
        public ICollection<Setting> FactorSettings { get; set; }

        public  ICollection<Category> Categories { get; set; }
        public ICollection<StaticTextCategory> StaticTextCategories { get; set; }
        public ICollection<StaticTextContent> StaticTextContents { get; set; }

        public  ICollection<Offer> Offers { get; set; }
        public  ICollection<Content> Contents { get; set; }
        public  ICollection<Content> Content2s { get; set; }
        public  ICollection<Content> Content3s { get; set; }
        public  ICollection<Product> Videos { get; set; }
        public  ICollection<Product> Catalogs { get; set; }

        public  ICollection<OtherImage> OtherImages { get; set; }

        public  ICollection<SliderImage> SliderImages { get; set; }

        public  ICollection<Social> Socials { get; set; }

        public  ICollection<SearchEngineFact> SearchEngineFacts { get; set; }

        public  ICollection<Menu> Menus { get; set; }
        public ICollection<Menu> Menu2s { get; set; }

        public  ICollection<ProductCategory> ProductCategories { get; set; }
        public  ICollection<ProductCategory> ProductCategorie2s { get; set; }
        public  ICollection<ProductCategory> ProductCategorie3s { get; set; }



        public  ICollection<GalleryCategory> GalleryCategories { get; set; }

        public  ICollection<GalleryImage> GalleryImages { get; set; }

        public  ICollection<ProductImage> ProductImages { get; set; }

        public  ICollection<Brand> Brands { get; set; }
        public  ICollection<Brand> Brand2s { get; set; }

        public  ICollection<ProductFileItem> ProductFileItems { get; set; }

        public  ICollection<ProductState> ProductStates { get; set; }

        public  ICollection<ProductIcon> ProductIcons { get; set; }

        public  ICollection<ProductSendWay> ProductSendWays { get; set; }

        public  ICollection<ProductCourseLesson> ProductCourseLessons { get; set; }
        public  ICollection<Adveresting> Adverestings { get; set; }
        public  ICollection<labelIcon> labelIcons { get; set; }
        public  ICollection<ProductComment> ProductComments { get; set; }
        public  ICollection<ProductQuestion> ProductQuestions { get; set; }
        public ICollection<ApplicationUser> Avatars { get; set; }
        #endregion
    }
}
