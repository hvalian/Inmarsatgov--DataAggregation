using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwGetPreviousNodeStatusValue
    {
        public long Id { get; set; }

        public int? ElapsedTime { get; set; }

        public int Defaultvalue { get; set; }
    }
}