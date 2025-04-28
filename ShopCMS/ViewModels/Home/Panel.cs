using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ahmadi.ViewModels.Home
{
    public class Panel
    {
        public Panel()
        {

        }
        #region Properties

        public int TotalContent { get; set; }
        public int TotalAd { get; set; }
        public int TotalProduct { get; set; }
        public int TotalProductPrice { get; set; }
        public int TotalComment { get; set; }
        public int TotalMessages { get; set; }
        public int TotalAdMessage{ get; set; }
        public int TotalAdMessageActive { get; set; }
        public int TotalAdMessageDeActive { get; set; }
        public int TotalCommentActive { get; set; }
        public int TotalCommentDeActive { get; set; }
        public int TotalCommentViewed { get; set; }
        public int TotalCommentUnViewed { get; set; }
        public int TotalContactUs{ get; set; }
        public int TotalContactUsViewed{ get; set; }
        public int TotalContactUsUnViewed{ get; set; }

        public ICollection<ContentTypeUsage> ContentTypeUsages { get; set; }
        public ICollection<ContentTypeCategoryUsae> ContentTypeCategoryUsaes { get; set; }

        public ICollection<CategoryUsage> CategoryUsages { get; set; }
        public ICollection<CategoryStateUsage> CategoryStateUsages { get; set; }
        public ICollection<ProductUsage> ProductUsages { get; set; }
        public ICollection<ProductStateUsage> ProductActiveUsages { get; set; }
        public ICollection<ProductStateUsage> ProductStateUsages { get; set; }
        #endregion

    }

    public class ContentTypeUsage
    {
        public int ContentTypeId { get; set; }
        public string ContentTypeName { get; set; }
        public string ContentTypeShortName { get; set; }
        public double UsageCount{ get; set; }

    }
    public class ContentTypeCategoryUsae
    {
        public int CategoryId { get; set; }
        public string CaegoryName { get; set; }
        public double UsageCount { get; set; }

    }
    public class CategoryUsage
    {
        public int CatId { get; set; }
        public string CatName { get; set; }
        public double UsageCount { get; set; }

    }
    public class CategoryStateUsage
    {
        public string CatStateName { get; set; }
        public double UsageCount { get; set; }

    }
    public class ProductUsage
    {
        public int CatId { get; set; }
        public string CatName { get; set; }
        public double UsageCount { get; set; }

    }
    public class ProductStateUsage
    {
        public string CatStateName { get; set; }
        public double UsageCount { get; set; }
    }
}