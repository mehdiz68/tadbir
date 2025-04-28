using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Diagnostics;

namespace CoreLib.ViewModel.Xml
{
    public class XMLReader
    {
        private string Url = "";
        #region GetUrl
        public XMLReader(string StaticUrl)
        {
            //Url = StaticUrl;
            Url="~";
        }
        #endregion

        #region ContentTypes
        public List<XContentType> ListOfXContentType()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath(Url + "/Content/Xml/Theme.xml"));
            var XContentTypes = new List<XContentType>();
            XContentTypes = (from rows in ds.Tables["ContentType"].AsEnumerable()
                             select new XContentType
                             {
                                 Id = Convert.ToInt32(rows[0].ToString()),
                                 Name = rows[1].ToString(),
                                 Title = rows[2].ToString(),
                                 LanguageId = Convert.ToInt32(rows[3].ToString()),
                                 Abstract = rows[4].ToString(),
                                 InSearch = Convert.ToBoolean(rows[5].ToString()),
                                 ShortName = rows[6].ToString(),
                                 IsVideo = Convert.ToBoolean(rows[7].ToString()),
                                 Cover = rows[8].ToString()
                             }).ToList();

            return XContentTypes;
        }

        public XContentType DetailOfXContentType(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath(Url + "/Content/Xml/Theme.xml"));
            XContentType XContentType = (from rows in ds.Tables["ContentType"].AsEnumerable()
                                         where rows.Field<string>("Id") == Id.ToString()
                                         select new XContentType
                                         {
                                             Id = Convert.ToInt32(rows[0].ToString()),
                                             Name = rows[1].ToString(),
                                             Title = rows[2].ToString(),
                                             LanguageId = Convert.ToInt32(rows[3].ToString()),
                                             Abstract = rows[4].ToString(),
                                             InSearch = Convert.ToBoolean(rows[5].ToString()),
                                             ShortName = rows[6].ToString(),
                                             IsVideo = Convert.ToBoolean(rows[7].ToString()),
                                             Cover = rows[8].ToString()
                                         }).SingleOrDefault();

            return XContentType;
        }

        public bool CreateOfXContentType(XContentType model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/Xml/Theme.xml"));
                XElement ContentTypes = originalXml.Root.Element("ContentTypes");
                ContentTypes.Add(new XElement("ContentType", new XElement("Id", model.Id), new XElement("Name", model.Name), new XElement("Title", model.Title), new XElement("LanguageId", model.LanguageId), new XElement("Abstract", model.Abstract), new XElement("InSearch", model.InSearch), new XElement("ShortName", model.ShortName), new XElement("IsVideo", model.IsVideo), new XElement("Cover", !String.IsNullOrEmpty(model.Cover) ? model.Cover : "")));

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXContentType(XContentType model,out string msg)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("ContentTypes")
      .Elements("ContentType")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();

                target.Element("Name").Value = model.Name;
                target.Element("Title").Value = model.Title;
                target.Element("LanguageId").Value = model.LanguageId.ToString();
                target.Element("Abstract").Value = model.Abstract;
                target.Element("InSearch").Value = model.InSearch.ToString();
                target.Element("ShortName").Value = model.ShortName;
                target.Element("IsVideo").Value = model.IsVideo.ToString();
                if (!String.IsNullOrEmpty(model.Cover))
                    target.Element("Cover").Value = model.Cover.ToString();
                else
                    target.Element("Cover").Value = "";

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/Xml/Theme.xml"));
                msg = "";
                return true;
            }
            catch (Exception x)
            {
                msg = x.Message;
                return false;
            }
        }

        public bool RemoveXContentType(int id)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("ContentTypes")
      .Elements("ContentType")
      .Where(e => e.Element("Id").Value == id.ToString())
      .Single();

                target.Remove();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Languages
        public List<XLanguage> ListOfXLanguage()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
           ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Theme.xml"));
            var XLanguages = new List<XLanguage>();
            XLanguages = (from rows in ds.Tables["Language"].AsEnumerable()
                          select new XLanguage
                          {
                              Id = Convert.ToInt16(rows[0].ToString()),
                              Name = rows[1].ToString(),
                              ShortName = rows[2].ToString()
                          }).ToList();

            return XLanguages;
        }

        public XLanguage DetailOfXLanguage(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
           ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Theme.xml"));
            XLanguage XLanguage = (from rows in ds.Tables["Language"].AsEnumerable()
                                   where rows.Field<string>("Id") == Id.ToString()
                                   select new XLanguage
                                   {
                                       Id = Convert.ToInt16(rows[0].ToString()),
                                       Name = rows[1].ToString(),
                                       ShortName = rows[2].ToString(),
                                   }).SingleOrDefault();

            return XLanguage;
        }

        public bool CreateOfXLanguage(XLanguage model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));
                XElement Languages = originalXml.Root.Element("Languages");
                Languages.Add(new XElement("Language", new XElement("Id", model.Id), new XElement("Name", model.Name), new XElement("ShortName", model.ShortName)));

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXLanguage(XLanguage model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("Languages")
      .Elements("Language")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();

                target.Element("Name").Value = model.Name;
                target.Element("ShortName").Value = model.ShortName;

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveXLanguage(int id)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("Languages")
      .Elements("Language")
      .Where(e => e.Element("Id").Value == id.ToString())
      .Single();

                target.Remove();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region MasterTheme
        public List<XMasterTheme> ListOfXMasterTheme()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
           ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Theme.xml"));
            var XMasterTheme = new List<XMasterTheme>();
            XMasterTheme = (from rows in ds.Tables["MasterTheme"].AsEnumerable()
                            select new XMasterTheme
                            {
                                Id = Convert.ToInt16(rows[0].ToString()),
                                HeaderAdvertising = rows[1].ToString(),
                                HeaderAdvertisingVisibility = Convert.ToBoolean(rows[2]),
                                HeaderAdvertisingLayout = rows[3].ToString(),
                                HeaderWorkNumber = rows[4].ToString(),
                                HeaderWorkNumberVisibility = Convert.ToBoolean(rows[5]),
                                HeaderWorkNumberLayout = rows[6].ToString(),
                                HeaderEmail = rows[7].ToString(),
                                HeaderEmailVisibility = Convert.ToBoolean(rows[8]),
                                HeaderEmailLayout = rows[9].ToString(),
                                HeaderSocialNetworksVisibility = Convert.ToBoolean(rows[10]),
                                HeaderSocialNetworksLayout = rows[11].ToString(),
                                HeaderLogoVisibility = Convert.ToBoolean(rows[12]),
                                HeaderLogoLayout = rows[13].ToString(),
                                HeaderLanguageVisibility = Convert.ToBoolean(rows[14]),
                                HeaderLanguageLayout = rows[15].ToString(),
                                HeaderLoginVisibility = Convert.ToBoolean(rows[16]),
                                HeaderLoginLayout = rows[17].ToString(),
                                HeaderRegistrationVisibility = Convert.ToBoolean(rows[18]),
                                HeaderRegistrationLayout = rows[19].ToString(),
                                HeaderSearchVisibility = Convert.ToBoolean(rows[20]),
                                HeaderSearchLayout = rows[21].ToString(),
                                HeaderBasketVisibility = Convert.ToBoolean(rows[22]),
                                HeaderBasketLayout = rows[23].ToString(),
                                HeaderMainMenuVisibility = Convert.ToBoolean(rows[24]),
                                HeaderMainMenuLayout = rows[25].ToString(),
                                HeaderSliderVisibility = Convert.ToBoolean(rows[26]),
                                HeaderSliderLayout = rows[27].ToString(),
                                HeaderSiteMapVisibility = Convert.ToBoolean(rows[28]),
                                HeaderSiteMapLayout = rows[29].ToString(),
                                FooterJoinNewsletterModuleVisibility = Convert.ToBoolean(rows[30]),
                                FooterJoinNewsletterLayout = rows[31].ToString(),
                                FooterContactUsModuleVisibility = Convert.ToBoolean(rows[32]),
                                FooterContactUsModuleLayout = rows[33].ToString(),
                                FooterWorkNumber = rows[34].ToString(),
                                FooterWorkNumberVisibility = Convert.ToBoolean(rows[35]),
                                FooterWorkNumberLayout = rows[36].ToString(),
                                FooterEmail = rows[37].ToString(),
                                FooterEmailVisibility = Convert.ToBoolean(rows[38]),
                                FooterEmailLayout = rows[39].ToString(),
                                FooterAddress = rows[40].ToString(),
                                FooterAddressVisibility = Convert.ToBoolean(rows[41]),
                                FooterAddressLayout = rows[42].ToString(),
                                FooterGoogleMapLongitude = rows[43].ToString(),
                                FooterGoogleMapLatitude = rows[44].ToString(),
                                FooterGoogleMapZoom = rows[45].ToString(),
                                FooterGoogleMapVisibility = Convert.ToBoolean(rows[46]),
                                FooterGoogleMapLayout = rows[47].ToString(),
                                FooterCopyright = rows[48].ToString(),
                                FooterCopyrightVisibility = Convert.ToBoolean(rows[49]),
                                FooterCopyrightLayout = rows[50].ToString(),
                                FooterSocialNetworksVisibility = Convert.ToBoolean(rows[51]),
                                FooterSocialNetworksLayout = rows[52].ToString(),
                                LanguageId = Convert.ToInt16(rows[53]),
                                FooterMainMenuVisibility = Convert.ToBoolean(rows[54]),
                                FooterMainMenuLayout = rows[55].ToString(),
                                FooterSliderVisibility = Convert.ToBoolean(rows[56]),
                                FooterSliderLayout = rows[57].ToString(),
                                FooterCustomMenuVisibility = Convert.ToBoolean(rows[58]),
                                FooterCustomMenuLayout = rows[59].ToString(),
                                FooterAdvertising = rows[60].ToString(),
                                FooterAdvertisingVisibility = Convert.ToBoolean(rows[61]),
                                FooterAdvertisingLayout = rows[62].ToString(),
                                FooterWowVisibility = Convert.ToBoolean(rows[63]),
                                MainMenuType = Convert.ToBoolean(rows[64]),
                                SlideShowType = Convert.ToInt16(rows[65]),
                                ItemSliderType = Convert.ToInt16(rows[66])
                            }).ToList();

            return XMasterTheme;
        }

        public XMasterTheme DetailOfXMasterTheme(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
           ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Theme.xml"));
            XMasterTheme XMasterTheme = (from rows in ds.Tables["MasterTheme"].AsEnumerable()
                                         where rows.Field<string>("Id") == Id.ToString()
                                         select new XMasterTheme
                                         {

                                             Id = Convert.ToInt16(rows[0].ToString()),
                                             HeaderAdvertising = rows[1].ToString(),
                                             HeaderAdvertisingVisibility = Convert.ToBoolean(rows[2]),
                                             HeaderAdvertisingLayout = rows[3].ToString(),
                                             HeaderWorkNumber = rows[4].ToString(),
                                             HeaderWorkNumberVisibility = Convert.ToBoolean(rows[5]),
                                             HeaderWorkNumberLayout = rows[6].ToString(),
                                             HeaderEmail = rows[7].ToString(),
                                             HeaderEmailVisibility = Convert.ToBoolean(rows[8]),
                                             HeaderEmailLayout = rows[9].ToString(),
                                             HeaderSocialNetworksVisibility = Convert.ToBoolean(rows[10]),
                                             HeaderSocialNetworksLayout = rows[11].ToString(),
                                             HeaderLogoVisibility = Convert.ToBoolean(rows[12]),
                                             HeaderLogoLayout = rows[13].ToString(),
                                             HeaderLanguageVisibility = Convert.ToBoolean(rows[14]),
                                             HeaderLanguageLayout = rows[15].ToString(),
                                             HeaderLoginVisibility = Convert.ToBoolean(rows[16]),
                                             HeaderLoginLayout = rows[17].ToString(),
                                             HeaderRegistrationVisibility = Convert.ToBoolean(rows[18]),
                                             HeaderRegistrationLayout = rows[19].ToString(),
                                             HeaderSearchVisibility = Convert.ToBoolean(rows[20]),
                                             HeaderSearchLayout = rows[21].ToString(),
                                             HeaderBasketVisibility = Convert.ToBoolean(rows[22]),
                                             HeaderBasketLayout = rows[23].ToString(),
                                             HeaderMainMenuVisibility = Convert.ToBoolean(rows[24]),
                                             HeaderMainMenuLayout = rows[25].ToString(),
                                             HeaderSliderVisibility = Convert.ToBoolean(rows[26]),
                                             HeaderSliderLayout = rows[27].ToString(),
                                             HeaderSiteMapVisibility = Convert.ToBoolean(rows[28]),
                                             HeaderSiteMapLayout = rows[29].ToString(),
                                             FooterJoinNewsletterModuleVisibility = Convert.ToBoolean(rows[30]),
                                             FooterJoinNewsletterLayout = rows[31].ToString(),
                                             FooterContactUsModuleVisibility = Convert.ToBoolean(rows[32]),
                                             FooterContactUsModuleLayout = rows[33].ToString(),
                                             FooterWorkNumber = rows[34].ToString(),
                                             FooterWorkNumberVisibility = Convert.ToBoolean(rows[35]),
                                             FooterWorkNumberLayout = rows[36].ToString(),
                                             FooterEmail = rows[37].ToString(),
                                             FooterEmailVisibility = Convert.ToBoolean(rows[38]),
                                             FooterEmailLayout = rows[39].ToString(),
                                             FooterAddress = rows[40].ToString(),
                                             FooterAddressVisibility = Convert.ToBoolean(rows[41]),
                                             FooterAddressLayout = rows[42].ToString(),
                                             FooterGoogleMapLongitude = rows[43].ToString(),
                                             FooterGoogleMapLatitude = rows[44].ToString(),
                                             FooterGoogleMapZoom = rows[45].ToString(),
                                             FooterGoogleMapVisibility = Convert.ToBoolean(rows[46]),
                                             FooterGoogleMapLayout = rows[47].ToString(),
                                             FooterCopyright = rows[48].ToString(),
                                             FooterCopyrightVisibility = Convert.ToBoolean(rows[49]),
                                             FooterCopyrightLayout = rows[50].ToString(),
                                             FooterSocialNetworksVisibility = Convert.ToBoolean(rows[51]),
                                             FooterSocialNetworksLayout = rows[52].ToString(),
                                             LanguageId = Convert.ToInt16(rows[53]),
                                             FooterMainMenuVisibility = Convert.ToBoolean(rows[54]),
                                             FooterMainMenuLayout = rows[55].ToString(),
                                             FooterSliderVisibility = Convert.ToBoolean(rows[56]),
                                             FooterSliderLayout = rows[57].ToString(),
                                             FooterCustomMenuVisibility = Convert.ToBoolean(rows[58]),
                                             FooterCustomMenuLayout = rows[59].ToString(),
                                             FooterAdvertising = rows[60].ToString(),
                                             FooterAdvertisingVisibility = Convert.ToBoolean(rows[61]),
                                             FooterAdvertisingLayout = rows[62].ToString(),
                                             FooterWowVisibility = Convert.ToBoolean(rows[63]),
                                             MainMenuType = Convert.ToBoolean(rows[64]),
                                             SlideShowType = Convert.ToInt16(rows[65]),
                                             ItemSliderType = Convert.ToInt16(rows[66])
                                         }).SingleOrDefault();

            return XMasterTheme;
        }

        public XMasterTheme DetailOfXMasterThemeWithLanguage(int LanguageId)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
           ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Theme.xml"));
            XMasterTheme XMasterTheme = (from rows in ds.Tables["MasterTheme"].AsEnumerable()
                                         where rows.Field<string>("LanguageId") == LanguageId.ToString()
                                         select new XMasterTheme
                                         {

                                             Id = Convert.ToInt16(rows[0].ToString()),
                                             HeaderAdvertising = rows[1].ToString(),
                                             HeaderAdvertisingVisibility = Convert.ToBoolean(rows[2]),
                                             HeaderAdvertisingLayout = rows[3].ToString(),
                                             HeaderWorkNumber = rows[4].ToString(),
                                             HeaderWorkNumberVisibility = Convert.ToBoolean(rows[5]),
                                             HeaderWorkNumberLayout = rows[6].ToString(),
                                             HeaderEmail = rows[7].ToString(),
                                             HeaderEmailVisibility = Convert.ToBoolean(rows[8]),
                                             HeaderEmailLayout = rows[9].ToString(),
                                             HeaderSocialNetworksVisibility = Convert.ToBoolean(rows[10]),
                                             HeaderSocialNetworksLayout = rows[11].ToString(),
                                             HeaderLogoVisibility = Convert.ToBoolean(rows[12]),
                                             HeaderLogoLayout = rows[13].ToString(),
                                             HeaderLanguageVisibility = Convert.ToBoolean(rows[14]),
                                             HeaderLanguageLayout = rows[15].ToString(),
                                             HeaderLoginVisibility = Convert.ToBoolean(rows[16]),
                                             HeaderLoginLayout = rows[17].ToString(),
                                             HeaderRegistrationVisibility = Convert.ToBoolean(rows[18]),
                                             HeaderRegistrationLayout = rows[19].ToString(),
                                             HeaderSearchVisibility = Convert.ToBoolean(rows[20]),
                                             HeaderSearchLayout = rows[21].ToString(),
                                             HeaderBasketVisibility = Convert.ToBoolean(rows[22]),
                                             HeaderBasketLayout = rows[23].ToString(),
                                             HeaderMainMenuVisibility = Convert.ToBoolean(rows[24]),
                                             HeaderMainMenuLayout = rows[25].ToString(),
                                             HeaderSliderVisibility = Convert.ToBoolean(rows[26]),
                                             HeaderSliderLayout = rows[27].ToString(),
                                             HeaderSiteMapVisibility = Convert.ToBoolean(rows[28]),
                                             HeaderSiteMapLayout = rows[29].ToString(),
                                             FooterJoinNewsletterModuleVisibility = Convert.ToBoolean(rows[30]),
                                             FooterJoinNewsletterLayout = rows[31].ToString(),
                                             FooterContactUsModuleVisibility = Convert.ToBoolean(rows[32]),
                                             FooterContactUsModuleLayout = rows[33].ToString(),
                                             FooterWorkNumber = rows[34].ToString(),
                                             FooterWorkNumberVisibility = Convert.ToBoolean(rows[35]),
                                             FooterWorkNumberLayout = rows[36].ToString(),
                                             FooterEmail = rows[37].ToString(),
                                             FooterEmailVisibility = Convert.ToBoolean(rows[38]),
                                             FooterEmailLayout = rows[39].ToString(),
                                             FooterAddress = rows[40].ToString(),
                                             FooterAddressVisibility = Convert.ToBoolean(rows[41]),
                                             FooterAddressLayout = rows[42].ToString(),
                                             FooterGoogleMapLongitude = rows[43].ToString(),
                                             FooterGoogleMapLatitude = rows[44].ToString(),
                                             FooterGoogleMapZoom = rows[45].ToString(),
                                             FooterGoogleMapVisibility = Convert.ToBoolean(rows[46]),
                                             FooterGoogleMapLayout = rows[47].ToString(),
                                             FooterCopyright = rows[48].ToString(),
                                             FooterCopyrightVisibility = Convert.ToBoolean(rows[49]),
                                             FooterCopyrightLayout = rows[50].ToString(),
                                             FooterSocialNetworksVisibility = Convert.ToBoolean(rows[51]),
                                             FooterSocialNetworksLayout = rows[52].ToString(),
                                             LanguageId = Convert.ToInt16(rows[53]),
                                             FooterMainMenuVisibility = Convert.ToBoolean(rows[54]),
                                             FooterMainMenuLayout = rows[55].ToString(),
                                             FooterSliderVisibility = Convert.ToBoolean(rows[56]),
                                             FooterSliderLayout = rows[57].ToString(),
                                             FooterCustomMenuVisibility = Convert.ToBoolean(rows[58]),
                                             FooterCustomMenuLayout = rows[59].ToString(),
                                             FooterAdvertising = rows[60].ToString(),
                                             FooterAdvertisingVisibility = Convert.ToBoolean(rows[61]),
                                             FooterAdvertisingLayout = rows[62].ToString(),
                                             FooterWowVisibility = Convert.ToBoolean(rows[63]),
                                             MainMenuType = Convert.ToBoolean(rows[64]),
                                             SlideShowType = Convert.ToInt16(rows[65]),
                                             ItemSliderType = Convert.ToInt16(rows[66])
                                         }).SingleOrDefault();

            return XMasterTheme;
        }

        public bool CreateOfXMasterTheme(XMasterTheme model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));
                XElement Languages = originalXml.Root.Element("MasterThemes");

                XElement HeaderAdvertising = new XElement("HeaderAdvertising", "");
                if (model.HeaderAdvertising != null)
                    HeaderAdvertising = new XElement("HeaderAdvertising", new XCData(model.HeaderAdvertising));

                XElement HeaderWorkNumber = new XElement("HeaderWorkNumber", "");
                if (model.HeaderWorkNumber != null)
                    HeaderWorkNumber = new XElement("HeaderWorkNumber", new XCData(model.HeaderWorkNumber));

                XElement HeaderEmail = new XElement("HeaderEmail", "");
                if (model.HeaderEmail != null)
                    HeaderEmail = new XElement("HeaderEmail", new XCData(model.HeaderEmail));


                XElement FooterAdvertising = new XElement("FooterAdvertising", "");
                if (model.FooterAdvertising != null)
                    FooterAdvertising = new XElement("FooterAdvertising", new XCData(model.FooterAdvertising));

                XElement FooterWorkNumber = new XElement("FooterWorkNumber", "");
                if (model.FooterWorkNumber != null)
                    FooterWorkNumber = new XElement("FooterWorkNumber", new XCData(model.FooterWorkNumber));

                XElement FooterEmail = new XElement("FooterEmail", "");
                if (model.FooterEmail != null)
                    FooterEmail = new XElement("FooterEmail", new XCData(model.FooterEmail));

                XElement FooterAddress = new XElement("FooterAddress", "");
                if (model.FooterAddress != null)
                    FooterAddress = new XElement("FooterAddress", new XCData(model.FooterAddress));

                XElement FooterAddressLayout = new XElement("FooterAddressLayout", "");
                if (model.FooterAddressLayout != null)
                    FooterAddressLayout = new XElement("FooterAddressLayout", new XCData(model.FooterAddressLayout));

                XElement FooterAdvertisingLayout = new XElement("FooterAdvertisingLayout", "");
                if (model.FooterAdvertisingLayout != null)
                    FooterAdvertisingLayout = new XElement("FooterAdvertisingLayout", new XCData(model.FooterAdvertisingLayout));

                XElement FooterContactUsModuleLayout = new XElement("FooterContactUsModuleLayout", "");
                if (model.FooterContactUsModuleLayout != null)
                    FooterContactUsModuleLayout = new XElement("FooterContactUsModuleLayout", new XCData(model.FooterContactUsModuleLayout));

                XElement FooterCopyrightLayout = new XElement("FooterCopyrightLayout", "");
                if (model.FooterCopyrightLayout != null)
                    FooterCopyrightLayout = new XElement("FooterCopyrightLayout", new XCData(model.FooterCopyrightLayout));

                XElement FooterCustomMenuLayout = new XElement("FooterCustomMenuLayout", "");
                if (model.FooterCustomMenuLayout != null)
                    FooterCustomMenuLayout = new XElement("FooterCustomMenuLayout", new XCData(model.FooterCustomMenuLayout));

                XElement FooterEmailLayout = new XElement("FooterEmailLayout", "");
                if (model.FooterEmailLayout != null)
                    FooterEmailLayout = new XElement("FooterEmailLayout", new XCData(model.FooterEmailLayout));

                XElement FooterGoogleMapLayout = new XElement("FooterGoogleMapLayout", "");
                if (model.FooterGoogleMapLayout != null)
                    FooterGoogleMapLayout = new XElement("FooterGoogleMapLayout", new XCData(model.FooterGoogleMapLayout));

                XElement FooterJoinNewsletterLayout = new XElement("FooterJoinNewsletterLayout", "");
                if (model.FooterJoinNewsletterLayout != null)
                    FooterJoinNewsletterLayout = new XElement("FooterJoinNewsletterLayout", new XCData(model.FooterJoinNewsletterLayout));

                XElement FooterMainMenuLayout = new XElement("FooterMainMenuLayout", "");
                if (model.FooterMainMenuLayout != null)
                    FooterMainMenuLayout = new XElement("FooterMainMenuLayout", new XCData(model.FooterMainMenuLayout));

                XElement FooterSliderLayout = new XElement("FooterSliderLayout", "");
                if (model.FooterSliderLayout != null)
                    FooterSliderLayout = new XElement("FooterSliderLayout", new XCData(model.FooterSliderLayout));

                XElement FooterSocialNetworksLayout = new XElement("FooterSocialNetworksLayout", "");
                if (model.FooterSocialNetworksLayout != null)
                    FooterSocialNetworksLayout = new XElement("FooterSocialNetworksLayout", new XCData(model.FooterSocialNetworksLayout));

                XElement FooterWorkNumberLayout = new XElement("FooterWorkNumberLayout", "");
                if (model.FooterWorkNumberLayout != null)
                    FooterWorkNumberLayout = new XElement("FooterWorkNumberLayout", new XCData(model.FooterWorkNumberLayout));

                XElement HeaderAdvertisingLayout = new XElement("HeaderAdvertisingLayout", "");
                if (model.HeaderAdvertisingLayout != null)
                    HeaderAdvertisingLayout = new XElement("HeaderAdvertisingLayout", new XCData(model.HeaderAdvertisingLayout));

                XElement HeaderBasketLayout = new XElement("HeaderBasketLayout", "");
                if (model.HeaderBasketLayout != null)
                    HeaderBasketLayout = new XElement("HeaderBasketLayout", new XCData(model.HeaderBasketLayout));

                XElement HeaderEmailLayout = new XElement("HeaderEmailLayout", "");
                if (model.HeaderEmailLayout != null)
                    HeaderEmailLayout = new XElement("HeaderEmailLayout", new XCData(model.HeaderEmailLayout));

                XElement HeaderLanguageLayout = new XElement("HeaderLanguageLayout", "");
                if (model.HeaderLanguageLayout != null)
                    HeaderLanguageLayout = new XElement("HeaderLanguageLayout", new XCData(model.HeaderLanguageLayout));


                XElement HeaderLoginLayout = new XElement("HeaderLoginLayout", "");
                if (model.HeaderLoginLayout != null)
                    HeaderLoginLayout = new XElement("HeaderLoginLayout", new XCData(model.HeaderLoginLayout));

                XElement HeaderLogoLayout = new XElement("HeaderLogoLayout", "");
                if (model.HeaderLogoLayout != null)
                    HeaderLogoLayout = new XElement("HeaderLogoLayout", new XCData(model.HeaderLogoLayout));

                XElement HeaderMainMenuLayout = new XElement("HeaderMainMenuLayout", "");
                if (model.HeaderMainMenuLayout != null)
                    HeaderMainMenuLayout = new XElement("HeaderMainMenuLayout", new XCData(model.HeaderMainMenuLayout));

                XElement HeaderRegistrationLayout = new XElement("HeaderRegistrationLayout", "");
                if (model.HeaderRegistrationLayout != null)
                    HeaderRegistrationLayout = new XElement("HeaderRegistrationLayout", new XCData(model.HeaderRegistrationLayout));

                XElement HeaderSearchLayout = new XElement("HeaderSearchLayout", "");
                if (model.HeaderSearchLayout != null)
                    HeaderSearchLayout = new XElement("HeaderSearchLayout", new XCData(model.HeaderSearchLayout));

                XElement HeaderSiteMapLayout = new XElement("HeaderSiteMapLayout", "");
                if (model.HeaderSiteMapLayout != null)
                    HeaderSiteMapLayout = new XElement("HeaderSiteMapLayout", new XCData(model.HeaderSiteMapLayout));

                XElement HeaderSliderLayout = new XElement("HeaderSliderLayout", "");
                if (model.HeaderSliderLayout != null)
                    HeaderSliderLayout = new XElement("HeaderSliderLayout", new XCData(model.HeaderSliderLayout));

                XElement HeaderSocialNetworksLayout = new XElement("HeaderSocialNetworksLayout", "");
                if (model.HeaderSocialNetworksLayout != null)
                    HeaderSocialNetworksLayout = new XElement("HeaderSocialNetworksLayout", new XCData(model.HeaderSocialNetworksLayout));

                XElement HeaderWorkNumberLayout = new XElement("HeaderWorkNumberLayout", "");
                if (model.HeaderWorkNumberLayout != null)
                    HeaderWorkNumberLayout = new XElement("HeaderWorkNumberLayout", new XCData(model.HeaderWorkNumberLayout));






                XElement FooterCopyright = new XElement("FooterCopyright", "");
                if (model.FooterCopyright != null)
                    FooterCopyright = new XElement("FooterCopyright", new XCData(model.FooterCopyright));

                Languages.Add(new XElement("MasterTheme", new XElement("Id", model.Id), HeaderAdvertising, new XElement("HeaderAdvertisingVisibility", model.HeaderAdvertisingVisibility), HeaderWorkNumber, new XElement("HeaderWorkNumberVisibility", model.HeaderWorkNumberVisibility), HeaderEmail, new XElement("HeaderEmailVisibility", model.HeaderEmailVisibility), new XElement("HeaderSocialNetworksVisibility", model.HeaderSocialNetworksVisibility), new XElement("HeaderLogoVisibility", model.HeaderLogoVisibility), new XElement("HeaderLanguageVisibility", model.HeaderLanguageVisibility), new XElement("HeaderLoginVisibility", model.HeaderLoginVisibility), new XElement("HeaderRegistrationVisibility", model.HeaderRegistrationVisibility), new XElement("HeaderSearchVisibility", model.HeaderSearchVisibility), new XElement("HeaderBasketVisibility", model.HeaderBasketVisibility), new XElement("HeaderMainMenuVisibility", model.HeaderMainMenuVisibility), new XElement("HeaderSliderVisibility", model.HeaderSliderVisibility), new XElement("HeaderSiteMapVisibility", model.HeaderSiteMapVisibility), new XElement("FooterJoinNewsletterModuleVisibility", model.FooterJoinNewsletterModuleVisibility), new XElement("FooterContactUsModuleVisibility", model.FooterContactUsModuleVisibility), FooterWorkNumber, new XElement("FooterWorkNumberVisibility", model.FooterWorkNumberVisibility), FooterEmail, new XElement("FooterEmailVisibility", model.FooterEmailVisibility), FooterAddress, new XElement("FooterAddressVisibility", model.FooterAddressVisibility), new XElement("FooterGoogleMapLongitude", model.FooterGoogleMapLongitude), new XElement("FooterGoogleMapLatitude", model.FooterGoogleMapLatitude), new XElement("FooterGoogleMapZoom", model.FooterGoogleMapZoom), new XElement("FooterGoogleMapVisibility", model.FooterGoogleMapVisibility), FooterCopyright, new XElement("FooterCopyrightVisibility", model.FooterCopyrightVisibility), new XElement("FooterSocialNetworksVisibility", model.FooterSocialNetworksVisibility), new XElement("LanguageId", model.LanguageId), new XElement("FooterMainMenuVisibility", model.FooterMainMenuVisibility), new XElement("FooterSliderVisibility", model.FooterSliderVisibility), new XElement("FooterCustomMenuVisibility", model.FooterCustomMenuVisibility), FooterAdvertising, new XElement("FooterAdvertisingVisibility", model.FooterAdvertisingVisibility), new XElement("FooterWowVisibility", model.FooterWowVisibility), new XElement("MainMenuType", model.MainMenuType), new XElement("HeaderAdvertisingLayout", model.HeaderAdvertisingLayout), new XElement("HeaderWorkNumberLayout", model.HeaderWorkNumberLayout), new XElement("HeaderEmailLayout", model.HeaderEmailLayout), new XElement("HeaderSocialNetworksLayout", model.HeaderSocialNetworksLayout), new XElement("HeaderLogoLayout", model.HeaderLogoLayout), new XElement("HeaderLanguageLayout", model.HeaderLanguageLayout), new XElement("HeaderLoginLayout", model.HeaderLoginLayout), new XElement("HeaderRegistrationLayout", model.HeaderRegistrationLayout), new XElement("HeaderSearchLayout", model.HeaderSearchLayout), new XElement("HeaderBasketLayout", model.HeaderBasketLayout), new XElement("HeaderMainMenuLayout", model.HeaderMainMenuLayout), new XElement("HeaderSliderLayout", model.HeaderSliderLayout), new XElement("HeaderSiteMapLayout", model.HeaderSiteMapLayout), new XElement("FooterJoinNewsletterLayout", model.FooterJoinNewsletterLayout), new XElement("FooterContactUsModuleLayout", model.FooterContactUsModuleLayout), new XElement("FooterWorkNumberLayout", model.FooterWorkNumberLayout), new XElement("FooterEmailLayout", model.FooterEmailLayout), new XElement("FooterAddressLayout", model.FooterAddressLayout), new XElement("FooterGoogleMapLayout", model.FooterGoogleMapLayout), new XElement("FooterCopyrightLayout", model.FooterCopyrightLayout), new XElement("FooterSocialNetworksLayout", model.FooterSocialNetworksLayout), new XElement("FooterMainMenuLayout", model.FooterMainMenuLayout), new XElement("FooterSliderLayout", model.FooterSliderLayout), new XElement("FooterCustomMenuLayout", model.FooterCustomMenuLayout), new XElement("FooterAdvertisingLayout", model.FooterAdvertisingLayout)), new XElement("ItemSliderType", model.ItemSliderType), new XElement("SlideShowType", model.SlideShowType));
                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXMasterTheme(XMasterTheme model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("MasterThemes")
      .Elements("MasterTheme")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();

                target.Element("Id").Value = model.Id.ToString();
                if (model.HeaderAdvertising != null)
                    target.Element("HeaderAdvertising").ReplaceWith(new XElement("HeaderAdvertising", new XCData(model.HeaderAdvertising.ToString())));
                else
                    target.Element("HeaderAdvertising").Value = "";

                if (model.HeaderWorkNumber != null)
                    target.Element("HeaderWorkNumber").ReplaceWith(new XElement("HeaderWorkNumber", new XCData(model.HeaderWorkNumber.ToString())));
                else
                    target.Element("HeaderWorkNumber").Value = "";

                if (model.HeaderEmail != null)
                    target.Element("HeaderEmail").ReplaceWith(new XElement("HeaderEmail", new XCData(model.HeaderEmail.ToString())));
                else
                    target.Element("HeaderEmail").Value = "";

                target.Element("HeaderAdvertisingVisibility").Value = model.HeaderAdvertisingVisibility.ToString();
                target.Element("HeaderWorkNumberVisibility").Value = model.HeaderWorkNumberVisibility.ToString();
                target.Element("HeaderEmailVisibility").Value = model.HeaderEmailVisibility.ToString();
                target.Element("HeaderSocialNetworksVisibility").Value = model.HeaderSocialNetworksVisibility.ToString();
                target.Element("HeaderLogoVisibility").Value = model.HeaderLogoVisibility.ToString();
                target.Element("HeaderLanguageVisibility").Value = model.HeaderLanguageVisibility.ToString();
                target.Element("HeaderLoginVisibility").Value = model.HeaderLoginVisibility.ToString();
                target.Element("HeaderRegistrationVisibility").Value = model.HeaderRegistrationVisibility.ToString();
                target.Element("HeaderSearchVisibility").Value = model.HeaderSearchVisibility.ToString();
                target.Element("HeaderBasketVisibility").Value = model.HeaderBasketVisibility.ToString();
                target.Element("HeaderMainMenuVisibility").Value = model.HeaderMainMenuVisibility.ToString();
                target.Element("HeaderSliderVisibility").Value = model.HeaderSliderVisibility.ToString();
                target.Element("HeaderSiteMapVisibility").Value = model.HeaderSiteMapVisibility.ToString();
                target.Element("FooterJoinNewsletterModuleVisibility").Value = model.FooterJoinNewsletterModuleVisibility.ToString();
                target.Element("FooterContactUsModuleVisibility").Value = model.FooterContactUsModuleVisibility.ToString();
                target.Element("FooterWorkNumberVisibility").Value = model.FooterWorkNumberVisibility.ToString();
                target.Element("FooterEmailVisibility").Value = model.FooterEmailVisibility.ToString();
                target.Element("FooterAddressVisibility").Value = model.FooterAddressVisibility.ToString();
                target.Element("FooterGoogleMapVisibility").Value = model.FooterGoogleMapVisibility.ToString();
                target.Element("FooterCopyrightVisibility").Value = model.FooterCopyrightVisibility.ToString();
                target.Element("FooterSocialNetworksVisibility").Value = model.FooterSocialNetworksVisibility.ToString();
                target.Element("LanguageId").Value = model.LanguageId.ToString();
                target.Element("FooterMainMenuVisibility").Value = model.FooterMainMenuVisibility.ToString();
                target.Element("FooterSliderVisibility").Value = model.FooterSliderVisibility.ToString();
                target.Element("FooterCustomMenuVisibility").Value = model.FooterCustomMenuVisibility.ToString();
                target.Element("MainMenuType").Value = model.MainMenuType.ToString();
                target.Element("SlideShowType").Value = model.SlideShowType.ToString();
                target.Element("ItemSliderType").Value = model.ItemSliderType.ToString();


                if (model.HeaderAdvertisingLayout != null)
                    target.Element("HeaderAdvertisingLayout").Value = model.HeaderAdvertisingLayout.ToString();
                else
                    target.Element("HeaderAdvertisingLayout").Value = "";

                if (model.HeaderWorkNumberLayout != null)
                    target.Element("HeaderWorkNumberLayout").Value = model.HeaderWorkNumberLayout.ToString();
                else
                    target.Element("HeaderWorkNumberLayout").Value = "";

                if (model.HeaderEmailLayout != null)
                    target.Element("HeaderEmailLayout").Value = model.HeaderEmailLayout.ToString();
                else
                    target.Element("HeaderEmailLayout").Value = "";

                if (model.HeaderSocialNetworksLayout != null)
                    target.Element("HeaderSocialNetworksLayout").Value = model.HeaderSocialNetworksLayout.ToString();
                else
                    target.Element("HeaderSocialNetworksLayout").Value = "";

                if (model.HeaderLogoLayout != null)
                    target.Element("HeaderLogoLayout").Value = model.HeaderLogoLayout.ToString();
                else
                    target.Element("HeaderLogoLayout").Value = "";

                if (model.HeaderLanguageLayout != null)
                    target.Element("HeaderLanguageLayout").Value = model.HeaderLanguageLayout.ToString();
                else
                    target.Element("HeaderLanguageLayout").Value = "";

                if (model.HeaderLoginLayout != null)
                    target.Element("HeaderLoginLayout").Value = model.HeaderLoginLayout.ToString();
                else
                    target.Element("HeaderLoginLayout").Value = "";

                if (model.HeaderRegistrationLayout != null)
                    target.Element("HeaderRegistrationLayout").Value = model.HeaderRegistrationLayout.ToString();
                else
                    target.Element("HeaderRegistrationLayout").Value = "";

                if (model.HeaderSearchLayout != null)
                    target.Element("HeaderSearchLayout").Value = model.HeaderSearchLayout.ToString();
                else
                    target.Element("HeaderSearchLayout").Value = "";

                if (model.HeaderBasketLayout != null)
                    target.Element("HeaderBasketLayout").Value = model.HeaderBasketLayout.ToString();
                else
                    target.Element("HeaderBasketLayout").Value = "";

                if (model.HeaderMainMenuLayout != null)
                    target.Element("HeaderMainMenuLayout").Value = model.HeaderMainMenuLayout.ToString();
                else
                    target.Element("HeaderMainMenuLayout").Value = "";

                if (model.HeaderSliderLayout != null)
                    target.Element("HeaderSliderLayout").Value = model.HeaderSliderLayout.ToString();
                else
                    target.Element("HeaderSliderLayout").Value = "";

                if (model.HeaderSiteMapLayout != null)
                    target.Element("HeaderSiteMapLayout").Value = model.HeaderSiteMapLayout.ToString();
                else
                    target.Element("HeaderSiteMapLayout").Value = "";

                if (model.FooterJoinNewsletterLayout != null)
                    target.Element("FooterJoinNewsletterLayout").Value = model.FooterJoinNewsletterLayout.ToString();
                else
                    target.Element("FooterJoinNewsletterLayout").Value = "";

                if (model.FooterContactUsModuleLayout != null)
                    target.Element("FooterContactUsModuleLayout").Value = model.FooterContactUsModuleLayout.ToString();
                else
                    target.Element("FooterContactUsModuleLayout").Value = "";

                if (model.FooterWorkNumberLayout != null)
                    target.Element("FooterWorkNumberLayout").Value = model.FooterWorkNumberLayout.ToString();
                else
                    target.Element("FooterWorkNumberLayout").Value = "";

                if (model.FooterEmailLayout != null)
                    target.Element("FooterEmailLayout").Value = model.FooterEmailLayout.ToString();
                else
                    target.Element("FooterEmailLayout").Value = "";

                if (model.FooterAddressLayout != null)
                    target.Element("FooterAddressLayout").Value = model.FooterAddressLayout.ToString();
                else
                    target.Element("FooterAddressLayout").Value = "";

                if (model.FooterGoogleMapLayout != null)
                    target.Element("FooterGoogleMapLayout").Value = model.FooterGoogleMapLayout.ToString();
                else
                    target.Element("FooterGoogleMapLayout").Value = "";

                if (model.FooterCopyrightLayout != null)
                    target.Element("FooterCopyrightLayout").Value = model.FooterCopyrightLayout.ToString();
                else
                    target.Element("FooterCopyrightLayout").Value = "";

                if (model.FooterSocialNetworksLayout != null)
                    target.Element("FooterSocialNetworksLayout").Value = model.FooterSocialNetworksLayout.ToString();
                else
                    target.Element("FooterSocialNetworksLayout").Value = "";

                if (model.FooterMainMenuLayout != null)
                    target.Element("FooterMainMenuLayout").Value = model.FooterMainMenuLayout.ToString();
                else
                    target.Element("FooterMainMenuLayout").Value = "";

                if (model.FooterSliderLayout != null)
                    target.Element("FooterSliderLayout").Value = model.FooterSliderLayout.ToString();
                else
                    target.Element("FooterSliderLayout").Value = "";

                if (model.FooterCustomMenuLayout != null)
                    target.Element("FooterCustomMenuLayout").Value = model.FooterCustomMenuLayout.ToString();
                else
                    target.Element("FooterCustomMenuLayout").Value = "";

                if (model.FooterAdvertisingLayout != null)
                    target.Element("FooterAdvertisingLayout").Value = model.FooterAdvertisingLayout.ToString();
                else
                    target.Element("FooterAdvertisingLayout").Value = "";


                if (model.FooterGoogleMapLongitude != null)
                    target.Element("FooterGoogleMapLongitude").ReplaceWith(new XElement("FooterGoogleMapLongitude", model.FooterGoogleMapLongitude));
                else
                    target.Element("FooterGoogleMapLongitude").Value = "";

                if (model.FooterGoogleMapLatitude != null)
                    target.Element("FooterGoogleMapLatitude").ReplaceWith(new XElement("FooterGoogleMapLatitude", model.FooterGoogleMapLatitude));
                else
                    target.Element("FooterGoogleMapLatitude").Value = "";

                if (model.FooterGoogleMapZoom != null)
                    target.Element("FooterGoogleMapZoom").ReplaceWith(new XElement("FooterGoogleMapZoom", model.FooterGoogleMapZoom));
                else
                    target.Element("FooterGoogleMapZoom").Value = "";

                if (model.FooterAdvertising != null)
                    target.Element("FooterAdvertising").ReplaceWith(new XElement("FooterAdvertising", new XCData(model.FooterAdvertising.ToString())));
                else
                    target.Element("FooterAdvertising").Value = "";

                if (model.FooterWorkNumber != null)
                    target.Element("FooterWorkNumber").ReplaceWith(new XElement("FooterWorkNumber", new XCData(model.FooterWorkNumber.ToString())));
                else
                    target.Element("FooterWorkNumber").Value = "";

                if (model.FooterEmail != null)
                    target.Element("FooterEmail").ReplaceWith(new XElement("FooterEmail", new XCData(model.FooterEmail.ToString())));
                else
                    target.Element("FooterEmail").Value = "";

                if (model.FooterAddress != null)
                    target.Element("FooterAddress").ReplaceWith(new XElement("FooterAddress", new XCData(model.FooterAddress.ToString())));
                else
                    target.Element("FooterAddress").Value = "";

                if (model.FooterCopyright != null)
                    target.Element("FooterCopyright").ReplaceWith(new XElement("FooterCopyright", new XCData(model.FooterCopyright.ToString())));
                else
                    target.Element("FooterCopyright").Value = "";

                target.Element("FooterAdvertisingVisibility").Value = model.FooterAdvertisingVisibility.ToString();
                target.Element("FooterWowVisibility").Value = model.FooterWowVisibility.ToString();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXMasterThemeGoogleMap(int LanguageId, string FooterGoogleMapLongitude, string FooterGoogleMapLatitude, string FooterGoogleMapZoom)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("MasterThemes")
      .Elements("MasterTheme")
      .Where(e => e.Element("LanguageId").Value == LanguageId.ToString())
      .Single();



                target.Element("FooterGoogleMapLongitude").ReplaceWith(new XElement("FooterGoogleMapLongitude", FooterGoogleMapLongitude));

                target.Element("FooterGoogleMapLatitude").ReplaceWith(new XElement("FooterGoogleMapLatitude", FooterGoogleMapLatitude));

                target.Element("FooterGoogleMapZoom").ReplaceWith(new XElement("FooterGoogleMapZoom", FooterGoogleMapZoom));


                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXMasterThemeFromUser(XMasterTheme model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("MasterThemes")
      .Elements("MasterTheme")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();

                target.Element("Id").Value = model.Id.ToString();

                if (model.HeaderAdvertising != null)
                    target.Element("HeaderAdvertising").ReplaceWith(new XElement("HeaderAdvertising", new XCData(model.HeaderAdvertising.ToString())));


                if (model.HeaderWorkNumber != null)
                    target.Element("HeaderWorkNumber").ReplaceWith(new XElement("HeaderWorkNumber", new XCData(model.HeaderWorkNumber.ToString())));


                if (model.HeaderEmail != null)
                    target.Element("HeaderEmail").ReplaceWith(new XElement("HeaderEmail", new XCData(model.HeaderEmail.ToString())));


                if (model.FooterAdvertising != null)
                    target.Element("FooterAdvertising").ReplaceWith(new XElement("FooterAdvertising", new XCData(model.FooterAdvertising.ToString())));


                if (model.FooterWorkNumber != null)
                    target.Element("FooterWorkNumber").ReplaceWith(new XElement("FooterWorkNumber", new XCData(model.FooterWorkNumber.ToString())));


                if (model.FooterEmail != null)
                    target.Element("FooterEmail").ReplaceWith(new XElement("FooterEmail", new XCData(model.FooterEmail.ToString())));


                if (model.FooterAddress != null)
                    target.Element("FooterAddress").ReplaceWith(new XElement("FooterAddress", new XCData(model.FooterAddress.ToString())));

                if (model.FooterGoogleMapLongitude != null)
                    target.Element("FooterGoogleMapLongitude").Value = model.FooterGoogleMapLongitude.ToString();
                if (model.FooterGoogleMapLatitude != null)
                    target.Element("FooterGoogleMapLatitude").Value = model.FooterGoogleMapLatitude.ToString();
                if (model.FooterGoogleMapZoom != null)
                    target.Element("FooterGoogleMapZoom").Value = model.FooterGoogleMapZoom.ToString();

                if (model.FooterCopyright != null)
                    target.Element("FooterCopyright").ReplaceWith(new XElement("FooterCopyright", new XCData(model.FooterCopyright.ToString())));


                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveXMasterTheme(int id)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("MasterThemes")
      .Elements("MasterTheme")
      .Where(e => e.Element("Id").Value == id.ToString())
      .Single();

                target.Remove();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region DefaultThemes

        public List<XDefaultTheme> ListOfXDefaultThemeByLanguage(int LanguageId)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
           ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Theme.xml"));
            var XDefaultThemes = new List<XDefaultTheme>();
            XDefaultThemes = (from rows in ds.Tables["DefaultTheme"].AsEnumerable()
                              where rows.Field<string>("languageId") == LanguageId.ToString()
                              select new XDefaultTheme
                              {
                                  Id = Convert.ToInt32(rows[0].ToString()),
                                  TypeId = Convert.ToInt16(rows[1].ToString()),
                                  TypeOrderShow = Convert.ToInt16(rows[2].ToString()),
                                  NumberOfRecord = Convert.ToInt16(rows[3].ToString()),
                                  DisplaySort = Convert.ToInt32(rows[4].ToString()),
                                  LanguageId = Convert.ToInt16(rows[5].ToString()),
                                  Title = (rows[6] != null ? rows[6].ToString() : null),
                                  LinkId = rows[7].ToString(),
                                  HtmlData = rows[8].ToString(),
                                  Layout = rows[9].ToString()
                              }).ToList();

            return XDefaultThemes;
        }
        public List<XDefaultTheme> ListOfXDefaultTheme()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
           ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Theme.xml"));
            var XDefaultThemes = new List<XDefaultTheme>();
            XDefaultThemes = (from rows in ds.Tables["DefaultTheme"].AsEnumerable()
                              select new XDefaultTheme
                              {
                                  Id = Convert.ToInt32(rows[0].ToString()),
                                  TypeId = Convert.ToInt16(rows[1].ToString()),
                                  TypeOrderShow = Convert.ToInt16(rows[2].ToString()),
                                  NumberOfRecord = Convert.ToInt16(rows[3].ToString()),
                                  DisplaySort = Convert.ToInt32(rows[4].ToString()),
                                  LanguageId = Convert.ToInt16(rows[5].ToString()),
                                  Title = (rows[6] != null ? rows[6].ToString() : null),
                                  LinkId = rows[7].ToString(),
                                  HtmlData = rows[8].ToString(),
                                  Layout = rows[9].ToString()
                              }).ToList();

            return XDefaultThemes;
        }

        public XDefaultTheme DetailOfXDefaultTheme(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
           ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Theme.xml"));
            XDefaultTheme XDefaultTheme = (from rows in ds.Tables["DefaultTheme"].AsEnumerable()
                                           where rows.Field<string>("Id") == Id.ToString()
                                           select new XDefaultTheme
                                           {
                                               Id = Convert.ToInt32(rows[0].ToString()),
                                               TypeId = Convert.ToInt16(rows[1].ToString()),
                                               TypeOrderShow = Convert.ToInt16(rows[2].ToString()),
                                               NumberOfRecord = Convert.ToInt16(rows[3].ToString()),
                                               DisplaySort = Convert.ToInt32(rows[4].ToString()),
                                               LanguageId = Convert.ToInt16(rows[5].ToString()),
                                               Title = (rows[6] != null ? rows[6].ToString() : null),
                                               LinkId = rows[7].ToString(),
                                               HtmlData = rows[8].ToString(),
                                               Layout = rows[9].ToString()
                                           }).SingleOrDefault();

            return XDefaultTheme;
        }

        public bool CreateOfXDefaultTheme(XDefaultTheme model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));
                XElement DefaultThemes = originalXml.Root.Element("DefaultThemes");
                XElement HtmlData = new XElement("HtmlData", "");
                if (model.HtmlData != null)
                    HtmlData = new XElement("HtmlData", new XCData(model.HtmlData));

                DefaultThemes.Add(new XElement("DefaultTheme", new XElement("Id", model.Id), new XElement("TypeId", model.TypeId), new XElement("TypeOrderShow", model.TypeOrderShow), new XElement("NumberOfRecord", model.NumberOfRecord), new XElement("DisplaySort", model.DisplaySort), new XElement("LanguageId", model.LanguageId), new XElement("Title", model.Title), new XElement("LinkId", model.LinkId), HtmlData, new XElement("Layout", model.Layout)));

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXDefaultTheme(XDefaultTheme model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("DefaultThemes")
      .Elements("DefaultTheme")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();

                target.Element("TypeId").Value = model.TypeId.ToString();
                target.Element("TypeOrderShow").Value = model.TypeOrderShow.ToString();
                target.Element("NumberOfRecord").Value = model.NumberOfRecord.ToString();
                target.Element("DisplaySort").Value = model.DisplaySort.ToString();
                target.Element("LanguageId").Value = model.LanguageId.ToString();
                if (model.Title != null)
                    target.Element("Title").Value = model.Title;
                else
                    target.Element("Title").Value = "";

                if (model.LinkId != null)
                    target.Element("LinkId").Value = model.LinkId.ToString();
                else
                    target.Element("LinkId").Value = "";

                if (model.HtmlData != null)
                    target.Element("HtmlData").ReplaceWith(new XElement("HtmlData", new XCData(model.HtmlData.ToString())));
                else
                    target.Element("HtmlData").Value = "";
                target.Element("Layout").Value = model.Layout.ToString();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXDefaultThemeUser(XDefaultTheme model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("DefaultThemes")
      .Elements("DefaultTheme")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();


                if (model.HtmlData != null)
                    target.Element("HtmlData").ReplaceWith(new XElement("HtmlData", new XCData(model.HtmlData.ToString())));


                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXDefaultThemeSort(XDefaultTheme model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("DefaultThemes")
      .Elements("DefaultTheme")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();

                target.Element("DisplaySort").Value = model.DisplaySort.ToString();
                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveXDefaultTheme(int id)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("DefaultThemes")
      .Elements("DefaultTheme")
      .Where(e => e.Element("Id").Value == id.ToString())
      .Single();

                target.Remove();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region InternalThemes
        public List<XInternalTheme> ListOfXInternalTheme()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
           ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Theme.xml"));
            var InternalThemes = new List<XInternalTheme>();
            InternalThemes = (from rows in ds.Tables["InternalTheme"].AsEnumerable()
                              select new XInternalTheme
                              {
                                  Id = Convert.ToInt32(rows[0].ToString()),
                                  ViewId = Convert.ToInt16(rows[1].ToString()),
                                  LanguageId = Convert.ToInt16(rows[2].ToString()),
                                  ContentTypeId = Convert.ToInt32(rows[3].ToString()),
                              }).ToList();

            return InternalThemes;
        }
        public XInternalTheme DetailOfXInternalTheme(int Id, int LanguageId, int ContentTypeId)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
           ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Theme.xml"));
            XInternalTheme InternalTheme = (from rows in ds.Tables["InternalTheme"].AsEnumerable()
                                            where rows.Field<string>("Id") == Id.ToString() && rows.Field<string>("LanguageId") == LanguageId.ToString() && rows.Field<string>("ContentTypeId") == ContentTypeId.ToString()
                                            select new XInternalTheme
                                            {
                                                Id = Convert.ToInt32(rows[0].ToString()),
                                                ViewId = Convert.ToInt16(rows[1].ToString()),
                                                LanguageId = Convert.ToInt16(rows[2].ToString()),
                                                ContentTypeId = Convert.ToInt16(rows[3].ToString())
                                            }).SingleOrDefault();

            return InternalTheme;
        }
        public XInternalTheme DetailOfXInternalTheme(int Id, int LanguageId)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Xml/Theme.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
           ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Theme.xml"));
            XInternalTheme InternalTheme = (from rows in ds.Tables["InternalTheme"].AsEnumerable()
                                            where rows.Field<string>("Id") == Id.ToString() && rows.Field<string>("LanguageId") == LanguageId.ToString()
                                            select new XInternalTheme
                                            {
                                                Id = Convert.ToInt32(rows[0].ToString()),
                                                ViewId = Convert.ToInt16(rows[1].ToString()),
                                                LanguageId = Convert.ToInt16(rows[2].ToString())
                                            }).SingleOrDefault();

            return InternalTheme;
        }
        public bool CreateOfXInternalTheme(XInternalTheme model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));
                XElement InternalTheme = originalXml.Root.Element("InternalThemes");
                XElement HtmlData = new XElement("HtmlData", "");

                if (model.ContentTypeId != -1)
                    InternalTheme.Add(new XElement("InternalTheme", new XElement("Id", model.Id), new XElement("ViewId", model.ViewId), new XElement("LanguageId", model.LanguageId), new XElement("ContentTypeId", model.ContentTypeId)));
                else
                    InternalTheme.Add(new XElement("InternalTheme", new XElement("Id", model.Id), new XElement("ViewId", model.ViewId), new XElement("LanguageId", model.LanguageId), new XElement("ContentTypeId", -1)));

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool EditXInternalTheme(XInternalTheme model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("InternalThemes")
      .Elements("InternalTheme")
      .Where(e => e.Element("Id").Value == model.Id.ToString() && e.Element("LanguageId").Value == model.LanguageId.ToString() && e.Element("ContentTypeId").Value == model.ContentTypeId.ToString())
      .Single();

                target.Element("ViewId").Value = model.ViewId.ToString();
                target.Element("LanguageId").Value = model.LanguageId.ToString();
                target.Element("ContentTypeId").Value = model.ContentTypeId.ToString();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool RemoveXInternalTheme(int id, int LanguageId, int ContentTypeId)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("InternalThemes")
      .Elements("InternalTheme")
      .Where(e => e.Element("Id").Value == id.ToString() && e.Element("LanguageId").Value == LanguageId.ToString() && e.Element("ContentTypeId").Value == ContentTypeId.ToString())
      .Single();

                target.Remove();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool RemoveXInternalTheme(int id, int LanguageId)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                var target = originalXml.Root
      .Element("InternalThemes")
      .Elements("InternalTheme")
      .Where(e => e.Element("Id").Value == id.ToString() && e.Element("LanguageId").Value == LanguageId.ToString())
      .Single();

                target.Remove();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Xml/Theme.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region States
        public List<XState> ListOfXState()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  

            ds.ReadXml(HttpContext.Current.Server.MapPath(Url + "/Content/Xml/Data.xml")); var XStates = new List<XState>();
            XStates = (from rows in ds.Tables["State"].AsEnumerable()
                       select new XState
                       {
                           Id = Convert.ToInt16(rows[0].ToString()),
                           Name = rows[1].ToString(),
                           LanguageId = Convert.ToInt16(rows[2])
                       }).ToList();

            return XStates;
        }

        public XState DetailOfXState(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            XState XState = (from rows in ds.Tables["State"].AsEnumerable()
                             where rows.Field<string>("Id") == Id.ToString()
                             select new XState
                             {
                                 Id = Convert.ToInt16(rows[0].ToString()),
                                 Name = rows[1].ToString(),
                                 LanguageId = Convert.ToInt16(rows[2])
                             }).SingleOrDefault();

            return XState;
        }

        public bool CreateOfXState(XState model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));
                XElement States = originalXml.Root.Element("States");
                States.Add(new XElement("State", new XElement("Id", model.Id), new XElement("Name", model.Name), new XElement("languageId", model.LanguageId)));

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXState(XState model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("States")
      .Elements("State")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();

                target.Element("Name").Value = model.Name;
                target.Element("languageId").Value = model.LanguageId.ToString();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region CurrentInnerState
        public List<XCurrentInnerState> ListOfXCurrentInnerState()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            var XCurrentInnerStates = new List<XCurrentInnerState>();
            XCurrentInnerStates = (from rows in ds.Tables["CurrentInnerState"].AsEnumerable()
                                   select new XCurrentInnerState
                                   {
                                       Id = Convert.ToInt16(rows[0].ToString())
                                   }).ToList();

            return XCurrentInnerStates;
        }

        public XCurrentInnerState DetailOfXCurrentInnerState(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            XCurrentInnerState XCurrentInnerState = (from rows in ds.Tables["CurrentInnerState"].AsEnumerable()
                                                     where rows.Field<string>("Id") == Id.ToString()
                                                     select new XCurrentInnerState
                                                     {
                                                         Id = Convert.ToInt16(rows[0].ToString())
                                                     }).SingleOrDefault();

            return XCurrentInnerState;
        }

        public bool CreateOfXCurrentInnerState(XCurrentInnerState model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));
                XElement CurrentInnerStates = originalXml.Root.Element("CurrentInnerStates");
                CurrentInnerStates.Add(new XElement("CurrentInnerState", new XElement("Id", model.Id)));

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool RemoveXCurrentInnerState(int id)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("CurrentInnerStates")
      .Elements("CurrentInnerState")
      .Where(e => e.Element("Id").Value == id.ToString())
      .Single();

                target.Remove();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region CurrentCity
        public List<XCurrentCity> ListOfXCurrentCity()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            var XCurrentCities = new List<XCurrentCity>();
            XCurrentCities = (from rows in ds.Tables["CurrentCity"].AsEnumerable()
                              select new XCurrentCity
                              {
                                  Name = rows[0].ToString()
                              }).ToList();

            return XCurrentCities;
        }

        public XCurrentCity DetailOfXCurrentCity(string Name)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            XCurrentCity XCurrentCity = (from rows in ds.Tables["CurrentCity"].AsEnumerable()
                                         where rows.Field<string>("Name") == Name.ToString()
                                         select new XCurrentCity
                                         {
                                             Name = rows[0].ToString()
                                         }).SingleOrDefault();

            return XCurrentCity;
        }

        public bool EditXCurrentCity(XCurrentCity model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("CurrentCities")
      .Elements("CurrentCity")
      .First();

                target.Element("Name").Value = model.Name;

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region CurrentState
        public List<XCurrentState> ListOfXCurrentState()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            var XCurrentStates = new List<XCurrentState>();
            XCurrentStates = (from rows in ds.Tables["CurrentState"].AsEnumerable()
                              select new XCurrentState
                              {
                                  StateId = Convert.ToInt32(rows[0])
                              }).ToList();

            return XCurrentStates;
        }

        public XCurrentState DetailOfXCurrentState(int StateId)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            XCurrentState XCurrentState = (from rows in ds.Tables["CurrentCity"].AsEnumerable()
                                         where rows.Field<string>("StateId") == StateId.ToString()
                                         select new XCurrentState
                                         {
                                             StateId = Convert.ToInt32(rows[0])
                                         }).SingleOrDefault();

            return XCurrentState;
        }

        public bool EditXCurrentState(XCurrentState model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("CurrentStates")
      .Elements("CurrentState")
      .First();

                target.Element("StateId").Value = model.StateId.ToString();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception x)
            {
                return false;
            }
        }

        #endregion

        #region Banks
        public List<XBank> ListOfXBank()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            var XBanks = new List<XBank>();
            XBanks = (from rows in ds.Tables["Bank"].AsEnumerable()
                      select new XBank
                      {
                          Id = Convert.ToInt16(rows[0].ToString()),
                          Name = rows[1].ToString(),
                          LanguageId = Convert.ToInt16(rows[2])
                      }).ToList();

            return XBanks;
        }

        public XBank DetailOfXBank(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            XBank XBank = (from rows in ds.Tables["Bank"].AsEnumerable()
                           where rows.Field<string>("Id") == Id.ToString()
                           select new XBank
                           {
                               Id = Convert.ToInt16(rows[0].ToString()),
                               Name = rows[1].ToString(),
                               LanguageId = Convert.ToInt16(rows[2])
                           }).SingleOrDefault();

            return XBank;
        }

        public bool CreateOfXBank(XBank model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));
                XElement Banks = originalXml.Root.Element("Banks");
                Banks.Add(new XElement("Bank", new XElement("Id", model.Id), new XElement("Name", model.Name), new XElement("languageId", model.LanguageId)));

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXBank(XBank model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("Banks")
      .Elements("Bank")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();

                target.Element("Name").Value = model.Name;
                target.Element("languageId").Value = model.LanguageId.ToString();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region PostPrices
        public List<XPostPrice> ListOfXPostPrice()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            var XPostPrices = new List<XPostPrice>();
            XPostPrices = (from rows in ds.Tables["PostPrice"].AsEnumerable()
                           select new XPostPrice
                           {
                               Post_Inner_City_250 = Convert.ToInt32(rows[1].ToString()),
                               Post_Inner_City_500 = Convert.ToInt32(rows[2].ToString()),
                               Post_Inner_City_1000 = Convert.ToInt32(rows[3]),
                               Post_Inner_City_2000 = Convert.ToInt32(rows[4]),
                               Post_Inner_City_More_2000 = Convert.ToInt32(rows[5]),
                               Post_Inner_State_250 = Convert.ToInt32(rows[6]),
                               Post_Inner_State_500 = Convert.ToInt32(rows[7]),
                               Post_Inner_State_1000 = Convert.ToInt32(rows[8]),
                               Post_Inner_State_2000 = Convert.ToInt32(rows[9]),
                               Post_Inner_State_More_2000 = Convert.ToInt32(rows[10]),
                               Post_Outer_State_Neighbor_250 = Convert.ToInt32(rows[11]),
                               Post_Outer_State_Neighbor_500 = Convert.ToInt32(rows[12]),
                               Post_Outer_State_Neighbor_1000 = Convert.ToInt32(rows[13]),
                               Post_Outer_State_Neighbor_2000 = Convert.ToInt32(rows[14]),
                               Post_Outer_State_Neighbor_More_2000 = Convert.ToInt32(rows[15]),
                               Post_Outer_State_NoNeighbor_250 = Convert.ToInt32(rows[16]),
                               Post_Outer_State_NoNeighbor_500 = Convert.ToInt32(rows[17]),
                               Post_Outer_State_NoNeighbor_1000 = Convert.ToInt32(rows[18]),
                               Post_Outer_State_NoNeighbor_2000 = Convert.ToInt32(rows[19]),
                               Post_Outer_State_NoNeighbor_More_2000 = Convert.ToInt32(rows[20])
                           }).ToList();

            return XPostPrices;
        }

        public XPostPrice DetailOfXPostPrice(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            XPostPrice XPostPrice = (from rows in ds.Tables["PostPrice"].AsEnumerable()
                                     where rows.Field<string>("Id") == Id.ToString()
                                     select new XPostPrice
                                     {
                                         Post_Inner_City_250 = Convert.ToInt32(rows[1].ToString()),
                                         Post_Inner_City_500 = Convert.ToInt32(rows[2].ToString()),
                                         Post_Inner_City_1000 = Convert.ToInt32(rows[3]),
                                         Post_Inner_City_2000 = Convert.ToInt32(rows[4]),
                                         Post_Inner_City_More_2000 = Convert.ToInt32(rows[5]),
                                         Post_Inner_State_250 = Convert.ToInt32(rows[6]),
                                         Post_Inner_State_500 = Convert.ToInt32(rows[7]),
                                         Post_Inner_State_1000 = Convert.ToInt32(rows[8]),
                                         Post_Inner_State_2000 = Convert.ToInt32(rows[9]),
                                         Post_Inner_State_More_2000 = Convert.ToInt32(rows[10]),
                                         Post_Outer_State_Neighbor_250 = Convert.ToInt32(rows[11]),
                                         Post_Outer_State_Neighbor_500 = Convert.ToInt32(rows[12]),
                                         Post_Outer_State_Neighbor_1000 = Convert.ToInt32(rows[13]),
                                         Post_Outer_State_Neighbor_2000 = Convert.ToInt32(rows[14]),
                                         Post_Outer_State_Neighbor_More_2000 = Convert.ToInt32(rows[15]),
                                         Post_Outer_State_NoNeighbor_250 = Convert.ToInt32(rows[16]),
                                         Post_Outer_State_NoNeighbor_500 = Convert.ToInt32(rows[17]),
                                         Post_Outer_State_NoNeighbor_1000 = Convert.ToInt32(rows[18]),
                                         Post_Outer_State_NoNeighbor_2000 = Convert.ToInt32(rows[19]),
                                         Post_Outer_State_NoNeighbor_More_2000 = Convert.ToInt32(rows[20])
                                     }).SingleOrDefault();

            return XPostPrice;
        }


        public bool EditXPostPrice(XPostPrice model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("PostPrices")
      .Elements("PostPrice")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();


                target.Element("Post_Inner_City_250").Value = model.Post_Inner_City_250.ToString();
                target.Element("Post_Inner_City_500").Value = model.Post_Inner_City_500.ToString();
                target.Element("Post_Inner_City_1000").Value = model.Post_Inner_City_1000.ToString();
                target.Element("Post_Inner_City_2000").Value = model.Post_Inner_City_2000.ToString();
                target.Element("Post_Inner_City_More_2000").Value = model.Post_Inner_City_More_2000.ToString();
                target.Element("Post_Inner_State_250").Value = model.Post_Inner_State_250.ToString();
                target.Element("Post_Inner_State_500").Value = model.Post_Inner_State_500.ToString();
                target.Element("Post_Inner_State_1000").Value = model.Post_Inner_State_1000.ToString();
                target.Element("Post_Inner_State_2000").Value = model.Post_Inner_State_2000.ToString();
                target.Element("Post_Inner_State_More_2000").Value = model.Post_Inner_State_More_2000.ToString();
                target.Element("Post_Outer_State_Neighbor_250").Value = model.Post_Outer_State_Neighbor_250.ToString();
                target.Element("Post_Outer_State_Neighbor_500").Value = model.Post_Outer_State_Neighbor_500.ToString();
                target.Element("Post_Outer_State_Neighbor_1000").Value = model.Post_Outer_State_Neighbor_1000.ToString();
                target.Element("Post_Outer_State_Neighbor_2000").Value = model.Post_Outer_State_Neighbor_2000.ToString();
                target.Element("Post_Outer_State_Neighbor_More_2000").Value = model.Post_Outer_State_Neighbor_More_2000.ToString();
                target.Element("Post_Outer_State_NoNeighbor_250").Value = model.Post_Outer_State_NoNeighbor_250.ToString();
                target.Element("Post_Outer_State_NoNeighbor_500").Value = model.Post_Outer_State_NoNeighbor_500.ToString();
                target.Element("Post_Outer_State_NoNeighbor_1000").Value = model.Post_Outer_State_NoNeighbor_1000.ToString();
                target.Element("Post_Outer_State_NoNeighbor_2000").Value = model.Post_Outer_State_NoNeighbor_2000.ToString();
                target.Element("Post_Outer_State_NoNeighbor_More_2000").Value = model.Post_Outer_State_NoNeighbor_More_2000.ToString();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region BikeDeliveries
        public List<XBikeDelivery> ListOfXBikeDelivery()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            var XBikeDeliveryies = new List<XBikeDelivery>();
            XBikeDeliveryies = (from rows in ds.Tables["BikeDelivery"].AsEnumerable()
                                select new XBikeDelivery
                                {
                                    Id = Convert.ToInt16(rows[0].ToString()),
                                    Price = Convert.ToInt16(rows[1].ToString()),
                                    CurrentState = Convert.ToInt16(rows[2])
                                }).ToList();

            return XBikeDeliveryies;
        }

        public XBikeDelivery DetailOfXBikeDelivery(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            XBikeDelivery XBikeDelivery = (from rows in ds.Tables["BikeDelivery"].AsEnumerable()
                                           where rows.Field<string>("Id") == Id.ToString()
                                           select new XBikeDelivery
                                           {
                                               Id = Convert.ToInt16(rows[0].ToString()),
                                               Price = Convert.ToInt16(rows[1].ToString()),
                                               CurrentState = Convert.ToInt16(rows[2])
                                           }).SingleOrDefault();

            return XBikeDelivery;
        }


        public bool EditXBikeDelivery(XBikeDelivery model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("BikeDeliveries")
      .Elements("BikeDelivery")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();

                target.Element("Price").Value = model.Price.ToString();
                target.Element("CurrentState").Value = model.CurrentState.ToString();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region FreightPrices
        public List<XFreightPrice> ListOfXFreightPrice()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            var XFreightPrices = new List<XFreightPrice>();
            XFreightPrices = (from rows in ds.Tables["FreightPrice"].AsEnumerable()
                              select new XFreightPrice
                              {
                                  Id = Convert.ToInt16(rows[0].ToString()),
                                  Price = Convert.ToInt16(rows[1].ToString())
                              }).ToList();

            return XFreightPrices;
        }

        public XFreightPrice DetailOfXFreightPrice(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            XFreightPrice XFreightPrice = (from rows in ds.Tables["FreightPrice"].AsEnumerable()
                                           where rows.Field<string>("Id") == Id.ToString()
                                           select new XFreightPrice
                                           {
                                               Id = Convert.ToInt16(rows[0].ToString()),
                                               Price = Convert.ToInt16(rows[1].ToString())
                                           }).SingleOrDefault();

            return XFreightPrice;
        }


        public bool EditXFreightPrice(XFreightPrice model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("FreightPrices")
      .Elements("FreightPrice")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();

                target.Element("Price").Value = model.Price.ToString();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region SendWayEdit
        public List<XSendWayEdit> ListOfXSendWayEdit()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            var XSendWayEdits = new List<XSendWayEdit>();
            XSendWayEdits = (from rows in ds.Tables["SendWayEdit"].AsEnumerable()
                             select new XSendWayEdit
                             {
                                 TypeId = Convert.ToInt16(rows[0].ToString()),
                                 Edit = Convert.ToInt16(rows[1].ToString())
                             }).ToList();

            return XSendWayEdits;
        }

        public XSendWayEdit DetailOfXSendWayEdit(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            XSendWayEdit XSendWayEdits = (from rows in ds.Tables["SendWayEdit"].AsEnumerable()
                                          where rows.Field<string>("TypeId") == Id.ToString()
                                          select new XSendWayEdit
                                          {
                                              TypeId = Convert.ToInt16(rows[0].ToString()),
                                              Edit = Convert.ToInt16(rows[1].ToString())
                                          }).SingleOrDefault();

            return XSendWayEdits;
        }

        public bool CreateOfXSendWayEdit(XSendWayEdit model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));
                XElement SendWayEdits = originalXml.Root.Element("SendWayEdits");
                SendWayEdits.Add(new XElement("SendWayEdit", new XElement("TypeId", model.TypeId), new XElement("Edit", model.Edit)));

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXSendWayEdit(XSendWayEdit model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("SendWayEdits")
      .Elements("SendWayEdit")
      .Where(e => e.Element("TypeId").Value == model.TypeId.ToString())
      .Single();

                target.Element("TypeId").Value = model.TypeId.ToString();
                target.Element("Edit").Value = model.Edit.ToString();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region PaymentWayEdit
        public List<XPaymentWayEdit> ListOfXPaymentWayEdit()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            var XPaymentWayEdits = new List<XPaymentWayEdit>();
            XPaymentWayEdits = (from rows in ds.Tables["PaymentWayEdit"].AsEnumerable()
                                select new XPaymentWayEdit
                                {
                                    TypeId = Convert.ToInt16(rows[0].ToString()),
                                    Edit = Convert.ToInt16(rows[1].ToString())
                                }).ToList();

            return XPaymentWayEdits;
        }

        public XPaymentWayEdit DetailOfXPaymentWayEdit(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            XPaymentWayEdit XPaymentWayEdits = (from rows in ds.Tables["PaymentWayEdit"].AsEnumerable()
                                                where rows.Field<string>("TypeId") == Id.ToString()
                                                select new XPaymentWayEdit
                                                {
                                                    TypeId = Convert.ToInt16(rows[0].ToString()),
                                                    Edit = Convert.ToInt16(rows[1].ToString())
                                                }).SingleOrDefault();

            return XPaymentWayEdits;
        }

        public bool CreateOfXPaymentWayEdit(XPaymentWayEdit model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));
                XElement XPaymentWayEdits = originalXml.Root.Element("PaymentWayEdits");
                XPaymentWayEdits.Add(new XElement("PaymentWayEdit", new XElement("TypeId", model.TypeId), new XElement("Edit", model.Edit)));

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditXPaymentWayEdit(XPaymentWayEdit model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("PaymentWayEdits")
      .Elements("PaymentWayEdit")
      .Where(e => e.Element("TypeId").Value == model.TypeId.ToString())
      .Single();

                target.Element("TypeId").Value = model.TypeId.ToString();
                target.Element("Edit").Value = model.Edit.ToString();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Menus
        public List<XMenu> ListOfXMenu()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            if (ds.Tables["Menu"] != null)
            {
                var XMenus = new List<XMenu>();
                XMenus = (from rows in ds.Tables["Menu"].AsEnumerable()
                          select new XMenu
                          {
                              Id = Convert.ToInt16(rows[0].ToString()),
                              Title = rows[1].ToString(),
                              Link = rows[2].ToString(),
                              Cover = rows[3].ToString(),
                              PlaceShow = Convert.ToInt32(rows[4].ToString()),
                              DisplayOrder = Convert.ToInt32(rows[5].ToString()),
                              MenuId = rows[6] != null ? Convert.ToInt32(rows[6].ToString()) : 0,
                              LinkId = rows[7] != null ? Convert.ToInt32(rows[7].ToString()) : 0,
                              TypeId = rows[8] != null ? Convert.ToInt32(rows[8].ToString()) : 0,
                              Data = rows[9] != null ? rows[6].ToString() : ""
                          }).ToList();

                return XMenus;
            }
            else
                return null;
        }

        public List<XMenu> ListOfXMenu(int parrentId)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            if (ds.Tables["Menu"] != null)
            {
                var XMenus = new List<XMenu>();
                XMenus = (from rows in ds.Tables["Menu"].AsEnumerable()
                          where rows.Field<string>("MenuId") == parrentId.ToString()
                          select new XMenu
                          {
                              Id = Convert.ToInt16(rows[0].ToString()),
                              Title = rows[1].ToString(),
                              Link = rows[2].ToString(),
                              Cover = rows[3].ToString(),
                              PlaceShow = Convert.ToInt32(rows[4].ToString()),
                              DisplayOrder = Convert.ToInt32(rows[5].ToString()),
                              MenuId = rows[6] != null ? Convert.ToInt32(rows[6].ToString()) : 0,
                              LinkId = rows[7] != null ? Convert.ToInt32(rows[7].ToString()) : 0,
                              TypeId = rows[8] != null ? Convert.ToInt32(rows[8].ToString()) : 0,
                              Data = rows[9] != null ? rows[6].ToString() : ""
                          }).ToList();

                return XMenus;
            }
            else
                return null;
        }

        public List<XMenu> ListOfXMenuSecondLevel()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            if (ds.Tables["Menu"] != null)
            {
                var rootIds = (from rows in ds.Tables["Menu"].AsEnumerable()
                          where rows.Field<string>("MenuId") == "0"
                          select rows.Field<string>("id")
                        ).ToList();

                var XMenus = new List<XMenu>();
                XMenus = (from rows in ds.Tables["Menu"].AsEnumerable()

                          where (rootIds.Contains(rows.Field<string>("MenuId")) && rows.Field<string>("TypeId") == "8") || (rows.Field<string>("TypeId") != "8" && rows.Field<string>("TypeId") != "10")
                          
                          select new XMenu
                          {
                              Id = Convert.ToInt16(rows[0].ToString()),
                              Title = rows[1].ToString(),
                              Link = rows[2].ToString(),
                              Cover = rows[3].ToString(),
                              PlaceShow = Convert.ToInt32(rows[4].ToString()),
                              DisplayOrder = Convert.ToInt32(rows[5].ToString()),
                              MenuId = rows[6] != null ? Convert.ToInt32(rows[6].ToString()) : 0,
                              LinkId = rows[7] != null ? Convert.ToInt32(rows[7].ToString()) : 0,
                              TypeId = rows[8] != null ? Convert.ToInt32(rows[8].ToString()) : 0,
                              Data = rows[9] != null ? rows[6].ToString() : ""
                          }).ToList();

                return XMenus;

            }
            else
                return null;
        }

        public List<XMenu> ListOfXMenuSecondLevelWithChilds()
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            if (ds.Tables["Menu"] != null)
            {
                var rootIds = (from rows in ds.Tables["Menu"].AsEnumerable()
                               where rows.Field<string>("MenuId") == "0"
                               select rows.Field<string>("id")
                        ).ToList();

                var XMenus = new List<XMenu>();
                XMenus = (from rows in ds.Tables["Menu"].AsEnumerable()
                          where !rootIds.Contains(rows.Field<string>("id"))
                          select new XMenu
                          {
                              Id = Convert.ToInt16(rows[0].ToString()),
                              Title = rows[1].ToString(),
                              Link = rows[2].ToString(),
                              Cover = rows[3].ToString(),
                              PlaceShow = Convert.ToInt32(rows[4].ToString()),
                              DisplayOrder = Convert.ToInt32(rows[5].ToString()),
                              MenuId = rows[6] != null ? Convert.ToInt32(rows[6].ToString()) : 0,
                              LinkId = rows[7] != null ? Convert.ToInt32(rows[7].ToString()) : 0,
                              TypeId = rows[8] != null ? Convert.ToInt32(rows[8].ToString()) : 0,
                              Data = rows[9] != null ? rows[6].ToString() : ""
                          }).ToList();

                return XMenus;

            }
            else
                return null;
        }

        public XMenu DetailOfXMenu(int Id)
        {
            //string xmlData = HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml");//Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(HttpContext.Current.Server.MapPath( Url+ "/Content/Xml/Data.xml"));
            XMenu XMenu = (from rows in ds.Tables["Menu"].AsEnumerable()
                           where rows.Field<string>("Id") == Id.ToString()
                           select new XMenu
                           {
                               Id = Convert.ToInt16(rows[0].ToString()),
                               Title = rows[1].ToString(),
                               Link = rows[2].ToString(),
                               Cover = rows[3].ToString(),
                               PlaceShow = Convert.ToInt32(rows[4].ToString()),
                               DisplayOrder = Convert.ToInt32(rows[5].ToString()),
                               MenuId = rows[6] != null ? Convert.ToInt32(rows[6].ToString()) : 0,
                               LinkId = rows[7] != null ? Convert.ToInt32(rows[7].ToString()) : 0,
                               TypeId = rows[8] != null ? Convert.ToInt32(rows[8].ToString()) : 0,
                               Data = rows[9] != null ? rows[6].ToString() : ""
                           }).SingleOrDefault();

            return XMenu;
        }

        public string CreateOfXMenu(XMenu model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));
                XElement Menus = originalXml.Root.Element("Menus");
                if (Menus == null)
                {
                    originalXml.Root.Add(new XElement("Menus", new XElement("Menu", new XElement("Id", model.Id), new XElement("Title", model.Title), new XElement("Link", model.Link), new XElement("Cover", model.Cover), new XElement("PlaceShow", model.PlaceShow), new XElement("DisplayOrder", model.DisplayOrder), new XElement("MenuId", model.MenuId.HasValue ? model.MenuId : 0), new XElement("LinkId", model.LinkId.HasValue ? model.LinkId : 0), new XElement("TypeId", model.TypeId.HasValue ? model.TypeId : 0), new XElement("Data", new XCData(model.Data == null ? "" : model.Data)))));
                }
                else
                {

                    Menus.Add(new XElement("Menu", new XElement("Id", model.Id), new XElement("Title", model.Title), new XElement("Link", model.Link), new XElement("Cover", model.Cover), new XElement("PlaceShow", model.PlaceShow), new XElement("DisplayOrder", model.DisplayOrder), new XElement("MenuId", model.MenuId.HasValue ? model.MenuId : 0), new XElement("LinkId", model.LinkId.HasValue ? model.LinkId : 0), new XElement("TypeId", model.TypeId.HasValue ? model.TypeId : 0), new XElement("Data", new XCData(model.Data==null?"":model.Data))));
                }
                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return "";
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }

        public string EditXMenu(XMenu model)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("Menus")
      .Elements("Menu")
      .Where(e => e.Element("Id").Value == model.Id.ToString())
      .Single();

                target.Element("Title").Value = model.Title;
                target.Element("Link").Value = model.Link;
                target.Element("Cover").Value =model.Cover!=null? model.Cover:"";
                target.Element("PlaceShow").Value = model.PlaceShow.ToString();
                target.Element("DisplayOrder").Value = model.DisplayOrder.ToString();
                target.Element("MenuId").Value = model.MenuId.HasValue? model.MenuId.ToString():"0";
                target.Element("LinkId").Value = model.LinkId.HasValue ? model.LinkId.ToString() : "0";
                target.Element("TypeId").Value = model.TypeId.HasValue ? model.TypeId.ToString() : "0";
                if (model.Data != null)
                {
                    if (target.Element("Data") == null)
                        target.Add(new XElement("Data", new XCData(model.Data.ToString())));
                    else
                        target.Element("Data").ReplaceWith(new XElement("Data", new XCData(model.Data.ToString())));
                }
                else
                {
                    if (target.Element("Data") == null)
                        target.Add(new XElement("Data", " " ));
                    else
                      target.Element("Data").ReplaceWith("");
                }
               originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return "";
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }

        public string RemoveXMenus(int id)
        {
            try
            {
                XDocument originalXml = XDocument.Load(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                var target = originalXml.Root
      .Element("Menus")
      .Elements("Menu")
      .Where(e => e.Element("Id").Value == id.ToString())
      .Single();

                target.Remove();

                originalXml.Save(HttpContext.Current.Server.MapPath("~/Content/XML/Data.xml"));

                return "";
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }
        #endregion
    }

}
