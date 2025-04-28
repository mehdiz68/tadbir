
namespace CoreLib.ViewModel.Email
{
    public class Template
    {
        public string Logo { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string SiteName { get; set; }
        public string SiteDescription { get; set; }
        public string Url { get; set; }
        public Template(string logo, string title, string text, string sitename, string url, string sitedescription)
        {
            Logo = logo;
            Title = title;
            Text = text;
            SiteName = sitename;
            Url = url;
            SiteDescription = sitedescription;
        }
    }
}
