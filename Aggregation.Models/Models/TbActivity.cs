using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbActivity
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public virtual ICollection<TbActivityLogger> TbActivityLoggers { get; set; } = new List<TbActivityLogger>();
    }
}