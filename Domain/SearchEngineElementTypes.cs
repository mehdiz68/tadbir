using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class SearchEngineElementType : Object
    {
        #region Ctor
        public SearchEngineElementType()
        {

        }
        public SearchEngineElementType(int id,string title,string description,Int16 priority,Int16 languageid)
        {
            SearchEngineElementTypeID = id;
            Title = title;
            Description = description;
            Priority = priority;
            LanguageId = languageid;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SearchEngineElementType>
        {
            public Configuration()
            {
                Property(current => current.SearchEngineElementTypeID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SearchEngineElementTypeID { get; set; }


        [Required]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "توضیحات متا")]
        public string Description { get; set; }


        [Required]
        [Display(Name = "اولویت")]
        public Int16 Priority { get; set; }


        [Required]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16 LanguageId { get; set; }


        public  ICollection<SearchEngineFact> SearchEngineFacts { get; set; }
        #endregion
    }
}
