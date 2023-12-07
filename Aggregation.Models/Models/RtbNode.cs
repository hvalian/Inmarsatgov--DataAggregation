using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class RtbNode
    {
        public int Id { get; set; }

        public int NodeId { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public int NodeTypeId { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime? ActivationDate { get; set; }

        public DateTime? DeactivationDate { get; set; }
    }
}
