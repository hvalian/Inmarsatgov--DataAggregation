using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbInterval
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public virtual ICollection<TbJob> TbJobs { get; set; } = new List<TbJob>();
    }
}