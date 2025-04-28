using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Routing;

namespace ahmadi
{
    public class ImageRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ImagepHandler(requestContext);
        }
        public class ImagepHandler : IHttpHandler
        {
            private RequestContext _requestContext;

            public ImagepHandler(RequestContext requestContext)
            {
                _requestContext = requestContext;
            }

            public bool IsReusable
            {
                get { return false; }
            }

            public void ProcessRequest(HttpContext context)
            {
                var routeValues = _requestContext.RouteData.Values;
                if (routeValues.ContainsKey("id"))
                {
                    string f = routeValues["id"].ToString();
                    string fullpath = f;
                    string folder = "";
                    if (routeValues.ContainsKey("folder"))
                        folder = routeValues["folder"].ToString();
                    if (folder != "-")
                        fullpath = string.Format(@"{0}\{1}", folder, f);
                    string size = "LG";
                    if (routeValues.ContainsKey("size"))
                        size = routeValues["size"].ToString();
                    int h = 1280;
                    if (routeValues.ContainsKey("h"))
                        h = Convert.ToInt32(routeValues["h"]);
                    int w = 1280;
                    if (routeValues.ContainsKey("w"))
                        w = Convert.ToInt32(routeValues["w"]);


                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + fullpath)))
                    {
                        if (size != "LG")
                        {
                            if (folder != "-")//file uploaded in folder
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}/{1}_{2}", folder, size, f.Remove(0, 3)))))
                                    fullpath = string.Format("{0}/{1}_{2}", folder, size, f.Remove(0, 3));
                            }
                            else
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}/{1}", size, f.Remove(0, 3)))))
                                    fullpath = string.Format("{0}_{1}", size, f.Remove(0, 3));
                            }
                        }

                        WebImage img = new WebImage(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + fullpath));
                        img.Resize(w, h);
                        img.FileName = f;
                        img.Crop(1, 1, 1, 1);

                        var buffer = img.GetBytes();
                        MemoryStream mem = new MemoryStream(buffer, 0, buffer.Length);
                        _requestContext.HttpContext.Response.Clear();

                        TimeSpan expire = new TimeSpan(0, 1, 5, 0);
                        DateTime now = DateTime.Now;
                        _requestContext.HttpContext.Response.Cache.SetExpires(now.Add(expire));
                        _requestContext.HttpContext.Response.Cache.SetMaxAge(expire);
                        _requestContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.Server);
                        _requestContext.HttpContext.Response.Cache.SetValidUntilExpires(true);

                        _requestContext.HttpContext.Response.ContentType = img.ImageFormat;
                        _requestContext.HttpContext.Response.BinaryWrite(mem.ToArray());
                        _requestContext.HttpContext.Response.Flush();
                    }

                    //string filename = _requestContext.HttpContext.Server.MapPath("~/Content/UploadFiles/" + routeValues["id"]);
                    //System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);


                    //if (fileInfo.Exists)
                    //{

                    //_requestContext.HttpContext.Response.AddHeader("Content-Disposition", "inline;attachment; filename=\""
                    //                                                + fileInfo.Name + "\"");
                    //_requestContext.HttpContext.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    //_requestContext.HttpContext.Response.ContentType = "application/octet-stream";
                    //_requestContext.HttpContext.Response.TransmitFile(fileInfo.FullName);
                    //_requestContext.HttpContext.Response.Flush();
                    //}
                }
            }

        }
    }
}