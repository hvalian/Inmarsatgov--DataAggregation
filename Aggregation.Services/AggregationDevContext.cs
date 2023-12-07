using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Website.Models;

namespace Website.Data;

public partial class AggregationDevContext : DbContext
{
    public AggregationDevContext()
    {
    }

    public AggregationDevContext(DbContextOptions<AggregationDevContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbConfiguration> TbConfigurations { get; set; }

    public virtual DbSet<TbConfigurationByDate> TbConfigurationByDates { get; set; }

    public virtual DbSet<TbInterval> TbIntervals { get; set; }

    public virtual DbSet<TbJob> TbJobs { get; set; }

    public virtual DbSet<TbJobType> TbJobTypes { get; set; }

    public virtual DbSet<TbLogActivity> TbLogActivities { get; set; }

    public virtual DbSet<TbLogError> TbLogErrors { get; set; }

    public virtual DbSet<TbMetricDefaultValue> TbMetricDefaultValues { get; set; }

    public virtual DbSet<TbNodeMetricValueDay> TbNodeMetricValueDays { get; set; }

    public virtual DbSet<TbNodeMetricValueHour> TbNodeMetricValueHours { get; set; }

    public virtual DbSet<TbNodeStatus> TbNodeStatuses { get; set; }

    public virtual DbSet<TbNodeStatusConfig> TbNodeStatusConfigs { get; set; }

    public virtual DbSet<TbNodeType> TbNodeTypes { get; set; }

    public virtual DbSet<TbObservation> TbObservations { get; set; }

    public virtual DbSet<TbPriority> TbPriorities { get; set; }

    public virtual DbSet<TbRawDatum> TbRawData { get; set; }

    public virtual DbSet<TbStatus> TbStatuses { get; set; }

    public virtual DbSet<Tbnodemetricvalue5minute> Tbnodemetricvalue5minutes { get; set; }

    public virtual DbSet<VwChildNodeStatus> VwChildNodeStatuses { get; set; }

    public virtual DbSet<VwGetAggregatedBase> VwGetAggregatedBases { get; set; }

    public virtual DbSet<VwGetAggregatedMedian> VwGetAggregatedMedians { get; set; }

    public virtual DbSet<VwGetAggregatedPercentile> VwGetAggregatedPercentiles { get; set; }

    public virtual DbSet<VwGetAggregatedStdDev> VwGetAggregatedStdDevs { get; set; }

    public virtual DbSet<VwGetPreviousNodeStatusValue> VwGetPreviousNodeStatusValues { get; set; }

    public virtual DbSet<VwNode> VwNodes { get; set; }

    public virtual DbSet<VwNodeMetric> VwNodeMetrics { get; set; }

    public virtual DbSet<VwNodeMetricValue> VwNodeMetricValues { get; set; }

    public virtual DbSet<VwNodeParent> VwNodeParents { get; set; }

    public virtual DbSet<VwNodeType> VwNodeTypes { get; set; }

    public virtual DbSet<VwNodeTypeMetric> VwNodeTypeMetrics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:AggregationContext");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbConfiguration>(entity =>
        {
            entity.ToTable("tbConfiguration", "aggregation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("key");
            entity.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("value");
        });

        modelBuilder.Entity<TbConfigurationByDate>(entity =>
        {
            entity.ToTable("tbConfigurationByDate", "aggregation");

            entity.HasIndex(e => e.AggregationDate, "UN_aggregationDate_tbConfigurationByDate").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AggregationDate)
                .HasColumnType("datetime")
                .HasColumnName("aggregationDate");
            entity.Property(e => e.CreatedDateTime)
                .HasColumnType("datetime")
                .HasColumnName("createdDateTime");
            entity.Property(e => e.JobStartTimeDelay).HasColumnName("Job_StartTimeDelay");
            entity.Property(e => e.RefreshDailyJobOnce).HasColumnName("Refresh_DailyJobOnce");
            entity.Property(e => e.RefreshEnabled).HasColumnName("Refresh_Enabled");
            entity.Property(e => e.RefreshInterval).HasColumnName("Refresh_Interval");
            entity.Property(e => e.RefreshNumberOfDays).HasColumnName("Refresh_NumberOfDays");
        });

        modelBuilder.Entity<TbInterval>(entity =>
        {
            entity.ToTable("tbInterval", "aggregation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("description");
        });

        modelBuilder.Entity<TbJob>(entity =>
        {
            entity.ToTable("tbJob", "aggregation");

            entity.HasIndex(e => new { e.Id, e.Interval, e.AggregationStartDateTime, e.AggregationEndDateTime }, "UN_Id_Inverval_aggregationStartDateTime_aggregationEndDateTime_tbaggregation").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AggregationEndDateTime)
                .HasColumnType("datetime")
                .HasColumnName("aggregationEndDateTime");
            entity.Property(e => e.AggregationStartDateTime)
                .HasColumnType("datetime")
                .HasColumnName("aggregationStartDateTime");
            entity.Property(e => e.ComputerName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("computerName");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("createdBy");
            entity.Property(e => e.CreatedDateTime)
                .HasColumnType("datetime")
                .HasColumnName("createdDateTime");
            entity.Property(e => e.EndDateTime)
                .HasColumnType("datetime")
                .HasColumnName("endDateTime");
            entity.Property(e => e.ExitCode).HasColumnName("exitCode");
            entity.Property(e => e.Interval).HasColumnName("interval");
            entity.Property(e => e.IsRecovery).HasColumnName("isRecovery");
            entity.Property(e => e.JobType).HasColumnName("jobType");
            entity.Property(e => e.ParentJobId).HasColumnName("parentJobId");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.ProcessId).HasColumnName("processId");
            entity.Property(e => e.Processed).HasColumnName("processed");
            entity.Property(e => e.StartAfterDateTime)
                .HasColumnType("datetime")
                .HasColumnName("startAfterDateTime");
            entity.Property(e => e.StartDateTime)
                .HasColumnType("datetime")
                .HasColumnName("startDateTime");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.IntervalNavigation).WithMany(p => p.TbJobs)
                .HasForeignKey(d => d.Interval)
                .HasConstraintName("fk_tbJob_interval");

            entity.HasOne(d => d.JobTypeNavigation).WithMany(p => p.TbJobs)
                .HasForeignKey(d => d.JobType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tbJob_JobType");

            entity.HasOne(d => d.PriorityNavigation).WithMany(p => p.TbJobs)
                .HasForeignKey(d => d.Priority)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tbJob_priority");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.TbJobs)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tbJob_status");
        });

        modelBuilder.Entity<TbJobType>(entity =>
        {
            entity.ToTable("tbJobType", "aggregation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("description");
        });

        modelBuilder.Entity<TbLogActivity>(entity =>
        {
            entity.ToTable("tbLogActivity", "aggregation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JobId).HasColumnName("jobId");
            entity.Property(e => e.MethodName)
                .IsRequired()
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("methodName");
            entity.Property(e => e.Parameters)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("parameters");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<TbLogError>(entity =>
        {
            entity.ToTable("tbLogError", "aggregation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ElapsedTime).HasColumnName("elapsedTime");
            entity.Property(e => e.ExceptionMessage)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("exceptionMessage");
            entity.Property(e => e.InnerException)
                .IsUnicode(false)
                .HasColumnName("innerException");
            entity.Property(e => e.JobId).HasColumnName("jobId");
            entity.Property(e => e.MethodName)
                .IsRequired()
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("methodName");
            entity.Property(e => e.NumberOfRecords).HasColumnName("numberOfRecords");
            entity.Property(e => e.Parameters)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("parameters");
            entity.Property(e => e.ReturnCode).HasColumnName("returnCode");
            entity.Property(e => e.SortOrder).HasColumnName("sortOrder");
            entity.Property(e => e.StackTrace)
                .IsUnicode(false)
                .HasColumnName("stackTrace");
            entity.Property(e => e.TaskId).HasColumnName("taskId");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<TbMetricDefaultValue>(entity =>
        {
            entity.ToTable("tbMetricDefaultValue", "aggregation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DefaultValue)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("defaultValue");
            entity.Property(e => e.MetricKey)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.UsePreviousValue).HasColumnName("usePreviousValue");
        });

        modelBuilder.Entity<TbNodeMetricValueDay>(entity =>
        {
            entity.ToTable("tbNodeMetricValue_Day", "summary");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Average).HasColumnName("average");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.EpochTimestamp).HasColumnName("epochTimestamp");
            entity.Property(e => e.First)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("first");
            entity.Property(e => e.Last)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("last");
            entity.Property(e => e.Max).HasColumnName("max");
            entity.Property(e => e.MaxCount).HasColumnName("maxCount");
            entity.Property(e => e.Median).HasColumnName("median");
            entity.Property(e => e.MetricKey)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.MetricValueType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metricValueType");
            entity.Property(e => e.Min).HasColumnName("min");
            entity.Property(e => e.Mode).HasColumnName("mode");
            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.NodeMetricId).HasColumnName("nodeMetricId");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.StdDev).HasColumnName("stdDev");
            entity.Property(e => e.Sum).HasColumnName("sum");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updatedDate");
            entity.Property(e => e.Value)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("value");
            entity.Property(e => e.ValueType)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("valueType");
            entity.Property(e => e._95pct).HasColumnName("95pct");
        });

        modelBuilder.Entity<TbNodeMetricValueHour>(entity =>
        {
            entity.HasKey(e => new { e.NodeId, e.NodeTypeId, e.MetricKey, e.Timestamp });

            entity.ToTable("tbNodeMetricValue_Hour", "summary");

            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.MetricKey)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.Average).HasColumnName("average");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.EpochTimestamp).HasColumnName("epochTimestamp");
            entity.Property(e => e.First)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("first");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Last)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("last");
            entity.Property(e => e.Max).HasColumnName("max");
            entity.Property(e => e.MaxCount).HasColumnName("maxCount");
            entity.Property(e => e.Median).HasColumnName("median");
            entity.Property(e => e.MetricValueType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metricValueType");
            entity.Property(e => e.Min).HasColumnName("min");
            entity.Property(e => e.Mode).HasColumnName("mode");
            entity.Property(e => e.NodeMetricId).HasColumnName("nodeMetricId");
            entity.Property(e => e.StdDev).HasColumnName("stdDev");
            entity.Property(e => e.Sum).HasColumnName("sum");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updatedDate");
            entity.Property(e => e.Value)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("value");
            entity.Property(e => e.ValueType)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("valueType");
            entity.Property(e => e._95pct).HasColumnName("95pct");
        });

        modelBuilder.Entity<TbNodeStatus>(entity =>
        {
            entity.HasKey(e => new { e.NodeId, e.Timestamp });

            entity.ToTable("tbNodeStatus", "aggregation");

            entity.HasIndex(e => new { e.NodeId, e.Timestamp }, "UN_Node_Timestamp_tbNodeStatus").IsUnique();

            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.OriginalStatus).HasColumnName("originalStatus");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updatedDate");
        });

        modelBuilder.Entity<TbNodeStatusConfig>(entity =>
        {
            entity.HasKey(e => new { e.NodeTypeId, e.MetricKey });

            entity.ToTable("tbNodeStatusConfig", "aggregation");

            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.MetricKey)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Include).HasColumnName("include");
        });

        modelBuilder.Entity<TbNodeType>(entity =>
        {
            entity.ToTable("tbNodeType", "aggregation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DefaultValue).HasColumnName("defaultValue");
            entity.Property(e => e.NodeType).HasColumnName("nodeType");
        });

        modelBuilder.Entity<TbObservation>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.NodeId, e.NodeTypeId, e.MetricKey, e.Timestamp });

            entity.ToTable("tbObservations", "aggregation");

            entity.HasIndex(e => new { e.NodeTypeId, e.MetricKey, e.Timestamp }, "NC-NodeTypeID-MetricKey_Timestamp");

            entity.HasIndex(e => new { e.NodeId, e.NodeTypeId, e.MetricKey, e.Timestamp }, "UN_Job_Node_NodeType_MetricKey_Timestamp_tbObservations").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.MetricKey)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.Average).HasColumnName("average");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.First)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("first");
            entity.Property(e => e.HasObservation).HasColumnName("hasObservation");
            entity.Property(e => e.Last)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("last");
            entity.Property(e => e.Max).HasColumnName("max");
            entity.Property(e => e.MaxCount).HasColumnName("maxCount");
            entity.Property(e => e.Median).HasColumnName("median");
            entity.Property(e => e.MetricValueType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metricValueType");
            entity.Property(e => e.Min).HasColumnName("min");
            entity.Property(e => e.Mode).HasColumnName("mode");
            entity.Property(e => e.NodeMetricId).HasColumnName("nodeMetricId");
            entity.Property(e => e.Observationid).HasColumnName("observationid");
            entity.Property(e => e.StdDev).HasColumnName("stdDev");
            entity.Property(e => e.Sum).HasColumnName("sum");
            entity.Property(e => e.UsePreviousValue).HasColumnName("usePreviousValue");
            entity.Property(e => e.Value)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("value");
            entity.Property(e => e._95pct).HasColumnName("95pct");
        });

        modelBuilder.Entity<TbPriority>(entity =>
        {
            entity.ToTable("tbPriority", "aggregation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("description");
        });

        modelBuilder.Entity<TbRawDatum>(entity =>
        {
            entity.HasKey(e => new { e.NodeId, e.NodeTypeId, e.MetricKey, e.Timestamp });

            entity.ToTable("tbRawData", "aggregation");

            entity.HasIndex(e => new { e.NodeId, e.NodeTypeId, e.MetricKey, e.Timestamp }, "UN_Job_Node_NodeType_MetricKey_Timestamp_tbRawData").IsUnique();

            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.MetricKey)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Reportingtimestamp)
                .HasColumnType("datetime")
                .HasColumnName("reportingtimestamp");
            entity.Property(e => e.Value)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("value");
        });

        modelBuilder.Entity<TbStatus>(entity =>
        {
            entity.ToTable("tbStatus", "aggregation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("description");
        });

        modelBuilder.Entity<Tbnodemetricvalue5minute>(entity =>
        {
            entity.HasKey(e => new { e.NodeId, e.NodeTypeId, e.MetricKey, e.Timestamp }).HasName("PK_ttbnodemetricvalue_5Minute");

            entity.ToTable("tbnodemetricvalue_5Minute", "summary");

            entity.HasIndex(e => e.Timestamp, "NCIndex-tbnodemetricvalue_5Minute_TimeStamp");

            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.MetricKey)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.Average).HasColumnName("average");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.First)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("first");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Last)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("last");
            entity.Property(e => e.Max).HasColumnName("max");
            entity.Property(e => e.MaxCount).HasColumnName("maxCount");
            entity.Property(e => e.Median).HasColumnName("median");
            entity.Property(e => e.MetricValueType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metricValueType");
            entity.Property(e => e.Min).HasColumnName("min");
            entity.Property(e => e.Mode).HasColumnName("mode");
            entity.Property(e => e.NodeMetricId).HasColumnName("nodeMetricId");
            entity.Property(e => e.OriginalStatus).HasColumnName("originalStatus");
            entity.Property(e => e.StdDev).HasColumnName("stdDev");
            entity.Property(e => e.Sum).HasColumnName("sum");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updatedDate");
            entity.Property(e => e.UsePreviousValue).HasColumnName("usePreviousValue");
            entity.Property(e => e.Value)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("value");
            entity.Property(e => e._95pct).HasColumnName("95pct");
        });

        modelBuilder.Entity<VwChildNodeStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwChildNodeStatus", "aggregation");

            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<VwGetAggregatedBase>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwGetAggregatedBase", "aggregation");

            entity.Property(e => e.Average).HasColumnName("average");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.First).HasColumnName("first");
            entity.Property(e => e.Last).HasColumnName("last");
            entity.Property(e => e.Max).HasColumnName("max");
            entity.Property(e => e.MetricKey)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.Min).HasColumnName("min");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.Nodeid).HasColumnName("nodeid");
            entity.Property(e => e.Reportingtimestamp)
                .HasColumnType("datetime")
                .HasColumnName("reportingtimestamp");
            entity.Property(e => e.Sum).HasColumnName("sum");
        });

        modelBuilder.Entity<VwGetAggregatedMedian>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwGetAggregatedMedian", "aggregation");

            entity.Property(e => e.Median).HasColumnName("median");
            entity.Property(e => e.MetricKey)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.Reportingtimestamp)
                .HasColumnType("datetime")
                .HasColumnName("reportingtimestamp");
        });

        modelBuilder.Entity<VwGetAggregatedPercentile>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwGetAggregatedPercentile", "aggregation");

            entity.Property(e => e.MetricKey)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.Percentile).HasColumnName("percentile");
            entity.Property(e => e.Reportingtimestamp)
                .HasColumnType("datetime")
                .HasColumnName("reportingtimestamp");
        });

        modelBuilder.Entity<VwGetAggregatedStdDev>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwGetAggregatedStdDev", "aggregation");

            entity.Property(e => e.MetricKey)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.Reportingtimestamp)
                .HasColumnType("datetime")
                .HasColumnName("reportingtimestamp");
            entity.Property(e => e.StdDev).HasColumnName("stdDev");
        });

        modelBuilder.Entity<VwGetPreviousNodeStatusValue>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwGetPreviousNodeStatusValue", "aggregation");

            entity.Property(e => e.Defaultvalue).HasColumnName("defaultvalue");
            entity.Property(e => e.ElapsedTime).HasColumnName("elapsedTime");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<VwNode>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwNode", "aggregation");

            entity.Property(e => e.ActivationDate)
                .HasColumnType("datetime")
                .HasColumnName("activationDate");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.ConnectorCreds)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("connectorCreds");
            entity.Property(e => e.ConnectorId).HasColumnName("connectorId");
            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasColumnName("dateCreated");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime")
                .HasColumnName("dateUpdated");
            entity.Property(e => e.DeactivationDate)
                .HasColumnType("datetime")
                .HasColumnName("deactivationDate");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LastPollDate)
                .HasColumnType("datetime")
                .HasColumnName("lastPollDate");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.NextPollDate)
                .HasColumnType("datetime")
                .HasColumnName("nextPollDate");
            entity.Property(e => e.NodeData)
                .HasColumnType("xml")
                .HasColumnName("nodeData");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.NodeTypeTemplateId).HasColumnName("nodeTypeTemplateId");
            entity.Property(e => e.PollPolicy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("pollPolicy");
            entity.Property(e => e.PollerId).HasColumnName("pollerId");
            entity.Property(e => e.PollingInterval).HasColumnName("pollingInterval");
            entity.Property(e => e.ProcessorName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("processorName");
            entity.Property(e => e.SourceInfo)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("sourceInfo");
            entity.Property(e => e.UseMetricTemplate).HasColumnName("useMetricTemplate");
        });

        modelBuilder.Entity<VwNodeMetric>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwNodeMetric", "aggregation");

            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasColumnName("dateCreated");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime")
                .HasColumnName("dateUpdated");
            entity.Property(e => e.Description)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MetricKey)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.MetricSourceInfo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metricSourceInfo");
            entity.Property(e => e.MetricUnits)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metricUnits");
            entity.Property(e => e.MetricValue1Label)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("metricValue1Label");
            entity.Property(e => e.MetricValue2Label)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("metricValue2Label");
            entity.Property(e => e.MetricValue3Label)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("metricValue3Label");
            entity.Property(e => e.MetricValueType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metricValueType");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.NodeMetricData)
                .HasColumnType("xml")
                .HasColumnName("nodeMetricData");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.PollingInterval).HasColumnName("pollingInterval");
            entity.Property(e => e.SampleValue)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("sampleValue");
        });

        modelBuilder.Entity<VwNodeMetricValue>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwNodeMetricValue", "aggregation");

            entity.Property(e => e.BackFillMetricValueId).HasColumnName("backFillMetricValueID");
            entity.Property(e => e.EpochTimestamp).HasColumnName("epochTimestamp");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsbackFilled).HasColumnName("isbackFilled");
            entity.Property(e => e.LastPolledTimestamp).HasColumnName("lastPolledTimestamp");
            entity.Property(e => e.MetricKey)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.MetricValueType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metricValueType");
            entity.Property(e => e.Metricvalue1)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("metricvalue1");
            entity.Property(e => e.Metricvalue2)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("metricvalue2");
            entity.Property(e => e.Metricvalue3)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("metricvalue3");
            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.NodeMetricId).HasColumnName("nodeMetricId");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("value");
        });

        modelBuilder.Entity<VwNodeParent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwNodeParent", "aggregation");

            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasColumnName("dateCreated");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime")
                .HasColumnName("dateUpdated");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NodeId).HasColumnName("nodeId");
            entity.Property(e => e.ParentnodeId).HasColumnName("parentnodeId");
        });

        modelBuilder.Entity<VwNodeType>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwNodeType", "aggregation");

            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Config)
                .HasColumnType("xml")
                .HasColumnName("config");
            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasColumnName("dateCreated");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime")
                .HasColumnName("dateUpdated");
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<VwNodeTypeMetric>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwNodeTypeMetric", "aggregation");

            entity.Property(e => e.Config)
                .HasColumnType("xml")
                .HasColumnName("config");
            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasColumnName("dateCreated");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime")
                .HasColumnName("dateUpdated");
            entity.Property(e => e.Description)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("displayName");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsCalculatedMetric)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("isCalculatedMetric");
            entity.Property(e => e.MetricKey)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("metricKey");
            entity.Property(e => e.MetricSourceInfo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metricSourceInfo");
            entity.Property(e => e.MetricUnits)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metricUnits");
            entity.Property(e => e.MetricValue1Label)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("metricValue1Label");
            entity.Property(e => e.MetricValue2Label)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("metricValue2Label");
            entity.Property(e => e.MetricValue3Label)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("metricValue3Label");
            entity.Property(e => e.MetricValueType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("metricValueType");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            entity.Property(e => e.PollingInterval).HasColumnName("pollingInterval");
            entity.Property(e => e.PreFillValueConfig)
                .HasColumnType("xml")
                .HasColumnName("preFillValueConfig");
            entity.Property(e => e.RollUpAggregateConfig)
                .HasColumnType("xml")
                .HasColumnName("rollUpAggregateConfig");
            entity.Property(e => e.SampleValue)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("sampleValue");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
