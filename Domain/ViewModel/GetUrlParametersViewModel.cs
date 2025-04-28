using System.Collections.Generic;

namespace Domain.ViewModel
{
    public class GetUrlParametersViewModel
    {
        public int id { get; set; }
        public ICollection<AttributeViewModel> attribute { get; set; }
        public ICollection<BrandViewModel> brands { get; set; }
        public int sortby { get; set; }
        public int pageno { get; set; }
    }
    public class AttributeViewModel
    {
        // شماره ای دی
        public int Key1 { get; set; }
        // ردیف
        public int Key2 { get; set; }
        // مقدار
        public string Value { get; set; }
    }
    public class LogicViewModel
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
    }
    public class BrandViewModel
    {
        public int Key { get; set; }
        public int Value { get; set; }
    }
    public class SellerConditionViewModel
    {
        public int Key { get; set; }
        public int Value { get; set; }
    }
    /// <summary>
    /// محدوده قیمتی برای سرچ 
    /// </summary>
    public class PriceRange
    {
        public long StartPrice { get; set; }
        public long EndPrice { get; set; }
    }
    public class getUrlParameter
    {
        public List<AttributeViewModel> AttributeViewModels { get; set; }
        public List<BrandViewModel> BrandViewModels { get; set; }
        public List<SellerConditionViewModel> SellerConditionViewModels { get; set; }
        public List<AttributeViewModel> DbListViewModels { get; set; }
        public List<LogicViewModel> LogicViewModels { get; set; }
        public bool OnlyInStore { get; set; }
        public bool IsExit { get; set; }
        public PriceRange PriceRange { get; set; }
        public bool IsOriginality { get; set; }
        public string SearchStr { get; set; }
        public int ProductStateId { get; set; }
        public int sortby { get; set; }
        public int pagenum { get; set; }
        public int perpage { get; set; }
        public string UrlPathname { get; set; }
        public int productTagId { get; set; }


    }
}
