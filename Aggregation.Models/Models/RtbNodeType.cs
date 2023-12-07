using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class RtbNodeType
    {
        public int Id { get; set; }

        public int NodeTypeId { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

        public string Config { get; set; }

        public int DefaultValue { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}
