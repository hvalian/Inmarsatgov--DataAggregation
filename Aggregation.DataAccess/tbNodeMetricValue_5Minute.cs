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
    
    public partial class tbNodeMetricValue_5Minute
    {
        public long id { get; set; }
        public int nodeId { get; set; }
        public int nodeMetricId { get; set; }
        public int nodeTypeId { get; set; }
        public string metricKey { get; set; }
        public string metricValueType { get; set; }
        public System.DateTime timestamp { get; set; }
        public string value { get; set; }
        public bool usePreviousValue { get; set; }
        public bool originalStatus { get; set; }
        public System.DateTime updatedDate { get; set; }
        public Nullable<double> average { get; set; }
        public Nullable<double> median { get; set; }
        public Nullable<double> mode { get; set; }
        public Nullable<double> stdDev { get; set; }
        public Nullable<double> C95pct { get; set; }
        public Nullable<double> sum { get; set; }
        public Nullable<double> min { get; set; }
        public Nullable<double> max { get; set; }
        public Nullable<int> count { get; set; }
        public Nullable<int> maxCount { get; set; }
        public string first { get; set; }
        public string last { get; set; }
    }
}
