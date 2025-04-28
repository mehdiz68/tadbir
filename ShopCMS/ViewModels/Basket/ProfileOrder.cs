using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;

namespace ahmadi.ViewModels.Basket
{
    public class ProfileOrder
    {
        #region Ctor
        public ProfileOrder()
        {

        }
        #endregion

        #region Properties

        public PagedList.IPagedList<Domain.Order> AllOrders { get; set; }
        public PagedList.IPagedList<Domain.Order> CurrentOrders { get; set; }
        public PagedList.IPagedList<Domain.Order> ProccessOrders { get; set; }
        public PagedList.IPagedList<Domain.Order> SentOrders { get; set; }
        public PagedList.IPagedList<Domain.Order> DeliveredOrders { get; set; }
        public PagedList.IPagedList<Domain.Order> ReturnedOrders { get; set; }
        public PagedList.IPagedList<Domain.Order> CancelOrders { get; set; }
        public PagedList.IPagedList<Domain.Order> CancelWaitOrders { get; set; }
        public PagedList.IPagedList<Domain.Order> EstelamOrders { get; set; }

        #endregion
    }
}
