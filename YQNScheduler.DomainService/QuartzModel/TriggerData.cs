namespace YQNScheduler.DomainService.QuartzModel
{
    using System;
    using YQNScheduler.DomainService.QuartzModel.TriggerTypes;

    public class TriggerData : Activity
    {
        public TriggerData(string name, ActivityStatus status) : base(name, status)
        {
        }

        public string GroupName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? NextFireDate { get; set; }

        public DateTime? PreviousFireDate { get; set; }

        public TriggerType TriggerType { get; set; }
    }
}