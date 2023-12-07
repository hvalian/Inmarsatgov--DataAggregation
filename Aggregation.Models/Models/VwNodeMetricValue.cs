using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwNodeMetricValue
    {
        public long Id { get; set; }

        public int NodeId { get; set; }

        public int NodeMetricId { get; set; }

        public int NodeTypeId { get; set; }

        public string MetricKey { get; set; }

        public string MetricValueType { get; set; }

        public DateTime Timestamp { get; set; }

        public string Value { get; set; }

        public string Metricvalue1 { get; set; }

        public string Metricvalue2 { get; set; }

        public string Metricvalue3 { get; set; }

        public long? EpochTimestamp { get; set; }

        public long LastPolledTimestamp { get; set; }

        public long? BackFillMetricValueId { get; set; }

        public bool? IsbackFilled { get; set; }
    }
}