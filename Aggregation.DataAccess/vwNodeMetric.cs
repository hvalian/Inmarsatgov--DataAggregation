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
    
    public partial class vwNodeMetric
    {
        public int id { get; set; }
        public int nodeId { get; set; }
        public int nodeTypeId { get; set; }
        public string metricKey { get; set; }
        public string metricValueType { get; set; }
        public string sampleValue { get; set; }
        public string description { get; set; }
        public string metricValue1Label { get; set; }
        public string metricValue2Label { get; set; }
        public string metricValue3Label { get; set; }
        public bool active { get; set; }
        public int pollingInterval { get; set; }
        public System.DateTime dateCreated { get; set; }
        public System.DateTime dateUpdated { get; set; }
        public string metricSourceInfo { get; set; }
        public string name { get; set; }
        public string metricUnits { get; set; }
        public string nodeMetricData { get; set; }
    }
}
