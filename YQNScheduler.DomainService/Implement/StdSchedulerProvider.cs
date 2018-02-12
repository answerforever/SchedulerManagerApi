using System;
using System.Collections.Generic;
using System.Text;
using Quartz;
using YQNScheduler.DomainService.Interface;
using System.Collections.Specialized;
using Quartz.Impl;
using System.Threading.Tasks;

namespace YQNScheduler.DomainService.Implement
{
    public class StdSchedulerProvider : ISchedulerProvider
    {
        protected IScheduler _scheduler;

        protected virtual bool IsLazy
        {
            get { return false; }
        }

        public void Init()
        {
            if (!IsLazy)
            {
                LazyInitAsync();
            }
        }

        private void LazyInitAsync()
        {
            NameValueCollection properties = null;
            try
            {
                properties = GetSchedulerProperties();
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
                _scheduler=schedulerFactory.GetScheduler();
                InitScheduler(_scheduler);
            }
            catch (Exception ex)
            {
                throw new SchedulerProviderException("Could not initialize scheduler", ex, properties);
            }

            if (_scheduler == null)
            {
                throw new SchedulerProviderException(
                    "Could not initialize scheduler", properties);
            }
        }

        protected virtual void InitScheduler(IScheduler scheduler)
        {
        }

        protected virtual NameValueCollection GetSchedulerProperties()
        {
            return new NameValueCollection();
        }

        public virtual IScheduler Scheduler
        {
            get
            {
                if (_scheduler == null)
                {
                    LazyInitAsync();
                }

                return _scheduler;
            }
        }
    }
}
