using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductRankGroup
    {

        #region Ctor
        public ProductRankGroup()
        {

        }
        public ProductRankGroup( string title, Int16 DisplayOrder, Int16 LanguageId)
        {
            this.Title = title;
            this.DisplayOrder = DisplayOrder;
            this.LanguageId = LanguageId;

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductRankGroup>
        {
            public Configuration()
            {
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان گروه")]
        public string Title { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "ترتیب نمایش")]
        public Int16 DisplayOrder { get; set; }

        [Required]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16 LanguageId { get; set; }

        public  ICollection<ProductRankGroupSelect> ProductRankGroupSelects { get; set; }
        #endregion
    }
}
