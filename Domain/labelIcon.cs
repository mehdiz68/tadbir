using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class labelIcon
    {
        public labelIcon()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<labelIcon>
        {
            public Configuration()
            {
                HasRequired(Current => Current.attachment).WithMany(Current => Current.labelIcons).HasForeignKey(Current => Current.Cover).WillCascadeOnDelete(false);
                HasMany(u => u.OrderDeliveries).WithMany(m => m.labelIcons).Map(m =>
                {
                    m.ToTable("OrderDeliveryIcons");
                    m.MapLeftKey("labelId");  // because it is the "left" column, isn't it?
                    m.MapRightKey("OrderDeliveryId"); // because it is the "right" column, isn't it?
                });
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Display(Name = "عنوان")]
        public string Title { get; set; }


        [Display(Name = "توضیحات")]
        public string Descr { get; set; }


        [Display(Name = "کاور(تصویر)")]
        public Guid Cover { get; set; }
        public  attachment attachment { get; set; }
        public  ICollection<OrderDelivery> OrderDeliveries { get; set; }


        #endregion
    }
}
