using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using YQNScheduler.ApiService.Models;
using YQNScheduler.DomainService.Implement;
using YQNScheduler.DomainService.Interface;
using YQNScheduler.DomainService.QuartzModel;

namespace YQNScheduler.ApiService.Controllers
{
    [RoutePrefix("api/scheduler")]
    public class SchedulerController : ApiController
    {
        private ISchedulerProvider _schedulerProvider;

        private ISchedulerDataProvider _schedulerDataProvider;

        private const string DEFAULTGROUP = "Default";

        public SchedulerController(ISchedulerProvider schedulerProvider)
        {
            _schedulerProvider = schedulerProvider;
            _schedulerDataProvider = new DefaultSchedulerDataProvider(_schedulerProvider);
        }

        #region 任务scheduler本身操作

        /// <summary>
        /// 启动调度
        /// </summary>
        /// <returns></returns>
        [Route("startscheduler")]
        [AcceptVerbs("POST")]
        public BaseResult StartScheduler()
        {
            var result = new BaseResult();
            try
            {
                _schedulerProvider.Scheduler.Start();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 停止调度
        /// </summary>
        /// <returns></returns>
        [Route("stopscheduler")]
        [AcceptVerbs("POST")]
        public BaseResult StopScheduler()
        {
            var result = new BaseResult();
            try
            {
                _schedulerProvider.Scheduler.Shutdown(false);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        #endregion


        #region 任务组group

        /// <summary>
        /// 暂停所有job
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("pausegroup")]
        [AcceptVerbs("POST")]
        public BaseResult PauseGroup([FromBody]GroupInput input)
        {
            var result = new BaseResult();
            try
            {
                _schedulerProvider.Scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals(input.Group));
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 重启所有job
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("resumegroup")]
        [AcceptVerbs("POST")]
        public BaseResult ResumeGroup([FromBody]GroupInput input)
        {
            var result = new BaseResult();
            try
            {
                _schedulerProvider.Scheduler.ResumeJobs(GroupMatcher<JobKey>.GroupEquals(input.Group));
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除所有job
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("deletegroup")]
        [AcceptVerbs("POST")]
        public BaseResult DeleteGroup([FromBody]GroupInput input)
        {
            var result = new BaseResult();
            try
            {
                var keys = _schedulerProvider.Scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(input.Group));
                _schedulerProvider.Scheduler.DeleteJobs(keys.ToList());
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        #endregion

        #region 任务job

        /// <summary>
        /// 获取调度任务数据
        /// </summary>
        /// <returns></returns>
        [Route("getschedulerdata")]
        [AcceptVerbs("GET")]
        public SchedulerData GetSchedulerdata()
        {
            SchedulerData schedulerData = null;
            try
            {
                ////获取scheduler数据
                schedulerData = _schedulerDataProvider.GetSchedulerData();
            }
            catch (Exception ex)
            {
                throw new Exception("获取scheduler数据出错", ex);
            }
            return schedulerData;
        }

        /// <summary>
        /// 新增job
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("addjob")]
        [AcceptVerbs("POST")]
        public BaseResult AddJob([FromBody]JobInput input)
        {
            var result = new BaseResult();
            try
            {
                //先使用Assembly类载入DLL，再根据类的全路径获取类
                Type typeofControl = null;
                Assembly tempAssembly;
                tempAssembly = Assembly.Load(input.NameSpace);
                typeofControl = tempAssembly.GetType(input.ClassName);
                IJobDetail job = JobBuilder.Create(typeofControl)
                  .WithIdentity(input.Job, input.Group)
                  .WithDescription(input.Description)
                  .UsingJobData("RequestUrl", input.RequestUrl)
                  .UsingJobData("Parameters", input.Parameters)
                  .StoreDurably(true)
                  .Build();
                _schedulerProvider.Scheduler.AddJob(job, true);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除job
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("deletejob")]
        [AcceptVerbs("POST")]
        public BaseResult DeleteJob([FromBody]JobInput input)
        {
            var result = new BaseResult();
            try
            {
                result.Success = _schedulerProvider.Scheduler.DeleteJob(new JobKey(input.Job, input.Group));
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 暂停所有触发器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("pausejob")]
        [AcceptVerbs("POST")]
        public BaseResult PauseJob([FromBody]JobInput input)
        {
            var result = new BaseResult();
            try
            {
                _schedulerProvider.Scheduler.PauseJob(new JobKey(input.Job, input.Group));
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 重启所有触发器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("resumejob")]
        [AcceptVerbs("POST")]
        public BaseResult ResumeJob([FromBody]JobInput input)
        {
            var result = new BaseResult();
            try
            {
                _schedulerProvider.Scheduler.ResumeJob(new JobKey(input.Job, input.Group));
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 立即执行
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("executenow")]
        [AcceptVerbs("POST")]
        public BaseResult ExecuteNow([FromBody]JobInput input)
        {
            var result = new BaseResult();
            try
            {
                _schedulerProvider.Scheduler.TriggerJob(new JobKey(input.Job, input.Group));
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取job明细数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("getjobdetails")]
        [AcceptVerbs("POST")]
        public JobDetailsOutput GetJobDetails([FromBody]JobInput input)
        {
            var result = new JobDetailsOutput();
            try
            {
                var detailsData = _schedulerDataProvider.GetJobDetailsData(input.Job, input.Group);

                result.JobDataMap = detailsData
                    .JobDataMap
                    .Select(pair => new Property(pair.Key.ToString(), pair.Value))
                    .ToArray();

                result.JobProperties = detailsData
                    .JobProperties
                    .Select(pair => new Property(pair.Key, pair.Value))
                    .ToArray();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        #endregion               

        #region 触发器

        /// <summary>
        /// 添加触发器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("addtrigger")]
        [AcceptVerbs("POST")]
        public BaseResult AddTrigger(AddTriggerInput input)
        {
            var result = new BaseResult();
            try
            {
                TriggerBuilder triggerBuilder = TriggerBuilder
                .Create()
                .ForJob(input.Job, input.Group);

                if (!string.IsNullOrEmpty(input.Name))
                {
                    triggerBuilder = triggerBuilder.WithIdentity(input.Name);
                }

                switch (input.TriggerType)
                {
                    case "Simple":
                        triggerBuilder = triggerBuilder.WithSimpleSchedule(x =>
                        {
                            if (input.RepeatForever)
                            {
                                x.RepeatForever();
                            }
                            else
                            {
                                x.WithRepeatCount(input.RepeatCount);
                            }

                            x.WithInterval(TimeSpan.FromMilliseconds(input.RepeatInterval));
                        });
                        if (!string.IsNullOrEmpty(input.StartDate))
                        {
                            var startDate = DateTimeOffset.Parse(input.StartDate);
                            triggerBuilder.StartAt(startDate);
                        }
                        break;
                    case "Cron":
                        triggerBuilder = triggerBuilder.WithCronSchedule(input.CronExpression);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _schedulerProvider.Scheduler.ScheduleJob(triggerBuilder.Build());
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 编辑触发器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("edittrigger")]
        [AcceptVerbs("POST")]
        public BaseResult EditTrigger(AddTriggerInput input)
        {
            var result = new BaseResult();
            try
            {
                if (string.IsNullOrEmpty(input.Name))
                {
                    result.ErrorMessage = "缺少触发器唯一标识";
                    return result;
                }
                
                //先删除触发器
                var deleteTriggerKey = new TriggerKey(input.Name, DEFAULTGROUP);
                _schedulerProvider.Scheduler.UnscheduleJob(deleteTriggerKey);

                //新增触发器
                TriggerBuilder triggerBuilder = TriggerBuilder
                .Create()
                .ForJob(input.Job, input.Group);

                if (!string.IsNullOrEmpty(input.Name))
                {
                    triggerBuilder = triggerBuilder.WithIdentity(input.Name);
                }

                switch (input.TriggerType)
                {
                    case "Simple":
                        triggerBuilder = triggerBuilder.WithSimpleSchedule(x =>
                        {
                            if (input.RepeatForever)
                            {
                                x.RepeatForever();
                            }
                            else
                            {
                                x.WithRepeatCount(input.RepeatCount);
                            }

                            x.WithInterval(TimeSpan.FromMilliseconds(input.RepeatInterval));
                        });
                        if (!string.IsNullOrEmpty(input.StartDate))
                        {
                            var startDate = DateTimeOffset.Parse(input.StartDate);
                            triggerBuilder.StartAt(startDate);
                        }
                        break;
                    case "Cron":
                        triggerBuilder = triggerBuilder.WithCronSchedule(input.CronExpression);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _schedulerProvider.Scheduler.ScheduleJob(triggerBuilder.Build());
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除触发器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("deletetrigger")]
        [AcceptVerbs("POST")]
        public BaseResult DeleteTrigger(TriggerInput input)
        {
            var result = new BaseResult();
            try
            {
                var triggerKey = new TriggerKey(input.Trigger, DEFAULTGROUP);
                _schedulerProvider.Scheduler.UnscheduleJob(triggerKey);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 暂停触发器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("pausetrigger")]
        [AcceptVerbs("POST")]
        public BaseResult PauseTrigger(TriggerInput input)
        {
            var result = new BaseResult();
            try
            {
                var triggerKey = new TriggerKey(input.Trigger, DEFAULTGROUP);
                _schedulerProvider.Scheduler.PauseTrigger(triggerKey);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 重启触发器
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("resumetrigger")]
        [AcceptVerbs("POST")]
        public BaseResult ResumeTrigger(TriggerInput input)
        {
            var result = new BaseResult();
            try
            {
                var triggerKey = new TriggerKey(input.Trigger, DEFAULTGROUP);
                _schedulerProvider.Scheduler.ResumeTrigger(triggerKey);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        #endregion


    }
}