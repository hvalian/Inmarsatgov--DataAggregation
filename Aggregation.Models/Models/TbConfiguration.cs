using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbConfiguration
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public bool? ReadOnly { get; set; }

        public bool? Immediate { get; set; }

        public virtual ICollection<TbActivityLogger> TbActivityLoggers { get; set; } = new List<TbActivityLogger>();
    }
}