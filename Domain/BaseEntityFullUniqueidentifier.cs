using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class BaseEntityFullUniqueidentifier : System.Object
    {

        #region Ctor
        public BaseEntityFullUniqueidentifier()
        {
            Id = Guid.NewGuid();
            InsertDate = DateTime.Now.Date;
            IsActive = true;
            DisplaySort = 0;
        }
        public BaseEntityFullUniqueidentifier(bool isActive, int displaySort)
        {
            Id = Guid.NewGuid();
            InsertDate = DateTime.Now.Date;
            IsActive = true;
            DisplaySort = 0;
        }
        #endregion

        #region Properties
        [Key]
        [Required]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public System.Guid Id { get; set; }

        [Required]
        public DateTime InsertDate { get; set; }

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
