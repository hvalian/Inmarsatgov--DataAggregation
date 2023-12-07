using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbLogError
    {
        public long Id { get; set; }

        public long JobId { get; set; }

        public string MethodName { get; set; }

        public string ExceptionMessage { get; set; }

        public string InnerException { get; set; }

        public int? ReturnCode { get; set; }

        public long ElapsedTime { get; set; }

        public DateTime? Timestamp { get; set; }

        public string StackTrace { get; set; }

        public string Parameters { get; set; }

        public virtual TbJob Job { get; set; }
    }
}