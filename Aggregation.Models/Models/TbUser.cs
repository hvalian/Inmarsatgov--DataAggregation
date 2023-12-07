using System;
using System.Collections.Generic;

namespace Aggregation_DataModels.Models
{
    public partial class TbUser
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public bool HasAdminAccess { get; set; }

        public bool HasAccessToConfiguration { get; set; }

        public bool HasAccessToQueue { get; set; }

        public bool HasAccessToRefreshJob { get; set; }

        public bool HasAccessToUpdateClock { get; set; }

        public virtual ICollection<TbActivityLogger> TbActivityLoggers { get; set; } = new List<TbActivityLogger>();
    }
}