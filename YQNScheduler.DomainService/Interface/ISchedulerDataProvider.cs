using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YQNScheduler.DomainService.QuartzModel;

namespace YQNScheduler.DomainService.Interface
{
    public interface ISchedulerDataProvider
    {
        SchedulerData GetSchedulerData();

        JobDetailsData GetJobDetailsData(string name, string group);

        TriggerData GetTriggerData(TriggerKey key);
    }
}
