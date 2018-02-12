using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YQNScheduler.ApiService.Jobs
{
    public class HttpJob : IJob
    {
        public const string CONTENTTYPE = "application/json";

        public void Execute(IJobExecutionContext context)
        {
            //待写入事件
            object parametersObj = null;
            var dataMap = context.JobDetail.JobDataMap;
            var requestUrl = context.JobDetail.JobDataMap.GetString("RequestUrl");
            var requestParameters = context.JobDetail.JobDataMap.GetString("Parameters");


        }
    }
}