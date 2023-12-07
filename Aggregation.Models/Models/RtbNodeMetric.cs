using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class RtbNodeMetric
    {
        public int Id { get; set; }

        public int NodeId { get; set; }

        public int? NodeMetricId { get; set; }

        public int NodeTypeId { get; set; }

        public string MetricKey { get; set; }

        public string Description { get; set; }

        public string MetricValueType { get; set; }

        public bool Active { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}