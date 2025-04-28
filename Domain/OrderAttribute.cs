using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class OrderAttribute
    {
        public OrderAttribute()
        {
        }

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "نام خصوصیت")]
        public string Name { get; set; }

        /*
         1-number
         2-Decimal
         3-text
         4-html
         5-boolean
         6-DateTime
         7-File
         15- Value Added (number)
         16- Gift Id (number)
         17- Tracking Code Send Way  (text)
         18- User Description (text)
         19- Site Description (text شماره بارنامه ، زمان ارسال کالا  و هر چیز دیگری ...)
         21-barbari Number (text)
         22-Used Bon (number)
         23-Price Off (number)
         24-PriceType (boolean 1 is speacialOffer , 2 is AmazingOffer, 3 nothing)
         25-Send Factor(boolean 0 is no send, 1 is send)
         */
        [Required]
        [Display(Name = "نوع داده")]
        public Int16 DataType { get; set; }

        [Required]
        [Display(Name = "تاثیر در قیمت")]
        public bool PriceEffect { get; set; }


        [Required]
        [Display(Name = "زبان ( وب سایت )")]
        public Int16 LanguageId { get; set; }

        public  ICollection<OrderAttributeOrder> OrderAttributeSelects { get; set; }
    }

}
