using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CoreLib.Infrastructure.SMS;
using Microsoft.AspNet.Identity;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace ahmadi.Models.Jobs
{
    public class SendSmsJob : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            SmsService sms = new SmsService();

            using (UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass())
            {
                try
                {
                    var date = DateTime.Now;
                    if (uow.UserOfferMessageMemberRepository.Any(x => x.Id, x => x.UserOfferMessage.state == false && x.state == Domain.OfferMessageSendMessageType.Waiting && x.UserOfferMessage.Offer.IsActive && x.UserOfferMessage.Offer.state && ((x.UserOfferMessage.Offer.ExpireDate != null && x.UserOfferMessage.Offer.ExpireDate >= date) || x.UserOfferMessage.Offer.ExpireDate == null) && ((x.UserOfferMessage.Offer.StartDate != null && x.UserOfferMessage.Offer.StartDate <= date) || x.UserOfferMessage.Offer.StartDate == null)))
                    {
                        var UserOfferMessage = uow.UserOfferMessageMemberRepository.GetQueryList().AsNoTracking().Where(x => x.UserOfferMessage.state == false && x.state == Domain.OfferMessageSendMessageType.Waiting && x.UserOfferMessage.Offer.IsActive && x.UserOfferMessage.Offer.state && ((x.UserOfferMessage.Offer.ExpireDate != null && x.UserOfferMessage.Offer.ExpireDate >= date) || x.UserOfferMessage.Offer.ExpireDate == null) && ((x.UserOfferMessage.Offer.StartDate != null && x.UserOfferMessage.Offer.StartDate <= date) || x.UserOfferMessage.Offer.StartDate == null)).OrderBy(x => x.UserOfferMessage.Offer.InsertDate).ThenBy(x => x.UserOfferMessage.InsertDate).Select(x => x.UserOfferMessage).First();
                        if (UserOfferMessage.OfferMessageType == Domain.OfferMessageType.Private)
                        {
                            foreach (var item in uow.UserOfferMessageMemberRepository.GetQueryList().Include("UserOfferMessage.Offer.offerProductCategories.productCategory").AsNoTracking().Include("User").Where(x => x.UserOfferMessage.state == false && x.state == Domain.OfferMessageSendMessageType.Waiting && x.UserOfferMessage.Offer.IsActive && x.UserOfferMessage.Offer.state && ((x.UserOfferMessage.Offer.ExpireDate != null && x.UserOfferMessage.Offer.ExpireDate >= date) || x.UserOfferMessage.Offer.ExpireDate == null) && ((x.UserOfferMessage.Offer.StartDate != null && x.UserOfferMessage.Offer.StartDate <= date) || x.UserOfferMessage.Offer.StartDate == null)).OrderBy(x => x.UserOfferMessage.Offer.InsertDate).ThenBy(x => x.UserOfferMessage.InsertDate).ThenBy(x => x.InsertDate).Skip(() => 0).Take(15))
                            {
                                if (UserOfferMessage.Text.Contains("%Name"))
                                    UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Name", uow.UserRepository.GetByID(item.UserId).FirstName);
                                if (UserOfferMessage.Text.Contains("%FullName"))
                                    UserOfferMessage.Text = UserOfferMessage.Text.Replace("%FullName", uow.UserRepository.GetByID(item.UserId).LastName);
                                if (UserOfferMessage.Text.Contains("%Mobile"))
                                    UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Mobile", uow.UserRepository.GetByID(item.UserId).PhoneNumber);
                                if (UserOfferMessage.Text.Contains("%Title"))
                                    UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Title", item.UserOfferMessage.Offer.Title);
                                if (UserOfferMessage.Text.Contains("%StartDate"))
                                {
                                    if (item.UserOfferMessage.Offer.StartDate.HasValue)
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%StartDate", CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(item.UserOfferMessage.Offer.StartDate.Value));
                                    else
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%StartDate", "");
                                }
                                if (UserOfferMessage.Text.Contains("%EndDate"))
                                {
                                    if (item.UserOfferMessage.Offer.ExpireDate.HasValue)
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%EndDate", CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(item.UserOfferMessage.Offer.ExpireDate.Value));
                                    else
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%EndDate", "");
                                }
                                if (UserOfferMessage.Text.Contains("%Groups"))
                                {
                                    if (item.UserOfferMessage.Offer.offerProductCategories.Any())
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Groups", String.Join(",", item.UserOfferMessage.Offer.offerProductCategories.Select(x => x.productCategory.Name).ToArray()));
                                    else
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Groups", "همه گروهها");
                                }
                                if (UserOfferMessage.Text.Contains("%Value"))
                                {
                                    var CodeTypeValueCode = item.UserOfferMessage.Offer.CodeTypeValueCode;
                                    if (CodeTypeValueCode == 1 || CodeTypeValueCode == 2)
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Value", "");
                                    else if (CodeTypeValueCode == 4 )
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Value", uow.UserCodeGiftRepository.Get(x => x.Value, x => x.UserId == item.UserId && x.OfferId == item.UserOfferMessage.OfferId).First().ToString());
                                    else if (CodeTypeValueCode == 5)
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Value", uow.GeneralCodeGiftRepository.Get(x => x.Value, x => x.OfferId == item.UserOfferMessage.OfferId).First().ToString());
                                }
                                if (UserOfferMessage.Text.Contains("%CodeGift"))
                                {
                                    var CodeTypeValueCode = item.UserOfferMessage.Offer.CodeTypeValueCode;
                                    if (CodeTypeValueCode == 1 || CodeTypeValueCode == 2)
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CodeGift", "");
                                    else if (CodeTypeValueCode == 4 )
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CodeGift", uow.UserCodeGiftRepository.Get(x => x.Code, x => x.UserId == item.UserId && x.OfferId == item.UserOfferMessage.OfferId).First().ToString());
                                    else if (CodeTypeValueCode == 5)
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CodeGift", uow.GeneralCodeGiftRepository.Get(x => x.Code, x => x.OfferId == item.UserOfferMessage.OfferId).First().ToString());

                                }
                                if (UserOfferMessage.Text.Contains("%CountUse"))
                                {
                                    var CodeTypeValueCode = item.UserOfferMessage.Offer.CodeTypeValueCode;
                                    if (CodeTypeValueCode == 1 || CodeTypeValueCode == 2)
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CountUse", "");
                                    else if (CodeTypeValueCode == 4)
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CountUse", uow.UserCodeGiftRepository.Get(x => x.CountUse, x => x.UserId == item.UserId && x.OfferId == item.UserOfferMessage.OfferId).First().ToString());
                                    else if (CodeTypeValueCode == 5)
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CountUse", uow.GeneralCodeGiftRepository.Get(x => x.CountUse, x => x.OfferId == item.UserOfferMessage.OfferId).First().ToString());

                                }
                                if (UserOfferMessage.Text.Contains("%MaxUse"))
                                {
                                    var CodeTypeValueCode = item.UserOfferMessage.Offer.CodeTypeValueCode;
                                    if (CodeTypeValueCode == 1 || CodeTypeValueCode == 2)
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%MaxUse", "");
                                    else if (CodeTypeValueCode == 4 )
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%MaxUse", uow.UserCodeGiftRepository.Get(x => x.MaxValue, x => x.UserId == item.UserId && x.OfferId == item.UserOfferMessage.OfferId).First().ToString());
                                    else if (CodeTypeValueCode == 5)
                                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%MaxUse", uow.GeneralCodeGiftRepository.Get(x => x.MaxValue, x => x.OfferId == item.UserOfferMessage.OfferId).First().ToString());

                                }

                                await sms.SendSMSAsync(new IdentityMessage() { Body = UserOfferMessage.Text, Destination = item.User.PhoneNumber }, null, null, null, null, null, null, true);
                                uow.UserOfferMessageMemberRepository.SqlQuery("EXEC [SetUserOfferMessageState] @id=@Id,@userMessageId=@userMessageId", new System.Data.SqlClient.SqlParameter("Id", item.Id), new System.Data.SqlClient.SqlParameter("userMessageId", item.UserOfferMessageId));
                            }
                        }
                        else
                        {
                            var userOfferMessageMembers = uow.UserOfferMessageMemberRepository.GetQueryList().AsNoTracking().Include("User").Where(x => x.UserOfferMessage.state == false && x.state == Domain.OfferMessageSendMessageType.Waiting && x.UserOfferMessage.Offer.IsActive && x.UserOfferMessage.Offer.state && ((x.UserOfferMessage.Offer.ExpireDate != null && x.UserOfferMessage.Offer.ExpireDate >= date) || x.UserOfferMessage.Offer.ExpireDate == null) && ((x.UserOfferMessage.Offer.StartDate != null && x.UserOfferMessage.Offer.StartDate <= date) || x.UserOfferMessage.Offer.StartDate == null)).OrderBy(x => x.UserOfferMessage.Offer.InsertDate).ThenBy(x => x.UserOfferMessage.InsertDate).OrderBy(x => x.UserOfferMessage.Offer.InsertDate).ThenBy(x => x.UserOfferMessage.InsertDate).ThenBy(x => x.InsertDate).Skip(() => 0).Take(120).Select(x => new { x.UserOfferMessageId, x.User.PhoneNumber, x.Id });
                            await sms.SendMultiDestinationAsync(UserOfferMessage.Text, userOfferMessageMembers.Select(x => x.PhoneNumber).ToList(), null, null, null, null, true);
                            uow.UserOfferMessageMemberRepository.SqlQuery("EXEC [SetUserOfferMessageState] @id=@Id,@userMessageId=@userMessageId", new System.Data.SqlClient.SqlParameter("Id", String.Join(",", userOfferMessageMembers.Select(x => x.Id).ToArray())), new System.Data.SqlClient.SqlParameter("userMessageId", userOfferMessageMembers.First().UserOfferMessageId));
                        }
                    }

                }
                catch (Exception ex)
                {
                    ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendSmsJob", "Index", true, 500, ex.Message, DateTime.Now,uow.UserRepository.Get(x=>x.Id).First());

                }
            }

        }

    }

    public class PauesSendSmsJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var date = DateTime.Now;
            UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass();
            if (!uow.UserOfferMessageMemberRepository.Any(x => x.Id, x => x.UserOfferMessage.state == false && x.state == Domain.OfferMessageSendMessageType.Waiting && x.UserOfferMessage.Offer.IsActive && x.UserOfferMessage.Offer.state && ((x.UserOfferMessage.Offer.ExpireDate != null && x.UserOfferMessage.Offer.ExpireDate >= date) || x.UserOfferMessage.Offer.ExpireDate == null) && ((x.UserOfferMessage.Offer.StartDate != null && x.UserOfferMessage.Offer.StartDate <= date) || x.UserOfferMessage.Offer.StartDate == null)))
            {
                StdSchedulerFactory factory = new StdSchedulerFactory();
                IScheduler scheduler = await factory.GetScheduler();
                await scheduler.PauseJob(new JobKey("job1"));
                await scheduler.PauseJob(new JobKey("job2"));
                await scheduler.ResumeJob(new JobKey("job3"));
            }



        }

    }

    public class resumeSendSmsJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var date = DateTime.Now;
            UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass();
            if (uow.UserOfferMessageMemberRepository.Any(x => x.Id, x => x.UserOfferMessage.state == false && x.state == Domain.OfferMessageSendMessageType.Waiting && x.UserOfferMessage.Offer.IsActive && x.UserOfferMessage.Offer.state && ((x.UserOfferMessage.Offer.ExpireDate != null && x.UserOfferMessage.Offer.ExpireDate >= date) || x.UserOfferMessage.Offer.ExpireDate == null) && ((x.UserOfferMessage.Offer.StartDate != null && x.UserOfferMessage.Offer.StartDate <= date) || x.UserOfferMessage.Offer.StartDate == null)))
            {
                StdSchedulerFactory factory = new StdSchedulerFactory();
                IScheduler scheduler = await factory.GetScheduler();
                await scheduler.ResumeJob(new JobKey("job1"));
                await scheduler.ResumeJob(new JobKey("job2"));
                await scheduler.PauseJob(new JobKey("job3"));
            }

        }

    }
}