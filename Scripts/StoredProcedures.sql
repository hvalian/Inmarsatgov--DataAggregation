IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spCleanUpDaily]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spCleanUpDaily]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spCleanUpHourly]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spCleanUpHourly]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spPopulate5Minute]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spPopulate5Minute]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spPopulateDaily]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spPopulateDaily]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spPopulateHourly]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spPopulateHourly]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spPopulateNodeStatus]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spPopulateNodeStatus]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spPopulateStaging]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].spPopulateStaging
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spMergeRefTables]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spMergeRefTables]
GO

CREATE PROCEDURE [Aggregation].[spCleanUpHourly]
	@jobId bigint = null
AS
BEGIN
	TRUNCATE TABLE [Aggregation].[tbRawData];
END
GO

CREATE PROCEDURE [Aggregation].[spCleanUpDaily]
	@jobId bigint = null
AS
BEGIN
	exec [Aggregation].[spCleanUpHourly] @jobid;

	TRUNCATE TABLE [Aggregation].[tbRawDataDaily];
END
GO

CREATE PROCEDURE [Aggregation].[spPopulate5Minute]
	@jobId bigint = null,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Counter int = 0,
	@nodeStatusMetricKey  varchar(255)

	SELECT  @nodeStatusMetricKey=value
	FROM [Aggregation].[tbConfiguration]
	where [key] = 'NodeStatus';

	TRUNCATE TABLE [Aggregation].[tbObservationsTemplate];
	
	WHILE @Counter< = 11
	BEGIN
		INSERT INTO [Aggregation].[tbObservationsTemplate] 
		SELECT DATEADD(MINUTE, (@Counter*5), @fromTimeStamp)
		SET @Counter= @Counter + 1
	END

	Insert into [summary].[tbnodemetricvalue_5Minute] ([nodeId], [nodeMetricId], [nodeTypeId], [metricKey],  [metricValueType], [timestamp],[value],[usePreviousValue],[originalStatus],[updatedDate]) 
	select q.nodeid, q.[nodeMetricId], q.[nodeTypeId], @nodeStatusMetricKey, 'bit', q.[timestamp],0,0,0,GETDATE()
	from [summary].[tbnodemetricvalue_5Minute] s right join
	(
		SELECT n.[nodeId] as nodeid, nm.[nodeMetricId], ntm.[nodeTypeId], ntm.[metricKey], ntm.[metricValueType], t.[timestamp]
		FROM [Aggregation].[rtbNode] n with (nolock) inner join
		[Aggregation].[rtbNodeTypeMetric]  ntm with (nolock) on  n.nodeTypeId = ntm.nodeTypeId inner join
		[Aggregation].[rtbNodeMetric] nm with (nolock) on n.[nodeId]=nm.[nodeId] and ntm.[metricKey] = nm.[metricKey] and n.[nodeTypeId] = nm.[nodeTypeId] cross join
		[Aggregation].[tbObservationsTemplate] t
		where  [activationDate] <@fromTimeStamp and ([deactivationDate] is null or [deactivationDate]>@toTimeStamp) and ntm.metricKey = @nodeStatusMetricKey
			and ntm.[rollUpAggregateConfig] is not null
	) q on s.[nodeId] = q.[nodeId] and s.[metricKey] = q.[metricKey] and s.[timestamp] = q.[timestamp]
	where s.nodeid is null
	order by q.[nodeTypeId], q.nodeid, q.[metricKey], q.[timestamp];

	UPDATE [summary].[tbnodemetricvalue_5Minute] 
	SET [value]=q.[status], [updatedDate] = GETDATE()
	FROM [summary].[tbnodemetricvalue_5Minute] o inner join 
	(
		SELECT s.[nodeId], @nodeStatusMetricKey as [metricKey], [timestamp], [status]
		FROM [summary].[tbNodeStatus] s left join
			[Aggregation].[rtbNodeMetric] nm on nm.nodeId = s.nodeId and nm.metricKey = @nodeStatusMetricKey
		where [timestamp] >= @fromTimeStamp and [timestamp] < @toTimeStamp
	) q on o.[nodeId]=q.[nodeId] and o.[metricKey] = @nodeStatusMetricKey and o.[timestamp] = q.[timestamp];

	Insert into [summary].[tbnodemetricvalue_5Minute] ([nodeId], [nodeMetricId], [nodeTypeId], [metricKey], [metricValueType], [timestamp],[usePreviousValue],[originalStatus],[updatedDate]) 
	select q.nodeid, q.[nodeMetricId], q.[nodeTypeId], q.[metricKey], q.[metricValueType], q.[timestamp],0,0,GETDATE()
	from [summary].[tbnodemetricvalue_5Minute] s right join
	(
		SELECT n.[nodeId] as nodeid, nm.[nodeMetricId], ntm.[nodeTypeId], ntm.[metricKey], ntm.[metricValueType], t.[timestamp]
		FROM [Aggregation].[rtbNode] n with (nolock) inner join
		[Aggregation].[rtbNodeTypeMetric]  ntm with (nolock) on  n.nodeTypeId = ntm.nodeTypeId inner join
		[Aggregation].[rtbNodeMetric] nm with (nolock) on n.[nodeId]=nm.[nodeId] and ntm.[metricKey] = nm.[metricKey] and n.[nodeTypeId] = nm.[nodeTypeId] cross join
		[Aggregation].[tbObservationsTemplate] t
		where  [activationDate] <@fromTimeStamp and ([deactivationDate] is null or [deactivationDate]>@toTimeStamp) and ntm.metricKey <> @nodeStatusMetricKey
			and ntm.[rollUpAggregateConfig] is not null
	) q on s.[nodeId] = q.[nodeId] and s.[metricKey] = q.[metricKey] and s.[timestamp] = q.[timestamp]
	where s.nodeid is null
	order by q.[nodeTypeId], q.nodeid, q.[metricKey], q.[timestamp];

	truncate table [Aggregation].[tbObservations];

	Insert into [Aggregation].[tbObservations] ([nodeId], [nodeMetricId], [nodeTypeId], [metricKey], [metricValueType], [timestamp], [usePreviousValue], [hasObservation]) 
	SELECT n.[nodeId], nm.[nodeMetricId], ntm.[nodeTypeId], ntm.[metricKey], ntm.[metricValueType], t.[timestamp], 0, 0
	FROM [Aggregation].[rtbNode] n with (nolock) inner join
	[Aggregation].[rtbNodeTypeMetric]  ntm with (nolock) on  n.nodeTypeId = ntm.nodeTypeId inner join
	[Aggregation].[rtbNodeMetric] nm with (nolock) on n.[nodeId]=nm.[nodeId] and ntm.[metricKey] = nm.[metricKey] and n.[nodeTypeId] = nm.[nodeTypeId] cross join
	[Aggregation].[tbObservationsTemplate] t
	where  [activationDate] <@fromTimeStamp and ([deactivationDate] is null or [deactivationDate]>@toTimeStamp) and ntm.metricKey <> @nodeStatusMetricKey
		and ntm.[rollUpAggregateConfig] is not null 
	order by n.id, ntm.[nodeTypeId],ntm.[metricKey];

	truncate table [Aggregation].[tbObservationsTemplate];

	UPDATE [Aggregation].[tbObservations] 
	SET [observationid] = 0, [average]=q.[average], [stdDev]=q.[stdDev], [95pct]=q.[percentile], [sum]=q.[sum], [min]=q.[min], [max]=q.[max], [count]=q.[count], 
	[median] = q. [median], [mode] = q.[mode], maxcount = q. maxcount, [first]=q.[first], [last]=q.[last], [hasObservation]=1
	FROM [Aggregation].[tbObservations] o inner join 
	(
		SELECT b.[nodeTypeId], b.[nodeid], b.[metricKey], b.[reportingtimestamp] as 'timestamp', b.[average], b.[sum], b.[min], b.[max], b.[count], [percentile], [stdDev], m.median,
		[Aggregation].[fnGetBaseModeValue](b.[nodeTypeId], b.[metricKey], b.[nodeId], @fromTimeStamp, @toTimeStamp) as 'mode',
		[Aggregation].[fnGetBaseMaxCountValue] (b.[nodeTypeId], b.[metricKey], b.[nodeId], @fromTimeStamp, @toTimeStamp) as 'maxcount',f.value as 'first', l.value as 'last'
		FROM [Aggregation].[vwGetAggregatedBase] b inner join
			[Aggregation].[vwGetAggregatedPercentile] p on b.[nodeTypeId] = p.[nodeTypeId]  and b.[nodeId] =  p.[nodeId]  and b.[metricKey] = p.[metricKey]  and b.[reportingtimestamp] = p.[reportingtimestamp] inner join
			[Aggregation].[vwGetAggregatedStdDev] sd on b.[nodeTypeId] = sd.[nodeTypeId] and b.[nodeId] =  sd.[nodeId] and b.[metricKey] = sd.[metricKey] and b.[reportingtimestamp] = sd.[reportingtimestamp] inner join
			[Aggregation].[vwGetAggregatedMedian] m on b.[nodeTypeId] = m.[nodeTypeId]  and b.[nodeId] =  m.[nodeId]  and b.[metricKey] = m.[metricKey]  and b.[reportingtimestamp] = m.[reportingtimestamp] inner join
			[Aggregation].[tbRawData] f on f.id = b.first inner join
			[Aggregation].[tbRawData] l on l.id = b.last 
	) q on o.[nodeId]=q.[nodeId] and o.[metricKey] = q.[metricKey] and o.[timestamp] = q.[timestamp]
	where [hasObservation]=0;

	UPDATE [Aggregation].[tbObservations]
	SET [value] = q.[Value]
	FROM [Aggregation].[tbObservations] o inner join 
	(
		SELECT  [nodeId], h.[metricKey], [timestamp], [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') as [ValueType], 
		CASE
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'avg' THEN LTRIM(STR(h.[average], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'median' THEN LTRIM(STR(h.median, 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'mode' THEN LTRIM(STR(h.[mode], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'stdDev' THEN LTRIM(STR(h.stdDev, 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = '95pct' THEN LTRIM(STR(h.[95pct], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'sum' THEN LTRIM(STR(h.[sum], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'min' THEN LTRIM(STR(h.[min], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'max' THEN LTRIM(STR(h.[max], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'count' THEN LTRIM(STR(h.[count], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'first' THEN h.first
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'last' THEN h.last
			ELSE null
		END as [Value]
		FROM [Aggregation].[tbObservations] h inner join 
			[Aggregation].[rtbNodeTypeMetric] ntm with (nolock) on h.nodeTypeId = ntm.nodeTypeId and h.metricKey = ntm.metricKey
		Where ntm.metricKey <> @nodeStatusMetricKey and [rollUpAggregateConfig] is not null 
	) q on o.[nodeId]=q.[nodeId] and o.[metricKey] = q.[metricKey] and o.[timestamp] = q.[timestamp] and o.[hasObservation]=1;

	UPDATE [Aggregation].[tbObservations] 
	SET [value]  = q.[defaultValue]
	FROM [Aggregation].[tbObservations] o inner join 
	(
		SELECT distinct o.[nodeId],o. [metricKey], dv.defaultValue
		FROM [Aggregation].[tbObservations] o inner join
		[Aggregation].[rtbNodeTypeMetric] dv on o.[nodeTypeId] = dv.[nodeTypeId] and o.[metricKey] = dv.[metricKey]
		where o.[observationid] is null
	) q on o.[nodeId]=q.[nodeId] and o.[metricKey] = q.[metricKey] 
	where o.[observationid] is null;

	UPDATE [Aggregation].[tbObservations]	
	SET [usePreviousValue]=1
	From [Aggregation].tbObservations o inner join
	  [summary].[tbNodeStatus] s on o.nodeId = s.nodeId and o.[timestamp] = s.[timestamp] inner join
	  [Aggregation].[rtbNodeTypeMetric] dv on o.[nodeTypeId] = dv.[nodeTypeId] and o.[metricKey] = dv.[metricKey]
	Where o.nodeId = s.nodeId and o.[timestamp] = s.[timestamp] and  o.[observationid] is null and dv.[usePreviousValue] = 1 and s.[status]=1;

	UPDATE [Aggregation].[tbObservations]	
	SET [usePreviousValue]=1
	Where [usePreviousValue] = 0 and Id in (
		SELECT o.id
		FROM aggregation.tbObservations o INNER JOIN
			aggregation.rtbNodeTypeMetric dv ON o.nodeTypeId = dv.nodeTypeId AND o.metricKey = dv.metricKey INNER JOIN
			aggregation.vwGetChildNodeStatus s ON o.nodeId = s.nodeId AND o.[timestamp] = s.[timestamp]
		WHERE o.timestamp >= @fromTimeStamp and o.timestamp < @toTimeStamp and  o.[observationid] is null and dv.usePreviousValue = 1 AND s.[status] = 1 AND o.usePreviousValue = 0 
	);

	update [Aggregation].[tbObservations]
	set [value] = null
	where [value] = 'NULL';

	DECLARE @id bigint,
	@nodeid int,
	@nodetypeid int,
	@metricKey varchar(255),
	@timestamp datetime,
	@aggregateType varchar(255),
	@value varchar(1024)

	DECLARE observation_cursor CURSOR FOR 
	SELECT  o.[id], o.[nodeId], o.[nodeTypeId], o.[metricKey], [timestamp], [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') as 'aggregateType'
	FROM [Aggregation].[tbObservations] o inner join 
		[Aggregation].[rtbNodeTypeMetric] ntm with (nolock) on o.nodeTypeId = ntm.nodeTypeId and o.metricKey = ntm.metricKey
	WHERE o.[usePreviousValue] = 1 and [hasObservation] = 0 and [observationid] is null 
	order by o.[nodeId], o.[nodeTypeId], o.[metricKey], [timestamp]

	OPEN observation_cursor 
	FETCH NEXT FROM observation_cursor INTO @id, @nodeId, @nodeTypeId, @metricKey, @timestamp, @aggregateType

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		set @value = [Aggregation].[fnGetLastObservationValue] (@nodeId, @nodeTypeId, @metricKey, @timestamp)

		if @aggregateType='avg'
		begin
			update [Aggregation].[tbObservations] set [value] = @value, [average] = @value where id = @id
		end
		if @aggregateType='sum'
		begin
			update [Aggregation].[tbObservations] set [value] = @value, [sum] = @value where id = @id
		end
		if @aggregateType='median'
		begin
			update [Aggregation].[tbObservations] set [value] = @value, [median] = @value where id = @id
		end
		if @aggregateType='mode'
		begin
			update [Aggregation].[tbObservations] set [value] = @value, [mode] = @value where id = @id
		end
		if @aggregateType='stdDev'
		begin
			update [Aggregation].[tbObservations] set [value] = @value, [stdDev] = @value where id = @id
		end
		if @aggregateType='95pct'
		begin
			update [Aggregation].[tbObservations] set [value] = @value, [95pct] = @value where id = @id
		end
		if @aggregateType='min'
		begin
			update [Aggregation].[tbObservations] set [value] = @value, [min] = @value where id = @id
		end
		if @aggregateType='max'
		begin
			update [Aggregation].[tbObservations] set [value] = @value, [max] = @value where id = @id
		end
		if @aggregateType='count'
		begin
			update [Aggregation].[tbObservations] set [value] = @value, [count] = @value where id = @id
		end
		if @aggregateType='first'
		begin
			update [Aggregation].[tbObservations] set [value] = @value, [first] = @value where id = @id
		end
		if @aggregateType='last'
		begin
			update [Aggregation].[tbObservations] set [value] = @value, [last] = @value where id = @id
		end

		FETCH NEXT FROM observation_cursor INTO @id, @nodeId, @nodeTypeId, @metricKey, @timestamp, @aggregateType
	END 
	CLOSE observation_cursor  
	DEALLOCATE observation_cursor

	UPDATE [summary].[tbnodemetricvalue_5Minute] 
	SET [value]=q.[value], [usePreviousValue]=q.[usePreviousValue], [originalStatus] = 0, [average]=q.[average], [stdDev]=q.[stdDev], [95pct]=q.[95pct], [sum]=q.[sum], [min]=q.[min], 
		[max]=q.[max], [count]=q.[count], [median] = q. [median], [mode] = q.[mode], maxcount = q. maxcount, [first]=q.[first], [last]=q.[last], [updatedDate] = GETDATE()
	FROM [summary].[tbnodemetricvalue_5Minute] o inner join 
	(
	    Select [id], [nodeId], [nodeMetricId], [nodeTypeId], [metricKey], [metricValueType], [timestamp], [value], [usePreviousValue], 
		[average],[median],[mode],[stdDev],[95pct],[sum],[min],[max],[count],[maxCount],[first],[last]
		From [Aggregation].[tbObservations]
		where [timestamp] >= @fromTimeStamp and [timestamp] < @toTimeStamp and [metricKey]<>@nodeStatusMetricKey
	) q on o.[nodeId]=q.[nodeId] and o.[metricKey] = q.[metricKey] and o.[timestamp] = q.[timestamp];

	truncate table [Aggregation].[tbObservations];

END
GO

CREATE PROCEDURE [Aggregation].[spPopulateDaily]
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	
	declare @nodeStatusMetricKey  varchar(255),
	@epochTimestamp bigint=0

	SELECT @epochTimestamp = CAST(DATEDIFF(s,'1970-01-01 00:00:00', @fromTimeStamp) as bigint)

	SELECT  @nodeStatusMetricKey=value
	FROM [Aggregation].[tbConfiguration]
	where [key] = 'NodeStatus';

	Insert into [summary].[tbNodeMetricValue_Day] ([nodeId], [nodeMetricId], [nodeTypeId], [metricKey],  [metricValueType], [timestamp],[createdDate], [updatedDate])
	select q.[nodeId], q.[nodeMetricId], q.[nodeTypeId], q.[metricKey], q.[metricValueType], q.[timestamp], GETDATE(), GETDATE()
	from [summary].[tbNodeMetricValue_Day] s right join
	(
		SELECT distinct [nodeId], [nodeMetricId], [nodeTypeId], [metricKey],  [metricValueType], @fromTimeStamp as [timestamp]
		FROM [summary].[tbnodemetricvalue_5Minute]
		where timestamp>=@fromTimeStamp and timestamp < @toTimeStamp
	) q on s.[nodeId] = q.[nodeId] and s.[metricKey] = q.[metricKey] and s.[timestamp] = @fromTimeStamp
	where s.nodeid is null
	order by q.[nodeTypeId], q.nodeid, q.[metricKey], q.[timestamp];

	UPDATE [summary].[tbNodeMetricValue_Day]
	SET [average] = q.[average], [stdDev] = q.[stdDev], [95pct] = q.[95pct], [sum] = q.[sum], [min] = q.[min], [max] = q.[max], [count]=q.[count], 
		[median] = q.[median], [mode] = q.[mode], maxcount = q.maxcount, [first] = q.[first], [last] = q.[last], [updatedDate] = GETDATE(), [epochTimestamp] = @epochTimestamp
	FROM [summary].[tbNodeMetricValue_Day] o inner join 
	(
		SELECT [nodeId],[nodeMetricId],[nodeTypeId],[metricKey],[metricValueType]
		,cast(NULLIF(sum([average]),0)/NULLIF(count([average]),0) as decimal(25,3)) as 'average'
		,cast([Aggregation].[fnGetMedianValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp) as decimal(25,3)) as [median]
		,cast([Aggregation].[fnGetModeValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp) as decimal(25,3)) as [mode]
		,cast([Aggregation].[fnGetDailyStdDevValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp) as decimal(25,3)) as [stdDev]
		,cast([Aggregation].[fnGetDaily95pctValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp)  as decimal(25,3)) as [95pct]
		,cast(sum([sum]) as decimal(25,3)) as 'sum'
		,cast(min([min]) as decimal(25,3)) as 'min'
		,cast(max([max]) as decimal(25,3)) as 'max'
		,288 as 'count'
		,[Aggregation].[fnGetMaxCountValue]([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp ) as [maxCount]
		,[Aggregation].[fnGetFirstValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp ) as [first]
		,[Aggregation].[fnGetLastValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp ) as [last]
		FROM [summary].[tbnodemetricvalue_5Minute]
		where timestamp>=@fromTimeStamp and timestamp<@toTimeStamp and metricKey<>@nodeStatusMetricKey
		group by [nodeId],[nodeMetricId],[nodeTypeId],[metricKey],[metricValueType]
	) q on o.[nodeId]=q.[nodeId] and o.[metricKey] = q.[metricKey] and o.[timestamp] = @fromTimeStamp;

	UPDATE [summary].[tbNodeMetricValue_Day]
	SET [value] = q.[Value], valueType = q.[ValueType], [updatedDate] = GETDATE()
	FROM [summary].[tbNodeMetricValue_Day] o inner join 
	(
		SELECT  [nodeId], h.[metricKey], [timestamp], [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') as [ValueType], 
		CASE
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'avg' THEN LTRIM(STR(h.[average], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'median' THEN LTRIM(STR(h.median, 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'mode' THEN LTRIM(STR(h.[mode], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'stdDev' THEN LTRIM(STR(h.stdDev, 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = '95pct' THEN LTRIM(STR(h.[95pct], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'sum' THEN LTRIM(STR(h.[sum], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'min' THEN LTRIM(STR(h.[min], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'max' THEN LTRIM(STR(h.[max], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'count' THEN LTRIM(STR(h.[count], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'first' THEN h.first
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'last' THEN h.last
			ELSE null
		END as [Value]
		FROM [summary].[tbNodeMetricValue_Day] h inner join 
			[Aggregation].[rtbNodeTypeMetric] ntm with (nolock) on h.nodeTypeId = ntm.nodeTypeId and h.metricKey = ntm.metricKey
		Where timestamp>=@fromTimeStamp and timestamp < @toTimeStamp and ntm.metricKey <> @nodeStatusMetricKey and [rollUpAggregateConfig] is not null 
	) q on o.[nodeId]=q.[nodeId] and o.[metricKey] = q.[metricKey] and o.[timestamp] = @fromTimeStamp;

	TRUNCATE TABLE [Aggregation].[tbNodeStatusTemp];

	INSERT INTO [Aggregation].[tbNodeStatusTemp]
	SELECT nodeid, [nodeTypeId], 0, sum(CASE [status] WHEN 1 THEN 1 else 0 END)*100/12/24 
	FROM [summary].tbNodeStatus 
	where timestamp>= @fromTimeStamp and timestamp<@toTimeStamp 
	group by nodeid, [nodeTypeId];

	update [Aggregation].[tbNodeStatusTemp]
	set [value] = 1
	where average<>'0';

	UPDATE [summary].[tbNodeMetricValue_Day] 
	SET  [value]=q.[value],  [nodeMetricId]=q.[nodeMetricId], [metricValueType] = 'bit', [timestamp] =@fromTimeStamp , [valueType] = 'avg', [epochTimestamp]=@epochTimestamp, [updatedDate]=GETDATE()
	FROM [summary].[tbNodeMetricValue_Day] o inner join 
	(
		select ns.[nodeId] as [nodeId], ns.[average] as [value], ns.[nodeTypeId] as [nodeTypeId], nm.[nodeMetricId]
		from [Aggregation].[tbNodeStatusTemp] ns inner join
		[Aggregation].[rtbNodeTypeMetric]  ntm with (nolock) on  ns.nodeTypeId = ntm.nodeTypeId inner join
		[Aggregation].[rtbNodeMetric] nm with (nolock) on ns.[nodeId]=nm.[nodeId] and nm.[metricKey] = @nodeStatusMetricKey and ns.[nodeTypeId] = nm.[nodeTypeId]
	) q on o.[nodeId]=q.[nodeId] and o.[metricKey] = @nodeStatusMetricKey and o.[timestamp] = @fromTimeStamp;

	TRUNCATE TABLE [Aggregation].[tbNodeStatusTemp];
END
GO

CREATE PROCEDURE [Aggregation].[spPopulateHourly]
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	declare @nodeStatusMetricKey  varchar(255),
	@epochTimestamp bigint=0

	SELECT @epochTimestamp = CAST(DATEDIFF(s,'1970-01-01 00:00:00', @fromTimeStamp) as bigint)

	SELECT  @nodeStatusMetricKey=value
	FROM [Aggregation].[tbConfiguration]
	where [key] = 'NodeStatus';

	Insert into [summary].[tbNodeMetricValue_Hour] ([nodeId], [nodeMetricId], [nodeTypeId], [metricKey],  [metricValueType], [timestamp],[createdDate], [updatedDate])
	select q.[nodeId], q.[nodeMetricId], q.[nodeTypeId], q.[metricKey], q.[metricValueType], q.[timestamp], GETDATE(), GETDATE()
	from [summary].[tbNodeMetricValue_Hour] s right join
	(
		SELECT distinct [nodeId], [nodeMetricId], [nodeTypeId], [metricKey],  [metricValueType], @fromTimeStamp as [timestamp]
		FROM [summary].[tbnodemetricvalue_5Minute]
		where timestamp>=@fromTimeStamp and timestamp < @toTimeStamp
	) q on s.[nodeId] = q.[nodeId] and s.[metricKey] = q.[metricKey] and s.[timestamp] = @fromTimeStamp
	where s.nodeid is null
	order by q.[nodeTypeId], q.nodeid, q.[metricKey], q.[timestamp];

	UPDATE [summary].[tbNodeMetricValue_Hour]
	SET [average] = q.[average], [stdDev] = q.[stdDev], [95pct] = q.[95pct], [sum] = q.[sum], [min] = q.[min], [max] = q.[max], [count]=q.[count], 
		[median] = q.[median], [mode] = q.[mode], maxcount = q.maxcount, [first] = q.[first], [last] = q.[last], [updatedDate] = GETDATE(), [epochTimestamp] = @epochTimestamp
	FROM [summary].[tbNodeMetricValue_Hour] o inner join 
	(
		SELECT [nodeId],[nodeMetricId],[nodeTypeId],[metricKey],[metricValueType]
		,cast(NULLIF(sum([average]),0)/NULLIF(count([average]),0) as decimal(25,3)) as 'average'
		,cast([Aggregation].[fnGetMedianValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp) as decimal(25,3)) as [median]
		,cast([Aggregation].[fnGetModeValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp) as decimal(25,3)) as [mode]
		,cast([Aggregation].[fnGetStdDevValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp) as decimal(25,3)) as [stdDev]
		,cast([Aggregation].[fnGet95pctValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp)  as decimal(25,3)) as [95pct]
		,cast(sum([sum]) as decimal(25,3)) as 'sum'
		,cast(min([min]) as decimal(25,3)) as 'min'
		,cast(max([max]) as decimal(25,3)) as 'max'
		,12 as 'count'
		,[Aggregation].[fnGetMaxCountValue]([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp ) as [maxCount]
		,[Aggregation].[fnGetFirstValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp ) as [first]
		,[Aggregation].[fnGetLastValue] ([nodeTypeId],[metricKey],[nodeId],@fromTimeStamp ,@toTimeStamp ) as [last]
		FROM [summary].[tbnodemetricvalue_5Minute]
		where timestamp>=@fromTimeStamp and timestamp<@toTimeStamp and metricKey<>@nodeStatusMetricKey
		group by [nodeId],[nodeMetricId],[nodeTypeId],[metricKey],[metricValueType]
	) q on o.[nodeId]=q.[nodeId] and o.[metricKey] = q.[metricKey] and o.[timestamp] = @fromTimeStamp;

	UPDATE [summary].[tbNodeMetricValue_Hour]
	SET [value] = q.[Value], valueType = q.[ValueType], [updatedDate] = GETDATE()
	FROM [summary].[tbNodeMetricValue_Hour] o inner join 
	(
		SELECT  [nodeId], h.[metricKey], [timestamp], [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') as [ValueType], 
		CASE
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'avg' THEN LTRIM(STR(h.[average], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'median' THEN LTRIM(STR(h.median, 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'mode' THEN LTRIM(STR(h.[mode], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'stdDev' THEN LTRIM(STR(h.stdDev, 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = '95pct' THEN LTRIM(STR(h.[95pct], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'sum' THEN LTRIM(STR(h.[sum], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'min' THEN LTRIM(STR(h.[min], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'max' THEN LTRIM(STR(h.[max], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'count' THEN LTRIM(STR(h.[count], 25, 3))
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'first' THEN h.first
			WHEN  [rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') = 'last' THEN h.last
			ELSE null
		END as [Value]
		FROM [summary].[tbNodeMetricValue_Hour] h inner join 
			[Aggregation].[rtbNodeTypeMetric] ntm with (nolock) on h.nodeTypeId = ntm.nodeTypeId and h.metricKey = ntm.metricKey
		Where timestamp>=@fromTimeStamp and timestamp < @toTimeStamp and ntm.metricKey <> @nodeStatusMetricKey and [rollUpAggregateConfig] is not null 
	) q on o.[nodeId]=q.[nodeId] and o.[metricKey] = q.[metricKey] and o.[timestamp] = @fromTimeStamp;

	TRUNCATE TABLE [Aggregation].[tbNodeStatusTemp];

	INSERT INTO [Aggregation].[tbNodeStatusTemp]
	SELECT nodeid, [nodeTypeId], 0, sum(CASE [status] WHEN 1 THEN 1 else 0 END)*100/12 
	FROM [summary].tbNodeStatus 
	where timestamp>= @fromTimeStamp and timestamp<@toTimeStamp 
	group by nodeid, [nodeTypeId];

	update [Aggregation].[tbNodeStatusTemp]
	set [value] = 1
	where average<>'0';

	UPDATE [summary].[tbNodeMetricValue_Hour] 
	SET  [value]=q.[value],  [nodeMetricId]=q.[nodeMetricId], [metricValueType] = 'bit', [timestamp] =@fromTimeStamp , [valueType] = 'avg', [epochTimestamp]=@epochTimestamp, [updatedDate]=GETDATE()
	FROM [summary].[tbNodeMetricValue_Hour] o inner join 
	(
		select ns.[nodeId] as [nodeId], ns.[average] as [value], ns.[nodeTypeId] as [nodeTypeId], nm.[nodeMetricId]
		from [Aggregation].[tbNodeStatusTemp] ns inner join
		[Aggregation].[rtbNodeTypeMetric]  ntm with (nolock) on  ns.nodeTypeId = ntm.nodeTypeId inner join
		[Aggregation].[rtbNodeMetric] nm with (nolock) on ns.[nodeId]=nm.[nodeId] and nm.[metricKey] = @nodeStatusMetricKey and ns.[nodeTypeId] = nm.[nodeTypeId]
	) q on o.[nodeId]=q.[nodeId] and o.[metricKey] = @nodeStatusMetricKey and o.[timestamp] = @fromTimeStamp;

	TRUNCATE TABLE [Aggregation].[tbNodeStatusTemp];

	DELETE FROM [Aggregation].[tbRawDataDaily]
	where  timestamp >= @fromTimeStamp and timestamp < @toTimeStamp;

	Insert Into [Aggregation].[tbRawDataDaily] ([nodeId], [nodeTypeId], [metricKey], [timestamp], [reportingtimestamp], [value], [isbackFilled])
	SELECT [nodeId],[nodeTypeId],[metricKey],[timestamp], [Aggregation].[fnGetMinute] ([timestamp]), [value], [isbackFilled]
	FROM [Aggregation].[tbRawData]
	where  timestamp >= @fromTimeStamp and timestamp < @toTimeStamp;
END
GO

CREATE PROCEDURE [Aggregation].[spPopulateStaging]
	@jobId bigint = null,
	@fromTimeStamp DATETIME =  null , 
	@toTimeStamp   DATETIME = null
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Rows int,
		@nodeStatusMetricKey  varchar(255),
		@HHMMSS varchar(8)

	SELECT  @nodeStatusMetricKey=value
	FROM [Aggregation].[tbConfiguration]
	where [key] = 'NodeStatus';

	SELECT @HHMMSS = convert(varchar, getdate(), 108);

	if @HHMMSS = '00:00:00'
	begin
		TRUNCATE TABLE [Aggregation].[tbRawDataDaily];
	end

	TRUNCATE TABLE [Aggregation].[tbRawData];

	Insert Into [Aggregation].[tbRawData] ([nodeId], [nodeTypeId], [metricKey], [timestamp], [reportingtimestamp], [value], [isbackFilled])
	SELECT [nodeId],[nodeTypeId],[metricKey],[timestamp], [Aggregation].[fnGetMinute] ([timestamp]), [value], [isbackFilled]
	FROM [Aggregation].[vwNodeMetricValue]
	where  timestamp >= @fromTimeStamp and timestamp < @toTimeStamp and
		nodeId in (
			SELECT  [nodeId]
			FROM [Aggregation].[rtbNode] n with (nolock)  
			where  [activationDate] <@fromTimeStamp and ([deactivationDate] is null or [deactivationDate]>@toTimeStamp)
		) and [metricKey] in (
			SELECT [metricKey]
			FROM [Aggregation].[rtbNodeTypeMetric]  with (nolock) 
			Where metricKey <>  @nodeStatusMetricKey and [rollUpAggregateConfig] is not null
		)

	SELECT @Rows=@@ROWCOUNT;
	update [Aggregation].[tbJob] set [processed] = @Rows where id = @jobId;
END
GO

CREATE PROCEDURE [Aggregation].[spPopulateNodeStatus]
	@jobId bigint = null,
	@fromTimeStamp DATETIME =  null , 
	@toTimeStamp   DATETIME = null
AS
BEGIN
	SET NOCOUNT ON;

	declare @nodeid int,
		@nodeTypeId int,
		@Counter int= 0,
		@timeStamp DATETIME,
		@nodeStatusMetricKey  varchar(255)

	SELECT  @nodeStatusMetricKey=value
	FROM [Aggregation].[tbConfiguration]
	where [key] = 'NodeStatus';

	TRUNCATE TABLE [Aggregation].[tbObservationsTemplate];
	
	set @Counter = 0
	WHILE @Counter <= 11
	BEGIN
		INSERT INTO [Aggregation].[tbObservationsTemplate] 
		SELECT DATEADD(MINUTE, (@Counter*5), @fromTimeStamp)
		SET @Counter= @Counter + 1
	END

	Insert into [summary].[tbNodeStatus] ([nodeId], [nodeTypeId], [timestamp], [status], [originalStatus], [createdDate], [updatedDate]) 
	Select q.[nodeId],q.[nodeTypeId],q.[timestamp],q.[status],q.[originalStatus],q.[createdDate],q.[updatedDate]
	FROM [summary].[tbNodeStatus] s right join
	(
		SELECT [nodeId], [nodeTypeId], t.[timestamp], 0 as [status], 0 as [originalStatus], GETDATE() as [createdDate], GETDATE() as [updatedDate]
		FROM [Aggregation].[rtbNode] n  with (nolock) cross join
			[Aggregation].[tbObservationsTemplate] t
		where  [activationDate] < @fromTimeStamp and ([deactivationDate] is null or [deactivationDate]>@toTimeStamp) and [nodeTypeId] in (
			SELECT distinct [nodeTypeId] FROM [Aggregation].[tbNodeStatusConfig])
	) q on s.[nodeId] = q.[nodeId] and s.[nodeTypeId] = q.[nodeTypeId] and s.[timestamp] = q.[timestamp]
	where s.nodeid is null
	order by q.[nodeTypeId], q.nodeid,  q.[timestamp];

	TRUNCATE TABLE [Aggregation].[tbObservationsTemplate];

	declare @occurance int = 1
	WHILE @occurance <= 3
		BEGIN
			SET @nodeTypeId = CASE @occurance
                    WHEN 1 THEN 2
                    WHEN 2 THEN 4
                    WHEN 3 THEN 10
                END 

			set @Counter = 0
			WHILE @Counter <= 11
			BEGIN
				Update [summary].[tbNodeStatus]
				Set [status] = 1, [originalStatus] = 1, [updatedDate]=GETDATE()
				FROM [summary].[tbNodeStatus] ns inner join
				(
					Select [nodeId],  DATEADD(MINUTE, (@Counter*5), @fromTimeStamp) as [timestamp]
					FROM [Aggregation].[tbRawData] 
					Where [nodeTypeId] = @nodeTypeId and (isbackfilled=0 or isbackfilled is null) and [nodeId] in (
						SELECT [nodeId]
						FROM [Aggregation].[rtbNode] with (nolock)
						where [activationDate] <@fromTimeStamp and ([deactivationDate] is null or [deactivationDate]>@toTimeStamp)
					) and 
					[metricKey] in (select [metricKey] from [Aggregation].[fnGetMetricKeys] (@nodeTypeId)) AND
					[timestamp] >= DATEADD(MINUTE, (@Counter*5), @fromTimeStamp) and [timestamp] < DATEADD(MINUTE, ((@Counter+1)*5), @fromTimeStamp)
					Group by [nodeId]
					HAVING COUNT(*) > 0
				) q on ns.[nodeId] = q.nodeId and ns.[timestamp] = q.[timestamp]
				SET @Counter= @Counter + 1
			END

			SET @occurance= @occurance + 1
		END

	set @Counter = 0
	WHILE @Counter <= 11
	BEGIN
		set @timeStamp = DATEADD(MINUTE, (@Counter*5), @fromTimeStamp)
		update [summary].[tbNodeStatus] 
		set [status] = 1, [originalStatus] = 1, [updatedDate]=GETDATE() 
		where  [timestamp] = @timeStamp and [nodeId] in (
			SELECT p.[nodeId]
			FROM [summary].[tbNodeStatus] s inner join 
				[Aggregation].[rtbNode] n with (nolock) on s.[nodeId] = n.[nodeId] inner join 
				[Aggregation].[rtbNodeParent] p with (nolock) on n.[nodeId] = p.[parentnodeId]
			where n.[nodeTypeId]=2 and [timestamp] = @timeStamp and [status] = 1
		)
		SET @Counter= @Counter + 1
	END

	set @timeStamp  =  @fromTimeStamp
	set @counter  = 0
	WHILE @Counter <= 11
	BEGIN
		update [summary].[tbNodeStatus] 
		set [status] = 1, [originalStatus] = 1, [updatedDate]=GETDATE()
		where [timestamp] = @timeStamp and status = 0 and [nodeId] in (
			SELECT[parentnodeId]
			FROM [Aggregation].[rtbNodeParent]  with (nolock)
			where [nodeId] in (
				Select distinct [nodeId]
				FROM [Aggregation].[tbRawData]
				Where [nodeTypeId] = 3 and (isbackfilled=0 or isbackfilled is null) and [nodeId] in (
					SELECT s.[nodeId]
					FROM [summary].[tbNodeStatus] s inner join 
						[Aggregation].[rtbNode] n with (nolock) on s.[nodeId] = n.[nodeId] 
					where n.[nodeTypeId]=3 and [timestamp] = @timeStamp and [status] = 0
				) and 
				[timestamp] >= @timeStamp and [timestamp] < DATEADD(MINUTE, ((@Counter+1)*5), @fromTimeStamp)
				Group by [nodeid]
				HAVING COUNT(*) > 0
			)
		)
		SET @Counter= @Counter + 1
		SET @timeStamp = DATEADD(MINUTE, (@Counter*5), @fromTimeStamp)
	END

	Update [summary].[tbNodeStatus]
	set status = 1, [updatedDate]=GETDATE()
	where id in (
		SELECT s.[id]
		FROM [summary].[tbNodeStatus] s inner join 
			[Aggregation].[rtbNodeType] nt on s.[nodeTypeId] = nt.[nodeTypeId]
		WHERE s.[timestamp] >= @fromTimeStamp and s.[timestamp] < @toTimeStamp and status = 0 and ([Aggregation].[fnGetPreviousNodeStatusValue] ( s.[nodeId],s.[nodeTypeId],s.[timestamp]) - nt.[defaultvalue]) <= 0
	)
END
GO

CREATE PROCEDURE [Aggregation].[spMergeRefTables]
AS
BEGIN
	MERGE [Aggregation].[rtbNode] AS Target
	USING [Aggregation].[vwNode] AS Source
	ON Source.[name] = Target.[name] and Source.[nodeTypeId] = Target.[nodeTypeId] 
	-- For Inserts
	WHEN NOT MATCHED BY Target THEN
		INSERT ([nodeId], [name],[active],[nodeTypeId],[dateCreated],[dateUpdated],[activationDate],[deactivationDate]) 
		VALUES (Source.id, Source.[name],Source.[active],Source.[nodeTypeId],Source.[dateCreated],Source.[dateUpdated],Source.[activationDate],Source.[deactivationDate])
	-- For Updates
	WHEN MATCHED THEN UPDATE SET
		Target.[name] = Source.[name],
		Target.[active] = Source.[active],
		Target.[dateCreated] = Source.[dateCreated],
		Target.[dateUpdated] = Source.[dateUpdated],
		Target.[activationDate] = Source.[activationDate],
		Target.[deactivationDate] = Source.[deactivationDate]
	-- For Deletes
	WHEN NOT MATCHED BY Source THEN
		DELETE;
	
	MERGE [Aggregation].[rtbNodeMetric] AS Target
	USING [Aggregation].[vwNodeMetric] AS Source
	ON Source.[nodeId] = Target.[nodeId] and Source.[nodeTypeId] = Target.[nodeTypeId] and Source.[metricKey] = Target.[metricKey]
	-- For Inserts
	WHEN NOT MATCHED BY Target THEN
		INSERT ([nodeId],[nodeMetricId],[nodeTypeId],[metricKey],[description],[metricValueType],[active],[dateCreated],[dateUpdated]) 
		VALUES (Source.[nodeId],Source.[id],Source.[nodeTypeId],Source.[metricKey],SUBSTRING(Source.[description], 1, 255),Source.[metricValueType],Source.[active],Source.[dateCreated],Source.[dateUpdated])
	-- For Updates
	WHEN MATCHED THEN UPDATE SET
		Target.[metricValueType] = Source.[metricValueType],
		Target.[description] = SUBSTRING(Source.[description], 1, 255),
		Target.[active] = Source.[active],
		Target.[dateCreated] = Source.[dateCreated],
		Target.[dateUpdated] = Source.[dateUpdated]
	-- For Deletes
	WHEN NOT MATCHED BY Source THEN
		DELETE;
	
	MERGE [Aggregation].[rtbNodeParent] AS Target
	USING [Aggregation].[vwNodeParent] AS Source
	ON Source.[nodeId] = Target.[nodeId] and Source.[parentnodeId] = Target.[parentnodeId] 
	-- For Inserts
	WHEN NOT MATCHED BY Target THEN
		INSERT ([nodeId],[parentnodeId],[active],[dateCreated],[dateUpdated]) 
		VALUES (Source.[nodeId],Source.[parentnodeId],Source.[active],Source.[dateCreated],Source.[dateUpdated])
	-- For Updates
	WHEN MATCHED THEN UPDATE SET
		Target.[active] = Source.[active],
		Target.[dateCreated] = Source.[dateCreated],
		Target.[dateUpdated] = Source.[dateUpdated]
	-- For Deletes
	WHEN NOT MATCHED BY Source THEN
		DELETE;

	MERGE [Aggregation].[rtbNodeType] AS Target
	USING [Aggregation].[vwNodeType]AS Source
	ON Source.[Id] = Target.[nodeTypeId]
	-- For Inserts
	WHEN NOT MATCHED BY Target THEN
		INSERT ([nodeTypeId], [description],[active],[config],[dateCreated],[dateUpdated]) 
		VALUES (Source.[Id], Source.[description], Source.[active],Source.[config],Source.[dateCreated],Source.[dateUpdated])
	-- For Updates
	WHEN MATCHED THEN UPDATE SET
		Target.[active] = Source.[active],
		Target.[description] = Source.[description],
		Target.[config] = Source.[config],
		Target.[dateCreated] = Source.[dateCreated],
		Target.[dateUpdated] = Source.[dateUpdated]
	-- For Deletes
	WHEN NOT MATCHED BY Source THEN
		DELETE;

	MERGE [Aggregation].[rtbNodeTypeMetric] AS Target
	USING [Aggregation].[vwNodeTypeMetric] AS Source
	ON Source.[nodeTypeId] = Target.[nodeTypeId] and Source.[metricKey] = Target.[metricKey] 
	-- For Inserts
	WHEN NOT MATCHED BY Target THEN
		INSERT ([nodeTypeId],[metricKey],[description],[metricValueType],[dateCreated],[dateUpdated],[rollUpAggregateConfig],[preFillValueConfig]) 
		VALUES (Source.[nodeTypeId],Source.[metricKey],SUBSTRING(Source.[description], 1, 255),Source.[metricValueType],Source.[dateCreated],Source.[dateUpdated],
		'<config><aggregatetype>' + Source.[rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') + '</aggregatetype></config>',
		Source.[preFillValueConfig])
	-- For Updates
	WHEN MATCHED THEN UPDATE SET
		Target.[description] = SUBSTRING(Source.[description], 1, 255),
		Target.[metricValueType] = Source.[metricValueType],
		Target.[dateCreated] = Source.[dateCreated],
		Target.[dateUpdated] = Source.[dateUpdated],
		Target.[rollUpAggregateConfig] ='<config><aggregatetype>' + Source.[rollUpAggregateConfig].value('(/config/aggregatetype/node())[1]', 'nvarchar(50)') + '</aggregatetype></config>',
		Target.[preFillValueConfig] = Source.[preFillValueConfig]
	-- For Deletes
	WHEN NOT MATCHED BY Source THEN
		DELETE;

	update [Aggregation].[rtbNodeTypeMetric]
	set [defaultValue]=r.defaultValue, [usePreviousValue]=r.usePreviousValue
	from 
	(
		select dv.nodeTypeId, dv.metricKey, q.defaultValue, q.usePreviousValue
		from [Aggregation].[rtbNodeTypeMetric] dv inner join
		(
			SELECT distinct [nodeTypeId], [metricKey]
			,CASE WHEN preFillValueConfig.value('(/preFillValueConfig/fillprevvalue/node())[1]', 'nvarchar(1)') ='Y' THEN 1 ELSE 0 END as usePreviousValue
			,CASE WHEN preFillValueConfig.value('(/preFillValueConfig/defaultvalue/node())[1]', 'nvarchar(50)') is null 
				THEN 'NULL'
				ELSE preFillValueConfig.value('(/preFillValueConfig/defaultvalue/node())[1]', 'nvarchar(50)') END as defaultValue
			FROM [Aggregation].[rtbNodeTypeMetric]  
			where preFillValueConfig is not null
		) q on dv.nodeTypeId = q.nodeTypeId and dv.metricKey = q.metricKey  
	) r
	where [Aggregation].[rtbNodeTypeMetric].[nodeTypeId] = r.[nodeTypeId] and [Aggregation].[rtbNodeTypeMetric].[metricKey] = r.[metricKey];

	Update [Aggregation].[rtbNodeType]
	set [defaultValue] = q.[defaultValue]
	from [Aggregation].[rtbNodeType] nt inner join 
	(
		SELECT distinct [nodeTypeId], 
		CASE WHEN [config].value('(/config/PrevMetricRecvdInterval/node())[1]', 'nvarchar(50)') is null 
		THEN '60'
		ELSE [config].value('(/config/PrevMetricRecvdInterval/node())[1]', 'nvarchar(50)') END as defaultValue
		FROM [Aggregation].[rtbNodeType]
		where [config] is not null
	) q on nt.[nodeTypeId] = q.[nodeTypeId];

	exec [Aggregation].[spCleanRefTables];
END
GO