using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class WalletAttribute
    {
        public WalletAttribute()
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
         7- کد پیگیری کارت به کارت (text)
         8- شماره کارت مبدا کارت به کارت (text)
         9- شماره فیش (text)
         10- اتصال به زرین پال(number , if 100 ====> success)
         11- شناسه اتصال به زرین پال (text )
         12- وضعیت پرداخت زرین پال (text , if OK====>success , if NOK =======>Failure)
         13- کد پیگیری خرید از زرین پال (text )
         14- اتصال به بانک(number , if 0 ====> success)
         15- شناسه اتصال به بانک (text )
         16- وضعیت پرداخت بانک (number , if 0====>success)
         17- کد پیگیری خرید از بانک (text )
         18- تاریخ و زمان پرداخت (DateTime)
         19- وضعیت پرداخت به پیک نقدی (boolean)
         20- وضعیت پرداخت به پیک کارتخوان (boolean)
         21- کد پیگیری قبض دستگاه Pos (text)
         22- تاریخ و زمان پرداخت  قبض دستگاه Pos (text)
         */
        [Required]
        [Display(Name = "نوع داده")]
        public Int16 DataType { get; set; }

        [Required]
        [Display(Name = "زبان ( وب سایت )")]
        public Int16 LanguageId { get; set; }

        public  ICollection<WalletAttributeWallet> WalletAttributeWallets { get; set; }
    }

}
