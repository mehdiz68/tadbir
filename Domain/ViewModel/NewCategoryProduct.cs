using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;

namespace Domain.ViewModels
{
    public class NewCategoryProduct
    {
        #region Ctor
        public NewCategoryProduct()
        {

        }
        #endregion

        #region Properties
        public int CatId { get; set; }
        public List<int> ProductIds { get; set; }
        public string CatName { get; set; }
        public string CatPageAddress { get; set; }
        public string CatDescription { get; set; }
        public string CatImage { get; set; }
        public IEnumerable<ProductItem> ProductItems { get; set; }
        #endregion
    }
}
