using System;
using System.Collections.Generic;
using System.Text;
using Quartz;

namespace YQNScheduler.DomainService.Interface
{
    public interface ISchedulerProvider
    {
        /// <summary>
        /// Initializes provider and creates all necessary instances 
        /// (scheduler factory and scheduler itself).
        /// </summary>
        void Init();

        /// <summary>
        /// Gets scheduler instance. Should return same instance on every call.
        /// </summary>
        IScheduler Scheduler { get; }
    }
}
