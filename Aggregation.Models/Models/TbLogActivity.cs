using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbLogActivity
    {
        public long Id { get; set; }

        public long JobId { get; set; }

        public string MethodName { get; set; }

        public DateTime? Timestamp { get; set; }

        public long ElapsedTime { get; set; }

        public string Parameters { get; set; }

        public virtual TbJob Job { get; set; }
    }
}