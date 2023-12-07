using System;
using System.Globalization;

namespace Aggregation_DataModels.Models
{
    public class ConfigObject
    {
        public int CommandTimeout { get; set; }
        public bool Disabled { get; set; }
        public int Job_StartTimeDelay { get; set; }
        public DateTime Job_SuspendProcessingAfter { get; set; }
        public DateTime LastHourlyJob_Scheduled_DateTime { get; set; }
        public string Node_Status { get; set; }
        public int NumberOfRefresh { get; set; }
        public string ProjectName { get; set; }
        public bool Refresh_Enabled { get; set; }
        public int Refresh_Interval { get; set; }
        public int ReRun_StartTimeDelay { get; set; }
        public int Retention_ActivityLog_NumberOfHours { get; set; }
        public int SP_CommandTimeout { get; set; }

        public ConfigObject() {
            CommandTimeout = 300;
            Disabled = false;
            Job_StartTimeDelay = 125;
            Job_SuspendProcessingAfter = DateTime.ParseExact("2023/12/31 23:59:59", "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
            Node_Status = "9999";
            NumberOfRefresh = 2;
            ProjectName = "";
            Refresh_Enabled = false;
            Refresh_Interval = 1;
            ReRun_StartTimeDelay = 30;
            Retention_ActivityLog_NumberOfHours = 72;
            SP_CommandTimeout = 1500;
        }
    }
}
