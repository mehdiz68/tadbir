using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class TicketCategory : Object
    {
        #region Ctor
        public TicketCategory()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<TicketCategory>
        {
            public Configuration()
            {
                Property(current => current.Title).IsUnicode(true).HasMaxLength(100).IsVariableLength().IsRequired();
                Property(current => current.Descr).IsUnicode(true).HasMaxLength(150).IsVariableLength().IsRequired();

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }



        [Required(ErrorMessage = "عنوان سئو باید وارد شود")]
        [Display(Name = "عنوان")]
        [MaxLength(80, ErrorMessage = "حداکثر طول کارکتر ، 80")]
        public string PageAddress { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "توضیحات متای گوگل")]
        [MaxLength(150, ErrorMessage = "حداکثر طول کارکتر ، 150")]
        public string Descr { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "چکیده")]
        public string Abstract { get; set; }

        [Display(Name = "محتوا(متن،عکس،فایل و ... )")]
        public string Data { get; set; }

        [Display(Name = "زبان ( وب سایت )")]
        public Int16? LanguageId { get; set; }


        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int Sort { get; set; }


        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }


        public  ICollection<Ticket> Tickets { get; set; }
        #endregion
    }
}
