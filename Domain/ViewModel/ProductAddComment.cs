using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;

namespace Domain.ViewModels
{
    public class ProductAddComment
    {
        #region Ctor
        public ProductAddComment()
        {

        }
        #endregion

        #region Properties

        public ProductComment ProductComment { get; set; }
        public IEnumerable<ProductRank> productRanks { get; set; }
        public string breadCrumb { get; set; }
        public ProductItem productItem { get; set; }
        public bool IsBuy { get; set; }
        public bool AllowAddComment { get; set; }
        #endregion
    }
}
