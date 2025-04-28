using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class SuspendOrder
    {
        public SuspendOrder()
        {

        }

        public long price { get; set; }
        public long bankOrderid { get; set; }
        public int deliverId { get; set; }
        public string customerOrderid { get; set; }
        public int BankId { get; set; }
    }
}
