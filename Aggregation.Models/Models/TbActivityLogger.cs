using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbActivityLogger
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ActivityId { get; set; }

        public int? ConfigurationId { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public DateTime Timestamp { get; set; }

        public virtual TbActivity Activity { get; set; }

        public virtual TbConfiguration Configuration { get; set; }

        public virtual TbUser User { get; set; }
    }
}