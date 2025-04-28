using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("XMasterThemes"), XmlType("XMasterThemes")]
    public class XMasterTheme
    {
        #region Ctor
        public XMasterTheme(int id,string headerAdvertising,bool headerAdvertisingVisibility,string headerWorkNumber,bool headerWorkNumberVisibility,string headerEmail,bool headerEmailVisibility,bool headerSocialNetworksVisibility,bool headerLogoVisibility,bool HeaderLanguageVisibility,bool HeaderLoginVisibility,bool HeaderRegistrationVisibility,bool HeaderSearchVisibility,bool HeaderBasketVisibility,bool HeaderMainMenuVisibility,bool HeaderSliderVisibility,bool HeaderSiteMapVisibility,bool FooterJoinNewsletterModuleVisibility,bool FooterContactUsModuleVisibility,string FooterWorkNumber,bool FooterWorkNumberVisibility,string FooterEmail,bool FooterEmailVisibility,string FooterAddress,bool FooterAddressVisibility,string FooterGoogleMapLongitude, string FooterGoogleMapLatitude, string FooterGoogleMapZoom, bool FooterGoogleMapVisibility,string FooterCopyright,bool FooterCopyrightVisibility,bool FooterSocialNetworksVisibility,Int16 LanguageId,bool FooterMainMenuVisibility,bool FooterSliderVisibility,bool FooterCustomMenuVisibility,string FooterAdvertising,bool FooterAdvertisingVisibility,bool footerWowVisibility,string HeaderAdvertisingLayout, string HeaderWorkNumberLayout, string HeaderEmailLayout, string HeaderSocialNetworksLayout, string HeaderLogoLayout, string HeaderLanguageLayout, string HeaderLoginLayout, string HeaderRegistrationLayout, string HeaderSearchLayout, string HeaderBasketLayout, string HeaderMainMenuLayout, string HeaderSliderLayout, string HeaderSiteMapLayout, string FooterJoinNewsletterLayout, string FooterContactUsModuleLayout, string FooterWorkNumberLayout, string FooterEmailLayout, string FooterAddressLayout, string FooterGoogleMapLayout, string FooterCopyrightLayout, string FooterSocialNetworksLayout, string FooterMainMenuLayout, string FooterSliderLayout, string FooterCustomMenuLayout, string FooterAdvertisingLayout)
        {
            this.Id = id;
            this.HeaderAdvertising = headerAdvertising;
            this.HeaderAdvertisingVisibility = headerAdvertisingVisibility;
            this.HeaderWorkNumber = headerWorkNumber;
            this.HeaderWorkNumberVisibility = headerWorkNumberVisibility;
            this.HeaderEmail = headerEmail;
            this.HeaderEmailVisibility = headerEmailVisibility;
            this.HeaderSocialNetworksVisibility = headerSocialNetworksVisibility;
            this.HeaderLogoVisibility = headerLogoVisibility;
            this.HeaderLanguageVisibility = HeaderLanguageVisibility;
            this.HeaderLoginVisibility = HeaderLoginVisibility;
            this.HeaderRegistrationVisibility = HeaderRegistrationVisibility;
            this.HeaderSearchVisibility = HeaderSearchVisibility;
            this.HeaderBasketVisibility = HeaderBasketVisibility;
            this.HeaderMainMenuVisibility = HeaderMainMenuVisibility;
            this.HeaderSliderVisibility = HeaderSliderVisibility;
            this.HeaderSiteMapVisibility = HeaderSiteMapVisibility;
            this.FooterJoinNewsletterModuleVisibility = FooterJoinNewsletterModuleVisibility;
            this.FooterContactUsModuleVisibility = FooterContactUsModuleVisibility;
            this.FooterWorkNumber = FooterWorkNumber;
            this.FooterWorkNumberVisibility = FooterWorkNumberVisibility;
            this.FooterEmail = FooterEmail;
            this.FooterEmailVisibility = FooterEmailVisibility;
            this.FooterAddress = FooterAddress;
            this.FooterAddressVisibility = FooterAddressVisibility;
            this.FooterGoogleMapLatitude = FooterGoogleMapLatitude;
            this.FooterGoogleMapLongitude = FooterGoogleMapLongitude;
            this.FooterGoogleMapZoom = FooterGoogleMapZoom;
            this.FooterGoogleMapVisibility = FooterGoogleMapVisibility;
            this.FooterCopyright = FooterCopyright;
            this.FooterCopyrightVisibility = FooterCopyrightVisibility;
            this.FooterSocialNetworksVisibility = FooterSocialNetworksVisibility;
            this.LanguageId = LanguageId;
            this.FooterMainMenuVisibility = FooterMainMenuVisibility;
            this.FooterSliderVisibility = FooterSliderVisibility;
            this.FooterCustomMenuVisibility = FooterCustomMenuVisibility;
            this.FooterAdvertising = FooterAdvertising;
            this.FooterAdvertisingVisibility = FooterAdvertisingVisibility;
            FooterWowVisibility = footerWowVisibility;
            this.FooterAddressLayout = FooterAddressLayout;
            this.FooterAdvertisingLayout = FooterAdvertisingLayout;
            this.FooterContactUsModuleLayout = FooterContactUsModuleLayout;
            this.FooterCopyrightLayout = FooterCopyrightLayout;
            this.FooterCustomMenuLayout = FooterCustomMenuLayout;
            this.FooterEmailLayout = FooterEmailLayout;
            this.FooterGoogleMapLayout = FooterGoogleMapLayout;
            this.FooterJoinNewsletterLayout = FooterJoinNewsletterLayout;
            this.FooterMainMenuLayout = FooterMainMenuLayout;
            this.FooterSliderLayout = FooterSliderLayout;
            this.FooterSocialNetworksLayout = FooterSocialNetworksLayout;
            this.FooterWorkNumberLayout = FooterWorkNumberLayout;
            this.HeaderAdvertisingLayout = HeaderAdvertisingLayout;
            this.HeaderBasketLayout = HeaderBasketLayout;
            this.HeaderEmailLayout = HeaderEmailLayout;
            this.HeaderLanguageLayout = HeaderLanguageLayout;
            this.HeaderLoginLayout = HeaderLoginLayout;
            this.HeaderLogoLayout = HeaderLogoLayout;
            this.HeaderMainMenuLayout = HeaderMainMenuLayout;
            this.HeaderRegistrationLayout = HeaderRegistrationLayout;
            this.HeaderSearchLayout = HeaderSearchLayout;
            this.HeaderSiteMapLayout = HeaderSiteMapLayout;
            this.HeaderSliderLayout = HeaderSliderLayout;
            this.HeaderSocialNetworksLayout = HeaderSliderLayout;
            this.HeaderWorkNumberLayout = HeaderWorkNumberLayout;
        }
        public XMasterTheme()
        {

        }
        #endregion


        #region Properties

        [Required(ErrorMessage = "اجباری")]
        public int Id { get; set; }

        [Display(Name = "تبلیغات هدر")]
        public string HeaderAdvertising { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش تبلیغات هدر")]
        public bool HeaderAdvertisingVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای تبلیغات هدر")]
        public string HeaderAdvertisingLayout { get; set; }

        [Display(Name = "شماره تماس")]
        public string HeaderWorkNumber { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش شماره تماس")]
        public bool HeaderWorkNumberVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای شماره تماس")]
        public string HeaderWorkNumberLayout { get; set; }

        [Display(Name = "ایمیل")]
        public string HeaderEmail { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش ایمیل")]
        public bool HeaderEmailVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای ایمیل")]
        public string HeaderEmailLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش شبکه های اجتماعی")]
        public bool HeaderSocialNetworksVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای شبکه های اجتماعی")]
        public string HeaderSocialNetworksLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش لوگو")]
        public bool HeaderLogoVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای لوگو")]
        public string HeaderLogoLayout { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش زبان های سایت")]
        public bool HeaderLanguageVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای زبان های سایت")]
        public string HeaderLanguageLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش قسمت لاگین")]
        public bool HeaderLoginVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده برای قسمت لاگین")]
        public string HeaderLoginLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش قسمت ثبت نام")]
        public bool HeaderRegistrationVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای قسمت ثبت نام")]
        public string HeaderRegistrationLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش قسمت جستجو")]
        public bool HeaderSearchVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای جست و جو")]
        public string HeaderSearchLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش سبد خرید")]
        public bool HeaderBasketVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای سبد خرید")]
        public string HeaderBasketLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش منو")]
        public bool HeaderMainMenuVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای منو")]
        public string HeaderMainMenuLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش اسلایدر در صفحه اصلی")]
        public bool HeaderSliderVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای اسلایدر")]
        public string HeaderSliderLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش نقشه سایت در صفحه اصلی")]
        public bool HeaderSiteMapVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای نقشه سایت")]
        public string HeaderSiteMapLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نوع منوی(مگا منو ، همراه با عکس )")]
        public bool MainMenuType { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش عضویت در خبرنامه")]
        public bool FooterJoinNewsletterModuleVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای عضویت در خبرنامه")]
        public string FooterJoinNewsletterLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش فرم تماس با ما")]
        public bool FooterContactUsModuleVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای فرم تماس با ما")]
        public string FooterContactUsModuleLayout { get; set; }

        [Display(Name = "شماره تماس")]
        public string FooterWorkNumber { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش شماره تماس")]
        public bool FooterWorkNumberVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای شماره تماس فوتر")]
        public string FooterWorkNumberLayout { get; set; }

        [Display(Name = "تبلیغات فوتر")]
        public string FooterAdvertising { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش تبلیغات فوتر")]
        public bool FooterAdvertisingVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای تبلیغات فوتر")]
        public string FooterAdvertisingLayout { get; set; }

        [Display(Name = "ایمیل")]
        public string FooterEmail { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش ایمیل")]
        public bool FooterEmailVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای ایمیل فوتر")]
        public string FooterEmailLayout { get; set; }

        [Display(Name = "آدرس")]
        public string FooterAddress { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش آدرس")]
        public bool FooterAddressVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای آدرس فوتر")]
        public string FooterAddressLayout { get; set; }

        [Display(Name = "طول جغرافیای گوگل مپ")]
        public string FooterGoogleMapLongitude { get; set; }

        [Display(Name = "عرض جغرافیایی گوگل مپ")]
        public string FooterGoogleMapLatitude { get; set; }

        [Display(Name = "زوم گوگل مپ")]
        public string FooterGoogleMapZoom { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش گوگل مپ")]
        public bool FooterGoogleMapVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای گوگل مپ")]
        public string FooterGoogleMapLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش منوی اصلی")]
        public bool FooterMainMenuVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای منوی اصلی فوتر")]
        public string FooterMainMenuLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش اسلایدر")]
        public bool FooterSliderVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای اسلایدر فوتر ")]
        public string FooterSliderLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش منوی فوتر")]
        public bool FooterCustomMenuVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای منوی سفارشی فوتر ")]
        public string FooterCustomMenuLayout { get; set; }

        [Display(Name = "کپی رایت")]
        public string FooterCopyright { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش کپی رایت")]
        public bool FooterCopyrightVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای کپی رایت فوتر")]
        public string FooterCopyrightLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش شبکه های اجتماعی")]
        public bool FooterSocialNetworksVisibility { get; set; }

        [Display(Name = "المنت نگه دارنده ( HTML ID ) برای شبکه های اجتماعی فوتر")]
        public string FooterSocialNetworksLayout { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش آیتم ها به صورت انیمیشنی (wow)")]
        public bool FooterWowVisibility { get; set; }

        /*
         * 1- Bootstrap Default
         * 2- PgwSlideShow
         * 3- FourBoxes 1
         * 4- FourBoxes 2
         * 5- FourBoxes 3
         * 6- Slicebox 1
         * 7- Slicebox 2
         * 8- Slicebox 3
         * 9- Slicebox 4
         */
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نوع اسلاید شو")]
        public Int16 SlideShowType { get; set; }

        /*
        * 1- olw default
        * 2- olw Hover Full Item
        * 3- LightSlider 1
        * 4- LightSlider 2
        */
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نوع اسلایدر آیتم ها")]
        public Int16 ItemSliderType { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16 LanguageId { get; set; }

        #endregion
    }
}
