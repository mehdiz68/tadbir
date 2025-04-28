using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;

namespace Domain.ViewModels
{
    public class ProductReviewGoogleList
    {
        #region Ctor
        public ProductReviewGoogleList()
        {

        }
        #endregion

        #region Properties
        public string @context { get; set; }
        public string @type { get; set; }
        public string name { get; set; }
        public IEnumerable<string> image { get; set; }
        public string description { get; set; }
        public int sku { get; set; }
        public brand brand { get; set; }
        public IEnumerable<reviewitem> review{ get; set; }
        public aggregateRating aggregateRating { get; set; }
        public offers offers { get; set; }

        #endregion
    }
}
public class brand
{

    public string @type { get; set; }
    public string name { get; set; }
}


public class reviewitem
{
    public string @type { get; set; }
    public author author { get; set; }
    public string reviewBody { get; set; }
    public reviewRating reviewRating { get; set; }

}
public class author
{
    public string @type { get; set; }
    public string name { get; set; }
}
public class reviewRating
{
    public string @type { get; set; }
    public double ratingValue { get; set; }
    public int bestRating { get; set; }
    public int worstRating { get; set; }

}
public class aggregateRating
{
    public string @type { get; set; }
    public double ratingValue { get; set; }
    public int reviewCount { get; set; }
}
public class offers
{
    public string priceValidUntil { get; set; }
    public string @type { get; set; }
    public string url { get; set; }
    public string priceCurrency { get; set; }
    public long price { get; set; }
    public string availability { get; set; }

}