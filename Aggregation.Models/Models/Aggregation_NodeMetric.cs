namespace Aggregation_DataModels.Models
{
    public class Aggregation_NodeMetric
    {
        public int Id { get; set; }
        public int NodeTypeId { get; set; }
        public string MetricKey { get; set; }
        public string MetricValueType { get; set; }
        public string rollupinterval { get; set; }
        public string defaultaggregatetype { get; set; }
        public string aggregatetype { get; set; }
     }
}
