using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class FileType : Object
    {
        #region Ctor
        public FileType()
        {

        }
        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "وارد کردن پسوند، اجباری است")]
        [Display(Name = " پسوند( مثلِ jpg.)")]
        [StringLength(10)] 
        [Index("IX_FileTypeName", IsClustered = false, IsUnique = true)]  
        public string FileTypeName { get; set; }
        public  ICollection<attachment> attachments { get; set; }
        #endregion
    }
}
