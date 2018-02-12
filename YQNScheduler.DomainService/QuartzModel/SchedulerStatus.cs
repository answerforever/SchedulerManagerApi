using System;
using System.Collections.Generic;
using System.Text;

namespace YQNScheduler.DomainService.QuartzModel
{
    public enum SchedulerStatus
    {
        Empty,
        Ready,
        Started,
        Shutdown
    }
}
