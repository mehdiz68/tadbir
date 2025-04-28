using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UserGroupSelect : Object
    {
        #region Ctor
        public UserGroupSelect()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserGroupSelect>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.UserGroupSelects).HasForeignKey(Current => Current.UserId);
                HasRequired(Current => Current.userGroup).WithMany(Current => Current.UserGroupSelects).HasForeignKey(Current => Current.userGroupId);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


        [Display(Name = "گروه کاربری")]
        public int userGroupId { get; set; }
        public UserGroup userGroup { get; set; }


        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }

        #endregion
    }
}
