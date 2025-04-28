using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;

namespace Domain.ViewModels
{
    public class ProductCommentViewModel
    {
        #region Ctor
        public ProductCommentViewModel()
        {

        }
        #endregion

        #region Properties

        public ProductItem ProductItem { get; set; }

        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsBuy { get; set; }
        public string Text { get; set; }
        public double AvgPoint { get; set; }
        public bool IsActive { get; set; }
        public int Useful { get; set; }
        public ProductCommentSatisfaction? Satisfaction { get; set; }
        public DateTime InsertDate { get; set; }

        #endregion
    }
}
