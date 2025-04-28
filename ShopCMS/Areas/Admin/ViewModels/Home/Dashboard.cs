using System.Linq;
using Domain;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace ahmadi.Areas.Admin.ViewModel.AdminPanel
{
    public class Dashboard
    {
        public Dashboard()
        {
          
        }
        public string Logo { get; set; }
        public string WebSiteName { get; set; }
        public int ProductComments { get; set; }
        public int ProductQuestions { get; set; }
        public int UserMessages { get; set; }
        public double DelayOrders { get; set; }
        public double CancelOrders { get; set; }
        public double ReturnedOrders { get; set; }
        public int UsersSatisfactionCount { get; set; }
        public double UsersSatisfaction { get; set; }
        public IEnumerable<Content> Blogs{ get; set; }
        public IEnumerable<OrderRate> OrderRates { get; set; }
        public IEnumerable<Domain.ViewModels.OrderItemAvg> OrderItemAvgs { get; set; }
        public IEnumerable<ProductComment> ProductCommentList { get; set; }
        public IEnumerable<Comment> CommentList { get; set; }
        public List<ChartState> ProductPriceState { get; set; }
        public List<ChartState> OrderState { get; set; }
        public List<ChartState> BlogState{ get; set; }
        public int Products { get; set; }
        public int Products30s { get; set; }
        public int ProductPrices { get; set; }
        public int ProductPricesEstelam { get; set; }
        public int ProductPricesExist { get; set; }
        public int ProductPricesNotExist { get; set; }
        public int ProductPricesCommingSoon { get; set; }
        public int ProductPricesStopProduce { get; set; }
        public int ProductLetMeKhows { get; set; }
        public int ProductFavorites { get; set; }

        public IEnumerable<ChartState> OrdersChart { get; set; }


    }

    public class ChartState
    {
        public string Name { get; set; }
        public double Count { get; set; }
        public string Link { get; set; }

    }
}
