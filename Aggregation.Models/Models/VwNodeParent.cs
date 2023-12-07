using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwNodeParent
    {
        public int Id { get; set; }

        public int NodeId { get; set; }

        public int ParentnodeId { get; set; }

        public bool Active { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}