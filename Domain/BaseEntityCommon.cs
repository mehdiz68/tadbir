using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class BaseEntityCommon : System.Object
    {

        #region Ctor
        public BaseEntityCommon()
        {
            IsActive = true;
            DisplaySort = 0;
        }
        public BaseEntityCommon(bool isActive,  int displaySort)
        {
            this.IsActive = isActive;
            this.DisplaySort = displaySort;

        }
        #endregion

       
        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }


        [Required(ErrorMessage = "ترتیب استفاده، اجباری است")]
        [Display(Name = "ترتیب استفاده")]
        public int DisplaySort { get; set; }

        #endregion
    }

}
