using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbNodeStatusConfig
    {
        public int Id { get; set; }

        public int NodeTypeId { get; set; }

        public string MetricKey { get; set; }

        public bool Include { get; set; }
    }
}