using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductRankGroupSelect : Object
    {
        #region Ctor
        public ProductRankGroupSelect()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductRankGroupSelect>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductRank).WithMany(Current => Current.ProductRankGroupSelects).HasForeignKey(Current => Current.RankId).WillCascadeOnDelete(true);
                HasOptional(Current => Current.ProductRankGroup).WithMany(Current => Current.ProductRankGroupSelects).HasForeignKey(Current => Current.GroupId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "آیتم امتیاز دهی")]
        [Index("IX_RankIdId", IsClustered = false, IsUnique = false)]
        public int RankId { get; set; }
        public  ProductRank ProductRank { get; set; }

        [Display(Name = "گروه دسته بندی")]
        [Index("IX_GroupId", IsClustered = false, IsUnique = false)]
        public int? GroupId { get; set; }
        public  ProductRankGroup ProductRankGroup { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش خصوصیت")]
        public int DisplayOrder { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش دسته")]
        public int DisplayGroupOrder { get; set; }

        public  ICollection<ProductRankSelect> ProductRankSelects { get; set; }
        
        #endregion

    }
}
