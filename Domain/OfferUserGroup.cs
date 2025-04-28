using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class OfferUserGroup : Object
    {
        #region Ctor
        public OfferUserGroup()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<OfferUserGroup>
        {
            public Configuration()
            {
                HasRequired(Current => Current.offer).WithMany(Current => Current.offerUserGroups).HasForeignKey(Current => Current.OfferId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.UserGroup).WithMany(Current => Current.offerUserGroups).HasForeignKey(Current => Current.UserGroupId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }


        [Display(Name = "تخفیف")]
        public int OfferId { get; set; }
        public  Offer offer{ get; set; }


        [Display(Name = "گروه")]
        public int UserGroupId { get; set; }
        public UserGroup UserGroup { get; set; }

        #endregion
    }
}
