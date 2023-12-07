using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class VwNode
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public int NodeTypeId { get; set; }

        public int ConnectorId { get; set; }

        public string ConnectorCreds { get; set; }

        public string SourceInfo { get; set; }

        public int PollerId { get; set; }

        public bool UseMetricTemplate { get; set; }

        public int? NodeTypeTemplateId { get; set; }

        public int PollingInterval { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public string PollPolicy { get; set; }

        public DateTime? LastPollDate { get; set; }

        public DateTime? NextPollDate { get; set; }

        public string NodeData { get; set; }

        public string ProcessorName { get; set; }

        public DateTime? ActivationDate { get; set; }

        public DateTime? DeactivationDate { get; set; }
    }
}