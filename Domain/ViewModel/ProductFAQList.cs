using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;

namespace Domain.ViewModels
{
    public class ProductFAQList
    {
        #region Ctor
        public ProductFAQList()
        {

        }
        #endregion

        #region Properties
        public int Id { get; set; }
        public string UserId { get; set; }
        public Guid? UserAvatar { get; set; }
        public bool? UserGender { get; set; }
        public string Text { get; set; }
        public string InsertDate { get; set; }
        public string InsertTime { get; set; }
        public string FullName { get; set; }
        public string FakeUserFullName { get; set; }
        public  IEnumerable<ProductQuestion> ChildComment { get; set; }
        public int unlike { get; set; }
        public int like { get; set; }
        public int? parrentid { get; set; }
        #endregion
    }
}
