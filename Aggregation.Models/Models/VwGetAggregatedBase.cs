using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwGetAggregatedBase
    {
        public int NodeTypeId { get; set; }

        public string MetricKey { get; set; }

        public int Nodeid { get; set; }

        public DateTime? Reportingtimestamp { get; set; }

        public double? Average { get; set; }

        public double? Sum { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }

        public int? Count { get; set; }

        public int? First { get; set; }

        public int? Last { get; set; }
    }
}