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
    public class TotobItem
    {
        #region Ctor
        public TotobItem()
        {

        }
        #endregion

        #region Properties
        
        public int count { get; set; }

        public int max_pages { get; set; }

        public List<TorobProduct> products { get; set; }

     
        #endregion
    }
}
