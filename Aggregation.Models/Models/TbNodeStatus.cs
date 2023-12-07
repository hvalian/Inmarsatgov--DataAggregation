using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbNodeStatus
    {
        public long Id { get; set; }

        public int NodeId { get; set; }

        public int NodeTypeId { get; set; }

        public DateTime Timestamp { get; set; }

        public bool Status { get; set; }

        public bool OriginalStatus { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}