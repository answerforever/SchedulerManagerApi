using System;
using System.Collections.Generic;
using System.Text;
using Quartz;
using YQNScheduler.DomainService.Interface;
using YQNScheduler.DomainService.QuartzModel;
using System.Threading.Tasks;
using Quartz.Impl.Matchers;
using System.Linq;
using YQNScheduler.DomainService.QuartzModel.TriggerTypes;

namespace YQNScheduler.DomainService.Implement
{
    public class DefaultSchedulerDataProvider : ISchedulerDataProvider
    {
        private readonly ISchedulerProvider _schedulerProvider;

        private readonly static TriggerTypeExtractor TriggerTypeExtractor = new TriggerTypeExtractor();

        public DefaultSchedulerDataProvider(ISchedulerProvider schedulerProvider)
        {
            _schedulerProvider = schedulerProvider;
        }

        public SchedulerData GetSchedulerData()
        {
            var scheduler = _schedulerProvider.Scheduler;
            var metadata = scheduler.GetMetaData();
            var jobGroups = GetJobGroups(scheduler);
            var triggerGroups = GetTriggerGroups(scheduler);
            var scheduleStatus = GetSchedulerStatus(scheduler);
            var jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            return new SchedulerData
            {
                Name = scheduler.SchedulerName,
                InstanceId = scheduler.SchedulerInstanceId,
                JobGroups = jobGroups,
                TriggerGroups = triggerGroups,
                Status = scheduleStatus,
                IsRemote = metadata.SchedulerRemote,
                JobsExecuted = metadata.NumberOfJobsExecuted,
                JobsTotal = jobKeys.Count,
                RunningSince = metadata.RunningSince.ToDateTime(),
                SchedulerType = metadata.SchedulerType,
            };
        }

        public SchedulerStatus GetSchedulerStatus(IScheduler scheduler)
        {
            if (scheduler.IsShutdown)
            {
                return SchedulerStatus.Shutdown;
            }

            var jobGroupNames = scheduler.GetJobGroupNames();
            if (jobGroupNames == null || jobGroupNames.Count == 0)
            {
                return SchedulerStatus.Empty;
            }

            if (scheduler.IsStarted)
            {
                return SchedulerStatus.Started;
            }

            return SchedulerStatus.Ready;
        }

        private static IList<TriggerGroupData> GetTriggerGroups(IScheduler scheduler)
        {
            var result = new List<TriggerGroupData>();
            if (!scheduler.IsShutdown)
            {
                var triggerGroupNames = scheduler.GetTriggerGroupNames();
                foreach (var groupName in triggerGroupNames)
                {
                    var data = new TriggerGroupData(groupName);
                    data.Init();
                    result.Add(data);
                }
            }

            return result;
        }

        public JobDetailsData GetJobDetailsData(string name, string group)
        {
            var scheduler = _schedulerProvider.Scheduler;
            if (scheduler.IsShutdown)
            {
                return null;
            }

            IJobDetail job;
            JobDetailsData detailsData = new JobDetailsData
            {
                PrimaryData = GetJobData(scheduler, name, group)
            };

            try
            {
                job = scheduler.GetJobDetail(new JobKey(name, @group));
            }
            catch (Exception)
            {
                // GetJobDetail method throws exceptions for remote 
                // scheduler in case when JobType requires an external 
                // assembly to be referenced.
                // see https://github.com/guryanovev/CrystalQuartz/issues/16 for details

                detailsData.JobDataMap.Add("Data", "Not available for remote scheduler");
                detailsData.JobProperties.Add("Data", "Not available for remote scheduler");

                return detailsData;
            }

            if (job == null)
            {
                return null;
            }

            foreach (var key in job.JobDataMap.Keys)
            {
                var jobData = job.JobDataMap[key];
                detailsData.JobDataMap.Add(key, jobData);
            }

            detailsData.JobProperties.Add("Description", job.Description);
            detailsData.JobProperties.Add("Full name", job.Key.Name);
            detailsData.JobProperties.Add("Job type", GetJobType(job));
            detailsData.JobProperties.Add("Durable", job.Durable);
            detailsData.JobProperties.Add("ConcurrentExecutionDisallowed", job.ConcurrentExecutionDisallowed);
            detailsData.JobProperties.Add("PersistJobDataAfterExecution", job.PersistJobDataAfterExecution);
            detailsData.JobProperties.Add("RequestsRecovery", job.RequestsRecovery);

            return detailsData;
        }

        private static string GetJobType(IJobDetail job)
        {
            return job.JobType.Name;
        }

        public TriggerData GetTriggerData(TriggerKey key)
        {
            var scheduler = _schedulerProvider.Scheduler;
            if (scheduler.IsShutdown)
            {
                return null;
            }

            ITrigger trigger = scheduler.GetTrigger(key);
            if (trigger == null)
            {
                return null;
            }

            return GetTriggerData(scheduler, trigger);
        }

        private static IList<JobGroupData> GetJobGroups(IScheduler scheduler)
        {
            var result = new List<JobGroupData>();

            if (!scheduler.IsShutdown)
            {
                var jobGroupNames = scheduler.GetJobGroupNames();
                foreach (var groupName in jobGroupNames)
                {
                    var jobs = GetJobs(scheduler, groupName);
                    var groupData = new JobGroupData(
                        groupName, jobs
                        );
                    groupData.Init();
                    result.Add(groupData);
                }
            }

            return result;
        }

        private static IList<JobData> GetJobs(IScheduler scheduler, string groupName)
        {
            var result = new List<JobData>();

            var jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName));

            foreach (var jobKey in jobKeys)
            {
                var jobData = GetJobData(scheduler, jobKey.Name, groupName);
                result.Add(jobData);
            }

            return result;
        }

        private static JobData GetJobData(IScheduler scheduler, string jobName, string group)
        {
            var triggers = GetTriggers(scheduler, jobName, group);
            var jobData = new JobData(jobName, group, triggers);
            jobData.Init();
            return jobData;
        }

        private static IList<TriggerData> GetTriggers(IScheduler scheduler, string jobName, string group)
        {
            var triggersJobs = scheduler
                .GetTriggersOfJob(new JobKey(jobName, @group));
            //Task.Factory.StartNew(async () =>
            //{
            //    await Task.Delay(5000);
            //});
            if (triggersJobs == null|| triggersJobs.Count==0)
            {
                return null;
            }
            var triggerDataList = new List<TriggerData>();
            foreach (var trigger in triggersJobs)
            {
                var taskTriggerData = GetTriggerData(scheduler, trigger);
                triggerDataList.Add(taskTriggerData);
            }
            return triggerDataList;
            //return triggerJob.ToList()
            //    .Select(async(trigger) =>await GetTriggerData(scheduler, trigger))
            //    .ToList();
        }

        private static TriggerData GetTriggerData(IScheduler scheduler, ITrigger trigger)
        {
            var triggerStatus = GetTriggerStatus(trigger, scheduler);
            return new TriggerData(trigger.Key.Name, triggerStatus)
            {
                GroupName = trigger.Key.Group,
                StartDate = trigger.StartTimeUtc.DateTime,
                EndDate = trigger.EndTimeUtc.ToDateTime(),
                NextFireDate = trigger.GetNextFireTimeUtc().ToDateTime(),
                PreviousFireDate = trigger.GetPreviousFireTimeUtc().ToDateTime(),
                TriggerType = TriggerTypeExtractor.GetFor(trigger)
            };
        }

        private static ActivityStatus GetTriggerStatus(ITrigger trigger, IScheduler scheduler)
        {
            var triggerStatus = GetTriggerStatus(trigger.Key.Name, trigger.Key.Group, scheduler);
            return triggerStatus;
            //return GetTriggerStatus(trigger.Name, trigger.Group, scheduler);
        }

        private static ActivityStatus GetTriggerStatus(string triggerName, string triggerGroup, IScheduler scheduler)
        {
            var state = scheduler.GetTriggerState(new TriggerKey(triggerName, triggerGroup));
            switch (state)
            {
                case TriggerState.Paused:
                    return ActivityStatus.Paused;
                case TriggerState.Complete:
                    return ActivityStatus.Complete;
                default:
                    return ActivityStatus.Active;
            }
        }



    }
}
