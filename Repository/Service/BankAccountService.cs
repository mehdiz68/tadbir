using CoreLib.Infrastructure;
using DataLayer;
using Domain;
using Domain.ViewModel;
using Repository.ir.shaparak.bpm;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Repository.Service
{
    /// <summary>
    /// سرویس مربوط به پرداخت های اینترنتی
    /// </summary>
    public class BankAccountService : GenericRepository<BankAccount>
    {
        private readonly CreateOrderKeyService _createOrderKeyService;

        public BankAccountService(ahmadiDbContext context, CreateOrderKeyService createOrderKeyService) : base(context)
        {
            this._createOrderKeyService = createOrderKeyService;
        }
        // بانک ملت
        public string MellatPayMentStart(long amount, string callBackUrl, int forwhatId, string userId, int? paymentType, string systemDescription, long orderId)
        {
            string errLine = string.Empty;
            try
            {
                errLine = "خوندن بانک ";
                // بانک ای شماره 1 بانم ملت است
                var bank = dbSet.AsNoTracking().AsQueryable().Include(c => c.BankAccountOnlineInfo).FirstOrDefault(c => c.BankId == 1);

                string st = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                string et = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');

                errLine = "  'گرفتن سرویس ";
                PaymentGatewayImplService bpService = new ir.shaparak.bpm.PaymentGatewayImplService();
                errLine = "گرفتن اوردر ای دی  ";
                //long orderId = _createOrderKeyService.GetOrderId();
                errLine = "شروع ارتباط با بانک ";
                string result = bpService.bpPayRequest(Convert.ToInt64(bank.BankAccountOnlineInfo.TerminalId),
                    bank.BankAccountOnlineInfo.UserName,
                    bank.BankAccountOnlineInfo.Password,
                    orderId,
                    amount * 10,
                    DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0'),
                    DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0'),
                    "", callBackUrl, "0");
                errLine = " 'گرفتن نتیجه از بانک ";
                String[] resultArray = result.Split(',');
                if (resultArray[0] == "0")
                {
                    var order = context.Orders.Include("OrderWallets.wallet.WalletAttributeWallets").Include(x => x.OrderDeliveries).Where(x => x.BankOrderId == orderId).Single();
                    errLine = " نتیجه از بانک دریافت شد";
                    var wallet = new Wallet();
                    wallet.WalletAttributeWallets = new List<WalletAttributeWallet>();
                    wallet.ForWhatId = forwhatId;
                    wallet.UserId = userId;
                    wallet.BankAccountId = bank.Id;
                    wallet.DepositOrWithdrawal = true;
                    wallet.PaymentType = paymentType;
                    wallet.Price = amount;
                    wallet.State = false;
                    wallet.SystemDescription = systemDescription;
                    //wallet.WalletAttributeWallets.Clear();
                    wallet.InsertDate = DateTime.Now;
                    context.Wallets.Add(wallet);
                    context.SaveChanges();
                    //walletorder
                    order.OrderWallets.Add(new OrderWallet() { Wallet = wallet });
                    context.SaveChanges();
                    //Add info
                    WalletAttribute PaymentAuthority = context.WalletAttributes.Where(x => x.DataType == 15).FirstOrDefault();
                    WalletAttribute PaymentAuthorityStatus = context.WalletAttributes.Where(x => x.DataType == 14).FirstOrDefault();
                    errLine = " دادن اتریبیوت ها" + resultArray[1] + "-" + resultArray[0] + "-" + PaymentAuthority.Id + "-" + PaymentAuthorityStatus.Id;

                    if (!wallet.WalletAttributeWallets.Any())
                    {
                        wallet.WalletAttributeWallets = new List<WalletAttributeWallet>();
                        foreach (var item in order.OrderDeliveries)
                        {
                            wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthority.Id, Value = resultArray[1] });
                            wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthorityStatus.Id, Value = resultArray[0] });

                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        foreach (var item in order.OrderDeliveries)
                        {
                            if (wallet.WalletAttributeWallets.Any(x => x.WalletAttributeId == PaymentAuthority.Id))
                                wallet.WalletAttributeWallets.Where(x => x.WalletAttributeId == PaymentAuthority.Id).First().Value = resultArray[1];
                            else
                                wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthority.Id, Value = resultArray[1] });

                            if (wallet.WalletAttributeWallets.Any(x => x.WalletAttributeId == PaymentAuthorityStatus.Id))
                                wallet.WalletAttributeWallets.Where(x => x.WalletAttributeId == PaymentAuthorityStatus.Id).First().Value = resultArray[0];
                            else
                                wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthorityStatus.Id, Value = resultArray[0] });

                        }
                        context.SaveChanges();
                    }

                    errLine = "2 دادن اتریبیوت ها";
                }
                return result;
            }
            catch (Exception ex)
            {
                return errLine + " - " + ex;
            }
            // بانک مورد نظر

        }
        public string MellatPayMentStart(long amount, string callBackUrl, long orderId)
        {
            string errLine = string.Empty;
            try
            {
                var order = context.Orders.Include("OrderWallets.wallet.WalletAttributeWallets").Include(x => x.OrderDeliveries).Where(x => x.BankOrderId == orderId).Single();
                order.BankOrderId = _createOrderKeyService.GetOrderId();
                order.CustomerOrderId = CoreLib.Infrastructure.CommonFunctions.GetOrderCode(order.BankOrderId);
                context.SaveChanges();
                orderId = order.BankOrderId;

                errLine = "خوندن بانک ";
                // بانک ای شماره 1 بانم ملت است
                var bank = dbSet.AsNoTracking().AsQueryable().Include(c => c.BankAccountOnlineInfo).FirstOrDefault(c => c.BankId == 1);

                string st = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                string et = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');

                errLine = "  'گرفتن سرویس ";
                PaymentGatewayImplService bpService = new ir.shaparak.bpm.PaymentGatewayImplService();
                errLine = "گرفتن اوردر ای دی  ";
                //long orderId = _createOrderKeyService.GetOrderId();
                errLine = "شروع ارتباط با بانک ";
                string result = bpService.bpPayRequest(Convert.ToInt64(bank.BankAccountOnlineInfo.TerminalId),
                    bank.BankAccountOnlineInfo.UserName,
                    bank.BankAccountOnlineInfo.Password,
                    orderId,
                    amount * 10,
                    DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0'),
                    DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0'),
                    "", callBackUrl, "0");
                errLine = " 'گرفتن نتیجه از بانک ";
                String[] resultArray = result.Split(',');
                if (resultArray[0] == "0")
                {
                    errLine = " نتیجه از بانک دریافت شد";
                    var wallet = order.OrderWallets.First().Wallet;

                    //Add info
                    WalletAttribute PaymentAuthority = context.WalletAttributes.Where(x => x.DataType == 15).FirstOrDefault();
                    WalletAttribute PaymentAuthorityStatus = context.WalletAttributes.Where(x => x.DataType == 14).FirstOrDefault();
                    errLine = " دادن اتریبیوت ها" + resultArray[1] + "-" + resultArray[0] + "-" + PaymentAuthority.Id + "-" + PaymentAuthorityStatus.Id;

                    if (!wallet.WalletAttributeWallets.Any())
                    {
                        wallet.WalletAttributeWallets = new List<WalletAttributeWallet>();
                        foreach (var item in order.OrderDeliveries)
                        {
                            wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthority.Id, Value = resultArray[1] });
                            wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthorityStatus.Id, Value = resultArray[0] });

                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        foreach (var item in order.OrderDeliveries)
                        {
                            if (wallet.WalletAttributeWallets.Any(x => x.WalletAttributeId == PaymentAuthority.Id))
                                wallet.WalletAttributeWallets.Where(x => x.WalletAttributeId == PaymentAuthority.Id).First().Value = resultArray[1];
                            else
                                wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthority.Id, Value = resultArray[1] });

                            if (wallet.WalletAttributeWallets.Any(x => x.WalletAttributeId == PaymentAuthorityStatus.Id))
                                wallet.WalletAttributeWallets.Where(x => x.WalletAttributeId == PaymentAuthorityStatus.Id).First().Value = resultArray[0];
                            else
                                wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthorityStatus.Id, Value = resultArray[0] });

                        }
                        context.SaveChanges();
                    }

                  
                    errLine = "2 دادن اتریبیوت ها";
                }
                return result;
            }
            catch (Exception ex)
            {
                return errLine + " - " + ex;
            }
            // بانک مورد نظر

        }


        //بانک سامان
        public string SamanPayMentStart(long amount, string callBackUrl, long orderId)
        {
            string token = "";
            string errLine = string.Empty;
            try
            {
                errLine = "خوندن بانک ";
                // بانک ای شماره 1 بانم ملت است
                var bank = dbSet.AsNoTracking().AsQueryable().Include(c => c.BankAccountOnlineInfo).FirstOrDefault(c => c.BankId == 8);


                errLine = "  'گرفتن سرویس ";
                ir.shaparak.sep.PaymentIFBinding paymentIFBinding = new ir.shaparak.sep.PaymentIFBinding();
                errLine = "گرفتن اوردر ای دی  ";
                //long orderId = _createOrderKeyService.GetOrderId();
                errLine = "شروع ارتباط با بانک ";
                token = paymentIFBinding.RequestToken(bank.BankAccountOnlineInfo.TerminalId.ToString(), orderId.ToString(), amount * 10, 0, 0, 0, 0, 0, 0, "", "", 0, callBackUrl);

                errLine = " 'گرفتن نتیجه از بانک ";

                var order = context.Orders.Include("OrderWallets.wallet.WalletAttributeWallets").Include(x => x.OrderDeliveries).Where(x => x.BankOrderId == orderId).Single();
                errLine = " نتیجه از بانک دریافت شد";
                var wallet = order.OrderWallets.First().Wallet;

                //Add info
                WalletAttribute PaymentAuthority = context.WalletAttributes.Where(x => x.DataType == 15).FirstOrDefault();
                WalletAttribute PaymentAuthorityStatus = context.WalletAttributes.Where(x => x.DataType == 14).FirstOrDefault();
                errLine = " دادن اتریبیوت ها";
                if (!wallet.WalletAttributeWallets.Any())
                {
                    wallet.WalletAttributeWallets = new List<WalletAttributeWallet>();
                    foreach (var item in order.OrderDeliveries)
                    {
                        wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthority.Id, Value = token });
                        wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthorityStatus.Id, Value = token });

                    }
                    context.SaveChanges();
                }
                else
                {
                    foreach (var item in order.OrderDeliveries)
                    {
                        if (wallet.WalletAttributeWallets.Any(x => x.WalletAttributeId == PaymentAuthority.Id))
                            wallet.WalletAttributeWallets.Where(x => x.WalletAttributeId == PaymentAuthority.Id).First().Value = token;
                        else
                            wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthority.Id, Value = token });

                        if (wallet.WalletAttributeWallets.Any(x => x.WalletAttributeId == PaymentAuthorityStatus.Id))
                            wallet.WalletAttributeWallets.Where(x => x.WalletAttributeId == PaymentAuthorityStatus.Id).First().Value = token;
                        else
                            wallet.WalletAttributeWallets.Add(new WalletAttributeWallet() { OrderDeliveryId = item.Id, WalletAttributeId = PaymentAuthorityStatus.Id, Value = token });

                    }
                    context.SaveChanges();
                }
                errLine = "2 دادن اتریبیوت ها";

                return token;
            }
            catch (Exception ex)
            {
                return errLine + " - " + ex + token;
            }
            // بانک مورد نظر

        }

        public BankReturnViewModel MellatPayReurn(long orderId)
        {
            throw new NotImplementedException();
        }
    }
}
