//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aggregation_DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class vwNodeMetricValue
    {
        public long id { get; set; }
        public int nodeId { get; set; }
        public int nodeMetricId { get; set; }
        public int nodeTypeId { get; set; }
        public string metricKey { get; set; }
        public string metricValueType { get; set; }
        public System.DateTime timestamp { get; set; }
        public string value { get; set; }
        public string metricvalue1 { get; set; }
        public string metricvalue2 { get; set; }
        public string metricvalue3 { get; set; }
        public Nullable<long> epochTimestamp { get; set; }
        public long lastPolledTimestamp { get; set; }
        public Nullable<long> backFillMetricValueID { get; set; }
        public Nullable<bool> isbackFilled { get; set; }
    }
}