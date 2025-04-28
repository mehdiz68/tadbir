using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductAttributeTab
    {
        #region Ctor
        public ProductAttributeTab()
        {

        }
        #endregion

        #region Properties
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان تب")]
        public string Name { get; set; }

        [Display(Name = "آیکن")]
        public string Icon { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "ترتیب نمایش")]
        public Int16 DisplayOrder { get; set; }

        [Required]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16 LanguageId { get; set; }


        public  ICollection<ProductAttributeGroupSelect> ProductAttributeGroupSelects { get; set; }

        #endregion
    }
}
