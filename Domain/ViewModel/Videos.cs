using CoreLib.ViewModel.Xml;
using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class Videos
    {
        public Videos()
        {

        }
        public string Name { get; set; }
        public IEnumerable<Adveresting> TopAdveresting { get; set; }
        public IEnumerable<Slider> Sliders { get; set; }
        public List<TopContentCat> TopCatContentsByNew { get; set; }
        public IEnumerable<Adveresting> RightAdveresting { get; set; }
        public List<TopContentCat> TopCatContentsByVisit { get; set; }
        public IEnumerable<Content> TopContentsByRandom1 { get; set; }
        public IEnumerable<Content> News { get; set; }
        public IEnumerable<Content> Events { get; set; }
        public IEnumerable<Adveresting> LeftAdveresting { get; set; }
        public IEnumerable<Content> TopContentsByRandom2 { get; set; }
        public IEnumerable<Content> TopContentsByRandom3 { get; set; }
        public IEnumerable<Content> TopContentsByRandom4 { get; set; }
        public IEnumerable<Adveresting> BottomAdveresting { get; set; }

    }

}
