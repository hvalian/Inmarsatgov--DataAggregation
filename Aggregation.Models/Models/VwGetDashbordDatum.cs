using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwGetDashbordDatum
    {
        public int? AverageProcessingTimeHourlyJob { get; set; }

        public int? AverageProcessingTimeDailyJob { get; set; }

        public bool? JobProcessingHasIssues { get; set; }

        public bool? JobProcessingIsSuspended { get; set; }

        public DateTime? LastErrJobAggregationDateTime { get; set; }

        public int? LastErrJobElapsedtime { get; set; }

        public int? LastErrJobId { get; set; }

        public string LastErrJobInterval { get; set; }

        public DateTime? LastErrJobStartDateTime { get; set; }

        public DateTime? LastErrJobEndDateTime { get; set; }

        public string LastErrJobStatus { get; set; }

        public DateTime? LastJobAggregationDateTime { get; set; }

        public int? LastJobElapsedtime { get; set; }

        public int? LastJobId { get; set; }

        public string LastJobInterval { get; set; }

        public DateTime? LastJobStartDateTime { get; set; }

        public DateTime? LastJobEndDateTime { get; set; }

        public string LastJobStatus { get; set; }

        public DateTime? NextJobAggregationDateTime { get; set; }

        public int? NextJobId { get; set; }

        public string NextJobInterval { get; set; }

        public DateTime? NextJobStartAfterDateTime { get; set; }

        public DateTime? NextJobStartDateTime { get; set; }

        public string NextJobStatus { get; set; }

        public int? NextJobWillStartInMinutes { get; set; }

        public int? CommandTimeout { get; set; }

        public int? JobStartTimeDelay { get; set; }

        public DateTime? JobSuspendProcessingAfter { get; set; }

        public int? NumberOfDaysForStats { get; set; }

        public int? NumberOfRefresh { get; set; }

        public string ProjectName { get; set; }

        public bool? RefreshEnabled { get; set; }

        public int? RefreshInterval { get; set; }

        public int? RetentionActivityLogNumberOfHours { get; set; }

        public int? SpCommandTimeout { get; set; }
    }
}