using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;
using Domain.ViewModel;

namespace Domain.ViewModels
{
    public class AmazingOffers
    {
        #region Ctor
        public AmazingOffers()
        {

        }
        #endregion

        #region Properties
        public int Id { get; set; }
        public string Title { get; set; }
        public string Cover { get; set; }
        public string EndDate { get; set; }
        public bool finish { get; set; }
        public int Value { get; set; }
        public IEnumerable<ProductItem> ProductItems { get; set; }
        #endregion
    }
}
