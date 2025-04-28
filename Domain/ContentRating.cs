using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ContentRating : Object
    {
        #region Ctor
        public ContentRating()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ContentRating>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Content).WithRequiredDependent(Current => Current.ContentRating).WillCascadeOnDelete(true);
            }
        }

        #endregion

        #region Properties

        [Key]
        public int ContentID { get; set; }

        public  Content Content { get; set; }

        public double Rating { get; set; }
        public int TotalRaters { get; set; }
        public double AverageRating { get; set; }

        #endregion
    }
}
