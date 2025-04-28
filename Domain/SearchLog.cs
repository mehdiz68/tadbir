using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class SearchLog : Object
    {
        #region Ctor
        public SearchLog()
        {

        }
        #endregion


        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SearchLog>
        {
            public Configuration()
            {
               
                HasOptional(Current => Current.User).WithMany(Current => Current.SearchLogs).HasForeignKey(Current => Current.UserId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        public string keyword { get; set; }

        public DateTime insertDate { get; set; }

        public string ClientIP { get; set; }

        public string Browser { get; set; }

        public string UserAgent { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        #endregion
    }
}
