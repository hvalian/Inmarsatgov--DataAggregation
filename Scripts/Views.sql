IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwGetAggregatedBase]') AND type='v' )
DROP VIEW [Aggregation].[vwGetAggregatedBase]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwGetAggregatedMedian]') AND type='v' )
DROP VIEW [Aggregation].[vwGetAggregatedMedian]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwGetAggregatedPercentile]') AND type='v' )
DROP VIEW [Aggregation].[vwGetAggregatedPercentile]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwGetAggregatedStdDev]') AND type='v' )
DROP VIEW [Aggregation].[vwGetAggregatedStdDev]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwGetPreviousNodeStatusValue]') AND type='v' )
DROP VIEW [Aggregation].[vwGetPreviousNodeStatusValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwGetChildNodeStatus]') AND type='v' )
DROP VIEW [Aggregation].[vwGetChildNodeStatus]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwGetJobs]') AND type='v' )
DROP VIEW [Aggregation].[vwGetJobs]
GO

CREATE view [Aggregation].[vwGetAggregatedBase] as
SELECT [nodeTypeId], [metricKey], [nodeid],  [reportingtimestamp],
AVG(TRY_CAST(value AS FLOAT)) as average, 
Sum(TRY_CAST(value AS  FLOAT)) as sum, 
Min(TRY_CAST(value AS  FLOAT)) as min, 
Max(TRY_CAST(value AS  FLOAT)) as max, 
Count(value) as count,
Min(id) as 'first',
Max(id) as 'last'
FROM [Aggregation].[tbRawData]  
group by [nodeTypeId], [metricKey], [nodeid], [reportingtimestamp]
GO

CREATE view [Aggregation].[vwGetAggregatedMedian] as
SELECT DISTINCT [nodeTypeId], [nodeId], [metricKey], [reportingtimestamp], PERCENTILE_CONT(0.5) 
	WITHIN GROUP (ORDER BY TRY_CAST( value AS FLOAT))
	OVER (PARTITION BY [nodeTypeId],[nodeId],[metricKey],[reportingtimestamp] ) as 'median'
FROM [Aggregation].[tbRawData]
GO

CREATE view [Aggregation].[vwGetAggregatedPercentile] as
SELECT DISTINCT [nodeTypeId], [nodeId], [metricKey], [reportingtimestamp], PERCENTILE_CONT(0.95) 
	WITHIN GROUP (ORDER BY TRY_CAST( value AS FLOAT))
	OVER (PARTITION BY [nodeTypeId],[nodeId],[metricKey],[reportingtimestamp] ) as 'percentile'
FROM [Aggregation].[tbRawData]
GO

CREATE view [Aggregation].[vwGetAggregatedStdDev] as
SELECT [nodeTypeId], [nodeId], [metricKey], [reportingtimestamp], STDEV(TRY_CAST( value AS FLOAT)) as stdDev
FROM [Aggregation].[tbRawData]  
Group by [nodeTypeId], [nodeId], [metricKey], [reportingtimestamp]
GO

CREATE view [Aggregation].[vwGetChildNodeStatus] as
SELECT DISTINCT p.nodeId, o.timestamp, s.status
FROM  aggregation.tbObservations AS o INNER JOIN
[summary].[tbNodeStatus] s ON o.nodeId = s.nodeId AND o.timestamp = s.timestamp INNER JOIN
aggregation.rtbNodeParent p ON o.nodeId = p.parentnodeId
WHERE (s.[status] = 1)
GO

CREATE view [Aggregation].[vwGetPreviousNodeStatusValue] as
SELECT s.[id],[Aggregation].[fnGetPreviousNodeStatusValue] ( s.[nodeId],s.[nodeTypeId],s.[timestamp]) as elapsedTime, nt.defaultvalue
FROM [summary].[tbNodeStatus] s inner join 
	[Aggregation].[rtbNodeType] nt on s.nodeTypeId = nt.nodeTypeId
WHERE status = 1
GO

CREATE OR ALTER view [Aggregation].[vwGetDashbordData] as
SELECT AverageProcessingTime_HourlyJob, AverageProcessingTime_DailyJob,JobProcessingHasIssues, JobProcessingIsSuspended,LastErrJobAggregationDateTime,LastErrJobElapsedtime,LastErrJobId,
		LastErrJobInterval,LastErrJobStartDateTime,LastErrJobEndDateTime,LastErrJobStatus,LastJobAggregationDateTime,LastJobElapsedtime,LastJobId,LastJobInterval,LastJobStartDateTime,
		LastJobEndDateTime,LastJobStatus,NextJobAggregationDateTime,NextJobId,NextJobInterval,NextJobStartAfterDateTime,NextJobStartDateTime,NextJobStatus,NextJobWillStartInMinutes,
		CommandTimeout, Job_StartTimeDelay, Job_SuspendProcessingAfter, NumberOfDaysForStats, NumberOfRefresh, ProjectName, 
		Refresh_Enabled, Refresh_Interval, Retention_ActivityLog_NumberOfHours, SP_CommandTimeout
FROM aggregation.fnGetStats() 
GO

CREATE OR ALTER VIEW [Aggregation].[vwRNodeTypeMetric]
AS
Select Distinct NTM.[id],NTM.[nodeTypeId],NT.[description] As 'NodeType_Description', NTM.[metricKey],NM.[description]  As 'MetricKey_Description',NTM.[metricValueType],NTM.[dateCreated],NTM.[dateUpdated],NTM.[defaultValue],NTM.[usePreviousValue]
FROM [Aggregation].[rtbNodeTypeMetric] NTM inner join
	[Aggregation].[rtbNodeMetric] NM on NTM.metricKey = NM.metricKey inner Join
	[Aggregation].[rtbNodeType] NT on ntm.nodeTypeId = nt.nodeTypeId
Where NTM.[metricKey] <> '9999' and (
	(NTM.[nodeTypeId] = 2 and NTM.[metricKey] not in (select [metricKey] from [Aggregation].[tbNodeStatusConfig] where [nodeTypeId] = 2   and [include]=0))	OR 
	(NTM.[nodeTypeId] = 4 and NTM.[metricKey] not in (select [metricKey] from [Aggregation].[tbNodeStatusConfig] where [nodeTypeId] = 4   and [include]=0))	OR 
	(NTM.[nodeTypeId] =10 and NTM.[metricKey]     in (select [metricKey] from [Aggregation].[tbNodeStatusConfig] where [nodeTypeId] = 10  and [include]=1)) 
)
GO

CREATE VIEW [Aggregation].[vwGetJobs]
AS

SELECT J.id, T.description as 'JobType', J.computerName, j.status as 'StatusId', S.description AS 'Status', j.interval as 'IntervalId', I.description AS 'Interval', 
	j.priority as 'PriorityId',  P.description AS 'Priority', J.startDateTime, J.endDateTime, J.startAfterDateTime, J.processId, J.aggregationStartDateTime, J.aggregationEndDateTime, 
	J.createdDateTime, J.processed, J.isRecovery, J.createdBy, 	j.exitCode,
	aggregation.fnGetListOfJobs(J.id, J.aggregationStartDateTime, J.aggregationEndDateTime) AS listOfJobs
FROM  aggregation.tbJob J INNER JOIN
	aggregation.tbJobType T ON J.jobType = T.id INNER JOIN
	aggregation.tbPriority P ON J.priority = p.id INNER JOIN
	aggregation.tbStatus S ON J.status = s.id INNER JOIN
	aggregation.tbInterval I ON J.interval = i.id
WHERE J.id IN (
	SELECT MAX(id) AS Expr1
	FROM aggregation.tbJob AS tbJob_1
	WHERE [status] IN (1,2,3)
	GROUP BY aggregationStartDateTime, aggregationEndDateTime
)
GO