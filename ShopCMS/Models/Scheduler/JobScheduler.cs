using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using ahmadi.Models.Jobs;
using UnitOfWork;

namespace ahmadi.Models.Scheduler
{
    public class JobScheduler
    {
        public static async void Start()
        {
            UnitOfWork.UnitOfWorkClass uow = new UnitOfWorkClass();

            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            // Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            // and start it off
            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<SendSmsJob>()
                .WithIdentity("job1")
                .Build();
            IJobDetail job2 = JobBuilder.Create<PauesSendSmsJob>()
              .WithIdentity("job2")
              .Build();
            IJobDetail job3 = JobBuilder.Create<resumeSendSmsJob>()
              .WithIdentity("job3")
              .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(15)
                    .RepeatForever())
                .ForJob("job1")
                .Build();

            ITrigger trigger2 = TriggerBuilder.Create()
               .WithIdentity("trigger2")
               .StartNow()
               .WithSimpleSchedule(x => x
                   .WithIntervalInSeconds(15)
                   .RepeatForever())
               .ForJob("job2")
               .Build();

            ITrigger trigger3 = TriggerBuilder.Create()
               .WithIdentity("trigger3")
               .StartNow()
               .WithSimpleSchedule(x => x
                   .WithIntervalInSeconds(15)
                   .RepeatForever())
               .ForJob("job3")
               .Build();

            // Tell quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job,trigger);
            await scheduler.ScheduleJob(job2, trigger2);
            await scheduler.ScheduleJob(job3, trigger3);


        }

        // simple log provider to get something to the console
        private class ConsoleLogProvider : ILogProvider
        {
            public Logger GetLogger(string name)
            {
                return (level, func, exception, parameters) =>
                {
                    if (level >= LogLevel.Info && func != null)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                    }
                    return true;
                };
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenMappedContext(string key, string value)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
            {
                throw new NotImplementedException();
            }
        }
    }


}
