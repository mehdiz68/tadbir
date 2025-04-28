using CoreLib.ViewModel.Xml;
using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class ContentType
    {
        public ContentType()
        {

        }
        public XContentType contentType { get; set; }
        public IEnumerable<Content> BlogMainContents { get; set; }
        public IEnumerable<Content> LatestContents { get; set; }
        public int LatestContentCount { get; set; }
        public IEnumerable<Content> HotContent { get; set; }
        public IEnumerable<Slider> Sliders { get; set; }
        public IEnumerable<Adveresting> TopAdveresting { get; set; }
        public IEnumerable<Adveresting> RightAdveresting { get; set; }
        public IEnumerable<Adveresting> BottomAdveresting { get; set; }
        public IEnumerable<Adveresting> LeftAdveresting { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Social> Socials { get; set; }
        public IEnumerable<Domain.ViewModels.TopContentCat> TopCatContents { get; set; }



    }

}
