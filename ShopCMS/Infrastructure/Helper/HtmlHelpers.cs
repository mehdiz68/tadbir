using Domain;
using UnitOfWork;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net;
using CoreLib.ViewModel.Xml;

namespace ahmadi.Infrastructure.Helper
{
    public static class HtmlHelpers
    {
        /// <summary>
        /// کلاس هلپر های مورد نیاز برای کار با اچ تی ام ال
        /// </summary>
        private static UnitOfWorkClass uow = null;
        /// <summary>
        /// متدِ نمایش فایل با استفاده آدرس نسبی
        /// </summary>
        /// <param name="helper">کلاس مرجع هلپر ام وی سی</param>
        /// <param name="fileName">نام فایلِ فایل</param>
        /// <param name="title">عنوان فایلِ فایل</param>
        /// <param name="size">انتخاب سایز تصویر در صورت نیاز LG,MD,SM,XS </param>
        /// <param name="HasMultiSize"> وضعیت تغییر اندازه </param>
        /// <param name="htmlAttributes"> خصوصیات و کلاس های اچ تی ام ال برای هلپر </param>
        /// <returns></returns>
        public static System.Web.IHtmlString Displayattachment(this System.Web.Mvc.HtmlHelper helper, string fileName, string title, string size = "LG", bool HasMultiSize = true, object htmlAttributes = null)
        {

            string result = string.Format("<span> فایل موجود نیست! </span>");
            try
            {
                if (fileName.LastIndexOf(".") > 0)
                {
                    //Check MultiSize
                    if (HasMultiSize)
                    {
                        if (size != "LG")
                        {
                            if (fileName.IndexOf("/") > 0)//file uploaded in folder
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3)))))
                                    fileName = string.Format("{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3));
                            }
                            else
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}_{1}", size, fileName.Remove(0, 3)))))
                                    fileName = string.Format("{0}_{1}", size, fileName.Remove(0, 3));
                            }
                        }
                    }
                    switch (fileName.Substring(fileName.LastIndexOf(".")).ToLower())
                    {
                        case ".bmp":
                        case ".png":
                        case ".jpg":
                        case ".svg":
                        case ".jpeg":
                        case ".gif":
                        case ".tif":
                            var builder = new TagBuilder("img");
                            builder.MergeAttribute("src", GetAppRootFolder() + "Content/UploadFiles/" + fileName);
                            builder.MergeAttribute("alt", title);
                            builder.MergeAttribute("title", title);
                            if (htmlAttributes != null)
                                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                            return helper.Raw(builder.ToString(TagRenderMode.SelfClosing));
                        default:
                            builder = new TagBuilder("a");
                            builder.SetInnerText(title);
                            builder.MergeAttribute("href", GetAppRootFolder() + "Content/UploadFiles/" + fileName);
                            builder.MergeAttribute("alt", title);
                            builder.MergeAttribute("title", title);
                            builder.MergeAttribute("target", "_blank");
                            if (htmlAttributes != null)
                                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                            return helper.Raw(builder.ToString(TagRenderMode.Normal));
                    }
                }
                else
                    return helper.Raw(string.Format("<span> فایل پسوند ندارد! </span>"));

            }
            catch (Exception)
            {

                return helper.Raw(string.Format("<span> فایل موجود نیست! </span>"));
            }

        }

        /// <summary>
        /// متدِ نمایش فایل با استفاده از دامنه ای که کوکی ندارد
        /// </summary>
        /// <param name="helper">کلاس مرجع هلپر ام وی سی</param>
        /// <param name="StaticContentDomain">نام دامنه ای که کوکی ندارد</param>
        /// <param name="fileName">نام فایلِ فایل</param>
        /// <param name="title">عنوان فایلِ فایل</param>
        /// <param name="size">انتخاب سایز تصویر در صورت نیاز LG,MD,SM,XS </param>
        /// <param name="HasMultiSize"> وضعیت تغییر اندازه </param>
        /// <param name="htmlAttributes"> خصوصیات و کلاس های اچ تی ام ال برای هلپر </param>
        /// 
        /// <returns></returns>
        public static System.Web.IHtmlString DisplayattachmentCookieFree(this System.Web.Mvc.HtmlHelper helper, string StaticContentDomain, string fileName, string title, string size = "LG", bool HasMultiSize = true, object htmlAttributes = null)
        {

            string result = string.Format("<span> فایل موجود نیست! </span>");
            try
            {
                if (fileName.LastIndexOf(".") > 0)
                {
                    //Check MultiSize
                    if (HasMultiSize)
                    {
                        if (size != "LG")
                        {

                            if (fileName.IndexOf("/") > 0)//file uploaded in folder
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3)))))
                                    fileName = string.Format("{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3));
                            }
                            else
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}_{1}", size, fileName.Remove(0, 3)))))
                                    fileName = string.Format("{0}_{1}", size, fileName.Remove(0, 3));
                            }

                        }
                    }
                    switch (fileName.Substring(fileName.LastIndexOf(".")).ToLower())
                    {
                        case ".bmp":
                        case ".png":
                        case ".jpg":
                        case ".svg":
                        case ".jpeg":
                        case ".gif":
                        case ".tif":
                            var builder = new TagBuilder("img");
                            builder.MergeAttribute("src", StaticContentDomain + "/" + "UploadFiles/" + fileName);
                            builder.MergeAttribute("alt", title);
                            builder.MergeAttribute("title", title);
                            if (htmlAttributes != null)
                                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                            return helper.Raw(builder.ToString(TagRenderMode.SelfClosing));
                        default:
                            builder = new TagBuilder("a");
                            builder.SetInnerText(title);
                            builder.MergeAttribute("href", StaticContentDomain + "/" + "UploadFiles/" + fileName);
                            builder.MergeAttribute("alt", title);
                            builder.MergeAttribute("title", title);
                            builder.MergeAttribute("target", "_blank");
                            if (htmlAttributes != null)
                                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                            return helper.Raw(builder.ToString(TagRenderMode.Normal));
                    }
                }
                else
                    result = string.Format("<span> فایل پسوند ندارد! </span>");

                return helper.Raw(result);
            }
            catch (Exception)
            {

                return helper.Raw(string.Format("<span> فایل موجود نیست! </span>"));
            }

        }



        /// <summary>
        /// متدِ نمایش فایل با استفاده آدرس نسبی
        /// </summary>
        /// <param name="helper">کلاس مرجع هلپر ام وی سی</param>
        /// <param name="fileName">نام فایلِ فایل</param>
        /// <param name="title">عنوان فایلِ فایل</param>
        /// <param name="size">انتخاب سایز تصویر در صورت نیاز LG,MD,SM,XS </param>
        /// <param name="HasMultiSize"> وضعیت تغییر اندازه </param>
        /// <param name="htmlAttributes"> خصوصیات و کلاس های اچ تی ام ال برای هلپر </param>
        /// <returns></returns>
        public static System.Web.IHtmlString DisplayattachmentCookieFreeUrl(this System.Web.Mvc.HtmlHelper helper, string StaticContentDomain, string fileName, string size = "LG", bool HasMultiSize = true)
        {

            string result = string.Format("<span> فایل موجود نیست! </span>");
            try
            {
                //Check MultiSize
                if (HasMultiSize)
                {
                    if (size != "LG")
                    {
                        if (fileName.IndexOf("/") > 0)//file uploaded in folder
                        {
                            if (CheckExists(StaticContentDomain + string.Format("/UploadFiles/{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3))))
                                fileName = string.Format("{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3));
                        }
                        else
                        {
                            if (CheckExists(StaticContentDomain + string.Format("/UploadFiles/{0}_{1}", size, fileName.Remove(0, 3))))
                                fileName = string.Format("{0}_{1}", size, fileName.Remove(0, 3));
                        }
                    }
                }

                return helper.Raw(StaticContentDomain + "/UploadFiles/" + fileName);

            }
            catch (Exception)
            {

                return helper.Raw(string.Format("<span> فایل موجود نیست! </span>"));
            }

        }


        /// <summary>
        /// متدِ نمایش فایل با استفاده آدرس نسبی
        /// </summary>
        /// <param name="helper">کلاس مرجع هلپر ام وی سی</param>
        /// <param name="fileName">نام فایلِ فایل</param>
        /// <param name="title">عنوان فایلِ فایل</param>
        /// <param name="size">انتخاب سایز تصویر در صورت نیاز LG,MD,SM,XS </param>
        /// <param name="HasMultiSize"> وضعیت تغییر اندازه </param>
        /// <param name="htmlAttributes"> خصوصیات و کلاس های اچ تی ام ال برای هلپر </param>
        /// <returns></returns>
        public static string DisplayattachmentCookieFreeUrl(string StaticContentDomain, string fileName, string size = "LG", bool HasMultiSize = true)
        {

            string result = string.Format("<span> فایل موجود نیست! </span>");
            try
            {
                //Check MultiSize
                if (HasMultiSize)
                {
                    if (size != "LG")
                    {
                        if (fileName.IndexOf("/") > 0)//file uploaded in folder
                        {
                            if (CheckExists(StaticContentDomain + string.Format("/UploadFiles/{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3))))
                                fileName = string.Format("{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3));
                        }
                        else
                        {
                            if (CheckExists(StaticContentDomain + string.Format("/UploadFiles/{0}_{1}", size, fileName.Remove(0, 3))))
                                fileName = string.Format("{0}_{1}", size, fileName.Remove(0, 3));
                        }
                    }
                }

                return StaticContentDomain + "/UploadFiles/" + fileName;

            }
            catch (Exception)
            {

                return StaticContentDomain + "/images/default-thumbnail.jpg";
            }

        }

        /// <summary>
        /// متدِ نمایش فایل با استفاده آدرس نسبی
        /// </summary>
        /// <param name="helper">کلاس مرجع هلپر ام وی سی</param>
        /// <param name="fileName">نام فایلِ فایل</param>
        /// <param name="title">عنوان فایلِ فایل</param>
        /// <param name="size">انتخاب سایز تصویر در صورت نیاز LG,MD,SM,XS </param>
        /// <param name="HasMultiSize"> وضعیت تغییر اندازه </param>
        /// <param name="htmlAttributes"> خصوصیات و کلاس های اچ تی ام ال برای هلپر </param>
        /// <returns></returns>
        public static System.Web.IHtmlString DisplayattachmentUrl(this System.Web.Mvc.HtmlHelper helper, string fileName, string size = "LG", bool HasMultiSize = true)
        {

            string result = string.Format("<span> فایل موجود نیست! </span>");
            try
            {
                //Check MultiSize
                if (HasMultiSize)
                {
                    if (size != "LG")
                    {
                        if (fileName.IndexOf("/") > 0)//file uploaded in folder
                        {
                            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3)))))
                                fileName = string.Format("{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3));
                        }
                        else
                        {
                            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}_{1}", size, fileName.Remove(0, 3)))))
                                fileName = string.Format("{0}_{1}", size, fileName.Remove(0, 3));
                        }
                    }
                }

                return helper.Raw(GetAppRootFolder() + "Content/UploadFiles/" + fileName);

            }
            catch (Exception)
            {

                return helper.Raw(string.Format("<span> فایل موجود نیست! </span>"));
            }

        }

        /// <summary>
        /// متدِ نمایش فایل با استفاده آدرس نسبی
        /// </summary>
        /// <param name="helper">کلاس مرجع هلپر ام وی سی</param>
        /// <param name="fileName">نام فایلِ فایل</param>
        /// <param name="title">عنوان فایلِ فایل</param>
        /// <param name="size">انتخاب سایز تصویر در صورت نیاز LG,MD,SM,XS </param>
        /// <param name="HasMultiSize"> وضعیت تغییر اندازه </param>
        /// <param name="htmlAttributes"> خصوصیات و کلاس های اچ تی ام ال برای هلپر </param>
        /// <returns></returns>
        public static string DisplayattachmentUrl(string fileName, string size = "LG", bool HasMultiSize = true)
        {

            string result = string.Format("<span> فایل موجود نیست! </span>");
            try
            {
                //Check MultiSize
                if (HasMultiSize)
                {
                    if (size != "LG")
                    {
                        if (fileName.IndexOf("/") > 0)//file uploaded in folder
                        {
                            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3)))))
                                fileName = string.Format("{0}/{1}_{2}", fileName.Substring(0, fileName.LastIndexOf("/")), size, fileName.Substring(fileName.LastIndexOf("/") + 1).Remove(0, 3));
                        }
                        else
                        {
                            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}_{1}", size, fileName.Remove(0, 3)))))
                                fileName = string.Format("{0}_{1}", size, fileName.Remove(0, 3));
                        }
                    }
                }

                return GetAppRootFolder() + "Content/UploadFiles/" + fileName;

            }
            catch (Exception)
            {

                return GetAppRootFolder() + "Content/Default/images/default-thumbnail.jpg";
            }

        }


        /// <summary>
        /// متدِ نمایش فایل با استفاده آدرس نسبی
        /// </summary>
        /// <param name="helper">کلاس مرجع هلپر ام وی سی</param>
        /// <param name="fileName">نام فایلِ فایل</param>
        /// <param name="title">عنوان فایلِ فایل</param>
        /// <param name="size">انتخاب سایز تصویر در صورت نیاز LG,MD,SM,XS </param>
        /// <param name="HasMultiSize"> وضعیت تغییر اندازه </param>
        /// <param name="htmlAttributes"> خصوصیات و کلاس های اچ تی ام ال برای هلپر </param>
        /// <returns></returns>
        public static System.Web.IHtmlString DisplayAttributeValue(this System.Web.Mvc.HtmlHelper helper, Int16 DataType, string Value, string size, string currency = "", object htmlAttributes = null)
        {
            uow = new UnitOfWorkClass();
            try
            {
                switch (DataType)
                {
                    case 12:
                    case 34:
                    case 44:
                    case 48:

                        return helper.Raw(string.Format("<span class='PriceAttribute'> {0:n0} {1} </span>", Convert.ToInt64(Value), currency));
                    case 43:
                    case 47:
                        return helper.Raw(string.Format("<span class='PercentAttribute'> {0} درصد </span>", Convert.ToDouble(Value) * 100));

                    case 20:
                        return helper.Raw(string.Format("<span class='colorBoxAttribute' style='color:{0};background-color:{0}'></span>", Value));

                    case 5:
                    case 33:
                    case 50:
                        if (Value == "1")
                            return helper.Raw(string.Format("<span class='CheckedAttribute glyphicon glyphicon-ok green'></span>"));
                        else
                            return helper.Raw(string.Format("<span class='CheckedAttribute glyphicon glyphicon-remove red'></span>"));

                    case 6:
                    case 39:
                    case 40:
                        return helper.Raw(string.Format("<span class='DateTimeAttribute'> {0} </span>", CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(Convert.ToDateTime(Value))));
                    case 7:
                    case 42:
                    case 46:
                    case 29:
                    case 30:
                        Guid attachementId = new Guid(Value);
                        var attachment = uow.AttachmentRepository.GetByID(attachementId);
                        if (attachment != null)
                        {
                            if (attachment.FileName.IndexOf("/") > 0)//file uploaded in folder
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}/{1}_{2}", attachment.FileName.Substring(0, attachment.FileName.LastIndexOf("/")), size, attachment.FileName.Substring(attachment.FileName.LastIndexOf("/") + 1).Remove(0, 3)))))
                                    attachment.FileName = string.Format("{0}/{1}_{2}", attachment.FileName.Substring(0, attachment.FileName.LastIndexOf("/")), size, attachment.FileName.Substring(attachment.FileName.LastIndexOf("/") + 1).Remove(0, 3));

                            }
                            else
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}_{1}", size, attachment.FileName.Remove(0, 3)))))
                                    attachment.FileName = string.Format("{0}_{1}", size, attachment.FileName.Remove(0, 3));
                            }

                            var builder = new TagBuilder("a");
                            builder.SetInnerText("دانلود فایل");
                            builder.MergeAttribute("href", GetAppRootFolder() + "Content/UploadFiles/" + attachment.FileName);
                            builder.MergeAttribute("alt", attachment.Title);
                            builder.MergeAttribute("title", attachment.Title);
                            builder.MergeAttribute("target", "_blank");
                            if (htmlAttributes != null)
                                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                            return helper.Raw(builder.ToString(TagRenderMode.Normal));
                        }
                        else
                        {
                            return helper.Raw(string.Format("<span> فایل موجود نیست! </span>"));
                        }
                    case 18:
                    case 21:
                    case 25:
                        attachementId = new Guid(Value);
                        attachment = uow.AttachmentRepository.GetByID(attachementId);
                        if (attachment != null)
                        {
                            if (attachment.FileName.IndexOf("/") > 0)//file uploaded in folder
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}/{1}_{2}", attachment.FileName.Substring(0, attachment.FileName.LastIndexOf("/")), size, attachment.FileName.Substring(attachment.FileName.LastIndexOf("/") + 1).Remove(0, 3)))))
                                    attachment.FileName = string.Format("{0}/{1}_{2}", attachment.FileName.Substring(0, attachment.FileName.LastIndexOf("/")), size, attachment.FileName.Substring(attachment.FileName.LastIndexOf("/") + 1).Remove(0, 3));

                            }
                            else
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}_{1}", size, attachment.FileName.Remove(0, 3)))))
                                    attachment.FileName = string.Format("{0}_{1}", size, attachment.FileName.Remove(0, 3));
                            }

                            var builder = new TagBuilder("img");
                            builder.MergeAttribute("src", GetAppRootFolder() + "Content/UploadFiles/" + attachment.FileName);
                            builder.MergeAttribute("alt", attachment.Title);
                            builder.MergeAttribute("title", attachment.Title);
                            if (htmlAttributes != null)
                                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                            return helper.Raw(builder.ToString(TagRenderMode.SelfClosing));
                        }
                        else
                        {
                            return helper.Raw(string.Format("<span> فایل موجود نیست! </span>"));
                        }

                    case 8:
                    //case 24:
                    //    var ItemId = Convert.ToInt32(Value);
                    //    var item = uow.ProductAttributeItems.Where(x => x.Id == ItemId).SingleOrDefault();
                    //    if (item != null)
                    //        return helper.Raw(string.Format("<span> {0} </span>", item.Text));
                    //    else
                    //        return helper.Raw(string.Format("<span>  موجود نیست! </span>"));
                    case 19:
                    case 27:
                    case 32:
                    //ItemId = Convert.ToInt32(Value);
                    //item = db.ProductAttributeItems.Where(x => x.Id == ItemId).SingleOrDefault();
                    //if (item != null)
                    //{
                    //    if (item.Image.HasValue)
                    //    {
                    //        attachment = db.attachments.Find(item.Image.Value);
                    //        if (attachment != null)
                    //        {
                    //            if (attachment.FileName.IndexOf("/") > 0)//file uploaded in folder
                    //            {
                    //                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}/{1}_{2}", attachment.FileName.Substring(0, attachment.FileName.LastIndexOf("/")), size, attachment.FileName.Substring(attachment.FileName.LastIndexOf("/") + 1).Remove(0, 3)))))
                    //                    attachment.FileName = string.Format("{0}/{1}_{2}", attachment.FileName.Substring(0, attachment.FileName.LastIndexOf("/")), size, attachment.FileName.Substring(attachment.FileName.LastIndexOf("/") + 1).Remove(0, 3));

                    //            }
                    //            else
                    //            {
                    //                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}_{1}", size, attachment.FileName.Remove(0, 3)))))
                    //                    attachment.FileName = string.Format("{0}_{1}", size, attachment.FileName.Remove(0, 3));
                    //            }

                    //            var builder = new TagBuilder("img");
                    //            builder.MergeAttribute("src", GetAppRootFolder() + "Content/UploadFiles/" + attachment.FileName);
                    //            builder.MergeAttribute("alt", attachment.Title);
                    //            builder.MergeAttribute("title", attachment.Title);
                    //            if (htmlAttributes != null)
                    //                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                    //            return helper.Raw(builder.ToString(TagRenderMode.SelfClosing));
                    //        }
                    //        else
                    //            return helper.Raw(string.Format("<span> {0} </span>", item.Text));
                    //    }
                    //    else
                    //        return helper.Raw(string.Format("<span> {0} </span>", item.Text));
                    //}
                    //else
                    //    return helper.Raw(string.Format("<span>  موجود نیست! </span>"));



                    case 9:
                    case 36:
                        int ContentId = Convert.ToInt32(Value);
                        var page = uow.ContentRepository.GetByID(ContentId);
                        if (page != null)
                        {
                            var builder = new TagBuilder("a");
                            builder.SetInnerText("لینک صفحه");
                            builder.MergeAttribute("href", GetAppRootFolder() + "content/" + page.Id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(page.Title));
                            builder.MergeAttribute("alt", page.Title);
                            builder.MergeAttribute("title", page.Title);
                            builder.MergeAttribute("target", "_blank");
                            if (htmlAttributes != null)
                                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                            return helper.Raw(builder.ToString(TagRenderMode.Normal));
                        }
                        else
                        {
                            return helper.Raw(string.Format("<span>  لینک موجود نیست ! </span>"));
                        }
                    case 10:
                    case 26:
                        int ProductId = Convert.ToInt32(Value);
                        var Product = uow.ContentRepository.GetByID(ProductId);
                        if (Product != null)
                        {
                            var builder = new TagBuilder("a");
                            builder.SetInnerText("لینک محصول");
                            builder.MergeAttribute("href", GetAppRootFolder() + "Product/" + Product.Id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(Product.Title));
                            builder.MergeAttribute("alt", Product.Title);
                            builder.MergeAttribute("title", Product.Title);
                            builder.MergeAttribute("target", "_blank");
                            if (htmlAttributes != null)
                                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                            return helper.Raw(builder.ToString(TagRenderMode.Normal));
                        }
                        else
                        {
                            return helper.Raw(string.Format("<span>  لینک موجود نیست ! </span>"));
                        }

                    case 37:
                        if (Value == "1")
                            return helper.Raw(string.Format("<img class='SpecialofferAttribute' src='" + GetAppRootFolder() + "Content/Default/images/SpecialOffer.png" + "' alt='پیشنهاد ویژه' width='110' height='110'/>"));
                        else
                            return helper.Raw("");
                    case 38:
                        if (Value == "1")
                            return helper.Raw(string.Format("<img class='AmazingOfferAttribute' src='" + GetAppRootFolder() + "Content/Default/images/AmazingOffer.png" + "' alt='پیشنهاد شگفت انگیز' width='110' height='110'/>"));
                        else
                            return helper.Raw("");


                    default: return helper.Raw(string.Format("<span class='OtherAttribute'> {0} </span>", Value));
                }
            }
            catch (Exception)
            {
                return helper.Raw(string.Format("<span> {0} </span>", Value));
            }
            finally
            {
                uow.Dispose();
            }

        }


        public static System.Web.IHtmlString DisplayFileAttribute(this System.Web.Mvc.HtmlHelper helper, Guid attachmentId, string size = "MD", object htmlAttributes = null)
        {
            uow = new UnitOfWorkClass();
            string result = string.Format("<img src=''  width='50' height='50' />");
            try
            {
                var attachment = uow.AttachmentRepository.GetByID(attachmentId);
                if (attachment != null)
                {
                    //Check MultiSize
                    string filename = attachment.FileName;
                    if (attachment.HasMultiSize)
                    {
                        if (size != "LG")
                        {


                            if (filename.IndexOf("/") > 0)//file uploaded in folder
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}/{1}_{2}", filename.Substring(0, filename.LastIndexOf("/")), size, filename.Substring(filename.LastIndexOf("/") + 1).Remove(0, 3)))))
                                    filename = string.Format("{0}/{1}_{2}", filename.Substring(0, filename.LastIndexOf("/")), size, filename.Substring(filename.LastIndexOf("/") + 1).Remove(0, 3));

                            }
                            else
                            {
                                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/Content/UploadFiles/" + string.Format("{0}_{1}", size, filename.Remove(0, 3)))))
                                    filename = string.Format("{0}_{1}", size, filename.Remove(0, 3));
                            }

                        }
                    }
                    string img = "<img src='" + GetAppRootFolder() + "Content/UploadFiles/" + filename + "' width='50' height='50' /> " + attachment.Title;
                    return helper.Raw(img);

                }
                return helper.Raw(result);
            }
            catch (Exception)
            {

                return helper.Raw(string.Format("<img src=''  width='50' height='50' />"));
            }
            finally
            {
                uow.Dispose();
            }
        }

        public static string DisplaySelectAttribute(this System.Web.Mvc.HtmlHelper helper, int AttributeItemId)
        {
            uow = new UnitOfWorkClass();
            string result = string.Format(" آیتم موجود نیست! ");
            try
            {

                var item = uow.ProductAttributeItemRepository.Get(x => x, x => x.Id == AttributeItemId).Single();
                if (item != null)
                    return item.Value;
                else
                    return "";
            }
            catch (Exception)
            {

                return string.Format(" آیتم موجود نیست!");
            }
        }

        public static string DisplayContentAttribute(this System.Web.Mvc.HtmlHelper helper, int AttributeItemId)
        {
            uow = new UnitOfWorkClass();
            string result = string.Format(" آیتم موجود نیست! ");
            try
            {

                var item = uow.ContentRepository.Get(x => x, x => x.Id == AttributeItemId).Single();
                if (item != null)
                    return item.Title;
                else
                    return "";
            }
            catch (Exception)
            {

                return string.Format(" آیتم موجود نیست!");
            }
            finally
            {
                uow.Dispose();
            }
        }

        public static string DisplayAdsPage(this System.Web.Mvc.HtmlHelper helper, int TypeId, int LinkId)
        {
            uow = new UnitOfWorkClass();

            XMLReader readXML = new XMLReader("");
            string result = string.Format(" آیتم موجود نیست! ");
            try
            {
                switch (TypeId)
                {
                    case 1: result = string.Format("{0} - {1}", "محتوا", uow.ContentRepository.Get(x => x, x => x.Id == LinkId).Single().Title);break;
                    case 2: result = string.Format("{0} - {1}", "دسته محتوا", uow.CategoryRepository.Get(x => x, x => x.Id == LinkId).Single().Title); break;
                    case 3: result = string.Format("{0} - {1}", "برچسب", uow.TagRepository.Get(x => x, x => x.Id == LinkId).Single().Title); break;
                    case 4: result = string.Format("{0} - {1}", "گروه", uow.ProductCategoryRepository.Get(x => x, x => x.Id == LinkId).Single().Name); break;
                    case 5: result = string.Format("{0} - {1}", "گالری", uow.GalleryCategoryRepository.Get(x => x, x => x.Id == LinkId).Single().Title); break;
                    case 6: result = string.Format("{0} - {1}", "فیلتر", uow.ProductCategoryRepository.Get(x => x, x => x.Id == LinkId).Single().Name); break;
                    case 7: result = string.Format("{0} - {1}", "صفحه پایه", uow.ContentRepository.Get(x => x, x => x.Id == LinkId).Single().Title); break;
                    case 8: result = string.Format("{0} - {1}", "نوع محتوا", readXML.ListOfXContentType().Where(x => x.LanguageId == 1 && x.Id == LinkId).SingleOrDefault().Title); break;
                    default:
                        break;
                }
                return result;
            }
            catch (Exception)
            {

                return string.Format(" آیتم موجود نیست!");
            }
            finally
            {
                uow.Dispose();
            }
        }

        public static string DisplaySelectColorAttribute(this System.Web.Mvc.HtmlHelper helper, int AttributeItemId)
        {
            uow = new UnitOfWorkClass();
            string result = string.Format(" آیتم موجود نیست! ");
            try
            {

                var item = uow.ProductAttributeItemColorRepository.Get(x => x, x => x.Id == AttributeItemId).SingleOrDefault();
                if (item != null)
                    return item.Color;
                else
                    return "";
            }
            catch (Exception)
            {

                return string.Format(" آیتم موجود نیست!");
            }
        }

        public static bool CheckExists(string url)
        {
            Uri uri = new Uri(url);
            if (uri.IsFile) // File is local
                return System.IO.File.Exists(uri.LocalPath);

            try
            {
                HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                request.Method = "HEAD"; // No need to download the whole thing
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                return (response.StatusCode == HttpStatusCode.OK); // Return true if the file exists
            }
            catch
            {
                return false; // URL does not exist
            }
        }

        public static string GetAppRootFolder()
        {
            var appRootFolder = HttpContext.Current.Request.ApplicationPath.ToLower();

            if (!appRootFolder.EndsWith("/"))
            {
                appRootFolder += "/";
            }

            return appRootFolder;
        }
    }
}
