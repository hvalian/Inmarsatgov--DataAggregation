using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwNodeType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public string Config { get; set; }
    }
}