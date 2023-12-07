using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbRawDataDaily
    {
        public int Id { get; set; }

        public int NodeId { get; set; }

        public int NodeTypeId { get; set; }

        public string MetricKey { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime? Reportingtimestamp { get; set; }

        public string Value { get; set; }

        public bool? IsbackFilled { get; set; }
    }
}