using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Aggregation_DataModels.Models;

namespace Aggregation_Services
{
    public partial class AggregationDbContext : DbContext
    {
        public AggregationDbContext()
        {
        }

        public AggregationDbContext(DbContextOptions<AggregationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RtbNode> RtbNodes { get; set; }

        public virtual DbSet<RtbNodeMetric> RtbNodeMetrics { get; set; }

        public virtual DbSet<RtbNodeParent> RtbNodeParents { get; set; }

        public virtual DbSet<RtbNodeType> RtbNodeTypes { get; set; }

        public virtual DbSet<RtbNodeTypeMetric> RtbNodeTypeMetrics { get; set; }

        public virtual DbSet<TbActivity> TbActivities { get; set; }

        public virtual DbSet<TbActivityLogger> TbActivityLoggers { get; set; }

        public virtual DbSet<TbConfiguration> TbConfigurations { get; set; }

        public virtual DbSet<TbConfigurationByDate> TbConfigurationByDates { get; set; }

        public virtual DbSet<TbInterval> TbIntervals { get; set; }

        public virtual DbSet<TbJob> TbJobs { get; set; }

        public virtual DbSet<TbJobType> TbJobTypes { get; set; }

        public virtual DbSet<TbLogActivity> TbLogActivities { get; set; }

        public virtual DbSet<TbLogError> TbLogErrors { get; set; }

        public virtual DbSet<TbNodeMetricValue5minute> TbNodeMetricValue5minutes { get; set; }

        public virtual DbSet<TbNodeMetricValueDay> TbNodeMetricValueDays { get; set; }

        public virtual DbSet<TbNodeMetricValueHour> TbNodeMetricValueHours { get; set; }

        public virtual DbSet<TbNodeStatus> TbNodeStatuses { get; set; }

        public virtual DbSet<TbNodeStatusConfig> TbNodeStatusConfigs { get; set; }

        public virtual DbSet<TbNodeStatusTemp> TbNodeStatusTemps { get; set; }

        public virtual DbSet<TbObservation> TbObservations { get; set; }

        public virtual DbSet<TbObservationsTemplate> TbObservationsTemplates { get; set; }

        public virtual DbSet<TbPriority> TbPriorities { get; set; }

        public virtual DbSet<TbRawDataDaily> TbRawDataDailies { get; set; }

        public virtual DbSet<TbRawDatum> TbRawData { get; set; }

        public virtual DbSet<TbStatus> TbStatuses { get; set; }

        public virtual DbSet<TbUser> TbUsers { get; set; }

        public virtual DbSet<TbWebServer> TbWebServers { get; set; }

        public virtual DbSet<VwGetAggregatedBase> VwGetAggregatedBases { get; set; }

        public virtual DbSet<VwGetAggregatedMedian> VwGetAggregatedMedians { get; set; }

        public virtual DbSet<VwGetAggregatedPercentile> VwGetAggregatedPercentiles { get; set; }

        public virtual DbSet<VwGetAggregatedStdDev> VwGetAggregatedStdDevs { get; set; }

        public virtual DbSet<VwGetChildNodeStatus> VwGetChildNodeStatuses { get; set; }

        public virtual DbSet<VwGetDashbordDatum> VwGetDashbordData { get; set; }

        public virtual DbSet<VwGetJob> VwGetJobs { get; set; }

        public virtual DbSet<VwGetPreviousNodeStatusValue> VwGetPreviousNodeStatusValues { get; set; }

        public virtual DbSet<VwNode> VwNodes { get; set; }

        public virtual DbSet<VwNodeMetric> VwNodeMetrics { get; set; }

        public virtual DbSet<VwNodeMetricValue> VwNodeMetricValues { get; set; }

        public virtual DbSet<VwNodeParent> VwNodeParents { get; set; }

        public virtual DbSet<VwNodeType> VwNodeTypes { get; set; }

        public virtual DbSet<VwNodeTypeMetric> VwNodeTypeMetrics { get; set; }

        public virtual DbSet<VwRnodeTypeMetric> VwRnodeTypeMetrics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:AggregationContext");

        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        //        => optionsBuilder.UseSqlServer("Server=igenmsdevdb01;Database=Aggregation;user id=Aggregation;password=@Password#202211;MultipleActiveResultSets=true;Encrypt=False;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RtbNode>(entity =>
            {
                entity.ToTable("rtbNode", "Aggregation");

                entity.HasIndex(e => new { e.NodeId, e.NodeTypeId }, "UC_Node_NodeType").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ActivationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("activationDate");
                entity.Property(e => e.Active).HasColumnName("active");
                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("dateCreated");
                entity.Property(e => e.DateUpdated)
                    .HasColumnType("datetime")
                    .HasColumnName("dateUpdated");
                entity.Property(e => e.DeactivationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("deactivationDate");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");
                entity.Property(e => e.NodeId).HasColumnName("nodeId");
                entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            });

            modelBuilder.Entity<RtbNodeMetric>(entity =>
            {
                entity.ToTable("rtbNodeMetric", "Aggregation");

                entity.HasIndex(e => new { e.NodeId, e.NodeTypeId, e.MetricKey }, "UC_Node_NodeType_MetricKey").IsUnique();

                entity.HasIndex(e => new { e.NodeId, e.MetricKey, e.NodeTypeId }, "UQ_NodeID_MetricKey_NodeTypeID").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Active).HasColumnName("active");
                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("dateCreated");
                entity.Property(e => e.DateUpdated)
                    .HasColumnType("datetime")
                    .HasColumnName("dateUpdated");
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description");
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
                entity.Property(e => e.NodeId).HasColumnName("nodeId");
                entity.Property(e => e.NodeMetricId).HasColumnName("nodeMetricId");
                entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            });

            modelBuilder.Entity<RtbNodeParent>(entity =>
            {
                entity.ToTable("rtbNodeParent", "Aggregation");

                entity.HasIndex(e => new { e.NodeId, e.ParentnodeId }, "UC_Node_ParentNode").IsUnique();

                entity.HasIndex(e => new { e.NodeId, e.ParentnodeId }, "UQ_NodeID_parentNodeID").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Active).HasColumnName("active");
                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("dateCreated");
                entity.Property(e => e.DateUpdated)
                    .HasColumnType("datetime")
                    .HasColumnName("dateUpdated");
                entity.Property(e => e.NodeId).HasColumnName("nodeId");
                entity.Property(e => e.ParentnodeId).HasColumnName("parentnodeId");
            });

            modelBuilder.Entity<RtbNodeType>(entity =>
            {
                entity.ToTable("rtbNodeType", "Aggregation");

                entity.Property(e => e.Id).HasColumnName("id");
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
                entity.Property(e => e.DefaultValue)
                    .HasDefaultValueSql("((60))")
                    .HasColumnName("defaultValue");
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description");
                entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
            });

            modelBuilder.Entity<RtbNodeTypeMetric>(entity =>
            {
                entity.ToTable("rtbNodeTypeMetric", "Aggregation");

                entity.HasIndex(e => new { e.NodeTypeId, e.MetricKey }, "UC_NodeType_MetricKey").IsUnique();

                entity.HasIndex(e => new { e.MetricKey, e.NodeTypeId }, "UQ_MetricKey_NodeTypeID").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("dateCreated");
                entity.Property(e => e.DateUpdated)
                    .HasColumnType("datetime")
                    .HasColumnName("dateUpdated");
                entity.Property(e => e.DefaultValue)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('NULL')")
                    .HasColumnName("defaultValue");
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description");
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
                entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
                entity.Property(e => e.PreFillValueConfig)
                    .HasColumnType("xml")
                    .HasColumnName("preFillValueConfig");
                entity.Property(e => e.RollUpAggregateConfig)
                    .HasColumnType("xml")
                    .HasColumnName("rollUpAggregateConfig");
                entity.Property(e => e.UsePreviousValue).HasColumnName("usePreviousValue");
            });

            modelBuilder.Entity<TbActivity>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Activity");

                entity.ToTable("tbActivity", "Aggregation");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<TbActivityLogger>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PKtbActivityLogger");

                entity.ToTable("tbActivityLogger", "Aggregation");

                entity.HasIndex(e => e.Timestamp, "UC_tbActivityLogger_timestamp").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ActivityId).HasColumnName("activityId");
                entity.Property(e => e.ConfigurationId).HasColumnName("configurationId");
                entity.Property(e => e.NewValue)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("newValue");
                entity.Property(e => e.OldValue)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("oldValue");
                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp");
                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Activity).WithMany(p => p.TbActivityLoggers)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tbActivityLogger_TypeId");

                entity.HasOne(d => d.Configuration).WithMany(p => p.TbActivityLoggers)
                    .HasForeignKey(d => d.ConfigurationId)
                    .HasConstraintName("fk_tbActivityLogger_ConfigurationId");

                entity.HasOne(d => d.User).WithMany(p => p.TbActivityLoggers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tbActivityLogger_UserId");
            });

            modelBuilder.Entity<TbConfiguration>(entity =>
            {
                entity.ToTable("tbConfiguration", "Aggregation");

                entity.HasIndex(e => e.Key, "UQ__tbConfig__DFD83CAF1D229F9A").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");
                entity.Property(e => e.Immediate)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasColumnName("immediate");
                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("key");
                entity.Property(e => e.ReadOnly)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasColumnName("readOnly");
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("type");
                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<TbConfigurationByDate>(entity =>
            {
                entity.ToTable("tbConfigurationByDate", "Aggregation");

                entity.HasIndex(e => e.AggregationDate, "UN_aggregationDate_tbConfigurationByDate").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AggregationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("aggregationDate");
                entity.Property(e => e.CreatedDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("createdDateTime");
                entity.Property(e => e.JobStartTimeDelay).HasColumnName("Job_StartTimeDelay");
                entity.Property(e => e.RefreshEnabled).HasColumnName("Refresh_Enabled");
                entity.Property(e => e.RefreshInterval).HasColumnName("Refresh_Interval");
                entity.Property(e => e.RefreshNumberOfDays).HasColumnName("Refresh_NumberOfDays");
            });

            modelBuilder.Entity<TbInterval>(entity =>
            {
                entity.ToTable("tbInterval", "Aggregation");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<TbJob>(entity =>
            {
                entity.ToTable("tbJob", "Aggregation");

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
                entity.ToTable("tbJobType", "Aggregation");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<TbLogActivity>(entity =>
            {
                entity.ToTable("tbLogActivity", "Aggregation");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ElapsedTime).HasColumnName("elapsedTime");
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

                entity.HasOne(d => d.Job).WithMany(p => p.TbLogActivities)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tbJob_LogActivity");
            });

            modelBuilder.Entity<TbLogError>(entity =>
            {
                entity.ToTable("tbLogError", "Aggregation");

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
                entity.Property(e => e.Parameters)
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasColumnName("parameters");
                entity.Property(e => e.ReturnCode).HasColumnName("returnCode");
                entity.Property(e => e.StackTrace)
                    .IsUnicode(false)
                    .HasColumnName("stackTrace");
                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp");

                entity.HasOne(d => d.Job).WithMany(p => p.TbLogErrors)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tbJob_logs");
            });

            modelBuilder.Entity<TbNodeMetricValue5minute>(entity =>
            {
                entity.HasKey(e => new { e.NodeId, e.NodeTypeId, e.MetricKey, e.Timestamp }).HasName("PK_ttbnodemetricvalue_5Minute");

                entity.ToTable("tbNodeMetricValue_5Minute", "Summary");

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

            modelBuilder.Entity<TbNodeMetricValueDay>(entity =>
            {
                entity.ToTable("tbNodeMetricValue_Day", "Summary");

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

                entity.ToTable("tbNodeMetricValue_Hour", "Summary");

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

                entity.ToTable("tbNodeStatus", "Summary");

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

                entity.ToTable("tbNodeStatusConfig", "Aggregation");

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

            modelBuilder.Entity<TbNodeStatusTemp>(entity =>
            {
                entity.HasKey(e => new { e.NodeId, e.NodeTypeId });

                entity.ToTable("tbNodeStatusTemp", "Aggregation");

                entity.Property(e => e.NodeId).HasColumnName("nodeId");
                entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
                entity.Property(e => e.Average)
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasColumnName("average");
                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<TbObservation>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.NodeId, e.NodeTypeId, e.MetricKey, e.Timestamp });

                entity.ToTable("tbObservations", "Aggregation");

                entity.HasIndex(e => new { e.NodeTypeId, e.NodeId, e.MetricKey, e.Timestamp }, "NC-NodeTypeID-MetricKey_Timestamp").IsUnique();

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

            modelBuilder.Entity<TbObservationsTemplate>(entity =>
            {
                entity.HasKey(e => e.Timestamp);

                entity.ToTable("tbObservationsTemplate", "Aggregation");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp");
            });

            modelBuilder.Entity<TbPriority>(entity =>
            {
                entity.ToTable("tbPriority", "Aggregation");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<TbRawDataDaily>(entity =>
            {
                entity.HasKey(e => new { e.NodeId, e.NodeTypeId, e.MetricKey, e.Timestamp });

                entity.ToTable("tbRawDataDaily", "Aggregation");

                entity.HasIndex(e => e.Timestamp, "NCIndex_tbRawDataDaily_TimeStamp");

                entity.HasIndex(e => new { e.NodeId, e.NodeTypeId, e.MetricKey, e.Timestamp }, "UN_Job_Node_NodeType_MetricKey_Timestamp_tbRawDataDaily").IsUnique();

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
                entity.Property(e => e.IsbackFilled).HasColumnName("isbackFilled");
                entity.Property(e => e.Reportingtimestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("reportingtimestamp");
                entity.Property(e => e.Value)
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<TbRawDatum>(entity =>
            {
                entity.HasKey(e => new { e.NodeId, e.NodeTypeId, e.MetricKey, e.Timestamp });

                entity.ToTable("tbRawData", "Aggregation");

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
                entity.Property(e => e.IsbackFilled).HasColumnName("isbackFilled");
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
                entity.ToTable("tbStatus", "Aggregation");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<TbUser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PKtbUser");

                entity.ToTable("tbUser", "Aggregation");

                entity.HasIndex(e => e.UserId, "UC_tbUser_userId").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Active).HasColumnName("active");
                entity.Property(e => e.HasAccessToConfiguration).HasColumnName("hasAccessToConfiguration");
                entity.Property(e => e.HasAccessToQueue).HasColumnName("hasAccessToQueue");
                entity.Property(e => e.HasAccessToRefreshJob).HasColumnName("hasAccessToRefreshJob");
                entity.Property(e => e.HasAccessToUpdateClock).HasColumnName("hasAccessToUpdateClock");
                entity.Property(e => e.HasAdminAccess).HasColumnName("hasAdminAccess");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("userId");
            });

            modelBuilder.Entity<TbWebServer>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PKtbWebServer");

                entity.ToTable("tbWebServer", "Aggregation");

                entity.HasIndex(e => e.ServerName, "UC_tbWebServer_serverName").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Active).HasColumnName("active");
                entity.Property(e => e.InstanceName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.ServerIp)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("serverIP");
                entity.Property(e => e.ServerName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("serverName");
            });

            modelBuilder.Entity<VwGetAggregatedBase>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("vwGetAggregatedBase", "Aggregation");

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
                    .ToView("vwGetAggregatedMedian", "Aggregation");

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
                    .ToView("vwGetAggregatedPercentile", "Aggregation");

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
                    .ToView("vwGetAggregatedStdDev", "Aggregation");

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

            modelBuilder.Entity<VwGetChildNodeStatus>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("vwGetChildNodeStatus", "Aggregation");

                entity.Property(e => e.NodeId).HasColumnName("nodeId");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp");
            });

            modelBuilder.Entity<VwGetDashbordDatum>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("vwGetDashbordData", "Aggregation");

                entity.Property(e => e.AverageProcessingTimeDailyJob).HasColumnName("AverageProcessingTime_DailyJob");
                entity.Property(e => e.AverageProcessingTimeHourlyJob).HasColumnName("AverageProcessingTime_HourlyJob");
                entity.Property(e => e.JobStartTimeDelay).HasColumnName("Job_StartTimeDelay");
                entity.Property(e => e.JobSuspendProcessingAfter)
                    .HasColumnType("datetime")
                    .HasColumnName("Job_SuspendProcessingAfter");
                entity.Property(e => e.LastErrJobAggregationDateTime).HasColumnType("datetime");
                entity.Property(e => e.LastErrJobEndDateTime).HasColumnType("datetime");
                entity.Property(e => e.LastErrJobInterval)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.LastErrJobStartDateTime).HasColumnType("datetime");
                entity.Property(e => e.LastErrJobStatus)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.LastJobAggregationDateTime).HasColumnType("datetime");
                entity.Property(e => e.LastJobEndDateTime).HasColumnType("datetime");
                entity.Property(e => e.LastJobInterval)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.LastJobStartDateTime).HasColumnType("datetime");
                entity.Property(e => e.LastJobStatus)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.NextJobAggregationDateTime).HasColumnType("datetime");
                entity.Property(e => e.NextJobInterval)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.NextJobStartAfterDateTime).HasColumnType("datetime");
                entity.Property(e => e.NextJobStartDateTime).HasColumnType("datetime");
                entity.Property(e => e.NextJobStatus)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.ProjectName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.RefreshEnabled).HasColumnName("Refresh_Enabled");
                entity.Property(e => e.RefreshInterval).HasColumnName("Refresh_Interval");
                entity.Property(e => e.RetentionActivityLogNumberOfHours).HasColumnName("Retention_ActivityLog_NumberOfHours");
                entity.Property(e => e.SpCommandTimeout).HasColumnName("SP_CommandTimeout");
            });

            modelBuilder.Entity<VwGetJob>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("vwGetJobs", "Aggregation");

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
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Interval)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.IsRecovery).HasColumnName("isRecovery");
                entity.Property(e => e.JobType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.ListOfJobs)
                    .IsUnicode(false)
                    .HasColumnName("listOfJobs");
                entity.Property(e => e.Priority)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.ProcessId).HasColumnName("processId");
                entity.Property(e => e.Processed).HasColumnName("processed");
                entity.Property(e => e.StartAfterDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("startAfterDateTime");
                entity.Property(e => e.StartDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("startDateTime");
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VwGetPreviousNodeStatusValue>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("vwGetPreviousNodeStatusValue", "Aggregation");

                entity.Property(e => e.Defaultvalue).HasColumnName("defaultvalue");
                entity.Property(e => e.ElapsedTime).HasColumnName("elapsedTime");
                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<VwNode>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("vwNode", "Aggregation");

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
                    .HasMaxLength(50)
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
                    .ToView("vwNodeMetric", "Aggregation");

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
                    .ToView("vwNodeMetricValue", "Aggregation");

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
                    .ToView("vwNodeParent", "Aggregation");

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
                    .ToView("vwNodeType", "Aggregation");

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
                    .ToView("vwNodeTypeMetric", "Aggregation");

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

            modelBuilder.Entity<VwRnodeTypeMetric>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("vwRNodeTypeMetric", "Aggregation");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("dateCreated");
                entity.Property(e => e.DateUpdated)
                    .HasColumnType("datetime")
                    .HasColumnName("dateUpdated");
                entity.Property(e => e.DefaultValue)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("defaultValue");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MetricKey)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("metricKey");
                entity.Property(e => e.MetricKeyDescription)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("MetricKey_Description");
                entity.Property(e => e.MetricValueType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("metricValueType");
                entity.Property(e => e.NodeTypeDescription)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("NodeType_Description");
                entity.Property(e => e.NodeTypeId).HasColumnName("nodeTypeId");
                entity.Property(e => e.UsePreviousValue).HasColumnName("usePreviousValue");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}