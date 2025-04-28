using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Folder : Object
    {
        #region Ctor
        public Folder()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Folder>
        {
            public Configuration()
            {
                Property(current => current.FolderName).IsUnicode(true).HasMaxLength(50).IsVariableLength().IsRequired();
                Property(current => current.FolderID).IsOptional();
                HasOptional(Current => Current.ParrentFolder).WithMany(Current => Current.ChildFolder).HasForeignKey(Current => Current.FolderID);
                Property(current => current.LanguageId).IsOptional();

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "نام پوشه")]
        public string FolderName { get; set; }

        [Display(Name = "پوشه اصلی")]
        public int? FolderID { get; set; }
        public virtual Folder ParrentFolder { get; set; }

        [Display(Name = "زبان ( وب سایت )")]
        public Int16? LanguageId { get; set; }
        public virtual ICollection<Folder> ChildFolder { get; set; }
        public ICollection<attachment> attachments { get; set; }
        #endregion
    }
}
