using Aggregation_DataModels.Models;
using Aggregation_DataModels.Enums;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Aggregation_Services
{
    public class AggregationRepository : IAggregationRepository
    {
        private readonly AggregationDbContext context;

        public AggregationRepository(AggregationDbContext contex)
        {
            this.context = contex;
        }

        bool IAggregationRepository.ConfigurationExists(int? id)
        {
            return this.context.TbConfigurations.Any(m => m.Id == id);
        }

        async Task<bool> IAggregationRepository.ConfigurationExistsAsync(int? id)
        {
            var configuration = await this.context.TbConfigurations
             .FirstOrDefaultAsync(m => m.Id == id);

            return (configuration == null) ? false : true;
        }

        IEnumerable<TbActivity> IAggregationRepository.GetActivities()
        {
            return this.context.TbActivities.ToList();
        }

        async Task<List<TbActivity>> IAggregationRepository.GetActivitiesAsync()
        {
            return await this.context.TbActivities.ToListAsync();
        }

        List<TbActivityLogger> IAggregationRepository.GetActivityLoggerByConfigId(int? id)
        {
            return this.context.TbActivityLoggers
            .Include(t => t.Activity)
            .Include(t => t.Configuration)
            .Include(t => t.User)
            .Where(t => t.ConfigurationId == id).ToList();
        }

        async Task<List<TbActivityLogger>> IAggregationRepository.GetActivityLoggerByConfigIdAsync(int? id)
        {
            return await this.context.TbActivityLoggers
            .Include(t => t.Activity)
            .Include(t => t.Configuration)
            .Include(t => t.User)
            .Where(t => t.ConfigurationId == id).ToListAsync();
        }

        TbConfiguration? IAggregationRepository.GetConfigurationById(int? id)
        {
            return this.context.TbConfigurations
                .Include(x => x.TbActivityLoggers)
                .FirstOrDefault(m => m.Id == id);
        }

        async Task<TbConfiguration?> IAggregationRepository.GetConfigurationByIdAsync(int? id)
        {
            return await this.context.TbConfigurations
                .Include(x => x.TbActivityLoggers)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        TbConfiguration? IAggregationRepository.GetConfigurationByKey(string key)
        {
            return this.context.TbConfigurations
                .FirstOrDefault(m => m.Key == key);
        }

        async Task<TbConfiguration?> IAggregationRepository.GetConfigurationByKeyAsync(string key)
        {
            return await this.context.TbConfigurations
                .FirstOrDefaultAsync(m => m.Key == key);
        }

        IEnumerable<TbConfiguration> IAggregationRepository.GetConfigurations()
        {
            if (this.context.TbConfigurations != null)
            {
                var configurations = this.context.TbConfigurations
                    .ToList();

                var excludedKeys = new List<string>() { "NodeStatus", "Disabled", "LastHourlyJob_Scheduled_DateTime" };
                return configurations.Where(x => !excludedKeys.Contains(x.Key));
            }

            return Enumerable.Empty<TbConfiguration>();
        }

        async Task<List<TbConfiguration>> IAggregationRepository.GetConfigurationsAsync()
        {
            List<TbConfiguration> list = await Task.Run(() => this.context.TbConfigurations.ToList());
            return list;
        }

        VwGetDashbordDatum? IAggregationRepository.GetDashbordData()
        {
            return this.context.VwGetDashbordData.FirstOrDefault();
        }

        async Task<VwGetDashbordDatum?> IAggregationRepository.GetDashbordDataAsync()
        {
            return await this.context.VwGetDashbordData
                .FirstOrDefaultAsync();
        }

        TbJob? IAggregationRepository.GetJob(int? id)
        {
            return this.context.TbJobs
              .Include(x => x.StatusNavigation)
              .Include(x => x.IntervalNavigation)
              .Include(x => x.JobTypeNavigation)
              .Include(x => x.PriorityNavigation)
              .Include(x => x.TbLogErrors)
              .Include(x => x.TbLogActivities)
              .FirstOrDefault(m => m.Id == id);
        }

        async Task<TbJob?> IAggregationRepository.GetJobAsync(int? id)
        {
            return await this.context.TbJobs
              .Include(x => x.StatusNavigation)
              .Include(x => x.IntervalNavigation)
              .Include(x => x.JobTypeNavigation)
              .Include(x => x.PriorityNavigation)
              .Include(x => x.TbLogErrors)
              .Include(x => x.TbLogActivities)
              .FirstOrDefaultAsync(m => m.Id == id);
        }

        AggregationJobDetails IAggregationRepository.GetJobDetails(int interval, DateTime startDate, DateTime endDate)
        {
            var xJobs = this.context.TbJobs
                .Include(x => x.StatusNavigation)
                .Include(x => x.IntervalNavigation)
                .Include(x => x.JobTypeNavigation)
                .Include(x => x.PriorityNavigation)
                .Include(x => x.TbLogErrors)
                .Include(x => x.TbLogActivities)
                .Where(x => x.Interval == interval && x.AggregationStartDateTime >= startDate && x.AggregationEndDateTime <= endDate)
                .OrderByDescending(x => x.Id)
                .ToList();

            AggregationJobDetails aggregationJob = new AggregationJobDetails();
            aggregationJob.AggregationStartDateTime = startDate;
            aggregationJob.AggregationEndDateTime = endDate;
            aggregationJob.TbJobs = xJobs;

            return aggregationJob;
        }

        async Task<AggregationJobDetails> IAggregationRepository.GetJobDetailsAsync(int interval, DateTime startDate, DateTime endDate)
        {
            var xJobs = await this.context.TbJobs
                .Include(x => x.StatusNavigation)
                .Include(x => x.IntervalNavigation)
                .Include(x => x.JobTypeNavigation)
                .Include(x => x.PriorityNavigation)
                .Include(x => x.TbLogErrors)
                .Include(x => x.TbLogActivities)
                .Where(x => x.Interval == interval && x.AggregationStartDateTime >= startDate && x.AggregationEndDateTime <= endDate)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            AggregationJobDetails aggregationJob = new AggregationJobDetails();
            aggregationJob.AggregationStartDateTime = startDate;
            aggregationJob.AggregationEndDateTime = endDate;
            aggregationJob.TbJobs = xJobs;

            return aggregationJob;
        }

        int IAggregationRepository.GetJobCounts()
        {
            if (this.context.TbJobs != null)
            {
                List<TbJob> list = this.context.TbJobs
                .Where(x => x.Status < 3 && x.JobType == (int)Job_Type.Rerun)
                .ToList();

                return list.Count();
            }

            return 0;
        }

        IEnumerable<TbJob> IAggregationRepository.GetJobs()
        {
            if (this.context.TbJobs != null)
            {
                List<TbJob> list = this.context.TbJobs
                .Include(x => x.StatusNavigation)
                .Include(x => x.IntervalNavigation)
                .Include(x => x.JobTypeNavigation)
                .Include(x => x.PriorityNavigation)
                .Include(x => x.TbLogErrors)
                .Include(x => x.TbLogActivities)
                .Where(x => x.Status < 3)
                .ToList();

                foreach (TbJob job in list)
                {
                    if (job.JobType == (int)Job_Type.Rerun)
                    {
                        job.IsRecovery = false;
                    }
                }

                return list
                    .OrderBy(x => x.StartAfterDateTime)
                    .OrderBy(x => x.Interval)
                    .OrderBy(x => x.Priority);
            }

            return Enumerable.Empty<TbJob>();
        }

        IEnumerable<VwGetJob> IAggregationRepository.GetJobs(DateTime StartDate)
        {
            if (this.context.VwGetJobs != null)
            {
                TbConfiguration? configuration = this.context.TbConfigurations
                .FirstOrDefault(m => m.Key == "ActionViewOpenPeriod");

                DateTime openPeriod = (configuration != null) ? DateTime.ParseExact(configuration.Value, "yyyy/MM/dd", CultureInfo.InvariantCulture) : System.DateTime.MinValue;

                List<VwGetJob> list = this.context.VwGetJobs
                    .Where(x => x.AggregationStartDateTime >= openPeriod && x.AggregationStartDateTime >= StartDate && x.AggregationEndDateTime < StartDate.AddDays(1))
                    .ToList();

                return list
                    .OrderByDescending(x => x.AggregationStartDateTime)
                    .OrderByDescending(x => x.AggregationEndDateTime)
                    .OrderByDescending(x => x.StartDateTime)
                    .OrderBy(x => x.Interval);
            }

            return Enumerable.Empty<VwGetJob>();
        }

        IEnumerable<TbJob> IAggregationRepository.GetJobs(DateTime StartDate, DateTime EndDate, int selectedInterval, int selectedJobType, int selectedStatus)
        {
            if (this.context.TbJobs != null)
            {
                List<TbJob> list = this.context.TbJobs
                .Include(x => x.StatusNavigation)
                .Include(x => x.IntervalNavigation)
                .Include(x => x.JobTypeNavigation)
                .Include(x => x.PriorityNavigation)
                .Include(x => x.TbLogErrors)
                .Include(x => x.TbLogActivities)
                .Where(
                    x => x.AggregationStartDateTime >= StartDate && x.AggregationEndDateTime < EndDate.AddDays(1) &&
                    (selectedInterval == 0 || (selectedInterval > 0 && x.Interval == selectedInterval)) &&
                    (selectedJobType == 0 || (selectedJobType > 0 && x.JobType == selectedJobType)) &&
                    (selectedStatus == 0 || (selectedStatus > 0 && x.Status == selectedStatus))
                )
                .ToList();

                return list
                    .OrderByDescending(x => x.AggregationStartDateTime)
                    .OrderByDescending(x => x.AggregationEndDateTime)
                    .OrderByDescending(x => x.Interval)
                    .OrderByDescending(x => x.StartDateTime);
            }

            return Enumerable.Empty<TbJob>();
        }

        IEnumerable<TbJobType> IAggregationRepository.GetJobTypes(bool withSelect)
        {
            List<TbJobType> list = this.context.TbJobTypes.ToList();
            if (withSelect)
            {

                TbJobType d = new TbJobType();
                d.Description = "--Select Job Type--";
                d.Id = 0;

                list.Insert(0, d);
            }

            return list;
        }

        async Task<List<TbJobType>> IAggregationRepository.GetJobTypesAsync(bool withSelect)
        {
            List<TbJobType> list = await Task.Run(() => this.context.TbJobTypes.ToList());
            if (withSelect)
            {

                TbJobType d = new TbJobType();
                d.Description = "--Select Job Type--";
                d.Id = 0;

                list.Insert(0, d);
            }

            return list;
        }

        IEnumerable<TbInterval> IAggregationRepository.GetIntervals(bool withSelect)
        {
            List<TbInterval> list = this.context.TbIntervals.ToList();

            if (withSelect)
            {
                TbInterval d = new TbInterval();
                d.Description = "--Select Interval--";
                d.Id = 0;

                list.Insert(0, d);
            }
            return list;
        }

        async Task<List<TbInterval>> IAggregationRepository.GetIntervalsAsync(bool withSelect)
        {
            List<TbInterval> list = await Task.Run(() => this.context.TbIntervals.ToList());
            if (withSelect)
            {

                TbInterval d = new TbInterval();
                d.Description = "--Select Intervale--";
                d.Id = 0;

                list.Insert(0, d);
            }

            return list;
        }

        IEnumerable<VwRnodeTypeMetric> IAggregationRepository.GetMetrics()
        {
            return this.context.VwRnodeTypeMetrics.ToList();
        }

        async Task<List<VwRnodeTypeMetric>> IAggregationRepository.GetMetricsAsync()
        {
            List<VwRnodeTypeMetric> list = await Task.Run(() => this.context.VwRnodeTypeMetrics.ToList());
            return list;
        }

        IEnumerable<RtbNodeType> IAggregationRepository.GetNodeTypes()
        {
            return this.context.RtbNodeTypes.ToList();
        }

        async Task<List<RtbNodeType>> IAggregationRepository.GetNodeTypesAsync()
        {
            List<RtbNodeType> list = await Task.Run(() => this.context.RtbNodeTypes.ToList());
            return list;
        }

        IEnumerable<TbPriority> IAggregationRepository.GetPriorities()
        {
            return this.context.TbPriorities.ToList();
        }

        async Task<List<TbPriority>> IAggregationRepository.GetPrioritiesAsync()
        {
            List<TbPriority> list = await Task.Run(() => this.context.TbPriorities.ToList());
            return list;
        }

        IEnumerable<TbStatus> IAggregationRepository.GetStatuses(bool withSelect)
        {
            List<TbStatus> list = this.context.TbStatuses.ToList();

            if (withSelect)
            {
                TbStatus d = new TbStatus();
                d.Description = "--Select Status--";
                d.Id = 0;

                list.Insert(0, d);
            }

            return list;
        }

        async Task<List<TbStatus>> IAggregationRepository.GetStatusesAsync(bool withSelect)
        {
            List<TbStatus> list = await Task.Run(() => this.context.TbStatuses.ToList());
            if (withSelect)
            {

                TbStatus d = new TbStatus();
                d.Description = "--Select Status--";
                d.Id = 0;

                list.Insert(0, d);
            }

            return list;
        }

        async Task<TbUser?> IAggregationRepository.GetUserAsync(int id)
        {
            var tbuser = await this.context.TbUsers.FirstOrDefaultAsync(m => m.Id == id);

            return tbuser;
        }

        TbUser? IAggregationRepository.GetUser(string userId)
        {
            return this.context.TbUsers.FirstOrDefault(m => m.UserId == userId);
        }

        async Task<List<TbUser>> IAggregationRepository.GetUsersAsync()
        {
            List<TbUser> list = await Task.Run(() => this.context.TbUsers.ToList());

            return list;
        }

        IEnumerable<TbWebServer> IAggregationRepository.GetWebServers()
        {
            if (this.context.TbConfigurations != null)
            {
                var configObject = this.context.TbConfigurations
                    .FirstOrDefault(m => m.Key == "ProjectName");

                string instnaceName = (configObject == null) ? "" : configObject.Value;

                var webServers = this.context.TbWebServers
                    .Where(s => s.Active && s.InstanceName != instnaceName)
                    .OrderBy(s => s.InstanceName)
                    .ToList();

                return webServers;

            }

            return Enumerable.Empty<TbWebServer>();
        }

        async Task<bool> IAggregationRepository.CancelJob(int jobId, int userId)
        {
            var job = await this.context.TbJobs
             .FirstOrDefaultAsync(m => m.Id == jobId);

            if (job == null)
                return false;

            try
            {
                if (job.Status == (int)Status.Created)
                {
                    job.Status = (int)Status.Canceled;
                    this.context.Attach(job).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        async Task<bool> IAggregationRepository.RefreshJob(int jobId, int priority, int userId, string loginId)
        {
            var job = await this.context.TbJobs
             .FirstOrDefaultAsync(m => m.Id == jobId);

            if (job == null)
                return false;

            try
            {
                TbActivityLogger activityLogger = new TbActivityLogger();
                activityLogger.Timestamp = DateTime.Now;
                activityLogger.ActivityId = (int)Activity.RefreshDay;
                activityLogger.UserId = userId;
                activityLogger.OldValue = "";
                activityLogger.NewValue = job.AggregationStartDateTime.ToString() + " - " + job.AggregationEndDateTime.ToString();
                this.context.Attach(activityLogger).State = EntityState.Added;
                context.SaveChanges();

                TbJob newJob = new TbJob();
                newJob.AggregationStartDateTime = job.AggregationStartDateTime;
                newJob.AggregationEndDateTime = job.AggregationEndDateTime;
                newJob.StartAfterDateTime = job.StartAfterDateTime;
                newJob.Priority = priority;
                newJob.Interval = (int)Interval.Daily;
                newJob.JobType = (int)Job_Type.Rerun;
                newJob.IsRecovery = false;
                newJob.CreatedDateTime = System.DateTime.Now;
                newJob.CreatedBy = loginId;
                newJob.Status = (int)Status.Created;
                newJob.ExitCode = -1;
                this.context.Attach(newJob).State = EntityState.Added;
                context.SaveChanges();

                DateTime aggregationStartDateTime = job.AggregationStartDateTime;
                for (int i = 0; i < 24; i++)
                {
                    newJob = new TbJob();
                    newJob.AggregationStartDateTime = aggregationStartDateTime;
                    newJob.AggregationEndDateTime = aggregationStartDateTime.AddHours(1).AddSeconds(-1);
                    newJob.StartAfterDateTime = aggregationStartDateTime.AddHours(1).AddSeconds(-1).AddMinutes(5);
                    newJob.Priority = priority;
                    newJob.Interval = (int)Interval.Hourly;
                    newJob.JobType = (int)Job_Type.Rerun;
                    newJob.IsRecovery = false;
                    newJob.CreatedDateTime = System.DateTime.Now;
                    newJob.CreatedBy = loginId;
                    newJob.Status = (int)Status.Created;
                    newJob.ExitCode = -1;
                    this.context.Attach(newJob).State = EntityState.Added;
                    context.SaveChanges();
                    aggregationStartDateTime = aggregationStartDateTime.AddHours(1);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        async Task IAggregationRepository.SaveConfiguration(TbConfiguration configuration, string oldValue, int userId)
        {
            try
            {
                TbActivityLogger activityLogger = new TbActivityLogger();
                activityLogger.Timestamp = DateTime.Now;
                activityLogger.ActivityId = (int)Activity.ChangeConfiguration;
                activityLogger.UserId = userId;
                activityLogger.ConfigurationId = configuration.Id;
                activityLogger.OldValue = oldValue;
                activityLogger.NewValue = configuration.Value;
                this.context.Attach(activityLogger).State = EntityState.Added;
                context.SaveChanges();

                this.context.Attach(configuration).State = EntityState.Modified;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }

            _ = await this.context.SaveChangesAsync();
        }

        async Task IAggregationRepository.SaveUser(TbUser user, bool isNew, bool updateAdminstratorValue)
        {
            try
            {
                if (isNew)
                {
                    this.context.Attach(user).State = EntityState.Added;
                }
                else
                {
                    TbUser? cUser = this.context.TbUsers.FirstOrDefault(m => m.Id == user.Id);
                    if (cUser != null)
                    {
                        cUser.Active = user.Active;
                        if (updateAdminstratorValue)
                        {
                            cUser.HasAdminAccess = user.HasAdminAccess;
                        }
                        cUser.HasAccessToConfiguration = user.HasAccessToConfiguration;
                        cUser.HasAccessToQueue = user.HasAccessToQueue;
                        cUser.HasAccessToRefreshJob = user.HasAccessToRefreshJob;
                        cUser.HasAccessToUpdateClock = user.HasAccessToUpdateClock;
                        cUser.Name = user.Name;

                        this.context.Attach(cUser).State = EntityState.Modified;
                    }

                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }

            _ = await this.context.SaveChangesAsync();
        }

        async Task<bool> IAggregationRepository.UpdateClock(int jobId, int userId)
        {
            var configuration = await this.context.TbConfigurations
                .FirstOrDefaultAsync(m => m.Key == "LastHourlyJob_Scheduled_DateTime");

            if (configuration == null)
                return false;

            var job = await this.context.TbJobs
              .FirstOrDefaultAsync(m => m.Id == jobId);

            if (job == null)
                return false;

            IEnumerable<TbJob> jobs = this.context.TbJobs
                .Where(x => x.AggregationStartDateTime >= job.AggregationStartDateTime && x.Status == 1)
                .ToList();

            try
            {
                string newValue = job.AggregationEndDateTime.ToString("yyyy/MM/dd HH:mm:ss");

                TbActivityLogger activityLogger = new TbActivityLogger();
                activityLogger.Timestamp = DateTime.Now;
                activityLogger.ActivityId = (int)Activity.ResetAggregationClock;
                activityLogger.UserId = userId;
                activityLogger.ConfigurationId = configuration.Id;
                activityLogger.OldValue = configuration.Value;
                activityLogger.NewValue = newValue;
                this.context.Attach(activityLogger).State = EntityState.Added;

                foreach (TbJob xjob in jobs)
                {
                    xjob.Status = (int)Status.Canceled;
                    this.context.Attach(xjob).State = EntityState.Modified;
                }

                configuration.Value = newValue;
                this.context.Attach(configuration).State = EntityState.Modified;

                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }

            return true;
        }
    }
}
