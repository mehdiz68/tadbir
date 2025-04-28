using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;

namespace Domain.ViewModels
{
    public class ProductCommentList
    {
        #region Ctor
        public ProductCommentList()
        {

        }
        #endregion

        #region Properties
        public int Id { get; set; }
        public Guid? UserAvatar { get; set; }
        public string UserAvatarattachment { get; set; }
        public bool? UserGender { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string InsertDate { get; set; }
        public string InsertTime { get; set; }
        public string FullName { get; set; }
        public bool IsBuy { get; set; }
        public ProductCommentSatisfaction? ProductCommentSatisfaction { get; set; }
        public IEnumerable<ProductRankSelectValue> ProductRankSelectValues { get; set; }
        public IEnumerable<attachment> attachments { get; set; }
        public IEnumerable<string> ProductAdvantages { get; set; }
        public IEnumerable<string> ProductCommentDisAdvantages { get; set; }
        public int UnUseful { get; set; }
        public int Useful { get; set; }
        #endregion
    }
}
