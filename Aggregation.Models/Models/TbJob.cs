using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbJob
    {
        public long Id { get; set; }

        public int JobType { get; set; }

        public string ComputerName { get; set; }

        public int Status { get; set; }

        public int? Interval { get; set; }

        public int Priority { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public DateTime StartAfterDateTime { get; set; }

        public int? ProcessId { get; set; }

        public DateTime AggregationStartDateTime { get; set; }

        public DateTime AggregationEndDateTime { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public long? ParentJobId { get; set; }

        public int? Processed { get; set; }

        public bool IsRecovery { get; set; }

        public string CreatedBy { get; set; }

        public int? ExitCode { get; set; }

        public virtual TbInterval IntervalNavigation { get; set; }

        public virtual TbJobType JobTypeNavigation { get; set; }

        public virtual TbPriority PriorityNavigation { get; set; }

        public virtual TbStatus StatusNavigation { get; set; }

        public virtual ICollection<TbLogActivity> TbLogActivities { get; set; } = new List<TbLogActivity>();

        public virtual ICollection<TbLogError> TbLogErrors { get; set; } = new List<TbLogError>();
    }
}