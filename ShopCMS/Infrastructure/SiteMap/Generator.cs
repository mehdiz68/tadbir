using System;
using System.Collections.Generic;
using Domain;
using System.Data.Entity;
using System.Linq;
using CoreLib.ViewModel.Xml;
using System.Xml.Linq;
using System.Web;
using CoreLib.Infrastructure;

namespace ahmadi.Infrastructure.SiteMap
{
    public class Generator : Primary
    {
        public void WebUrl(string hostname, bool ContentLisence, bool ProductLisence)
        {
            UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            List<settingData> settingDatas = new List<settingData>();

            foreach (Setting Settingitem in uow.SettingRepository.Get(x => x))
            {
                XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                var language = readXml.DetailOfXLanguage(Settingitem.LanguageId.Value);
                settingDatas.Add(new settingData(Settingitem.WebSiteName, Settingitem.WebSiteTitle, Settingitem.WebSiteMetaDescription, Settingitem.WebSiteMetakeyword, Settingitem.LanguageId.Value, language.ShortName, Settingitem.HasHttps));
            }
            foreach (var Settingitem in settingDatas)
            {
                if (Settingitem.LanguageId == 1)
                    SetPath(System.Web.HttpContext.Current.Server.MapPath("~/SiteMap.xml"));
                else
                    SetPath(System.Web.HttpContext.Current.Server.MapPath("~/" + Settingitem.ShortName + "/SiteMap.xml"));
                siteMap.WriteStartDocument();
                siteMap.WriteStartElement("urlset");
                siteMap.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
                siteMap.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                siteMap.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

                if (ContentLisence)
                {
                    //site1.com/type/1/محتوا

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    var contentTypes = readXML.ListOfXContentType().Where(x => x.LanguageId == Settingitem.LanguageId);
                    foreach (XContentType item in contentTypes)
                    {
                        if (Settingitem.LanguageId == 1)
                        {
                            if (item.Id == 3)
                                addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/blog", System.DateTime.Now, 0.9);
                            else if (item.Id == 5)
                                addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/videos", System.DateTime.Now, 0.9);
                            else if (item.Id == 0)
                                addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/pages", System.DateTime.Now, 0.9);
                            else if (item.Id == 1)
                                addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/news", System.DateTime.Now, 0.9);
                            else if (item.Id == 2)
                                addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/mag", System.DateTime.Now, 0.9);
                            else if (item.Id == 6)
                                addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/events", System.DateTime.Now, 0.9);
                            else
                                addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/contentType/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.Title), System.DateTime.Now, 0.9);
                        }
                        else
                        {
                            addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/" + Settingitem.ShortName + "/contentType/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.Title), System.DateTime.Now, 0.9);
                        }

                    }
                    //site1.com/content/1/درباره ما
                    List<ContentTag> ContentTags = new List<ContentTag>();
                    foreach (Content item in uow.ContentRepository.Get(x => x, x => x.IsActive && x.LanguageId == Settingitem.LanguageId && (x.Category.IsActive || x.CatId == null), x => x.OrderByDescending(o => o.Id), "Tags,Category"))
                    {
                        if (Settingitem.LanguageId == 1)
                            addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/content/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.PageAddress), System.DateTime.Now, 0.9);
                        else
                            addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/" + Settingitem.ShortName + "/content/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.PageAddress), System.DateTime.Now, 0.9);

                        foreach (var tagitem in item.Tags)
                        {
                            if (!ContentTags.Any(x => x.Id == tagitem.Id && x.ContentTypeId == item.ContentTypeId))
                                ContentTags.Add(new ContentTag(tagitem.Id, tagitem.TagName, item.ContentTypeId));
                        }
                    }

                    ////site1.com/contentTag/2/1/برچسب
                    //foreach (ContentTag item in ContentTags)
                    //{
                    //    if (Settingitem.LanguageId == 1)
                    //    {
                    //        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/contentTag/" + item.ContentTypeId + "/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.TagName), System.DateTime.Now, 0.9);
                    //    }
                    //    else
                    //        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/" + Settingitem.ShortName + "/contentTag/" + item.ContentTypeId + "/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.TagName), System.DateTime.Now, 0.9);

                    //}
                    //site1.com/category/1/اخبار فرهنگی
                    foreach (Category item in uow.CategoryRepository.Get(x => x, x => x.IsActive && x.LanguageId == Settingitem.LanguageId))
                    {
                        if (Settingitem.LanguageId == 1)
                            addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/category/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.PageAddress), System.DateTime.Now, 0.9);
                        else
                            addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/" + Settingitem.ShortName + "/category/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.PageAddress), System.DateTime.Now, 0.9);
                    }
                    ////site1.com/tag/1/محتوا
                    //foreach (Tag item in uow.TagRepository.Get(x => x, x => x.LanguageId == Settingitem.LanguageId))
                    //{
                    //    if (Settingitem.LanguageId == 1)
                    //        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/tag/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.TagName), System.DateTime.Now, 0.9);
                    //    else
                    //        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/" + Settingitem.ShortName + "/tag/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.TagName), System.DateTime.Now, 0.9);
                    //}

                }
                //site1.com/TFC/1/محصولات فرهنگی
                foreach (ProductCategory item in uow.ProductCategoryRepository.Get(x => x, x => x.IsActive && x.LanguageId == Settingitem.LanguageId))
                {
                    if (Settingitem.LanguageId == 1)
                    {
                        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/TFC/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.PageAddress), System.DateTime.Now, 0.9);
                        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/TFS/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.PageAddress2), System.DateTime.Now, 0.9);
                    }
                    else
                    {
                        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/" + Settingitem.ShortName + "/TFC/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.PageAddress), System.DateTime.Now, 0.9);
                        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/" + Settingitem.ShortName + "/TFS/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.PageAddress2), System.DateTime.Now, 0.9);
                    }
                }
                //محصول
                foreach (var item in uow.ProductRepository.Get(x => new { x.Id, x.PageAddress }, x => x.IsActive && x.LanguageId == Settingitem.LanguageId))
                {
                    if (Settingitem.LanguageId == 1)
                        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/TFP/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.PageAddress), System.DateTime.Now, 0.9);
                    else
                        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/" + Settingitem.ShortName + "/TFP/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.PageAddress), System.DateTime.Now, 0.9);

                }
                //برند
                foreach (var item in uow.BrandRepository.Get(x => new { x.Id, x.Name }, x => x.LanguageId == Settingitem.LanguageId))
                {
                    if (Settingitem.LanguageId == 1)
                    {
                        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/TFB/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.Name), System.DateTime.Now, 0.9);
                    }
                    else
                    {
                        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/" + Settingitem.ShortName + "/TFB/" + item.Id + "/" + CommonFunctions.NormalizeAddress(item.Name), System.DateTime.Now, 0.9);
                    }
                }
                //تگ محصول
                foreach (var item in uow.TagRepository.Get(x => x, x => x.Product.Any() && x.LanguageId == Settingitem.LanguageId))
                {
                    if (Settingitem.LanguageId == 1)
                    {
                        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/PTag/" + CommonFunctions.NormalizeAddress(item.TagName), System.DateTime.Now, 0.9);
                    }
                    else
                    {
                        addStaticPage(siteMap, (Settingitem.Hashttps ? "https" : "http") + "://www." + hostname + "/" + Settingitem.ShortName + "/PTag/" + CommonFunctions.NormalizeAddress(item.TagName), System.DateTime.Now, 0.9);
                    }
                }
                siteMap.WriteEndElement();
                siteMap.WriteEndDocument();
                siteMap.Close();
            }
        }

    }

    internal class settingData
    {
        public settingData(string webSiteName, string webSiteTitle, string webSiteMetaDescription, string webSiteMetakeyword, Int16 languageId, string shortName, bool Hashttps)
        {
            this.WebSiteName = webSiteName;
            this.WebSiteTitle = webSiteTitle;
            this.WebSiteMetaDescription = webSiteMetaDescription;
            this.WebSiteMetakeyword = webSiteMetakeyword;
            this.LanguageId = languageId;
            this.ShortName = shortName;
            this.Hashttps = Hashttps;
        }
        public string WebSiteName { get; set; }
        public string WebSiteTitle { get; set; }
        public string WebSiteMetaDescription { get; set; }
        public string WebSiteMetakeyword { get; set; }
        public Int16 LanguageId { get; set; }
        public string ShortName { get; set; }
        public bool Hashttps { get; set; }
    }

    internal class ContentTag
    {
        public ContentTag(int id, string tagname, int contentTypeId)
        {
            Id = id;
            TagName = tagname;
            ContentTypeId = contentTypeId;
        }
        public int Id { get; set; }
        public string TagName { get; set; }
        public int ContentTypeId { get; set; }
    }
}
