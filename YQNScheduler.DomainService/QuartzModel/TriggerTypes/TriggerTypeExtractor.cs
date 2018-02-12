using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace YQNScheduler.DomainService.QuartzModel.TriggerTypes
{
    public class TriggerTypeExtractor
    {
        public TriggerType GetFor(ITrigger trigger)
        {
            var simpleTrigger = trigger as ISimpleTrigger;
            if (simpleTrigger != null)
            {
                return new SimpleTriggerType(
                    simpleTrigger.RepeatCount,
                    (long)simpleTrigger.RepeatInterval.TotalMilliseconds,
                    simpleTrigger.TimesTriggered);
            }

            var cronTrigger = trigger as ICronTrigger;
            if (cronTrigger != null)
            {
                return new CronTriggerType(cronTrigger.CronExpressionString);
            }

            return new TriggerType("unknown");
        }
    }
}
