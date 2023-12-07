using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwGetJob
    {
        public long Id { get; set; }

        public string JobType { get; set; }

        public string ComputerName { get; set; }

        public int StatusId { get; set; }

        public string Status { get; set; }

        public int? IntervalId { get; set; }

        public string Interval { get; set; }

        public int PriorityId { get; set; }

        public string Priority { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime StartAfterDateTime { get; set; }

        public int? ProcessId { get; set; }

        public DateTime AggregationStartDateTime { get; set; }

        public DateTime AggregationEndDateTime { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int? Processed { get; set; }

        public bool IsRecovery { get; set; }

        public string CreatedBy { get; set; }

        public int? ExitCode { get; set; }

        public string ListOfJobs { get; set; }
    }
}