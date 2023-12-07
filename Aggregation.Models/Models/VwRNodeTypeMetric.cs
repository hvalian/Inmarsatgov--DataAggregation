using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwRnodeTypeMetric
    {
        public int Id { get; set; }

        public int NodeTypeId { get; set; }

        public string NodeTypeDescription { get; set; }

        public string MetricKey { get; set; }

        public string MetricKeyDescription { get; set; }

        public string MetricValueType { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public string DefaultValue { get; set; }

        public bool UsePreviousValue { get; set; }
    }
}