using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;

namespace Domain.ViewModels
{
    public class ProductFAQGoogleList
    {
        #region Ctor
        public ProductFAQGoogleList()
        {

        }
        #endregion

        #region Properties
        public string @type { get; set; }
        public string name { get; set; }
        public acceptedAnswer acceptedAnswer { get; set; }

        #endregion
    }
}
public class acceptedAnswer
{

    public string @type { get; set; }
    public string text { get; set; }
}
