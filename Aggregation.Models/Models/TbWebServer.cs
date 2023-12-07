using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbWebServer
    {
        public int Id { get; set; }

        public string ServerName { get; set; }

        public string ServerIp { get; set; }

        public bool Active { get; set; }

        public string InstanceName { get; set; }
    }
}