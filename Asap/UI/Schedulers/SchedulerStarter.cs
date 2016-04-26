﻿using System;
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

        const int INTERVAL = 30;

        public static void StartTaskUpdateScheduler()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();

            var job = JobBuilder.Create<TasksUpdateJob>().Build();

            var trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(s => s.WithIntervalInSeconds(INTERVAL))
                .Build();

            scheduler.ScheduleJob(job, trigger);
                
        }
    }
}
