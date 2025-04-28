using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
   public class ProductAttribute
    {
        #region Ctor
        public ProductAttribute()
        {

        }
        public ProductAttribute(string name, Int16 datatype, string unit, bool priceeffect, Int16 languageid)
        {
            this.Name = name;
            this.DataType = datatype;
            this.Unit = unit;
            this.PriceEffect = priceeffect;
            this.LanguageId = languageid;
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نام خصوصیت")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        [Index("IX_AttributeName", IsClustered = false, IsUnique = true)]
        public string Name { get; set; }



        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان جهت نمایش در سایت")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }




        /*
         1-number
         2-Decimal
         3-text
         4-html
         5-boolean
         6-DateTime
         7-File
         8-Db List
         9-Content List
         10-Product List
         11-Color List
         12-Product Color
         13-Product Size
         14-Product Model
         15-Product Garanty
         16-Product weight
         17-Bon
         18-Max Bon
             */
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نوع داده")]
        public Int16 DataType { get; set; }

        [Display(Name = "واحد")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Unit { get; set; }

        [Display(Name = "تنوع پذیر")]
        public bool PriceEffect { get; set; }

        [Display(Name = "زبان ( وب سایت )")]
        public int LanguageId { get; set; }

        [Display(Name = "چند مقداری")]
        public bool HasMultipleValue { get; set; }

        [Display(Name = "مقدار پیش فرض")]
        public string DefaultValue { get; set; }



        public  IList<ProductAttributeItem> ProductAttributeItems { get; set; }

        public  IList<ProductAttributeItemColor> ProductAttributeItemColors { get; set; }

        public  ICollection<ProductAttributeGroupSelect> ProductAttributeGroupSelects { get; set; }


        public  ICollection<ProductCategoryAttribute> ProductCategoryAttributes { get; set; }

        #endregion

    }
}
