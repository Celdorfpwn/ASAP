using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using SushiPikant.UI.ViewModels;

namespace SushiPikant.UI.Schedulers
{
    public static class SchedulerStarter
    {

        const int TASKUPDATE_INTERVAL = 30;
        const int CODEREVIEW_INTERVAL = 120;

        public static void StartTaskUpdateScheduler()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();

            var taskUpdate = JobBuilder.Create<TasksUpdateJob>().Build();

            var codeReview = JobBuilder.Create<CodeReviewJob>().Build();

            var taskUpdateTrigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(s => s.WithIntervalInSeconds(TASKUPDATE_INTERVAL))
                .Build();

            var codeReviewTrigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(s => s.WithIntervalInSeconds(CODEREVIEW_INTERVAL))
                .Build();



            scheduler.ScheduleJob(taskUpdate, taskUpdateTrigger);
            scheduler.ScheduleJob(codeReview,codeReviewTrigger);
                
        }
    }
}
