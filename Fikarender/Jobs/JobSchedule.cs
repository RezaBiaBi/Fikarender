using System;

namespace Jobs
{
    public class JobSchedule
    {
        public Type JobType { get; set; }
        public string CronExpression { get; set; }
        public JobSchedule(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }
    }
}
