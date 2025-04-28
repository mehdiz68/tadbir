using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;

namespace Domain.ViewModels
{
    public class HighSaleCategoryProduct
    {
        #region Ctor
        public HighSaleCategoryProduct()
        {

        }
        #endregion

        #region Properties
        public int CatId { get; set; }
        public string CatName { get; set; }
        public string CatPageAddress { get; set; }
        public IEnumerable<ProductItem> ProductItems { get; set; }
        #endregion
    }
}
