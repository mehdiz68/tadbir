using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class AdverestingLog : Object
    {
        #region Ctor
        public AdverestingLog()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<AdverestingLog>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Adveresting).WithMany(Current => Current.AdverestingLogs).HasForeignKey(Current => Current.AdId);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }



        [Display(Name = "ClientIP")]
        public string ClientIP { get; set; }

        [Display(Name = "Browser")]
        public string Browser { get; set; }

        [Display(Name = "UserAgent")]
        public string UserAgent { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }
        
        [Required]
        [Display(Name = "تبلیغ")]
        public int AdId { get; set; }
        public  Adveresting Adveresting { get; set; }


        #endregion
    }
}
