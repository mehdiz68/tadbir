using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class CreateOrderKey
    {
        [Key]
        public long Id { get; set; }
    }
}
