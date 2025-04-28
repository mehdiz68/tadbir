using System;
using System.Text;
using System.Xml;

namespace ahmadi.Infrastructure.SiteMap
{
    public class Primary
    {
        protected XmlTextWriter siteMap;
        protected void SetPath(string Path)
        {
            siteMap = new XmlTextWriter(Path, Encoding.UTF8);

        }
        protected static void addStaticPage(XmlTextWriter xWriter, string relUrl,System.DateTime LastMod, double priority)
        {
            string url = relUrl;
            writeItemNode(xWriter, url, LastMod, "daily", priority);
        }

        protected static void writeItemNode(XmlTextWriter xWriter, string url, System.DateTime lastModified, string changeFrequency, double priority)
        {

            xWriter.WriteStartElement("url");
            xWriter.WriteElementString("loc", escapeUrl(url));
            xWriter.WriteElementString("lastmod", lastModified.ToString("yyyy-MM-dd"));
            xWriter.WriteElementString("changefreq", changeFrequency);
            xWriter.WriteElementString("priority", priority.ToString("0.#"));
            xWriter.WriteEndElement();
        }

        protected static void addStaticVideoPage(XmlTextWriter xWriter, string url, string thumbnail_loc, string title, string description, string content_loc, string duration, string rating, string view_count, string publication_date, string gallery_loc, string uploader, string live)
        {
            writeVideoItemNode(xWriter, url, thumbnail_loc, title, description, content_loc, duration, rating, view_count, publication_date, gallery_loc, uploader, live);
        }
        protected static void writeVideoItemNode(XmlTextWriter xWriter, string url, string thumbnail_loc, string title, string description, string content_loc, string duration, string rating, string view_count, string publication_date, string gallery_loc, string uploader, string live)
        {

            xWriter.WriteStartElement("url");

            xWriter.WriteElementString("loc", escapeUrl(url));

            xWriter.WriteStartElement("video:video");

            xWriter.WriteElementString("video:thumbnail_loc", thumbnail_loc);
            xWriter.WriteElementString("video:title", title);
            xWriter.WriteElementString("video:description", description);
            xWriter.WriteElementString("video:content_loc", content_loc);
            xWriter.WriteElementString("video:duration", duration);
            xWriter.WriteElementString("video:rating", rating);
            xWriter.WriteElementString("video:view_count", view_count);
            xWriter.WriteElementString("video:publication_date", Convert.ToDateTime(publication_date).ToString("yyyy-MM-dd") + "T" + Convert.ToDateTime(publication_date).ToString("HH:mm:ss") + "+03:30");
            xWriter.WriteElementString("video:family_friendly", "yes");
            xWriter.WriteElementString("video:gallery_loc", gallery_loc);
            xWriter.WriteElementString("video:requires_subscription", "no");
            xWriter.WriteElementString("video:uploader", uploader);
            xWriter.WriteElementString("video:live", "no");

            xWriter.WriteEndElement();
            xWriter.WriteEndElement();
        }
        protected static string escapeUrl(string url)
        {
            return url.Replace("&", "&amp;").Replace("'", "&apos").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;");
        }
    }
}
