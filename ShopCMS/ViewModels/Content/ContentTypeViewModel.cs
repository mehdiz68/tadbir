using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ahmadi.ViewModels.Content
{
    public class ContentTypeViewModel
    {
        public ContentTypeViewModel()
        {

        }
        #region Properties
        
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Title { get; set; }
        
        public string Abstract { get; set; }
        
        public int LanguageId { get; set; }

        public virtual IList<Domain.Content> Contents { get; set; }
        public bool IsVideo { get; set; }

        #endregion

    }
}