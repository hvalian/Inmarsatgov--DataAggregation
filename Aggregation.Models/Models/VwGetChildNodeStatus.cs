using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwGetChildNodeStatus
    {
        public int NodeId { get; set; }

        public DateTime Timestamp { get; set; }

        public bool Status { get; set; }
    }
}