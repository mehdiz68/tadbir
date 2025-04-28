using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class SuperDealViewModel
    {
        public string Name { get; set; }
        public string Abstract { get; set; }
        public string Data { get; set; }
        public List<AmazingOffers>  currentAmazingOffers { get; set; }
        public List<AmazingOffers> soonAmazingOffers { get; set; }
        public List<AmazingOffers> noTimerAmazingOffers { get; set; }
    }
}
