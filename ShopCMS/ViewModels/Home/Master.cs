using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace ahmadi.ViewModels.Home
{
    public class Master
    {
        public List<Domain.Social> HeaderSocialNetworks { get; set; }

        public string HeaderLogo { get; set; }

        public string StaticContentUrl { get; set; }
        public int CurrentLanguageId { get; set; }
        public List<DisplayMenu> HeaderMainMenu { get; set; }

        public List<DisplayMenu> FooterMainMenu { get; set; }

        public IEnumerable<Domain.Content> services { get; set; }

        public string WebSiteName { get; set; }

        public string WebSiteTitle { get; set; }
        public string WebsiteAddress { get; set; }
        public string WebsiteAddress2 { get; set; }
        public string WebsitePhoneNumber { get; set; }
        public string WebsiteTele { get; set; }
        public string WebsiteTele2 { get; set; }
        public string WebsiteTele3 { get; set; }
        public string Email { get; set; }

        public string AboutPageAbstract { get; set; }

        public bool PopUpActive { get; set; }

        public string PopUpMessage { get; set; }

        public string FooterGoogleMapLatitude { get; set; }
        public string FooterGoogleMapLongitude { get; set; }
        public string FooterGoogleMapZoom { get; set; }
        public bool PopUpType { get; set; }
        public IEnumerable<CoreLib.ViewModel.Xml.XContentType> ContentTypes { get; set; }

        public IEnumerable<Domain.ViewModels.Link> VideoCategories { get; set; }
        public IEnumerable<Domain.ViewModels.Link> Videos1 { get; set; }
        public IEnumerable<Domain.ViewModels.Link> Videos2 { get; set; }
        public IEnumerable<Domain.ViewModels.Link> Videos3 { get; set; }
        public Link VideoCat1 { get; set; }
        public Link VideoCat2 { get; set; }
        public Link VideoCat3 { get; set; }

        public bool root { get; set; }
        public ApplicationUser User { get; set; }
    }


    public class DisplayMenu
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public int PlaceShow { get; set; }
        public int DisplayOrder { get; set; }
        public string Cover { get; set; }
        public bool IsRoot { get; set; }
        public int parentId { get; set; }

        public List<DisplayMenu> ChildMenu;
    }


    public class NewsLetter
    {
        [Required(ErrorMessage = "وارد کردن ایمیل ، اجباری است")]
        [Display(Name = " ایمیل")]
        [MaxLength(255, ErrorMessage = "حداکثر طول کاراکتر ، 255")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "آدرس ایمیل وارد نمایید")]
        public string Email { get; set; }
    }

    public class HomePage
    {

        public IEnumerable<Domain.SliderImage> SliderImages { get; set; }
        public IEnumerable<Domain.Content> MainServices{ get; set; }
        public IEnumerable<Domain.Content> Services { get; set; }
        public IEnumerable<Domain.Content> Projects { get; set; }
        public IEnumerable<Domain.Content> Blogs { get; set; }
        public IEnumerable<Domain.Comment> Comments { get; set; }
        public IEnumerable<Domain.StaticTextCategory> StaticTextCategories { get; set; }
        public Domain.Content Video { get; set; }
        public IEnumerable<Domain.Adveresting> TopAdveresting { get; set; }
        public IEnumerable<Domain.Adveresting> RightAdveresting { get; set; }
        public IEnumerable<Domain.Adveresting> BottomAdveresting { get; set; }
        public IEnumerable<Domain.Adveresting> LeftAdveresting { get; set; }

    }
}