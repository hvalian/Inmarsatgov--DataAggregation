using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public class AggregationObject
    {
        public long jobId { get; set; }
        public long ?parentJobId { get; set; }
        public List<Logger> errors { get; set; }
    }
}
