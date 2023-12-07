using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregation_DataModels.Models
{
    public partial class AggregationJobDetails
    {
        public DateTime AggregationStartDateTime { get; set; }

        public DateTime AggregationEndDateTime { get; set; }

        public ICollection<TbJob> TbJobs { get; set; } = new List<TbJob>();
    }
}