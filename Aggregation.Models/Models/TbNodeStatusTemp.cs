using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbNodeStatusTemp
    {
        public int NodeId { get; set; }

        public int NodeTypeId { get; set; }

        public bool Value { get; set; }

        public string Average { get; set; }
    }
}