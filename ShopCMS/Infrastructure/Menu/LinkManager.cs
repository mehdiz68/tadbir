using CoreLib.Infrastructure;
using CoreLib.ViewModel.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ahmadi.Infrastructure.Menu
{
    public static class LinkManager
    {
        public static string GenerateLink(int id, int? typeid, int? linkid, Guid? LinkUniqIdentifier, string title, string offlink, Domain.Menu ParrentMenu,string Url)
        {
            UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                var HomePage = uow.ContentRepository.Get(x=>x,x => x.LanguageId == 1 && x.IsDefault == true).SingleOrDefault();

                int HomePageId = (HomePage != null ? Convert.ToInt32(HomePage.Id) : 0);
                XMLReader readXML = new XMLReader(Url);
                string FinallLink = "";
                if (typeid.HasValue)
                {
                    switch (typeid.Value)
                    {
                        case 1:
                        case 11:
                            FinallLink = (uow.ContentRepository.Get(x=>x,x => x.Id == linkid).Any() ? uow.ContentRepository.Get(x=>x,x => x.Id == linkid && x.Id == HomePageId).Any() ? GetAppRootFolder(): "/content/" + linkid + "/" + CommonFunctions.NormalizeAddress(uow.ContentRepository.GetByID(linkid).Title) : "#"); break;
                        case 2: FinallLink = (uow.AttachmentRepository.Get(x=>x,x => x.Id == LinkUniqIdentifier).Any() ? "/Content/UploadFiles/" + uow.AttachmentRepository.GetByID(LinkUniqIdentifier).FileName : "#"); break;
                        case 3: FinallLink = (uow.CategoryRepository.Get(x=>x,x => x.Id == linkid).Any() ? "/category/" + linkid + "/" + CommonFunctions.NormalizeAddress(uow.CategoryRepository.GetByID(linkid).Title) : "#"); break;
                        case 4: FinallLink = (uow.TagRepository.Get(x=>x,x => x.Id == linkid).Any() ? "/tag/" + linkid + "/" + CommonFunctions.NormalizeAddress(uow.TagRepository.GetByID(linkid).TagName) : "#"); break;
                        //case 5: dm.Link = db.Sliders.Find(item.LinkId); break;
                        case 6: FinallLink = uow.SocialRepository.GetByID(linkid).Link; break;
                        case 7: FinallLink = (readXML.ListOfXContentType().Any(x => x.Id == linkid) ? "/contentType/" + linkid + "/" + CommonFunctions.NormalizeAddress(readXML.DetailOfXContentType(linkid.Value).Title) : "#"); break;
                        case 8:
                            var cat = uow.ProductCategoryRepository.GetByID(linkid);
                            if (cat != null)
                            {
                                if (!cat.ParrentId.HasValue)
                                    FinallLink = "/TFC/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.Title);
                                else
                                    FinallLink = "/ProductSearch/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.Title);
                            }
                            else
                                FinallLink = "#";
                            break;
                        case 9: FinallLink = "/Galleries"; break;
                        case 10:
                            if (ParrentMenu != null)
                            {
                                cat = uow.ProductCategoryRepository.GetByID(ParrentMenu.LinkId);
                                if (cat != null)
                                    FinallLink = "/ProductSearch/" + ParrentMenu.LinkId + "/" + CommonFunctions.NormalizeAddress(cat.Title) + "/List-" + linkid;
                                else
                                    FinallLink = "#";
                            }
                            else
                                FinallLink = "#";
                            break;
                        case 12:
                            FinallLink = "/Page/" + id + "/" + CommonFunctions.NormalizeAddress(title); break;
                    }
                }
                else
                    FinallLink = offlink;

                return FinallLink;
            }
            catch (Exception)
            {
                return "";
            }
            
        }

        static string GetAppRootFolder()
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