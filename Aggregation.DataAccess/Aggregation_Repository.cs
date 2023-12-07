using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using System.Diagnostics;
using System.Xml.Linq;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace DataAccess
{
    #region Enums
    public enum Prioity
    {
        High = 1,
        Normal = 2,
        Low = 3
    }

    public enum Status
    {
        Created = 1,
        Started = 2,
        Completed = 3,
        CompletedWithError = 4,
        Canceled = 5
    }

    public enum Interval
    {
        Hourly = 1,
        Daily = 2
    }

    public enum Aggregate_Type
    {
        Min = 1,
        Max = 2,
        Avg = 3,
        First = 4,
        Last = 5,
        Sum = 6,
        Count = 7
    }

    public enum Job_Type
    {
        Normal = 1,
        Rerun = 2,
        Refresh = 3
    }

    public enum Log_Type
    {
        Acticity = 1,
        Info = 2
    }
    #endregion

    public class Aggregation_Repository
    {
        #region Variables
        public static ConfigObject configuration;
        public static AggregationObject aggregationObject;
        #endregion

        #region Properties
        private static string GetDisabledFileName
        {
            get
            {
                return ConfigurationManager.AppSettings["disabledFileName"];
            }
        }

        public static string GetExecutablePath
        {
            get
            {
                return ConfigurationManager.AppSettings["executablePath"];
            }
        }

        public static string GetServicePath
        {
            get
            {
                return ConfigurationManager.AppSettings["servicePath"];
            }
        }

        private static string GetEmailRecipient
        {
            get
            {
                return ConfigurationManager.AppSettings["emailRecipient"];
            }
        }

        private static string GetSMTPServer
        {
            get
            {
                return ConfigurationManager.AppSettings["smtpServer"];
            }
        }

        private static int GetSMTPPort
        {
            get
            {
                int port;
                bool success = int.TryParse(ConfigurationManager.AppSettings["smtpPort"], out port);
                return port;
            }
        }

        public static string GetQueueFolderPath
        {
            get
            {
                return ConfigurationManager.AppSettings["executablePath"] + @"OfflineQueue\";
            }
        }

        public static int GetInterval_NextJobDelay
        {
            get
            {
                int delay;
                bool success = int.TryParse(ConfigurationManager.AppSettings["interval_TimerDelay"], out delay);
                return delay;
            }
        }

        public static AggregationObject AggregationObj
        {
            get
            {
                return aggregationObject;
            }
            set
            {
                aggregationObject = value;
            }
        }
        #endregion

        private static void CheckFolders()
        {
            string path = GetExecutablePath;
            string[] appFolders = { "OfflineQueue", Helper.ErrorLogs, Helper.Logs };
            foreach (string folder in appFolders)
            {
                CreateFolders(path + @"\" + folder);
            }

            path = GetServicePath;
            string[] serviceFolders = { Helper.ErrorLogs, Helper.Logs };
            foreach (string folder in serviceFolders)
            {
                CreateFolders(path + @"\" + folder);
            }
        }

        private static int CopyJob(igenmsEntities context, string id)
        {
            Logger logger = new Logger(0, "CreateJob", "Job Id: " + id);

            try
            {
                long jobId = 0;
                bool success = long.TryParse(id, out jobId);

                tbJob job = (from j in context.tbJobs
                             where j.id == jobId
                             && j.status == (int)Status.Completed && j.jobType == (int)Job_Type.Normal && j.interval==(int)Interval.Hourly
                             select j).FirstOrDefault();

                if (job != null)
                {
                    tbJob newJob = new tbJob();
                    newJob.aggregationStartDateTime = job.aggregationStartDateTime;
                    newJob.aggregationEndDateTime = job.aggregationEndDateTime;
                    newJob.startAfterDateTime = job.aggregationEndDateTime.AddMinutes(job.interval == (int)Interval.Hourly ? 1 : 5);
                    newJob.priority = (int)Prioity.Low;
                    newJob.interval = job.interval;
                    newJob.jobType = (int)Job_Type.Rerun;
                    newJob.isRecovery = false;
                    newJob.createdDateTime = System.DateTime.Now;
                    newJob.createdBy = GetCurrentUserName();
                    newJob.status = (int)Status.Created;
                    newJob.exitCode = -1;
                    context.Entry(newJob).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
                AggregationObj.errors.Add(logger);
            }

            return logger.returnCode;
        }

        private static void CleanUp(igenmsEntities context, tbJob job)
        {
            Logger logger = new Logger(job.id, "CleanUp", null);

            string[] paths = { GetExecutablePath, GetServicePath };
            foreach (string path in paths)
            {
                string[] folders = { Helper.Logs, Helper.ErrorLogs };
                foreach (string folder in folders)
                {
                    string[] fileEntries = Directory.GetFiles(path + folder);
                    foreach (string fileName in fileEntries)
                    {
                        DateTime fileCreatedDate = File.GetCreationTime(fileName);
                        if ((System.DateTime.Now - fileCreatedDate).TotalHours > configuration.Retention_ActivityLog_NumberOfHours)
                        {
                            System.IO.File.Delete(fileName);
                        }
                    }
                }
            }

            try
            {
                context.Database.CommandTimeout = configuration.SP_CommandTimeout;

                var parameter = new List<object>();

                var jobIdParam = new SqlParameter("@jobId", SqlDbType.BigInt);
                jobIdParam.Value = job.id;
                parameter.Add(jobIdParam);

                var resultParameter = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                parameter.Add(resultParameter);

                string spName = (job.interval == (int)Interval.Hourly) ? "spCleanUpHourly" : "spCleanUpDaily";
                var data = context.Database.ExecuteSqlCommand("exec @ReturnCode = [aggregation].[" + spName + "] @jobId ", parameter.ToArray());

                context.Database.CommandTimeout = configuration.CommandTimeout;

                logger.Save();
            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
            }

            AggregationObj.errors.Add(logger);
        }

        private static void CreateFolders(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
            }
        }

        private static int CreateJob(igenmsEntities context, string aggregationTime, string priroty)
        {
            Logger logger = new Logger(0, "CreateJob", "aggregationTime: " + aggregationTime);

            int jobPriority = 3;
            switch (priroty)
            {
                case "1":
                    jobPriority = 1;
                    break;
                case "2":
                    jobPriority = 2;
                    break;
                default:
                    jobPriority = 3;
                    break;
            }


            try
            {
                string dateString = String.Format("{0}/{1}/{2} 00:00:00", aggregationTime.Substring(0, 4), aggregationTime.Substring(4, 2), aggregationTime.Substring(6, 2));
                DateTime aggregationStartDateTime = DateTime.ParseExact(dateString, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);

                tbJob newJob;

                newJob = new tbJob();
                newJob.aggregationStartDateTime = aggregationStartDateTime;
                newJob.aggregationEndDateTime = aggregationStartDateTime.AddHours(24).AddSeconds(-1); ;
                newJob.startAfterDateTime = aggregationStartDateTime.AddDays(1).AddHours(1).AddMinutes(5).AddSeconds(-1);
                newJob.priority = jobPriority;
                newJob.interval = (int)Interval.Daily;
                newJob.jobType = (int)Job_Type.Rerun;
                newJob.isRecovery = false;
                newJob.createdDateTime = System.DateTime.Now;
                newJob.createdBy = GetCurrentUserName();
                newJob.status = (int)Status.Created;
                newJob.exitCode = -1;
                context.Entry(newJob).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();

                for (int i = 0; i < 24; i++)
                {
                    newJob = new tbJob();
                    newJob.aggregationStartDateTime = aggregationStartDateTime;
                    newJob.aggregationEndDateTime = aggregationStartDateTime.AddHours(1).AddSeconds(-1);
                    newJob.startAfterDateTime = aggregationStartDateTime.AddHours(1).AddSeconds(-1).AddMinutes(5);
                    newJob.priority = jobPriority;
                    newJob.interval = (int)Interval.Hourly;
                    newJob.jobType = (int)Job_Type.Rerun;
                    newJob.isRecovery = false;
                    newJob.createdDateTime = System.DateTime.Now;
                    newJob.createdBy = GetCurrentUserName();
                    newJob.status = (int)Status.Created;
                    newJob.exitCode = -1;
                    context.Entry(newJob).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    aggregationStartDateTime = aggregationStartDateTime.AddHours(1);
                }
            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
                AggregationObj.errors.Add(logger);
            }

            return logger.returnCode;
        }

        private static int CreateJob(igenmsEntities context, tbJob lastJob)
        {
            Logger logger = null;
            DateTime aggregationEndDateTime = DateTime.MinValue;
            DateTime aggregationStartDateTime = DateTime.MinValue;
            DateTime dayStartDateTime = DateTime.MinValue;
            DateTime dayEndtDateTime = DateTime.MinValue;

            string parmeters = (lastJob is null) ? "" : lastJob.id.ToString();

            try
            {
                tbJob job;

                if (lastJob != null && lastJob.status == (int)Status.Completed && lastJob.interval == (int)Interval.Hourly)
                {
                    if (lastJob.jobType == (int)Job_Type.Refresh)
                    {
                        if ((lastJob.aggregationEndDateTime.Hour == 23 && configuration.Refresh_DailyJobOnce) || !configuration.Refresh_DailyJobOnce)
                        {
                            job = new tbJob();
                            job.aggregationStartDateTime = lastJob.aggregationEndDateTime.AddSeconds(1).AddHours(-1).AddHours(-lastJob.aggregationStartDateTime.Hour);
                            job.aggregationEndDateTime = job.aggregationStartDateTime.AddDays(1).AddSeconds(-1);
                            job.startAfterDateTime = lastJob.startAfterDateTime;
                            job.priority = (int)Prioity.Normal;
                            job.interval = (int)Interval.Daily;
                            job.jobType = lastJob.jobType;
                            job.isRecovery = false;
                            job.createdDateTime = System.DateTime.Now;
                            job.createdBy = GetCurrentUserName();
                            job.status = (int)Status.Created;
                            job.parentJobId = lastJob.id;
                            job.exitCode = -1;
                            context.Entry(job).State = System.Data.Entity.EntityState.Added;
                            context.SaveChanges();
                        }

                        return 0;
                    }
                    else
                    {
                        if (lastJob.aggregationEndDateTime.Hour == 23)
                        {
                            job = (from j in context.tbJobs
                                   where j.aggregationStartDateTime == dayStartDateTime && j.aggregationEndDateTime == dayEndtDateTime && j.status == (int)Status.Created
                                   select j).FirstOrDefault();

                            if (job == null)
                            {
                                job = new tbJob();
                                job.aggregationStartDateTime = lastJob.aggregationEndDateTime.AddDays(-1).AddSeconds(1);
                                job.aggregationEndDateTime = lastJob.aggregationEndDateTime;
                                job.startAfterDateTime = job.aggregationEndDateTime.AddMinutes(configuration.Job_StartTimeDelay);
                                job.priority = (int)Prioity.Normal;
                                job.interval = (int)Interval.Daily;
                                job.jobType = (int)Job_Type.Normal;
                                job.isRecovery = false;
                                job.createdDateTime = System.DateTime.Now;
                                job.createdBy = GetCurrentUserName();
                                job.status = (int)Status.Created;
                                job.exitCode = -1;
                                context.Entry(job).State = System.Data.Entity.EntityState.Added;
                                context.SaveChanges();

                                configuration = GetConfiguration(context, job.aggregationStartDateTime);
                            }
                        }
                    }
                }

                aggregationEndDateTime = configuration.LastHourlyJob_Scheduled_DateTime.AddHours(1);
                aggregationStartDateTime = configuration.LastHourlyJob_Scheduled_DateTime.AddSeconds(1);

                configuration = GetConfiguration(context, aggregationStartDateTime);

                tbJob jobExist = (from j in context.tbJobs
                                  where j.aggregationStartDateTime <= aggregationStartDateTime && j.aggregationEndDateTime <= aggregationEndDateTime && j.status == (int)Status.Created && j.jobType == (int)Job_Type.Normal
                                  select j).FirstOrDefault();

                if (jobExist != null)
                {
                    return 0;
                }

                tbJob newJob = new tbJob();
                newJob.aggregationEndDateTime = aggregationEndDateTime;
                newJob.aggregationStartDateTime = aggregationStartDateTime;
                newJob.createdDateTime = System.DateTime.Now;
                newJob.createdBy = GetCurrentUserName();
                newJob.jobType = (int)Job_Type.Normal;
                newJob.isRecovery = false;
                newJob.interval = (int)Interval.Hourly;
                newJob.priority = (int)Prioity.Normal;
                newJob.startAfterDateTime = newJob.aggregationEndDateTime.AddMinutes(configuration.Job_StartTimeDelay);
                newJob.status = (int)Status.Created;
                if (lastJob != null && lastJob.status == (int)Status.CompletedWithError)
                {
                    newJob.jobType = (int)lastJob.jobType;
                    newJob.isRecovery = true;
                    newJob.interval = (int)lastJob.interval;
                    newJob.aggregationStartDateTime = lastJob.aggregationStartDateTime;
                    newJob.aggregationEndDateTime = lastJob.aggregationEndDateTime;
                    newJob.parentJobId = lastJob.id;
                }
                newJob.exitCode = -1;
                context.Entry(newJob).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();

                logger = new Logger(newJob.id, "CreateJob", parmeters);

                if (lastJob != null && lastJob.status == (int)Status.CompletedWithError)
                {
                    return logger.returnCode;
                }

                if (newJob.jobType == (int)Job_Type.Normal)
                {
                    configuration.LastHourlyJob_Scheduled_DateTime = newJob.aggregationEndDateTime;
                    tbConfiguration Confitem = (from c in context.tbConfigurations
                                                where c.key == "LastHourlyJob_Scheduled_DateTime"
                                                select c).FirstOrDefault();
                    Confitem.value = newJob.aggregationEndDateTime.ToString("yyyy/MM/dd HH:mm:ss");
                    context.Entry(Confitem).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }

                if (configuration.Refresh_Enabled)
                {
                    configuration = GetConfiguration(context, newJob.aggregationStartDateTime);
                    for (int jc = 1; jc <= configuration.NumberOfRefresh; jc++)
                    {
                        tbJob refreshJob = new tbJob();
                        refreshJob.aggregationEndDateTime = newJob.aggregationEndDateTime;
                        refreshJob.aggregationStartDateTime = newJob.aggregationStartDateTime;
                        refreshJob.createdDateTime = System.DateTime.Now;
                        refreshJob.createdBy = GetCurrentUserName();
                        refreshJob.jobType = (int)Job_Type.Refresh;
                        refreshJob.isRecovery = false;
                        refreshJob.interval = newJob.interval;
                        refreshJob.priority = newJob.priority;
                        refreshJob.startAfterDateTime = newJob.startAfterDateTime.AddDays(jc * configuration.Refresh_Interval);
                        refreshJob.status = (int)Status.Created;
                        refreshJob.parentJobId = newJob.id;
                        refreshJob.exitCode = -1;
                        context.Entry(refreshJob).State = System.Data.Entity.EntityState.Added;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
                AggregationObj.errors.Add(logger);
            }

            return logger.returnCode;
        }

        private static ConfigObject GetConfiguration(igenmsEntities context, DateTime? date)
        {
            ConfigObject configuration = new ConfigObject();

            try
            {
                List<tbConfiguration> listOfConfiguration = (from c in context.tbConfigurations
                                                             orderby c.id
                                                             select c).ToList();

                foreach (tbConfiguration item in listOfConfiguration)
                {
                    if (item.key == "Job_StartTimeDelay")
                    {
                        configuration.Job_StartTimeDelay = Int32.Parse(item.value);
                    }
                    if (item.key == "NumberOfRefresh")
                    {
                        configuration.NumberOfRefresh = Int32.Parse(item.value);
                    }
                    if (item.key == "CommandTimeout")
                    {
                        configuration.CommandTimeout = Int32.Parse(item.value);
                    }
                    if (item.key == "LastHourlyJob_Scheduled_DateTime")
                    {
                        configuration.LastHourlyJob_Scheduled_DateTime = DateTime.ParseExact(item.value, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    if (item.key == "Refresh_Enabled")
                    {
                        configuration.Refresh_Enabled = (item.value == "1");
                    }
                    if (item.key == "Refresh_DailyJobOnce")
                    {
                        configuration.Refresh_DailyJobOnce = (item.value == "1");
                    }
                    if (item.key == "Refresh_Interval")
                    {
                        configuration.Refresh_Interval = Int32.Parse(item.value);
                    }
                    if (item.key == "Job_SuspendProcessingAfter")
                    {
                        configuration.Job_SuspendProcessingAfter = DateTime.ParseExact(item.value, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    if (item.key == "Retention_ActivityLog_NumberOfHours")
                    {
                        configuration.Retention_ActivityLog_NumberOfHours = Int32.Parse(item.value);
                    }
                    if (item.key == "SP_CommandTimeout")
                    {
                        configuration.SP_CommandTimeout = Int32.Parse(item.value);
                    }
                    if (item.key == "Log_ElapsedTime")
                    {
                        configuration.Log_ElapsedTime = (item.value == "1");
                    }
                    if (item.key == "NodeStatus")
                    {
                        configuration.Node_Status = item.value;
                    }
                    if (item.key == "ProjectName")
                    {
                        configuration.ProjectName = item.value;
                    }
                    configuration.SkipJobProcessing = false;
                }

                if (String.IsNullOrEmpty(configuration.Node_Status))
                {
                    configuration.Node_Status = "NodeStatus";
                }

                if (date != null)
                {
                    DateTime configDate = date.Value.Date;

                    tbConfigurationByDate configByDate = (from j in context.tbConfigurationByDates
                                                          where j.aggregationDate == configDate
                                                          select j).FirstOrDefault();

                    if (configByDate == null)
                    {
                        tbConfigurationByDate tbConfigByDate = new tbConfigurationByDate();
                        tbConfigByDate.aggregationDate = (DateTime)date;
                        tbConfigByDate.Job_StartTimeDelay = configuration.Job_StartTimeDelay;
                        tbConfigByDate.Refresh_NumberOfDays = configuration.NumberOfRefresh;
                        tbConfigByDate.Refresh_DailyJobOnce = configuration.Refresh_DailyJobOnce;
                        tbConfigByDate.Refresh_Interval = configuration.Refresh_Interval;
                        tbConfigByDate.Refresh_Enabled = configuration.Refresh_Enabled;
                        tbConfigByDate.createdDateTime = System.DateTime.Now;
                        context.Entry(tbConfigByDate).State = System.Data.Entity.EntityState.Added;
                        context.SaveChanges();
                    }
                    else
                    {
                        configuration.Job_StartTimeDelay = configByDate.Job_StartTimeDelay;
                        configuration.NumberOfRefresh = configByDate.Refresh_NumberOfDays;
                        configuration.Refresh_DailyJobOnce = configByDate.Refresh_DailyJobOnce;
                        configuration.Refresh_Interval = configByDate.Refresh_Interval;
                        configuration.Refresh_Enabled = configByDate.Refresh_Enabled;
                    }
                }

                return configuration;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetCurrentUserName()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        private static tbJob GetNextJob(igenmsEntities context, long jobId)
        {
            tbJob job = null;
            int rc = 0;

            string exceptionMessage = "";
            string innerException = "";
            string stackTrace = "";

            if (IsEnabled())
            {
                try
                {
                    if (jobId == 0)
                    {
                        if (!JobExist(context))
                        {
                            tbJob lastJob = null;
                            CreateJob(context, lastJob);
                        }

                        job = (from j in context.tbJobs
                               where j.startAfterDateTime <= System.DateTime.Now && j.status == (int)Status.Created && j.aggregationStartDateTime <= configuration.Job_SuspendProcessingAfter
                               orderby j.priority, j.startAfterDateTime ascending, j.id ascending
                               select j).FirstOrDefault();
                    }
                    else
                    {
                        job = (from j in context.tbJobs
                               where j.id == jobId
                               select j).FirstOrDefault();
                    }

                    if (job != null)
                    {
                        if (rc == 0 && job.status != (int)Status.Created)
                        {
                            rc = 1;
                        }

                        if (rc == 0 && job.aggregationStartDateTime > configuration.Job_SuspendProcessingAfter)
                        {
                            rc = 2;
                        }
                    }
                }
                catch (Exception ex)
                {
                    rc = -1;
                    exceptionMessage = ex.Message;
                    innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                    stackTrace = ex.StackTrace;
                }
            }
            else
            {
                rc = -2;
            }

            if (rc > 0)
            {
                string parmeters = "jobId:" + jobId.ToString();
                Logger logger = new Logger(jobId, "CanProcessJob", parmeters);
                logger.returnCode = rc;
                logger.exceptionMessage = (string.IsNullOrEmpty(exceptionMessage)) ? exceptionMessage : "Validation failed.";
                logger.innerException = innerException;
                logger.stackTrace = stackTrace;
                AggregationObj.errors.Add(logger);
            }

            return (rc == 0) ? job : null;
        }

        public static int GetNextJobDelay()
        {
            int delay = 0;

            try
            {
                using (igenmsEntities context = new igenmsEntities())
                {
                    context.Database.CommandTimeout = configuration.CommandTimeout;

                    configuration = GetConfiguration(context, null);

                    tbJob job = (from j in context.tbJobs
                                 where j.startAfterDateTime <= System.DateTime.Now && j.status == (int)Status.Created && j.aggregationStartDateTime <= configuration.Job_SuspendProcessingAfter
                                 orderby j.priority, j.startAfterDateTime ascending, j.id ascending
                                 select j).FirstOrDefault();

                    if (job == null)
                    {
                        bool success = int.TryParse(ConfigurationManager.AppSettings["interval_TimerDelay"], out delay);
                    }
                    else
                    {
                        delay = 1000;
                    }
                }
            }
            catch (Exception ex)
            {
                bool success = int.TryParse(ConfigurationManager.AppSettings["interval_TimerDelay"], out delay);
            }

            return delay;
        }

        private static int GetNumberOfProcessedRecords(igenmsEntities context, long jobId)
        {
            int? processed = (from j in context.tbJobs
                              where j.id == jobId
                              select j.processed).FirstOrDefault();
            return (processed == null) ? 0 : (int)processed;
        }

        public static bool IsEnabled()
        {
            bool fileExist = (File.Exists(GetExecutablePath + GetDisabledFileName));
            return !fileExist;
        }

        private static bool JobExist(igenmsEntities context)
        {
            tbJob job = (from j in context.tbJobs
                         where j.startAfterDateTime <= System.DateTime.Now && j.status == (int)Status.Created && j.jobType == (int)Job_Type.Normal && j.aggregationEndDateTime <= configuration.LastHourlyJob_Scheduled_DateTime
                         orderby j.priority, j.startAfterDateTime ascending, j.id ascending
                         select j).FirstOrDefault();

            return (job != null);
        }

        public static void KillAllProcesses()
        {
            Process[] processes = Helper.GetProcessesByName("AggregationApp");

            Array.ForEach(processes, (process) =>
            {
                process.Kill();
            });
        }

        public static int Main(string command, string parameter)
        {
            CheckFolders();

            AggregationObj = new AggregationObject();
            AggregationObj.errors = new List<Logger>();

            string parmeters = "Command:" + command + ", parameter:, " + parameter;
            Logger logger = new Logger(0, "Main", parmeters);

            string[] parameters = parameter.Split(' ');

            int rc = 1;
            bool processStated = false;

            try
            {
                using (igenmsEntities context = new igenmsEntities())
                {
                    configuration = GetConfiguration(context, null);

                    context.Database.CommandTimeout = configuration.CommandTimeout;

                    switch (command.ToUpper())
                    {
                        case "RERUN":
                            if (parameters.Length > 0)
                            {
                                string date = parameters[0];
                                string priority = (parameters.Length > 1) ? parameters[1]  : "3";
                                processStated = true;
                                rc = CreateJob(context, date, priority);
                            }
                            break;
                        case "COPY":
                            if (parameters.Length > 0)
                            {
                                processStated = true;
                                rc = CopyJob(context, parameters[0]);
                            }
                            break;
                        case "STARTJOB":
                            if (IsEnabled() && parameters.Length > 0)
                            {
                                processStated = true;
                                if (configuration.SkipJobProcessing)
                                {
                                    while (configuration.SkipJobProcessing)
                                    {
                                        long jobId = 0;
                                        bool success = long.TryParse(parameters[0], out jobId);
                                        rc = StartJob(context, jobId);
                                    }
                                }
                                else
                                {
                                    long jobId = 0;
                                    bool success = long.TryParse(parameters[0], out jobId);
                                    rc = StartJob(context, jobId);
                                }
                            }
                            break;
                        case "PERFORMRECOVERY":
                            if (!IsEnabled())
                            {
                                processStated = true;
                                rc = PerformRecovey(context);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                processStated = false;
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
            }

            AggregationObj.errors = null;
            AggregationObj = null;

            if (!processStated)
            {
                if (logger.returnCode == 0)
                {
                    logger.returnCode = -1;
                    logger.exceptionMessage = "Validation failed.";
                }

                string errMessage = "parmeters: " + parmeters + '\n';
                errMessage += "return code: " + logger.returnCode + '\n';
                errMessage += "exceptionMessage: " + logger.exceptionMessage + '\n';
                errMessage += "innerException: " + logger.innerException + '\n';
                errMessage += "stackTrace: " + logger.stackTrace + '\n';

                WriteToFile(null, errMessage, true);
            }

            return logger.returnCode;
        }

        private static int PerformCleanUP(igenmsEntities context, tbJob job)
        {
            job.startDateTime = System.DateTime.Now;
            job.status = (int)Status.Started;
            job.computerName = Environment.MachineName;
            job.processId = Process.GetCurrentProcess().Id;
            context.Entry(job).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            Logger logger = new Logger(job.id, "CleanUpJob", null);

            try
            {
                context.Database.CommandTimeout = configuration.SP_CommandTimeout;

                var parameter = new List<object>();

                var stateDateParam = new SqlParameter("@startDate", SqlDbType.DateTime);
                stateDateParam.Value = job.aggregationStartDateTime.ToString("yyyy-MM-dd") + " 00:00:00.000";
                parameter.Add(stateDateParam);

                var endDateParam = new SqlParameter("@endDate", SqlDbType.DateTime);
                endDateParam.Value = job.aggregationEndDateTime.ToString("yyyy-MM-dd") + " 23:59:59.000";
                parameter.Add(endDateParam);

                var resultParameter = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                parameter.Add(resultParameter);

                var data = context.Database.ExecuteSqlCommand("exec @ReturnCode = [aggregation].[spCleanUpJob] @startDate, @endDate ", parameter.ToArray());

                context.Database.CommandTimeout = configuration.CommandTimeout;
            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
            }

            AggregationObj.errors.Add(logger);

            job.status = (int)Status.Completed;
            job.endDateTime = System.DateTime.Now;
            job.exitCode = 0;
            context.Entry(job).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            return 0;
        }

        private static int PerformRecovey(igenmsEntities context)
        {
            Logger logger = new Logger(0, "PerformRecovey", null);

            Process process = null;

            try
            {
                List<tbJob> jobs = (from j in context.tbJobs
                                    where j.status == (int)Status.Started || j.status == (int)Status.CompletedWithError
                                    orderby j.id descending
                                    select j).ToList();

                foreach (tbJob job in jobs)
                {
                    int pid = (int)job.processId;
                    try
                    {
                        process = Process.GetProcessById(pid);
                    }
                    catch (Exception ex)
                    {
                    }
                    if (job != null && (process == null || (process != null && process.ProcessName != "AggregationApp")))
                    {

                        job.status = (int)Status.Canceled;
                        context.Entry(job).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                        tbJob newJob = new tbJob();
                        newJob.aggregationStartDateTime = job.aggregationStartDateTime;
                        newJob.aggregationEndDateTime = job.aggregationEndDateTime;
                        newJob.startAfterDateTime = job.startAfterDateTime;
                        newJob.priority = job.priority;
                        newJob.interval = job.interval;
                        newJob.jobType = (int)job.jobType;
                        newJob.isRecovery = true;
                        newJob.createdDateTime = System.DateTime.Now;
                        newJob.createdBy = GetCurrentUserName();
                        newJob.status = (int)Status.Created;
                        newJob.parentJobId = job.id;
                        newJob.exitCode = -1;
                        context.Entry(newJob).State = System.Data.Entity.EntityState.Added;
                        context.SaveChanges();
                    }
                }

                string[] fileEntries = Directory.GetFiles(GetQueueFolderPath);

                foreach (string fileName in fileEntries)
                {
                    string jsonString = System.IO.File.ReadAllText(fileName);
                    AggregationObject obj = JsonConvert.DeserializeObject<AggregationObject>(jsonString);

                    WriteLogItems(context, obj);

                    System.IO.File.Delete(fileName);
                }

                SetEnabledValue(true);

            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
                AggregationObj.errors.Add(logger);
            }

            return logger.returnCode;
        }

        private static int PerformRecovey_ReprocessJobs(igenmsEntities context)
        {
            Logger logger = new Logger(0, "PerformRecovey_ReprocessJobs", null);

            Process process = null;

            try
            {
                List<tbJob> jobs = (from j in context.tbJobs
                                    where j.status == (int)Status.Started
                                    orderby j.id descending
                                    select j).ToList();

                foreach (tbJob job in jobs)
                {
                    int pid = (int)job.processId;
                    try
                    {
                        process = Process.GetProcessById(pid);
                    }
                    catch (Exception ex)
                    {
                    }
                    if (job != null && (process == null || (process != null && process.ProcessName != "AggregationApp")))
                    {
                        job.status = (int)Status.Canceled;
                        context.Entry(job).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                        tbJob newJob = new tbJob();
                        newJob.aggregationStartDateTime = job.aggregationStartDateTime;
                        newJob.aggregationEndDateTime = job.aggregationEndDateTime;
                        newJob.startAfterDateTime = job.startAfterDateTime;
                        newJob.priority = job.priority;
                        newJob.interval = job.interval;
                        newJob.jobType = (int)job.jobType;
                        newJob.isRecovery = true;
                        newJob.createdDateTime = System.DateTime.Now;
                        newJob.createdBy = GetCurrentUserName();
                        newJob.status = (int)Status.Created;
                        newJob.parentJobId = job.id;
                        newJob.exitCode = -1;
                        context.Entry(newJob).State = System.Data.Entity.EntityState.Added;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return 0;
        }

        private static int PopulateDaily(igenmsEntities context, tbJob job)
        {
            Logger logger = new Logger(job.id, "PopulatePopulateDaily", null);

            try
            {
                context.Database.CommandTimeout = configuration.SP_CommandTimeout;

                var parameter = new List<object>();

                var jobDateFromParam = new SqlParameter("@fromDate", SqlDbType.DateTime);
                jobDateFromParam.Value = job.aggregationStartDateTime;
                parameter.Add(jobDateFromParam);

                var jobDateToParam = new SqlParameter("@toDate", SqlDbType.DateTime);
                jobDateToParam.Value = job.aggregationEndDateTime.AddSeconds(1);
                parameter.Add(jobDateToParam);

                var resultParameter = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                parameter.Add(resultParameter);

                var data = context.Database.ExecuteSqlCommand("exec @ReturnCode = [aggregation].[spPopulateDaily] " + "@fromDate, @toDate ", parameter.ToArray());

            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
            }

            context.Database.CommandTimeout = configuration.CommandTimeout;

            logger.Save();

            AggregationObj.errors.Add(logger);

            return logger.returnCode;
        }

        private static int PopulateHourly(igenmsEntities context, tbJob job)
        {
            Logger logger = new Logger(job.id, "PopulatePopulateHourly", null);

            try
            {
                context.Database.CommandTimeout = configuration.SP_CommandTimeout;

                var parameter = new List<object>();

                var jobIdParam = new SqlParameter("@jobId", SqlDbType.BigInt);
                jobIdParam.Value = job.id;
                parameter.Add(jobIdParam);

                var jobDateFromParam = new SqlParameter("@fromDate", SqlDbType.DateTime);
                jobDateFromParam.Value = job.aggregationStartDateTime;
                parameter.Add(jobDateFromParam);

                var jobDateToParam = new SqlParameter("@toDate", SqlDbType.DateTime);
                jobDateToParam.Value = job.aggregationEndDateTime.AddSeconds(1);
                parameter.Add(jobDateToParam);

                var resultParameter = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                parameter.Add(resultParameter);

                var data = context.Database.ExecuteSqlCommand("exec @ReturnCode = [aggregation].[spPopulate5Minute] " + "@jobId, @fromDate, @toDate ", parameter.ToArray());
                data = context.Database.ExecuteSqlCommand("exec @ReturnCode = [aggregation].[spPopulateHourly] " + "@fromDate, @toDate ", parameter.ToArray());

            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
            }

            context.Database.CommandTimeout = configuration.CommandTimeout;

            logger.Save();

            AggregationObj.errors.Add(logger);

            return logger.returnCode;
        }

        private static void SaveToQueue(AggregationObject aggregationObj)
        {
            SendNotification(aggregationObj);

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            string fileName = String.Format(GetQueueFolderPath + "{0}.txt", aggregationObj.jobId.ToString());
            using (StreamWriter sw = new StreamWriter(fileName))

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, AggregationObj);
            }
        }

        private static void SendNotification(AggregationObject aggregationObj)
        {
            string from = "Alert.Aggregation@inmarsatgov.com";
            string to = GetEmailRecipient;
            string messageBody = @"Aggregation job failed." + '\n' + '\n';
            string actionNeeded = "Action needed: Please review logs and information below.";
            long jobId = (aggregationObj.parentJobId == null) ? aggregationObj.jobId : (long)aggregationObj.parentJobId;

            messageBody += "Job Id: " + jobId.ToString() + '\n' + '\n';
            messageBody += actionNeeded + '\n' + '\n';
            messageBody += "_______________________________________________________________________" + '\n' + '\n';

            foreach (Logger logger in aggregationObj.errors)
            {
                if (logger != null)
                {
                    int c = 0;

                    if (logger.stackTrace != null)
                    {
                        messageBody += "StackTrace: " + ((logger.stackTrace != null) ? logger.stackTrace.ToString() : "") + '\n' + '\n';
                        c += 1;
                    }

                    if (logger.exceptionMessage != null)
                    {
                        messageBody += "exceptionMessage: " + ((logger.exceptionMessage != null) ? logger.exceptionMessage.ToString() : "") + '\n' + '\n';
                        c += 1;
                    }

                    if (logger.innerException != null)
                    {
                        messageBody += "innerException: " + ((logger.innerException != null) ? logger.innerException.ToString() : "") + '\n' + '\n';
                        c += 1;
                    }

                    if (c > 0)
                    {
                        messageBody += "_______________________________________________________________________" + '\n' + '\n';
                    }
                }
            }

            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(from, to);
                message.Subject = "Aggregation Alert (" + configuration.ProjectName + ") " + String.Format("Job # {0} failed.", aggregationObj.jobId.ToString());
                message.Body = messageBody;
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(GetSMTPServer, GetSMTPPort);
                client.UseDefaultCredentials = false;
                client.Send(message);
            }
            catch (Exception ex)
            {
            }
        }

        public static void SendNotification(string messageBody)
        {
            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage("Alert.Aggregation@inmarsatgov.com", GetEmailRecipient);
                message.Subject = "Aggregation Alert (" + configuration.ProjectName + ")";
                message.Body = messageBody;
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(GetSMTPServer, GetSMTPPort);
                client.UseDefaultCredentials = false;
                client.Send(message);
            }
            catch (Exception ex)
            {
                WriteToFile(null, ex.Message, true);
            }
        }

        private static void SetEnabledValue(bool value)
        {
            string path = GetExecutablePath + GetDisabledFileName;

            try
            {
                if (value)
                {
                    System.IO.File.Delete(path);
                }
                else
                {
                    using (var tw = new System.IO.StreamWriter(path, true))
                    {
                        tw.WriteLine(Guid.NewGuid());
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static int StartJob(igenmsEntities context, long jobId)
        {
            PerformRecovey_ReprocessJobs(context);

            tbJob job = GetNextJob(context, jobId);

            if (job == null)
            {
                return 0;
            }

            Logger logger = new Logger(job.id, "StartJob", null);

            AggregationObj.jobId = job.id;
            AggregationObj.errors.Add(logger);

            try
            {
                job.startDateTime = System.DateTime.Now;
                job.status = (int)Status.Started;
                job.computerName = Environment.MachineName;
                job.processId = Process.GetCurrentProcess().Id;
                context.Entry(job).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                if (!configuration.SkipJobProcessing)
                {
                    logger.returnCode = UpdateNodeTypeDefaultValue(context, job);

                    if (logger.returnCode == 0)
                    {
                        logger.returnCode = UpdateMetricDefaultValueTable(context, job);
                    }

                    if  (logger.returnCode == 0 && job.interval == (int)Interval.Hourly)
                    {
                        logger.returnCode = UpDateNodesStatus(context, job);
                    }

                    if (logger.returnCode == 0)
                    {
                        if (job.interval == (int)Interval.Daily)
                        {
                            logger.returnCode = PopulateDaily(context, job);
                        }
                        else if(job.interval == (int)Interval.Hourly)
                        {
                            logger.returnCode = PopulateHourly(context, job);
                        }
                        else
                        {
                            logger.returnCode = -1;
                        }

                        if (logger.returnCode == 0)
                        {
                            job.processed = GetNumberOfProcessedRecords(context, job.id);
                            if (job.processed == 0 && job.isRecovery.HasValue && (bool)job.isRecovery)
                            {
                                job.processed = 1;
                            }
                            job.status = (int)Status.Completed;
                        }
                        else
                        {
                            job.processed = -1;
                            job.status = (int)Status.CompletedWithError;
                        }
                    }
                }
                else
                {
                    job.processed = 1;
                    job.status = (int)Status.Completed;
                }

                job.endDateTime = System.DateTime.Now;
                job.exitCode = (job.status == (int)Status.Completed) ? 0 : -1;

                context.Entry(job).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                if (job.status == (int)Status.Completed)
                {
                    CleanUp(context, job);

                    if ((job.jobType == (int)Job_Type.Normal || job.jobType == (int)Job_Type.Refresh) && job.interval == (int)Interval.Hourly)
                    {
                        CreateJob(context, job);
                    }
                }

                if (job.status == (int)Status.CompletedWithError)
                {
                    logger.exceptionMessage = "Job completed with error.";
                    logger.returnCode = -1;
                    AggregationObj.errors.Add(logger);
                }
            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -2;
                AggregationObj.errors.Add(logger);
            }


            if (logger.returnCode != 0)
            {
                SetEnabledValue(false);

                SaveToQueue(AggregationObj);
            }
            else
            {
                logger.Save();

                WriteLogItems(context, AggregationObj);
            }

            return logger.returnCode;
        }

        private static int UpdateMetricDefaultValueTable(igenmsEntities context, tbJob job)
        {
            Logger logger = new Logger(job.id, "UpdateMetricDefaultValueTable", null);

            try
            {
                context.Database.CommandTimeout = configuration.SP_CommandTimeout;

                var parameter = new List<object>();

                var resultParameter = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                parameter.Add(resultParameter);

                var data = context.Database.ExecuteSqlCommand("exec @ReturnCode = [aggregation].[spUpdateMetricDefaultValue] ", parameter.ToArray());

                context.Database.CommandTimeout = configuration.CommandTimeout;
            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
            }

            logger.Save();

            AggregationObj.errors.Add(logger);

            return logger.returnCode;
        }

        private static int UpDateNodesStatus(igenmsEntities context, tbJob job)
        {
            string parmeters = "Job Id:" + job.id.ToString();
            Logger logger = new Logger(job.id, "UpDateNodesStatus", null);

            try
            {
                context.Database.CommandTimeout = configuration.SP_CommandTimeout;

                var parameter = new List<object>();

                var stateDateParam = new SqlParameter("@fromTimeStamp", SqlDbType.DateTime);
                stateDateParam.Value = job.aggregationStartDateTime;
                parameter.Add(stateDateParam);

                var endDateParam = new SqlParameter("@toTimeStamp", SqlDbType.DateTime);
                endDateParam.Value = job.aggregationEndDateTime;
                parameter.Add(endDateParam);

                var resultParameter = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                parameter.Add(resultParameter);

                var data = context.Database.ExecuteSqlCommand("exec @ReturnCode = [aggregation].[spPopulateNodeStatus] @fromTimeStamp, @toTimeStamp ", parameter.ToArray());

                context.Database.CommandTimeout = configuration.CommandTimeout;
            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
            }

            logger.Save();

            AggregationObj.errors.Add(logger);

            return logger.returnCode;
        }

        private static int UpdateNodeTypeDefaultValue(igenmsEntities context, tbJob job)
        {
            Logger logger = new Logger(job.id, "UpdateNodeTypeDefaultValue", null);

            try
            {
                context.Database.CommandTimeout = configuration.SP_CommandTimeout;

                var parameter = new List<object>();

                var resultParameter = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                parameter.Add(resultParameter);

                var data = context.Database.ExecuteSqlCommand("exec @ReturnCode = [aggregation].[spUpdateNodeTypeDefaultValue] ", parameter.ToArray());

                context.Database.CommandTimeout = configuration.CommandTimeout;
            }
            catch (Exception ex)
            {
                logger.exceptionMessage = ex.Message;
                logger.innerException = (ex.InnerException != null) ? ex.InnerException.ToString() : "";
                logger.stackTrace = ex.StackTrace;
                logger.returnCode = -1;
            }

            logger.Save();

            AggregationObj.errors.Add(logger);

            return logger.returnCode;
        }

        private static void WriteLogItems(igenmsEntities context, AggregationObject aggregationObject)
        {
            try
            {
                context.Database.CommandTimeout = configuration.CommandTimeout;

                foreach (Logger logItem in aggregationObject.errors)
                {
                    if (logItem != null)
                    {
                        if (logItem.returnCode == 0 && logItem.logType == Log_Type.Acticity)
                        {
                            tbLogActivity log = new tbLogActivity();
                            log.methodName = logItem.methodName;
                            log.parameters = logItem.parameters;
                            log.timestamp = logItem.timestamp;
                            log.jobId = logItem.jobId;
                            try
                            {
                                context.Entry(log).State = System.Data.Entity.EntityState.Added;
                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        else if (logItem.returnCode == 0 && logItem.logType == Log_Type.Info && configuration.Log_ElapsedTime)
                        {
                            tbLogError log = new tbLogError();
                            log.methodName = logItem.methodName;
                            log.parameters = logItem.parameters;
                            log.timestamp = logItem.timestamp;
                            log.jobId = logItem.jobId;
                            log.elapsedTime = logItem.elapsedTime;
                            log.exceptionMessage = "Elapsed time for " + logItem.methodName;
                            try
                            {
                                context.Entry(log).State = System.Data.Entity.EntityState.Added;
                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        else
                        {
                            tbLogError log = new tbLogError();
                            log.methodName = logItem.methodName;
                            log.parameters = logItem.parameters;
                            log.elapsedTime = logItem.elapsedTime;
                            log.jobId = logItem.jobId;
                            log.exceptionMessage = (logItem.exceptionMessage == null) ? "" : logItem.exceptionMessage;
                            log.innerException = logItem.innerException;
                            log.returnCode = logItem.returnCode;
                            log.timestamp = logItem.timestamp;
                            log.stackTrace = logItem.stackTrace;
                            try
                            {
                                context.Entry(log).State = System.Data.Entity.EntityState.Added;
                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void WriteToFile(long? jobId, string message, bool isError = false)
        {
            string folderName = @"\" + ((isError) ? Helper.ErrorLogs : Helper.Logs);
            string fileName = (isError) ? "JobId_" + jobId.ToString() : "Log_" + System.DateTime.Now.ToString("yyyy-MM-dd");
            if (jobId == null)
            {
                fileName = "Job_" + System.DateTime.Now.ToString("yyyy-MM-dd");
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + folderName;
            string filepath = AppDomain.CurrentDomain.BaseDirectory + folderName + "\\" + fileName + ".txt";

            string errMessage = "_____________________________________________" + '\n';
            errMessage += "(" + System.DateTime.Now.ToString() + ") " + '\n';
            errMessage += message + '\n';
            errMessage += "_____________________________________________";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(errMessage);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(errMessage);
                }
            }
        }
    }
}