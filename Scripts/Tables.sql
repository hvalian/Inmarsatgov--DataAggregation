IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Summary' )
	EXEC('CREATE SCHEMA [Summary] AUTHORIZATION [dbo]');
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Aggregation' )
	EXEC('CREATE SCHEMA [Aggregation] AUTHORIZATION [dbo]');
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[rtbNode]') AND type in (N'U'))
CREATE TABLE [Aggregation].[rtbNode](
	[id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[nodeId] [int] NOT NULL,
	[name] [varchar](255) NOT NULL,
	[active] [bit] NOT NULL,
	[nodeTypeId] [int] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[dateUpdated] [datetime] NOT NULL,
	[activationDate] [datetime] NULL,
	[deactivationDate] [datetime] NULL,
 CONSTRAINT [PK_rtbNode] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UC_Node_NodeType] UNIQUE NONCLUSTERED 
(
	[nodeId] ASC,
	[nodeTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[rtbNodeMetric]') AND type in (N'U'))
CREATE TABLE [Aggregation].[rtbNodeMetric](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nodeId] [int] NOT NULL,
	[nodeMetricId] [int] NULL,
	[nodeTypeId] [int] NOT NULL,
	[metricKey] [varchar](255) NOT NULL,
  	[description] [varchar](255) NOT NULL,
	[metricValueType] [varchar](50) NOT NULL,
	[active] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[dateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_rtbNodeMetric] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UC_Node_NodeType_MetricKey] UNIQUE NONCLUSTERED 
(
	[nodeId] ASC,
	[nodeTypeId] ASC,
	[metricKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_NodeID_MetricKey_NodeTypeID] UNIQUE NONCLUSTERED 
(
	[nodeId] ASC,
	[metricKey] ASC,
	[nodeTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[rtbNodeParent]') AND type in (N'U'))
CREATE TABLE [Aggregation].[rtbNodeParent](
	[id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[nodeId] [int] NOT NULL,
	[parentnodeId] [int] NOT NULL,
	[active] [bit] NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[dateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_rtbNodeParent] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UC_Node_ParentNode] UNIQUE NONCLUSTERED 
(
	[nodeId] ASC,
	[parentnodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_NodeID_parentNodeID] UNIQUE NONCLUSTERED 
(
	[nodeId] ASC,
	[parentnodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[rtbNodeType]') AND type in (N'U'))
CREATE TABLE [Aggregation].[rtbNodeType](
	[id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[nodeTypeId] [int] NOT NULL,
	[description] [varchar](255) NOT NULL,
	[active] [bit] NOT NULL,
	[config] [xml] NULL,
	[defaultValue] [int] NOT NULL DEFAULT 60,
	[dateCreated] [datetime] NOT NULL,
	[dateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_rtbNodeType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[rtbNodeTypeMetric]') AND type in (N'U'))
CREATE TABLE [Aggregation].[rtbNodeTypeMetric](
	[id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[nodeTypeId] [int] NOT NULL,
	[metricKey] [varchar](255) NOT NULL,
	[description] [varchar](255) NOT NULL,
	[metricValueType] [varchar](50) NOT NULL,
	[dateCreated] [datetime] NOT NULL,
	[dateUpdated] [datetime] NOT NULL,
	[rollUpAggregateConfig] [xml] NULL,
	[preFillValueConfig] [xml] NULL,
	[defaultValue] [varchar](50) NOT NULL DEFAULT 'NULL',
	[usePreviousValue] [bit] NOT NULL DEFAULT 0,
 CONSTRAINT [PK_rtbNodeTypeMetric] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UC_NodeType_MetricKey] UNIQUE NONCLUSTERED 
(
	[nodeTypeId] ASC,
	[metricKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_MetricKey_NodeTypeID] UNIQUE NONCLUSTERED 
(
	[metricKey] ASC,
	[nodeTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbConfiguration]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbConfiguration](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[key] [varchar](50) NOT NULL UNIQUE,
	[value] [varchar](50) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[type] [varchar](8) NOT NULL,
	[readOnly] [bit] NOT NULL DEFAULT 1,
	[immediate] [bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_tbConfiguration] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbConfigurationByDate]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbConfigurationByDate](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[aggregationDate] [datetime] NOT NULL,
	[Refresh_Enabled] [bit] NOT NULL,
	[Refresh_NumberOfDays] [int] NOT NULL,
	[Refresh_Interval] [int] NOT NULL,
	[Job_StartTimeDelay] [int] NOT NULL,
	[createdDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_tbConfigurationByDate] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UN_aggregationDate_tbConfigurationByDate] UNIQUE NONCLUSTERED 
(
	[aggregationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbInterval]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbInterval](
	[id] [int] NOT NULL,
	[description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tbInterval] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbJob]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbJob](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[jobType] [int] NOT NULL,
	[computerName] [varchar](255) NULL,
	[status] [int] NOT NULL,
	[interval] [int] NULL,
	[priority] [int] NOT NULL,
	[startDateTime] [datetime] NULL,
	[endDateTime] [datetime] NULL,
	[startAfterDateTime] [datetime] NOT NULL,
	[processId] int null,
	[aggregationStartDateTime] [datetime] NOT NULL,
	[aggregationEndDateTime] [datetime] NOT NULL,
	[createdDateTime] [datetime] NOT NULL,
	[parentJobId] [bigint] NULL,
	[processed] int null,
	[isRecovery] bit not null,
	[createdBy] [varchar](255) NULL,
	[exitCode] int null,
 CONSTRAINT [PK_tbJob] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UN_Id_Inverval_aggregationStartDateTime_aggregationEndDateTime_tbaggregation] UNIQUE NONCLUSTERED 
(
	[Id] ASC,
	[interval] ASC,
	[aggregationStartDateTime] ASC,
	[aggregationEndDateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbJobType]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbJobType](
	[id] [int] NOT NULL,
	[description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tbJobType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbLogActivity]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbLogActivity](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[jobId] [bigint] NOT NULL,
	[methodName] [varchar](1024) NOT NULL,
	[timestamp] [datetime] NULL,
	[elapsedTime] [bigint] NOT NULL,
	[parameters] [varchar](1024) NULL,
 CONSTRAINT [PK_tbLogActivity] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbLogError]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbLogError](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[jobId] [bigint] NOT NULL,
	[methodName] [varchar](1024) NOT NULL,
	[exceptionMessage] [varchar](max) NOT NULL,
	[innerException] [varchar](max) NULL,
	[returnCode] [int] NULL,
	[elapsedTime] [bigint] NOT NULL,
	[timestamp] [datetime] NULL,
	[stackTrace] [varchar](max) NULL,
	[parameters] [varchar](1024) NULL,
 CONSTRAINT [PK_tbLogError] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbNodeStatusConfig]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbNodeStatusConfig](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nodeTypeId] [int] NOT NULL,
	[metricKey] [varchar](255) NOT NULL,
	[include] [bit] NOT NULL,
 CONSTRAINT [PK_tbNodeStatusConfig] PRIMARY KEY CLUSTERED 
(
	[nodeTypeId] ASC,
	[metricKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbNodeStatusTemp]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbNodeStatusTemp](
	[nodeId] [int] NOT NULL,
	[nodeTypeId] [int] NOT NULL,
	[value] [bit] NOT NULL,
	[average] [varchar](1024) NULL,
 CONSTRAINT [PK_tbNodeStatusTemp] PRIMARY KEY CLUSTERED 
(
	[nodeId] ASC,
	[nodeTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbObservations]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbObservations](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[observationid] [bigint] NULL,
	[nodeId] [int] NOT NULL,
	[nodeMetricId] [int] NULL,
	[nodeTypeId] [int] NOT NULL,
	[metricKey] [varchar](255) NOT NULL,
	[metricValueType] [varchar](50) NULL,
	[timestamp] [datetime] NOT NULL,
	[value] [varchar](1024) NULL,
	[average] [FLOAT] NULL,
	[median] [FLOAT] NULL,
	[mode] [FLOAT] NULL,
	[stdDev] [FLOAT] NULL,
	[95pct] [FLOAT] NULL,
	[sum] [FLOAT] NULL,
	[min] [FLOAT] NULL,
	[max] [FLOAT] NULL,
	[count] [int] NULL,
	[maxCount] [int] NULL,
	[first] [varchar](1024) NULL,
	[last] [varchar](1024) NULL,
	[usePreviousValue] [bit] NOT NULL,
	[hasObservation] [bit] NOT NULL,
 CONSTRAINT [PK_tbObservations] PRIMARY KEY CLUSTERED 
(
	[id] ASC,
	[nodeId] ASC,
	[nodeTypeId] ASC,
	[metricKey] ASC,
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UN_Job_Node_NodeType_MetricKey_Timestamp_tbObservations] UNIQUE NONCLUSTERED 
(
	[nodeId] ASC,
	[nodeTypeId] ASC,
	[metricKey] ASC,
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [NC-NodeTypeID-MetricKey_Timestamp] UNIQUE NONCLUSTERED 
(
	[nodeTypeId] ASC,
	[nodeId] ASC,
	[metricKey] ASC,
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbObservationsTemplate]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbObservationsTemplate](
	[timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_tbObservationsTemplate] PRIMARY KEY CLUSTERED 
(
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbPriority]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbPriority](
	[id] [int] NOT NULL,
	[description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tbPriority] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbRawData]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbRawData](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nodeId] [int] NOT NULL,
	[nodeTypeId] [int] NOT NULL,
	[metricKey] [varchar](255) NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[reportingtimestamp] [datetime] NULL,
	[value] [varchar](1024) NULL,
	[isbackFilled] [bit],
 CONSTRAINT [PK_tbRawData] PRIMARY KEY CLUSTERED 
(
	[nodeId] ASC,
	[nodeTypeId] ASC,
	[metricKey] ASC,
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UN_Job_Node_NodeType_MetricKey_Timestamp_tbRawData] UNIQUE NONCLUSTERED 
(
	[nodeId] ASC,
	[nodeTypeId] ASC,
	[metricKey] ASC,
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbRawDataDaily]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbRawDataDaily](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nodeId] [int] NOT NULL,
	[nodeTypeId] [int] NOT NULL,
	[metricKey] [varchar](255) NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[reportingtimestamp] [datetime] NULL,
	[value] [varchar](1024) NULL,
	[isbackFilled] [bit],
 CONSTRAINT [PK_tbRawDataDaily] PRIMARY KEY CLUSTERED 
(
	[nodeId] ASC,
	[nodeTypeId] ASC,
	[metricKey] ASC,
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UN_Job_Node_NodeType_MetricKey_Timestamp_tbRawDataDaily] UNIQUE NONCLUSTERED 
(
	[nodeId] ASC,
	[nodeTypeId] ASC,
	[metricKey] ASC,
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE NAME = N'NCIndex_tbRawDataDaily_TimeStamp') 
CREATE NONCLUSTERED INDEX [NCIndex_tbRawDataDaily_TimeStamp] ON [Aggregation].[tbRawDataDaily]
(
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbStatus]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbStatus](
	[id] [int] NOT NULL,
	[description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tbStatus] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Summary].[tbNodeMetricValue_5Minute]') AND type in (N'U'))
CREATE TABLE [Summary].[tbNodeMetricValue_5Minute](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[nodeId] [int] NOT NULL,
	[nodeMetricId] [int] NOT NULL,
	[nodeTypeId] [int] NOT NULL,
	[metricKey] [varchar](255) NOT NULL,
	[metricValueType] [varchar](50) NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[value] [varchar](1024) NULL,
	[usePreviousValue] [bit] NOT NULL,
	[originalStatus] [bit] NOT NULL,
	[updatedDate] [datetime] NOT NULL,
	[average] [FLOAT] NULL,
	[median] [FLOAT] NULL,
	[mode] [FLOAT] NULL,
	[stdDev] [FLOAT] NULL,
	[95pct] [FLOAT] NULL,
	[sum] [FLOAT] NULL,
	[min] [FLOAT] NULL,
	[max] [FLOAT] NULL,
	[count] [int] NULL,
	[maxCount] [int] NULL,
	[first] [varchar](1024) NULL,
	[last] [varchar](1024) NULL,
 CONSTRAINT [PK_ttbnodemetricvalue_5Minute] PRIMARY KEY CLUSTERED 
(
	[nodeId] ASC,
	[nodeTypeId] ASC,
	[metricKey] ASC,
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE NAME = N'NCIndex-tbnodemetricvalue_5Minute_TimeStamp') 
CREATE NONCLUSTERED INDEX [NCIndex-tbnodemetricvalue_5Minute_TimeStamp] ON [Summary].[tbNodeMetricValue_5Minute]
(
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Summary].[tbNodeMetricValue_Day]') AND type in (N'U'))
CREATE TABLE [Summary].[tbNodeMetricValue_Day](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[nodeId] [int] NOT NULL,
	[nodeMetricId] [int] NOT NULL,
	[nodeTypeId] [int] NOT NULL,
	[metricKey] [varchar](255) NOT NULL,
	[metricValueType] [varchar](50) NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[value] [varchar](1024) NULL,
	[valueType] [varchar](1024) NULL,
	[epochTimestamp] [bigint] NULL,
	[average] [FLOAT] NULL,
	[median] [FLOAT] NULL,
	[mode] [FLOAT] NULL,
	[stdDev] [FLOAT] NULL,
	[95pct] [FLOAT] NULL,
	[sum] [FLOAT] NULL,
	[min] [FLOAT] NULL,
	[max] [FLOAT] NULL,
	[count] [int] NULL,
	[maxCount] [int] NULL,
	[first] [varchar](1024) NULL,
	[last] [varchar](1024) NULL,
	[createdDate] [datetime] NOT NULL,
	[updatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_tbNodeMetricValue_Day] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Summary].[tbNodeMetricValue_Hour]') AND type in (N'U'))
CREATE TABLE [Summary].[tbNodeMetricValue_Hour](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[nodeId] [int] NOT NULL,
	[nodeMetricId] [int] NOT NULL,
	[nodeTypeId] [int] NOT NULL,
	[metricKey] [varchar](255) NOT NULL,
	[metricValueType] [varchar](50) NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[value] [varchar](1024) NULL,
	[valueType] [varchar](1024) NULL,
	[epochTimestamp] [bigint] NULL,
	[average] [FLOAT] NULL,
	[median] [FLOAT] NULL,
	[mode] [FLOAT] NULL,
	[stdDev] [FLOAT] NULL,
	[95pct] [FLOAT] NULL,
	[sum] [FLOAT] NULL,
	[min] [FLOAT] NULL,
	[max] [FLOAT] NULL,
	[count] [int] NULL,
	[maxCount] [int] NULL,
	[first] [varchar](1024) NULL,
	[last] [varchar](1024) NULL,
	[createdDate] [datetime] NOT NULL,
	[updatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_tbNodeMetricValue_Hour] PRIMARY KEY CLUSTERED 
(
	[nodeId] ASC,
	[nodeTypeId] ASC,
	[metricKey] ASC,
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Summary].[tbNodeStatus]') AND type in (N'U'))
CREATE TABLE [Summary].[tbNodeStatus](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[nodeId] [int] NOT NULL,
	[nodeTypeId] [int] NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[status] [bit] NOT NULL,
	[originalStatus] [bit] NOT NULL,
	[createdDate] [datetime] NOT NULL,
	[updatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_tbNodeStatus] PRIMARY KEY CLUSTERED 
(
	[nodeId] ASC,
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UN_Node_Timestamp_tbNodeStatus] UNIQUE NONCLUSTERED 
(
	[nodeId] ASC,
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbUser]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbUser](
	[id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[userId] [varchar](255) NOT NULL,
	[name] [varchar](255) NOT NULL,
	[active] [bit] NOT NULL,
	[hasAdminAccess] [bit] NOT NULL,
	[hasAccessToConfiguration] [bit] NOT NULL,
	[hasAccessToQueue] [bit] NOT NULL,
	[hasAccessToRefreshJob] [bit] NOT NULL,
	[hasAccessToUpdateClock] [bit] NOT NULL
 CONSTRAINT [PKtbUser] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UC_tbUser_userId] UNIQUE NONCLUSTERED 
(
	[userId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbActivityLogger]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbActivityLogger](
	[id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[userId] [int] NOT NULL,
	[activityId] [int] NOT NULL,
	[configurationId] [int] NULL,
	[oldValue] [varchar](255) NOT NULL,
	[newValue] [varchar](255) NOT NULL,
	[timestamp] [datetime] NOT NULL
 CONSTRAINT [PKtbActivityLogger] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UC_tbActivityLogger_timestamp] UNIQUE NONCLUSTERED 
(
	[timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbWebServer]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbWebServer](
	[id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[serverName] [varchar](255) NOT NULL,
	[serverIP] [varchar](255) NOT NULL,
	[active] [bit] NOT NULL,
	[InstanceName] [varchar](255) NOT NULL,
 CONSTRAINT [PKtbWebServer] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UC_tbWebServer_serverName] UNIQUE NONCLUSTERED 
(
	[serverName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO


IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbActivity]') AND type in (N'U'))
CREATE TABLE [Aggregation].[tbActivity](
	[id] [int] NOT NULL,
	[description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[Aggregation].[fk_tbJob_interval]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [Aggregation].[tbJob] WITH NOCHECK ADD CONSTRAINT [fk_tbJob_interval]   FOREIGN KEY (interval) 
REFERENCES [Aggregation].[tbInterval] (id); 

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[Aggregation].[fk_tbJob_JobType]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [Aggregation].[tbJob] WITH NOCHECK ADD CONSTRAINT [fk_tbJob_JobType]   FOREIGN KEY (jobType) 
REFERENCES [Aggregation].[tbJobType] (id); 

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[Aggregation].[fk_tbJob_priority]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [Aggregation].[tbJob] WITH NOCHECK ADD CONSTRAINT [fk_tbJob_priority]   FOREIGN KEY ([priority]) 
REFERENCES [Aggregation].[tbPriority] (id); 

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[Aggregation].[fk_tbJob_status]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [Aggregation].[tbJob] WITH NOCHECK ADD CONSTRAINT [fk_tbJob_status]   FOREIGN KEY ([status]) 
REFERENCES [Aggregation].[tbstatus] (id); 

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[Aggregation].[fk_tbJob_logs]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [Aggregation].[tbLogError] WITH NOCHECK ADD CONSTRAINT [fk_tbJob_logs]   FOREIGN KEY ([jobId]) 
REFERENCES [Aggregation].[tbJob] ([id]); 

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[Aggregation].[fk_tbJob_LogActivity]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [Aggregation].[tbLogActivity] WITH NOCHECK ADD CONSTRAINT [fk_tbJob_LogActivity]   FOREIGN KEY ([jobId]) 
REFERENCES [Aggregation].[tbJob] ([id]); 

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[Aggregation].[fk_tbActivityLogger_TypeId]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [Aggregation].[tbActivityLogger] WITH NOCHECK ADD CONSTRAINT [fk_tbActivityLogger_TypeId]   FOREIGN KEY (activityId) 
REFERENCES [Aggregation].[tbActivity] (id); 

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[Aggregation].[fk_tbActivityLogger_UserId]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [Aggregation].[tbActivityLogger] WITH NOCHECK ADD CONSTRAINT [fk_tbActivityLogger_UserId]   FOREIGN KEY (userId) 
REFERENCES [Aggregation].[tbUser] (id);

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[Aggregation].[fk_tbActivityLogger_ConfigurationId]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1)
ALTER TABLE [Aggregation].[tbActivityLogger] WITH NOCHECK ADD CONSTRAINT [fk_tbActivityLogger_ConfigurationId]   FOREIGN KEY (configurationId) 
REFERENCES [Aggregation].[tbConfiguration] (id);
