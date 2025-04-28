using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class SettingDto
    {
        public int Id { get; set; }
        public string SettingName { get; set; }
        public string WebSiteName { get; set; }
        public string WebSiteTitle { get; set; }
        public string WebSiteAdress{ get; set; }
        public string Email{ get; set; }
        public string WebSitePhoneNumber{ get; set; }
        public string WebSiteMetaDescription { get; set; }
        public string WebSiteMetakeyword { get; set; }
        public string StaticContentDomain { get; set; }
        public string attachmentFileName { get; set; }
        public string WaterattachmentFileName { get; set; }
        public int WaterMarkPosition { get; set; }
        public bool LargeImageWaremark { get; set; }
        public string FaviconattachmentFileName { get; set; }
        public int LargeSizeWidth { get; set; }
        public int LargeSizeHeight { get; set; }
        public int MediumSizeWidth { get; set; }
        public int MediumSizeHeight { get; set; }
        public int SmallSizeWidth { get; set; }
        public int SmallSizeHeight { get; set; }
        public int XsmallSizeWidth { get; set; }
        public int XsmallSizeHeight { get; set; }
        public string WebmasterVerification { get; set; }
        public string AnalyticsVerification { get; set; }
        public Int16 DefaultCurrency { get; set; }
        public double CurrencyConvertionRate { get; set; }
        public double TaxRate { get; set; }
        public int BonPrice { get; set; }
        public int BonExpireDay { get; set; }
        public bool PopUpActive { get; set; }
        public bool PopUpType { get; set; }
        public string PopUpMessage { get; set; }
        public int PopUpEditVersion { get; set; }
        public bool HelpActiveShowInDefault { get; set; }
        public bool DisplayRootMenu { get; set; }
        public bool HasHttps { get; set; }
        public Int16? LanguageId { get; set; }
        public int ShoppingEstelamMinutes { get; set; }
        public int ShoppingPayEstelamMinutes { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Tele { get; set; }
        public string Tele2 { get; set; }
        public string Tele3 { get; set; }
        public string Mobile { get; set; }
        public string PostalCode { get; set; }
        public string TaxNumber { get; set; }
        public int CityId { get; set; }
        public string FooterGoogleMapLongitude { get; set; }

        public string FooterGoogleMapLatitude { get; set; }

        public string FooterGoogleMapZoom { get; set; }
        public bool yektanet { get; set; }
    }
}
