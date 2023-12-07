using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbNodeMetricValue5minute
    {
        public long Id { get; set; }

        public int NodeId { get; set; }

        public int NodeMetricId { get; set; }

        public int NodeTypeId { get; set; }

        public string MetricKey { get; set; }

        public string MetricValueType { get; set; }

        public DateTime Timestamp { get; set; }

        public string Value { get; set; }

        public bool UsePreviousValue { get; set; }

        public bool OriginalStatus { get; set; }

        public DateTime UpdatedDate { get; set; }

        public double? Average { get; set; }

        public double? Median { get; set; }

        public double? Mode { get; set; }

        public double? StdDev { get; set; }

        public double? _95pct { get; set; }

        public double? Sum { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }

        public int? Count { get; set; }

        public int? MaxCount { get; set; }

        public string First { get; set; }

        public string Last { get; set; }
    }
}