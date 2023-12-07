using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbConfigurationByDate
    {
        public int Id { get; set; }

        public DateTime AggregationDate { get; set; }

        public bool RefreshEnabled { get; set; }

        public int RefreshNumberOfDays { get; set; }

        public int RefreshInterval { get; set; }

        public int JobStartTimeDelay { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}