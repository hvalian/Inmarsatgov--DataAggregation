using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwGetAggregatedMedian
    {
        public int NodeTypeId { get; set; }

        public int NodeId { get; set; }

        public string MetricKey { get; set; }

        public DateTime? Reportingtimestamp { get; set; }

        public double? Median { get; set; }
    }
}