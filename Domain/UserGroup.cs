using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UserGroup : Object
    {
        #region Ctor
        public UserGroup()
        {

        }
        #endregion

      

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "نام گروه")]
        public string Name { get; set; }

        [Display(Name = "توضیحات")]
        public string Descr { get; set; }


        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "تاریخ آخرین بروزرسانی")]
        public DateTime UpdateDate { get; set; }

        public int DisplayOrder { get; set; }

        public ICollection<UserGroupSelect> UserGroupSelects { get; set; }
        public ICollection<OfferUserGroup> offerUserGroups { get; set; }

        #endregion
    }
}
