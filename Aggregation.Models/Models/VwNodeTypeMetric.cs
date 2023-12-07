using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwNodeTypeMetric
    {
        public int Id { get; set; }

        public int NodeTypeId { get; set; }

        public string MetricKey { get; set; }

        public string Name { get; set; }

        public string MetricValueType { get; set; }

        public string SampleValue { get; set; }

        public string Description { get; set; }

        public string MetricValue1Label { get; set; }

        public string MetricValue2Label { get; set; }

        public string MetricValue3Label { get; set; }

        public int PollingInterval { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public string MetricSourceInfo { get; set; }

        public string MetricUnits { get; set; }

        public string RollUpAggregateConfig { get; set; }

        public string IsCalculatedMetric { get; set; }

        public string DisplayName { get; set; }

        public string Config { get; set; }

        public string PreFillValueConfig { get; set; }
    }
}