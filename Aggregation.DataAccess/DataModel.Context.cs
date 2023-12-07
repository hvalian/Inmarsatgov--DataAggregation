﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class igenmsEntities : DbContext
    {
        public igenmsEntities()
            : base("name=igenmsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<rtbNode> rtbNodes { get; set; }
        public virtual DbSet<rtbNodeMetric> rtbNodeMetrics { get; set; }
        public virtual DbSet<rtbNodeParent> rtbNodeParents { get; set; }
        public virtual DbSet<rtbNodeType> rtbNodeTypes { get; set; }
        public virtual DbSet<rtbNodeTypeMetric> rtbNodeTypeMetrics { get; set; }
        public virtual DbSet<tbActivity> tbActivities { get; set; }
        public virtual DbSet<tbActivityLogger> tbActivityLoggers { get; set; }
        public virtual DbSet<tbConfiguration> tbConfigurations { get; set; }
        public virtual DbSet<tbConfigurationByDate> tbConfigurationByDates { get; set; }
        public virtual DbSet<tbInterval> tbIntervals { get; set; }
        public virtual DbSet<tbJob> tbJobs { get; set; }
        public virtual DbSet<tbJobType> tbJobTypes { get; set; }
        public virtual DbSet<tbLogActivity> tbLogActivities { get; set; }
        public virtual DbSet<tbLogError> tbLogErrors { get; set; }
        public virtual DbSet<tbNodeStatusConfig> tbNodeStatusConfigs { get; set; }
        public virtual DbSet<tbNodeStatusTemp> tbNodeStatusTemps { get; set; }
        public virtual DbSet<tbObservation> tbObservations { get; set; }
        public virtual DbSet<tbObservationsTemplate> tbObservationsTemplates { get; set; }
        public virtual DbSet<tbPriority> tbPriorities { get; set; }
        public virtual DbSet<tbRawData> tbRawDatas { get; set; }
        public virtual DbSet<tbRawDataDaily> tbRawDataDailies { get; set; }
        public virtual DbSet<tbStatu> tbStatus { get; set; }
        public virtual DbSet<tbUser> tbUsers { get; set; }
        public virtual DbSet<tbWebServer> tbWebServers { get; set; }
        public virtual DbSet<tbNodeMetricValue_5Minute> tbNodeMetricValue_5Minute { get; set; }
        public virtual DbSet<tbNodeMetricValue_Day> tbNodeMetricValue_Day { get; set; }
        public virtual DbSet<tbNodeMetricValue_Hour> tbNodeMetricValue_Hour { get; set; }
        public virtual DbSet<tbNodeStatu> tbNodeStatus { get; set; }
        public virtual DbSet<vwGetAggregatedBase> vwGetAggregatedBases { get; set; }
        public virtual DbSet<vwGetAggregatedMedian> vwGetAggregatedMedians { get; set; }
        public virtual DbSet<vwGetAggregatedPercentile> vwGetAggregatedPercentiles { get; set; }
        public virtual DbSet<vwGetAggregatedStdDev> vwGetAggregatedStdDevs { get; set; }
        public virtual DbSet<vwGetChildNodeStatu> vwGetChildNodeStatus { get; set; }
        public virtual DbSet<vwGetJob> vwGetJobs { get; set; }
        public virtual DbSet<vwGetPreviousNodeStatusValue> vwGetPreviousNodeStatusValues { get; set; }
        public virtual DbSet<vwNode> vwNodes { get; set; }
        public virtual DbSet<vwNodeMetric> vwNodeMetrics { get; set; }
        public virtual DbSet<vwNodeMetricValue> vwNodeMetricValues { get; set; }
        public virtual DbSet<vwNodeParent> vwNodeParents { get; set; }
        public virtual DbSet<vwNodeType> vwNodeTypes { get; set; }
        public virtual DbSet<vwNodeTypeMetric> vwNodeTypeMetrics { get; set; }
        public virtual DbSet<vwRNodeTypeMetric> vwRNodeTypeMetrics { get; set; }
    
        [DbFunction("igenmsEntities", "fnGetMetricKeys")]
        public virtual IQueryable<string> fnGetMetricKeys(Nullable<int> nodeTypeId)
        {
            var nodeTypeIdParameter = nodeTypeId.HasValue ?
                new ObjectParameter("nodeTypeId", nodeTypeId) :
                new ObjectParameter("nodeTypeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<string>("[igenmsEntities].[fnGetMetricKeys](@nodeTypeId)", nodeTypeIdParameter);
        }
    
        [DbFunction("igenmsEntities", "fnGetStats")]
        public virtual IQueryable<fnGetStats_Result> fnGetStats()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fnGetStats_Result>("[igenmsEntities].[fnGetStats]()");
        }
    
        [DbFunction("igenmsEntities", "fnStringList2Table")]
        public virtual IQueryable<Nullable<int>> fnStringList2Table(string list)
        {
            var listParameter = list != null ?
                new ObjectParameter("List", list) :
                new ObjectParameter("List", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<Nullable<int>>("[igenmsEntities].[fnStringList2Table](@List)", listParameter);
        }
    
        public virtual int spCleanRefTables()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spCleanRefTables");
        }
    
        public virtual int spCleanUpDaily(Nullable<long> jobId)
        {
            var jobIdParameter = jobId.HasValue ?
                new ObjectParameter("jobId", jobId) :
                new ObjectParameter("jobId", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spCleanUpDaily", jobIdParameter);
        }
    
        public virtual int spCleanUpHourly(Nullable<long> jobId)
        {
            var jobIdParameter = jobId.HasValue ?
                new ObjectParameter("jobId", jobId) :
                new ObjectParameter("jobId", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spCleanUpHourly", jobIdParameter);
        }
    
        public virtual int spMergeRefTables()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spMergeRefTables");
        }
    
        public virtual int spPopulate5Minute(Nullable<long> jobId, Nullable<System.DateTime> fromTimeStamp, Nullable<System.DateTime> toTimeStamp)
        {
            var jobIdParameter = jobId.HasValue ?
                new ObjectParameter("jobId", jobId) :
                new ObjectParameter("jobId", typeof(long));
    
            var fromTimeStampParameter = fromTimeStamp.HasValue ?
                new ObjectParameter("fromTimeStamp", fromTimeStamp) :
                new ObjectParameter("fromTimeStamp", typeof(System.DateTime));
    
            var toTimeStampParameter = toTimeStamp.HasValue ?
                new ObjectParameter("toTimeStamp", toTimeStamp) :
                new ObjectParameter("toTimeStamp", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spPopulate5Minute", jobIdParameter, fromTimeStampParameter, toTimeStampParameter);
        }
    
        public virtual int spPopulateDaily(Nullable<System.DateTime> fromTimeStamp, Nullable<System.DateTime> toTimeStamp)
        {
            var fromTimeStampParameter = fromTimeStamp.HasValue ?
                new ObjectParameter("fromTimeStamp", fromTimeStamp) :
                new ObjectParameter("fromTimeStamp", typeof(System.DateTime));
    
            var toTimeStampParameter = toTimeStamp.HasValue ?
                new ObjectParameter("toTimeStamp", toTimeStamp) :
                new ObjectParameter("toTimeStamp", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spPopulateDaily", fromTimeStampParameter, toTimeStampParameter);
        }
    
        public virtual int spPopulateHourly(Nullable<System.DateTime> fromTimeStamp, Nullable<System.DateTime> toTimeStamp)
        {
            var fromTimeStampParameter = fromTimeStamp.HasValue ?
                new ObjectParameter("fromTimeStamp", fromTimeStamp) :
                new ObjectParameter("fromTimeStamp", typeof(System.DateTime));
    
            var toTimeStampParameter = toTimeStamp.HasValue ?
                new ObjectParameter("toTimeStamp", toTimeStamp) :
                new ObjectParameter("toTimeStamp", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spPopulateHourly", fromTimeStampParameter, toTimeStampParameter);
        }
    
        public virtual int spPopulateNodeStatus(Nullable<long> jobId, Nullable<System.DateTime> fromTimeStamp, Nullable<System.DateTime> toTimeStamp)
        {
            var jobIdParameter = jobId.HasValue ?
                new ObjectParameter("jobId", jobId) :
                new ObjectParameter("jobId", typeof(long));
    
            var fromTimeStampParameter = fromTimeStamp.HasValue ?
                new ObjectParameter("fromTimeStamp", fromTimeStamp) :
                new ObjectParameter("fromTimeStamp", typeof(System.DateTime));
    
            var toTimeStampParameter = toTimeStamp.HasValue ?
                new ObjectParameter("toTimeStamp", toTimeStamp) :
                new ObjectParameter("toTimeStamp", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spPopulateNodeStatus", jobIdParameter, fromTimeStampParameter, toTimeStampParameter);
        }
    
        public virtual int spPopulateStaging(Nullable<long> jobId, Nullable<System.DateTime> fromTimeStamp, Nullable<System.DateTime> toTimeStamp)
        {
            var jobIdParameter = jobId.HasValue ?
                new ObjectParameter("jobId", jobId) :
                new ObjectParameter("jobId", typeof(long));
    
            var fromTimeStampParameter = fromTimeStamp.HasValue ?
                new ObjectParameter("fromTimeStamp", fromTimeStamp) :
                new ObjectParameter("fromTimeStamp", typeof(System.DateTime));
    
            var toTimeStampParameter = toTimeStamp.HasValue ?
                new ObjectParameter("toTimeStamp", toTimeStamp) :
                new ObjectParameter("toTimeStamp", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spPopulateStaging", jobIdParameter, fromTimeStampParameter, toTimeStampParameter);
        }
    }
}