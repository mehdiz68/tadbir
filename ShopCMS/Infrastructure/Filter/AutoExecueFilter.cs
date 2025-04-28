using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using UnitOfWork;

namespace ahmadi.Infrastructure.Filter
{


    public class AutoExecueFilter : System.Web.Mvc.ActionFilterAttribute
    {
       
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            UnitOfWorkClass uow = new UnitOfWorkClass();
            DateTime nowdateTime = DateTime.Now;
            var ShoppingPayEstelamMinutes = uow.SettingRepository.Get(s => s.ShoppingPayEstelamMinutes, s => s.LanguageId == 1).SingleOrDefault();


            #region حذف وضعیت استعلامی ها
            if (uow.OrderRepository.Any(x => x.IsOld == false && x.OrderStates.Last().state == Domain.OrderStatus.تایید_سفارش && x.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23 && r.Value.ToLower() == "true")) && x.OrderStates.Last().LogDate.AddMinutes(ShoppingPayEstelamMinutes) <= DateTime.Now))
            {
                var orders = uow.OrderRepository.GetByReturnQueryable(x => x,x=> x.IsOld == false, null, "OrderStates,OrderWallets,OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute,OrderAttributeSelects.OrderAttribute,OrderDeliveries");

                orders = orders.Where(x =>  x.OrderStates.Last().state == Domain.OrderStatus.تایید_سفارش && x.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23 && r.Value == "True")) && x.OrderStates.Last().LogDate.AddMinutes(ShoppingPayEstelamMinutes) <= DateTime.Now);
                foreach (var item in orders)
                {
                    foreach (var item2 in item.OrderDeliveries)
                    {

                        uow.OrderStateRepository.Delete(item2.OrderStates.Last());
                        uow.OrderStateRepository.Insert(new Domain.OrderState { OrderId = item2.OrderId.Value, OrderDeliveryId = item2.Id, Description = "لغو مرسوله به دلیل عدم پرداخت سفارش استعلامی در زمان معین", LogDate = DateTime.Now, state = Domain.OrderStatus.لغو_شده });
                    }

                    //var ConfirmEstelam = item.OrderAttributeSelects.Where(x => x.OrderAttribute.DataType == 23 && x.Value == "True").SingleOrDefault();
                    //uow.OrderAttributeOrderRepository.Delete(ConfirmEstelam);

                }
                uow.Save();

            }
            #endregion

            #region غیر فعال سازی و لغو سفارشات پرداخت نشده
            var expiredOrders = uow.OrderRepository.Get(x => x, x => x.IsOld == false && x.IsExpire==false  && (x.ExpireDate!=null && x.ExpireDate <= nowdateTime) && !x.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23 && r.Value.ToLower() == "true"))  && x.OrderStates.Any(s => s.state == Domain.OrderStatus.تایید_سفارش) && !x.OrderStates.Any(s => s.state == Domain.OrderStatus.تایید_پرداخت), null, "OrderDeliveries,OrderStates,OrderRows");
            foreach (var item in expiredOrders)
            {
                item.IsExpire = true;
                item.New = false;
                foreach (var delivery in item.OrderDeliveries)
                {
                    item.OrderStates.Add(new Domain.OrderState()
                    {
                        LogDate = DateTime.Now,
                        OrderDeliveryId = delivery.Id,
                        state = Domain.OrderStatus.لغو_شده
                    });
                }

                uow.OrderRepository.Update(item);

                //update quantity
                uow.OrderRepository.CheckQuantity(item);
            }
            uow.Save();
            #endregion
        }
    }

}