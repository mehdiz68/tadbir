using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class BaseEntityFullAutoId : System.Object
    {

        #region Ctor
        public BaseEntityFullAutoId()
        {
            InsertDate = DateTime.Now.Date;
            IsActive = true;
            DisplaySort = 0;
        }
        public BaseEntityFullAutoId(bool isActive,int displaySort)
        {
            InsertDate = DateTime.Now.Date;
            this.IsActive = isActive;
            this.DisplaySort = displaySort;
        }
        #endregion

        #region Properties
        [Key]
        [Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "تاریخ ویرایش")]
        public DateTime? UpdateDate { get; set; }

        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }


        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        #endregion
    }

}
