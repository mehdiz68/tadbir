using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ahmadi.ViewModels.Home
{
    public class Meta
    {
        public Meta()
        {

        }
        #region Properties

        public string PageCover { get; set; }

        public string Logo { get; set; }

        public string Favicon { get; set; }

        public string WebSiteTitle { get; set; }

        public string WebSiteName { get; set; }

        public string WebSiteMetaDescription { get; set; }

        public string WebSiteMetakeyword { get; set; }

        public string CanocicalUrl { get; set; }

        public string StaticContentUrl { get; set; }
        public string contenType { get; set; }

        #endregion

    }
}