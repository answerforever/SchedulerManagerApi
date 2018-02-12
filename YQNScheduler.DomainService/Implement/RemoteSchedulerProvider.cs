using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;

namespace YQNScheduler.DomainService.Implement
{
    public class RemoteSchedulerProvider : StdSchedulerProvider
    {
        private readonly string SchedulerHost = ConfigurationManager.AppSettings["SchedulerUrl"];

        protected override bool IsLazy
        {
            get { return true; }
        }

        protected override NameValueCollection GetSchedulerProperties()
        {
            var properties = base.GetSchedulerProperties();
            properties["quartz.scheduler.proxy"] = "true";
            properties["quartz.scheduler.proxy.address"] = SchedulerHost;
            return properties;
        }
    }
}
