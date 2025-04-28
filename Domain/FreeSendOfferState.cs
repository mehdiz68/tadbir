using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class FreeSendOfferState : Object
    {
        #region Ctor
        public FreeSendOfferState()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<FreeSendOfferState>
        {
            public Configuration()
            {
                HasRequired(Current => Current.FreeSendOffer).WithMany(Current => Current.FreeSendOfferStates).HasForeignKey(Current => Current.FreeSendOfferId).WillCascadeOnDelete(true);

                HasRequired(Current => Current.City).WithMany(Current => Current.FreeSendOfferStates).HasForeignKey(Current => Current.CityId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }


        [Required]
        [Display(Name = "تخفیف")]
        public int FreeSendOfferId { get; set; }
        public  FreeSendOffer FreeSendOffer { get; set; }

        public  City City { get; set; }
        [Display(Name = "شهر")]
        public int CityId { get; set; }


        #endregion
    }
}
