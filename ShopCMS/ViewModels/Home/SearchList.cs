using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ahmadi.ViewModels.Home
{
    public class SearchList
    {
        public SearchList(int id,string title,string oabstract,int typid,Guid? img,string urlContent)
        {
            UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass(); 
            this.Id = id;
            this.Title = title;
            this.Abstract = oabstract;
            this.TypeId = typid;
           
                if (img.HasValue)
                    this.Img = urlContent + uow.AttachmentRepository.GetByID(img).FileName;
                else
                    this.Img = "/Content/Default/images/default-thumbnail.jpg";


            if (typid == -5)
                PagaAdress = "AdCategory/" + id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid == -4)
                PagaAdress = "Ads/" + id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid == -3)
                PagaAdress = "TFC/" + id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid == -2)
                PagaAdress = "TFP/"+id+"/"+ CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid == -1)
                PagaAdress = "tag/" + id+"/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid == 0)
                PagaAdress = "category/" + id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid > 0)
                PagaAdress = "content/" + id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
        }
        #region Properties

        public int Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string PagaAdress { get; set; }
        public int TypeId { get; set; }
        public string Img { get; set; }


        #endregion

    }

    public class MainSearch
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
        public string PageAddress { get; set; }
        public int TypeId { get; set; }
    }
}