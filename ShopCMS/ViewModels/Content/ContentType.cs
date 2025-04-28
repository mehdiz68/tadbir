using CoreLib.ViewModel.Xml;
using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using PagedList;

namespace ahmadi.ViewModels.Content
{
    public class ContentType
    {
        public ContentType()
        {

        }
        public XContentType contentType { get; set; }
        public IEnumerable<Domain.Content> BlogMainContents { get; set; }
        public IPagedList<Domain.Content> LatestContents { get; set; }
        public int LatestContentCount { get; set; }
        public IEnumerable<Domain.Content> HotContent { get; set; }
        public IEnumerable<Domain.Slider> Sliders { get; set; }
        public IEnumerable<Adveresting> TopAdveresting { get; set; }
        public IEnumerable<Adveresting> RightAdveresting { get; set; }
        public IEnumerable<Adveresting> BottomAdveresting { get; set; }
        public IEnumerable<Adveresting> LeftAdveresting { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Social> Socials { get; set; }
        public IEnumerable<Domain.ViewModels.TopContentCat> TopCatContents { get; set; }



    }

}
