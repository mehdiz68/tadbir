using DataLayer;
using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Repository.Service
{
    public class OrderService : GenericRepository<Order>
    {
        private readonly OrderStateService _orderStateService;
        private readonly ProductPriceService _productPriceService;
        private readonly OrderAttributeOrderService _orderAttributeOrderService;
        private readonly ProductService _productService;
        private readonly CreateOrderKeyService _createOrderKeyService;
        public OrderService(ahmadiDbContext context, OrderStateService orderStateService, ProductPriceService productPriceService, OrderAttributeOrderService orderAttributeOrderService, ProductService productService, CreateOrderKeyService createOrderKeyService) : base(context)
        {

            this._orderStateService = orderStateService;
            this._productPriceService = productPriceService;
            this._productService = productService;
            this._orderAttributeOrderService = orderAttributeOrderService;
            this._createOrderKeyService = createOrderKeyService;
        }

        public IQueryable<Order> GetAllOrder(string userid)
        {

            return GetByReturnQueryable(x => x, x => x.UserId == userid, x=>x.OrderByDescending(o => o.BankOrderId),"OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount"); 
        }
        public IQueryable<Order> GetCurrentOrder(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                             //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true)
                         where x.Order.UserId == userid 
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state > 0 &&   x.state < OrderStatus.تحویل_داده_شده).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id), x => x.OrderByDescending(s => s.BankOrderId), "OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount");
        }
        public IQueryable<Order> GetProccessOrder(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                             //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true)
                         where x.Order.UserId == userid
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.پردازش_انبار).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id), x => x.OrderByDescending(s => s.BankOrderId), "OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount");
        }
        public IQueryable<Order> GetSentOrder(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                             //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true)
                         where x.Order.UserId == userid
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.ارسال_شده).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id), x => x.OrderByDescending(s => s.BankOrderId), "OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount");
        }
        public IQueryable<Order> GetCancelOrders(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                             //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true)
                         where x.Order.UserId == userid
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.لغو_شده).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id), x => x.OrderByDescending(s => s.BankOrderId), "OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount");
        }

        public IQueryable<Order> GetCancelWaitOrders(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                             //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true)
                         where x.Order.UserId == userid && !x.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.WalletAttribute.DataType == 23))
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.درخواست_لغو).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id), x => x.OrderByDescending(s => s.BankOrderId), "OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount");
        }

        public IQueryable<Order> GetDeliveredOrders(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                             //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true) && x.Order.UserId == userid && !x.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.WalletAttribute.DataType == 23))
                         where x.Order.UserId == userid && !x.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.WalletAttribute.DataType == 23))
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.تحویل_داده_شده).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id), x => x.OrderByDescending(s => s.BankOrderId), "OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount");
        }

        public IQueryable<Order> GetReturnedOrders(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                         where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true) && x.Order.UserId == userid && !x.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.WalletAttribute.DataType == 23))
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.مرجوعی).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id), x => x.OrderByDescending(s => s.BankOrderId), "OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount");
        }

        public IQueryable<Order> GetEstelamOrders(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                         where x.Order.UserId == userid && x.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.WalletAttribute.DataType == 23))
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.در_انتظار_تایید || x.state == OrderStatus.عدم_تایید_سفارش).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id), x => x.OrderByDescending(s => s.CustomerOrderId), "OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount");
        }

        public long AddOrder(string userid, List<Basket> BasketItems, List<ProductBox> productBoxes, string userDescription, bool sendFactor, int BankAccountId, int PaymentType, string codeGift, int bon, short languageid, out bool IsEstelam, out bool validFreeSend)
        {
            try
            {

                bool UserIsHaghighi = context.Users.Find(userid).Haghighi;
                validFreeSend = true;
                IsEstelam = false;
                DateTime dateTime = DateTime.Now;
                var setting = context.Settings.Where(x => x.LanguageId == languageid).FirstOrDefault();
                var ProductBasketItems = _productPriceService.ProductBasketItem(BasketItems);
                bool FreeSendPrice = false;
                bool? code_bon = true;
                long OffPrice = -1;
                int? UsergiftCodeId = null;
                int? giftCodeId = null;
                var bankaccount = context.BankAccounts.Find(BankAccountId);
                int hours = 0;
                switch (PaymentType)
                {
                    case 1: case 2: hours = bankaccount.OnliePaymentHours; break;
                    case 3: hours = bankaccount.CardNumberHours; break;
                    case 4: hours = bankaccount.HasCourierDeliveryPosHours; break;
                    case 5: hours = bankaccount.HasCourierDeliveryCashHours; break;
                    case 6: hours = bankaccount.HasFishHours; break;
                    default:
                        break;
                }



                //order
                Order order = new Order()
                {
                    CurrencyId = 1,
                    DisplaySort = 0,
                    InsertDate = DateTime.Now,
                    IsActive = false,
                    IsExpire = false,
                    LanguageId = languageid,
                    New = true,
                    UserId = userid,
                    Visited = false,
                    BankOrderId = _createOrderKeyService.GetOrderId(),
                    ExpireDate = dateTime.AddHours(hours)

                };

                order.CustomerOrderId = CoreLib.Infrastructure.CommonFunctions.GetOrderCode(order.BankOrderId);

                #region کد و بن تخفیف
                if (!ProductBasketItems.Any(x => x.hasoff))
                {
                    if (!String.IsNullOrEmpty(codeGift))
                    {
                        bool validUserCodeGift = false;
                        var usercode = context.UserCodeGifts.Where(x => x.UserId == userid && x.Code == codeGift).Include("Offer.offerProductCategories").Include("UserCodeGiftLogs").Include("UserCodeGiftLogs.Order").Select(x => new { x.Id, x.Value, x.CodeType, x.Offer, x.UserCodeGiftLogs, x.Code, x.CountUse, x.IsActive, x.MaxValue, x.Offer.ExpireDate, x.Offer.StartDate, eeExpireDate = x.ExpireDate, x.Offer.offerProductCategories }).SingleOrDefault();
                        if (usercode != null)
                        {
                            UsergiftCodeId = usercode.Id;
                            if (usercode.Offer.IsDeleted == false && usercode.IsActive && usercode.Offer.IsActive && usercode.Offer.state && (usercode.StartDate == null || usercode.StartDate <= dateTime) && (usercode.ExpireDate == null || usercode.ExpireDate >= dateTime) && (usercode.eeExpireDate == null || usercode.eeExpireDate >= dateTime) && (usercode.UserCodeGiftLogs.Where(b => b.state == true).Count() < usercode.CountUse || usercode.CountUse == 0))//کد تخفیف نامعتبر
                            {
                                if (usercode.Offer != null)
                                {
                                    if (usercode.Offer.CodeTypeValueCode == 3)
                                    {
                                        FreeSendPrice = true;
                                        OffPrice = 0;
                                        code_bon = true;
                                    }
                                    else
                                    {
                                        long BasketSum = ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity);
                                        if (usercode.offerProductCategories.Any())
                                        {
                                            List<int> Offercats = usercode.offerProductCategories.Select(a => a.CatId).ToList();
                                            List<int> OffercatsWithSubcats = new List<int>();
                                            foreach (var item in Offercats)
                                                OffercatsWithSubcats.AddRange(context.Database.SqlQuery<int>("exec GetSubCats @CatId", new SqlParameter("@CatId", item)).ToList());
                                            BasketSum = ProductBasketItems.Where(x => OffercatsWithSubcats.Contains(x.CatId)).Sum(x => x.finalPrice * x.UserQuantity);
                                        }

                                        long ofp = Convert.ToInt64(Math.Ceiling(usercode.CodeType == 1 ? usercode.Value : (usercode.Value / 100.0) * BasketSum));
                                        if (ofp > usercode.MaxValue && usercode.MaxValue > 0)
                                            ofp = usercode.MaxValue;
                                        OffPrice = ofp;
                                        code_bon = true;
                                    }
                                    validUserCodeGift = true;
                                }
                            }
                        }
                        if (validUserCodeGift == false)
                        {
                            var GeneralCodeGift = context.GeneralCodeGift.Include("Offer.offerProductCategories").Include("GeneralCodeGiftLogs").Include("GeneralCodeGiftLogs.Order").Where(x => x.Code == codeGift).Select(x => new { x.Id, x.Offer, x.IsActive, StartDate = x.Offer.StartDate, x.Offer.ExpireDate, x.generalCode, x.MaxValue, x.OfferId, x.Value, x.CodeType, x.Offer.CodeTypeValueCode, x.GeneralCodeGiftLogs, x.CountUse, x.Offer.offerProductCategories }).FirstOrDefault();
                            if (GeneralCodeGift != null)
                            {
                                giftCodeId = GeneralCodeGift.Id;
                                if (GeneralCodeGift.Offer.IsDeleted == false && GeneralCodeGift.IsActive && GeneralCodeGift.Offer.IsActive && GeneralCodeGift.Offer.state && (GeneralCodeGift.StartDate == null || GeneralCodeGift.StartDate <= dateTime) && (GeneralCodeGift.ExpireDate == null || GeneralCodeGift.ExpireDate >= dateTime) && (GeneralCodeGift.GeneralCodeGiftLogs.Where(b => b.state == true && b.Order.UserId == userid).Count() < GeneralCodeGift.CountUse || GeneralCodeGift.CountUse == 0))//کد تخفیف نامعتبر
                                {
                                    if (GeneralCodeGift.generalCode == GeneralCodeType.تخفیف_ارسال_رایگان)
                                    {

                                        FreeSendPrice = true;
                                        OffPrice = 0;
                                        code_bon = true;
                                    }
                                    else
                                    {
                                        long BasketSum = ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity);
                                        if (GeneralCodeGift.offerProductCategories.Any())
                                        {
                                            List<int> Offercats = GeneralCodeGift.offerProductCategories.Select(a => a.CatId).ToList();
                                            List<int> OffercatsWithSubcats = new List<int>();
                                            foreach (var item in Offercats)
                                                OffercatsWithSubcats.AddRange(context.Database.SqlQuery<int>("exec GetSubCats @CatId", new SqlParameter("@CatId", item)).ToList());
                                            BasketSum = ProductBasketItems.Where(x => OffercatsWithSubcats.Contains(x.CatId)).Sum(x => x.finalPrice * x.UserQuantity);
                                        }

                                        long ofp = Convert.ToInt64(Math.Ceiling(GeneralCodeGift.CodeType == 1 ? GeneralCodeGift.Value : (GeneralCodeGift.Value / 100.0) * BasketSum));
                                        if (ofp > GeneralCodeGift.MaxValue && GeneralCodeGift.MaxValue > 0)
                                            ofp = GeneralCodeGift.MaxValue;
                                        OffPrice = ofp;
                                        code_bon = true;
                                    }

                                }
                            }
                        }
                    }
                    else if (bon > 0)
                    {
                        int UserCurrentBon = context.UserBons.Where(x => x.UserId == userid && x.state == true && x.ExpireDate > dateTime && x.Value > x.UsedValue).Sum(x => x.Value - x.UsedValue);
                        int? UserMaxBon = ProductBasketItems.Sum(x => x.MaxBon);

                        if (UserCurrentBon > 0 && bon <= UserCurrentBon && (bon <= UserMaxBon || UserMaxBon == 0))//بن تخفیف نامعتبر
                        {
                            code_bon = false;
                            OffPrice = setting.BonPrice * bon;
                        }
                    }
                }
                #endregion


                try
                {
                    //order delivery
                    order.OrderDeliveries = new List<OrderDelivery>();


                    foreach (var item in BasketItems.First().shippings)
                    {
                        int? adid = null, wid = null;
                        if (item.addressId > 0)
                            adid = item.addressId;
                        if (item.sendwayWorktimeId > 0)
                            wid = item.sendwayWorktimeId;
                        DateTime? reqdate = null;
                        if (item.deliveryDate != null)
                        {
                            reqdate = Convert.ToDateTime(item.deliveryDate);
                            var timeselect = context.ProductSendWayWorkTimes.Find(wid.Value).EndTime;
                            if (PaymentType == 4)
                            {
                                hours = bankaccount.HasCourierDeliveryPosHours;
                                order.ExpireDate = reqdate.Value.AddHours(hours).AddHours(timeselect.TotalHours);
                            }
                            else if (PaymentType == 5)
                            {
                                hours = bankaccount.HasCourierDeliveryCashHours;
                                order.ExpireDate = reqdate.Value.AddHours(hours).AddHours(timeselect.TotalHours);
                            }

                        }
                        order.OrderDeliveries.Add(new OrderDelivery()
                        {
                            DeliveryState = DeliveryState.تحویل_به_موقع,
                            ProductSendWayWorkTimeId = wid,
                            RequestDate = reqdate,
                            SendWayId = item.sendwayId,
                            UserAddressId = adid,
                            ProductPackageType = item.ProductPackageType,
                            Insurance = item.extraprice
                        });
                    }
                    Insert(order);
                    context.SaveChanges();

                    //order row
                    order.OrderRows = new List<OrderRow>();
                    foreach (var item in ProductBasketItems.GroupBy(x => x.PacakgeType))
                    {
                        foreach (var item2 in ProductBasketItems.Where(x => x.PacakgeType == item.Key))
                        {
                            order.OrderRows.Add(new OrderRow()
                            {
                                Price = item2.finalPrice,
                                ProductId = item2.ProductId,
                                ProductPriceId = item2.Id,
                                Quantity = item2.UserQuantity,
                                RawPrice = item2.Price,
                                ProductOfferId = item2.offerid,
                                OrderDeliveryId = order.OrderDeliveries.Where(x => x.ProductPackageType == item2.ProductPackageType).First().Id,
                                taxValue = UserIsHaghighi ? Convert.ToInt32(Math.Ceiling(item2.finalPrice * item2.UserQuantity * (context.Taxes.Find(item2.TaxId).TaxPercent / 100.0))) : 0
                            });
                            var pr = context.Products.Find(item2.ProductId);
                            pr.SellCount += item2.UserQuantity;
                        }
                    }
                    Update(order);
                    context.SaveChanges();

                    //order attributes
                    order.OrderAttributeSelects = new List<OrderAttributeOrder>();
                    order.OrderStates = new List<OrderState>();
                    OrderAttribute sendPrice = context.OrderAttributes.Where(x => x.DataType == 14).Single();
                    OrderAttribute valueAdded = context.OrderAttributes.Where(x => x.DataType == 15).Single();
                    OrderAttribute usrdescr = context.OrderAttributes.Where(x => x.DataType == 18).Single();
                    OrderAttribute sendfactor = context.OrderAttributes.Where(x => x.DataType == 25).Single();
                    OrderAttribute priceOff = context.OrderAttributes.Where(x => x.DataType == 23).Single();
                    OrderAttribute priceOffType = context.OrderAttributes.Where(x => x.DataType == 24).Single();
                    OrderAttribute feeSend = context.OrderAttributes.Where(x => x.DataType == 29).Single();
                    long sumPrice = 0, sumOff = 0;
                    int cost = 0, sumcost = 0;
                    foreach (var item in context.OrderDeliveries.Where(x => x.OrderId == order.Id))
                    {
                        int CityId = item.UserAddressId > 0 ? context.UserAddresses.Find(item.UserAddressId.Value).CityId.Value : context.Users.Find(userid).CityId.Value;
                        sumOff = 0;


                        //هزینه ارسال
                        if (FreeSendPrice)
                            cost = 0;
                        else
                        {
                            int extraPrice = ProductBasketItems.Max(x => x.extraprice);
                            int InsuranceCost = (item.ProductSendWay.HasExtraPrice && BasketItems.First().shippings.Where(x => x.sendwayId == item.ProductSendWay.Id).First().extraprice ? Convert.ToInt32(Math.Ceiling((ProductBasketItems.Sum(x => x.finalPrice) * 0.005) * 0.001) * 1000) : 0);
                            InsuranceCost = InsuranceCost > 0 ? InsuranceCost > 100000 ? 100000 : InsuranceCost < 15000 ? 15000 : InsuranceCost : 0;
                            int PackageCost = (item.ProductSendWay.HasExtraPrice ? Convert.ToInt32(Math.Ceiling(extraPrice * 0.001) * 1000) : 0);

                            int? costvalue = _productPriceService.GetSendwayCost(ProductBasketItems.Select(x => x.Id).ToList(), CityId, BasketItems.First().shippings.Where(x => x.ProductPackageType == item.ProductPackageType).First().sendwayBoxId, item.SendWayId.Value, ProductBasketItems.Sum(x => x.productWeight * x.UserQuantity), ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity), out FreeSendPrice) + InsuranceCost + PackageCost;
                            if (costvalue == -1)
                            {
                                validFreeSend = false;
                                return order.BankOrderId;
                            }
                            else if (FreeSendPrice)
                                cost = 0;
                            else
                            {
                                var SendWayBoxPrice = productBoxes.Where(x => x.SendwayBox.ProductPackageType == item.ProductPackageType).SelectMany(x => x.sendWayBoxPrices.Where(a => a.productSendWay.Id == item.SendWayId)).OrderByDescending(x => x.BoxMass).ThenByDescending(s => s.IsDefault).ThenBy(s => s.PasKeraye).ThenBy(s => s.cost).First();
                                cost = SendWayBoxPrice.PasKeraye ? 0 : costvalue.HasValue ? costvalue.Value : 0;
                                if (FreeSendPrice)
                                    cost = 0;
                                //else if (!SendWayBoxPrice.PasKeraye)
                                //    cost = SendWayBoxPrice.cost;
                            }
                        }
                        sumcost += cost;
                        order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                        {
                            AttributeId = sendPrice.Id,
                            OrderDeliveryId = item.Id,
                            Value = cost.ToString()
                        });

                        //جمع محصولات
                        List<int> ids = productBoxes.Where(x => x.SendwayBox.ProductPackageType == item.ProductPackageType).SelectMany(x => x.ProductPriceIdList).ToList();
                        int valueAddedval = 0;
                        foreach (var product in ProductBasketItems.Where(x => x.ProductPackageType == item.ProductPackageType))
                        {

                            if (product.TaxId.HasValue && UserIsHaghighi)
                                valueAddedval += Convert.ToInt32(Math.Ceiling(product.finalPrice * product.UserQuantity * (context.Taxes.Find(product.TaxId).TaxPercent / 100.0)));
                            sumPrice += (product.finalPrice * product.UserQuantity) + valueAddedval;
                            sumOff += product.offFinalValue * product.UserQuantity;
                        }

                        if (OffPrice == -1)
                        {
                            if (UsergiftCodeId.HasValue || giftCodeId.HasValue)
                            {
                                order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                                {
                                    AttributeId = priceOffType.Id,
                                    OrderDeliveryId = item.Id,
                                    Value = "4"
                                });
                            }

                            //مقدار تخفیف محصول 
                            order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                            {
                                AttributeId = priceOff.Id,
                                OrderDeliveryId = item.Id,
                                Value = sumOff.ToString()
                            });
                        }
                        //ارزش افزوده
                        order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                        {
                            AttributeId = valueAdded.Id,
                            OrderDeliveryId = item.Id,
                            Value = valueAddedval.ToString()
                        });
                        //توضیحات کاربر
                        order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                        {
                            AttributeId = usrdescr.Id,
                            OrderDeliveryId = item.Id,
                            Value = !String.IsNullOrEmpty(userDescription) ? userDescription : "-"
                        });
                        //ارسال فاکتور چاپی
                        order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                        {
                            AttributeId = sendfactor.Id,
                            OrderDeliveryId = item.Id,
                            Value = sendFactor ? "1" : "0"
                        });
                        //وضعیت
                        order.OrderStates.Add(new OrderState()
                        {
                            LogDate = DateTime.Now,
                            OrderDeliveryId = item.Id,
                            state = OrderStatus.در_انتظار_تایید
                        });
                        List<int> productPriceId = order.OrderRows.Select(x => x.ProductPriceId.Value).ToList();
                        if (!context.ProductPrices.Any(x => productPriceId.Contains(x.Id) && x.ProductStateId == 2))
                        {
                            order.OrderStates.Add(new OrderState()
                            {
                                LogDate = DateTime.Now,
                                OrderDeliveryId = item.Id,
                                state = OrderStatus.تایید_سفارش
                            });
                        }
                        else
                            IsEstelam = true;

                        //off
                        if (OffPrice >= 0)
                        {
                            order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                            {
                                AttributeId = priceOffType.Id,
                                OrderDeliveryId = item.Id,
                                Value = code_bon.Value ? "4" : "3"
                            });
                            order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                            {
                                AttributeId = priceOff.Id,
                                OrderDeliveryId = item.Id,
                                Value = OffPrice.ToString()
                            });

                        }
                        //ارسال رایگان
                        if (FreeSendPrice)
                        {
                            order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                            {
                                AttributeId = feeSend.Id,
                                OrderDeliveryId = item.Id,
                                Value = "1"
                            });
                        }
                    }


                    Update(order);
                    context.SaveChanges();

                    //code and bon log
                    if (!String.IsNullOrEmpty(codeGift) && UsergiftCodeId.HasValue && code_bon == true)
                    {
                        order.UserCodeGiftLogs = new List<UserCodeGiftLog>();
                        order.UserCodeGiftLogs.Add(new UserCodeGiftLog()
                        {
                            InsertDate = dateTime,
                            UserCodeGiftId = UsergiftCodeId.Value,
                            Value = OffPrice,
                            state = false
                        });
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(codeGift) && giftCodeId.HasValue && code_bon == true)
                        {
                            order.GeneralCodeGiftLogs = new List<GeneralCodeGiftLog>();
                            order.GeneralCodeGiftLogs.Add(new GeneralCodeGiftLog()
                            {
                                InsertDate = dateTime,
                                GeneralCodeGiftId = giftCodeId.Value,
                                Value = OffPrice,
                                state = false
                            });
                        }
                    }
                    if (bon > 0 && code_bon == false && OffPrice > 0)
                    {
                        var tempbon = bon;
                        order.UserBonLogs = new List<UserBonLog>();
                        foreach (var item2 in context.UserBons.Where(x => x.UserId == userid && x.state == true && x.ExpireDate > dateTime && x.Value > x.UsedValue).OrderBy(x => x.ExpireDate))
                        {
                            if ((item2.Value - item2.UsedValue) > 0)
                            {
                                order.UserBonLogs.Add(new UserBonLog()
                                {
                                    InsertDate = dateTime,
                                    UserBonId = item2.Id,
                                    Value = OffPrice,
                                    state = false
                                });

                                if ((item2.Value - item2.UsedValue) >= tempbon)
                                {
                                    item2.UsedValue += tempbon;
                                    tempbon = 0;
                                    break;
                                }
                                else
                                {
                                    item2.UsedValue += (item2.Value - item2.UsedValue);
                                    tempbon = (item2.Value - item2.UsedValue);
                                }
                            }
                        }
                    }
                    if (ProductBasketItems.Sum(x => x.Bon) > 0)
                    {
                        order.UserBons = new List<UserBon>();
                        order.UserBons.Add(new UserBon()
                        {
                            InsertDate = dateTime,
                            state = false,
                            UsedValue = 0,
                            Value = ProductBasketItems.Sum(x => x.Bon).Value,
                            UserId = userid,
                            ExpireDate = dateTime.AddDays(setting.BonExpireDay)
                        });
                    }

                    Update(order);
                    context.SaveChanges();

                    //wallet
                    Wallet NewWallet = new Wallet();
                    NewWallet.BankAccountId = BankAccountId;
                    NewWallet.DepositOrWithdrawal = true;
                    NewWallet.ForWhat = context.ForWhats.Where(x => x.ForWhatType == ForWhatType.پرداخت_سفارش).SingleOrDefault();
                    NewWallet.InsertDate = DateTime.Now;
                    NewWallet.LanguageId = languageid;
                    NewWallet.PaymentType = PaymentType;
                    NewWallet.Price = sumPrice + sumcost - (OffPrice > 0 ? OffPrice : 0);
                    NewWallet.State = false;
                    NewWallet.UserId = order.UserId;
                    if (IsEstelam)
                    {
                        NewWallet.WalletAttributeWallets = new List<WalletAttributeWallet>();
                        foreach (var item in context.OrderDeliveries.Where(x => x.OrderId == order.Id))
                        {

                            NewWallet.WalletAttributeWallets.Add(new WalletAttributeWallet()
                            {
                                WalletAttributeId = context.WalletAttributes.Where(x => x.DataType == 23).Single().Id,
                                Value = "0",
                                OrderDeliveryId = item.Id
                            });

                        }

                        //if now shopping is off
                        var dateTimeday = DateTime.Now.TimeOfDay;
                        short weekday = (short)DateTime.Now.PersionDayOfWeek();
                        if (!context.ShoppingWorkTimes.Where(x => x.SettingId == setting.Id && x.IsActive && x.WeekDay == weekday && x.StartTime < dateTimeday && x.EndTime > dateTimeday).Any())
                        {
                            var nextDate = context.ShoppingWorkTimes.Where(x => x.SettingId == setting.Id && x.IsActive && x.WeekDay > weekday).OrderBy(x => x.WeekDay).ThenBy(x => x.StartTime).FirstOrDefault();
                            if (nextDate == null)
                                nextDate = context.ShoppingWorkTimes.OrderBy(x => x.WeekDay).ThenBy(x => x.StartTime).FirstOrDefault();
                            var finalDate = DateTime.Now.Date;
                            for (int i = 1; i < 7; i++)
                            {
                                if ((short)finalDate.AddDays(i).PersionDayOfWeek() == nextDate.WeekDay)
                                    finalDate = finalDate.AddDays(i).AddHours(nextDate.StartTime.TotalHours).AddHours(hours);

                            }
                            order.ExpireDate = finalDate;
                        }

                    }
                    context.Wallets.Add(NewWallet);
                    context.SaveChanges();
                    //walletorder
                    order.OrderWallets = new List<OrderWallet>();
                    order.OrderWallets.Add(new OrderWallet() { Wallet = NewWallet });
                    Update(order);
                    context.SaveChanges();



                    //codegiftlog
                    //bon gift
                    //bongiftlog

                    //order isactive and new set true
                    return order.BankOrderId;
                }
                catch (Exception ex)
                {
                    try
                    {

                        Delete(order);
                        context.SaveChanges();
                    }
                    catch (Exception)
                    {

                    }

                    EventLog eventLog = new EventLog()
                    {
                        IP = "127.0.0.1",
                        LogType = 5,
                        ControllerName = "Cart",
                        ActionName = "AddOrder",
                        RequestType = false,
                        StatusCode = 500,
                        Description = ex.Message + ex.InnerException != null ? ex.InnerException.Message : "",
                        LogDateTime = DateTime.Now,
                        UserId = userid
                    };
                    context.EventLogs.Add(eventLog);
                    context.SaveChanges();

                    return 0;
                }
            }
            catch (Exception ex)
            {
                IsEstelam = false;
                validFreeSend = true;
                EventLog eventLog = new EventLog()
                {
                    IP = "127.0.0.1",
                    LogType = 5,
                    ControllerName = "Cart",
                    ActionName = "AddOrder",
                    RequestType = false,
                    StatusCode = 500,
                    Description = ex.Message + ex.InnerException != null ? ex.InnerException.Message : "",
                    LogDateTime = DateTime.Now,
                    UserId = userid
                };
                context.EventLogs.Add(eventLog);
                context.SaveChanges();

                return 0;
            }
        }

        public async Task<bool> UpdateQuantity(Order order)
        {
            try
            {
                //update quantity
                foreach (var item in order.OrderRows)
                {
                    var prp = context.ProductPrices.Find(item.ProductPriceId);
                    prp.Quantity -= item.Quantity;
                    //prp.MaxBasketCount -= item.Quantity;
                    //ناموجود شود
                    if (prp.Quantity < 1)
                    {
                        prp.Quantity = 0;
                        prp.ProductStateId = 5;
                        ////اگر تنوع پیش فرض می باشد، محصول ناموجود شود
                        //if (prp.IsDefault)
                        //{
                        //    var pr = context.Products.Find(item.ProductId);
                        //    pr.ProductStateId = 5;
                        //}
                    }
                    //if (prp.MaxBasketCount < 1)
                    //    prp.MaxBasketCount = 0;
                    await context.SaveChangesAsync();

                    var proffer = context.ProductOffers.Where(a => a.ProductPrice.ProductStateId < 3 && a.ProductPriceId == item.ProductPriceId && a.Offer.IsActive && a.Offer.state == true && ((a.Offer.ExpireDate != null && a.Offer.ExpireDate >= DateTime.Now) || a.Offer.ExpireDate == null) && ((a.Offer.StartDate != null && a.Offer.StartDate <= DateTime.Now) || a.Offer.StartDate == null)).FirstOrDefault();
                    if (proffer != null)
                    {
                        proffer.Quantity -= item.Quantity;
                        //proffer.MaxBasketCount -= item.Quantity;
                        if (proffer.Quantity < 1)
                            proffer.Quantity = 0;
                        //if (proffer.MaxBasketCount < 1)
                        //    proffer.MaxBasketCount = 0;
                    }
                    await context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckQuantity(Order order)
        {
            try
            {
                //update quantity
                foreach (var item in order.OrderRows)
                {
                    var prp = context.ProductPrices.Find(item.ProductPriceId);
                    prp.Quantity += item.Quantity;
                    //موچود شود
                    if (prp.Quantity > 0 && prp.ProductStateId == 5)
                    {
                        prp.ProductStateId = 1;
                        ////اگر تنوع پیش فرض می باشد، محصول موجود شود
                        //if (prp.IsDefault)
                        //{
                        //    var pr = context.Products.Find(item.ProductId);
                        //    pr.ProductStateId = 1;
                        //    context.SaveChanges();
                        //}
                    }
                    context.SaveChanges();

                    var proffer = context.ProductOffers.Where(a => a.ProductPrice.ProductStateId < 3 && a.ProductPriceId == item.ProductPriceId && a.Offer.IsActive && a.Offer.state == true && ((a.Offer.ExpireDate != null && a.Offer.ExpireDate >= DateTime.Now) || a.Offer.ExpireDate == null) && ((a.Offer.StartDate != null && a.Offer.StartDate <= DateTime.Now) || a.Offer.StartDate == null)).FirstOrDefault();
                    if (proffer != null)
                    {
                        proffer.Quantity += item.Quantity;
                        context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool GiftCatValid(List<int> giftCatIds, List<int> basketCatIds)
        {
            bool valid = false;
            if (giftCatIds.Any())
            {
                foreach (var item in basketCatIds)
                {
                    if (giftCatIds.Any(x => x == item))
                        valid = true;
                }
            }
            return valid;
        }


    }
}
