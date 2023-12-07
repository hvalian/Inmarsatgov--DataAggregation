IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGet95pctValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGet95pctValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetBaseModeValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetBaseModeValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetBaseMaxCountValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetBaseMaxCountValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetDailyStdDevValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetDailyStdDevValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetDaily95pctValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetDaily95pctValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetFirstValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetFirstValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetLastObservationValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetLastObservationValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetLastValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetLastValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetMaxCountValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetMaxCountValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetMedianValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetMedianValue]
GO 

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetMinute]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetMinute]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetModeValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetModeValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetPreviousNodeStatusValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetPreviousNodeStatusValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetStdDevValue]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetStdDevValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnRoundTime]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnRoundTime]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnStringList2Table]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnStringList2Table]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetStats]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetStats]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetListOfJobs]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetListOfJobs]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetMetricKeys]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetMetricKeys]
GO

CREATE function [Aggregation].[fnGet95pctValue] (
	@nodeTypeId int,
	@metricKey varchar(20),
	@nodeId int,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
)
returns FLOAT

begin
	declare @value FLOAT

	SELECT  distinct @value = PERCENTILE_CONT(0.95) 
		WITHIN GROUP (ORDER BY TRY_CAST([value] AS FLOAT))
		OVER (PARTITION BY [nodeId],[nodeTypeId],[metricKey] ) 
	FROM [Aggregation].[tbRawData]  with (nolock)
	where timestamp >= @fromTimeStamp and timestamp < @toTimeStamp  and [nodeId] = @nodeId and [nodeTypeId]=@nodeTypeId and [metricKey]=@metricKey

	return @value
end
GO

CREATE function [Aggregation].[fnGetBaseModeValue] (
	@nodeTypeId int,
	@metricKey varchar(20),
	@nodeId int,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
)
returns FLOAT

begin
	declare @value FLOAT

	SELECT TOP 1  @value = TRY_CAST( [value] AS FLOAT) 
	FROM  [Aggregation].[tbRawData]
	where timestamp >= @fromTimeStamp and timestamp < @toTimeStamp  and [nodeId] = @nodeId and [nodeTypeId]=@nodeTypeId and [metricKey]=@metricKey 
	GROUP BY [value]
	HAVING COUNT(*)>1
	ORDER BY COUNT(*) DESC, [value]

	return @value
end
GO

CREATE function [Aggregation].[fnGetBaseMaxCountValue] (
	@nodeTypeId int,
	@metricKey varchar(20),
	@nodeId int,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
)
returns int

begin
	declare @value FLOAT,
	@count int

	select  @value = [Aggregation].[fnGetBaseModeValue](@nodeTypeId,@metricKey,@nodeId,@fromTimeStamp, @toTimeStamp)

	select @count = Count(*) 
	FROM  [Aggregation].[tbRawData]
	where timestamp >= @fromTimeStamp and timestamp < @toTimeStamp  and [nodeId] = @nodeId and [nodeTypeId]=@nodeTypeId and [metricKey]=@metricKey and [value] = LTRIM(STR(@value, 18, 14))
	HAVING COUNT(*)>1

	return @count
end
GO

CREATE function [Aggregation].[fnGetDaily95pctValue] (
	@nodeTypeId int,
	@metricKey varchar(20),
	@nodeId int,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
)
returns FLOAT

begin
	declare @value FLOAT

	SELECT  distinct @value = PERCENTILE_CONT(0.95) 
		WITHIN GROUP (ORDER BY TRY_CAST([value] AS FLOAT))
		OVER (PARTITION BY [nodeId],[nodeTypeId],[metricKey] ) 
	FROM [Aggregation].[tbRawDataDaily]  with (nolock)
	where timestamp >= @fromTimeStamp and timestamp < @toTimeStamp  and [nodeId] = @nodeId and [nodeTypeId]=@nodeTypeId and [metricKey]=@metricKey

	return @value
end
GO

CREATE function [Aggregation].[fnGetDailyStdDevValue] (
	@nodeTypeId int,
	@metricKey varchar(20),
	@nodeId int,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
)
returns FLOAT

begin
	declare @value FLOAT
	SELECT top 1 @value = STDEV(TRY_CAST([value] AS FLOAT))
	FROM [Aggregation].[tbRawDataDaily]  with (nolock)
	where timestamp >= @fromTimeStamp and timestamp < @toTimeStamp  and nodeid = @nodeId and nodeTypeId=@nodeTypeId and metricKey = @metricKey

	return @value
end
GO

CREATE function [Aggregation].[fnGetFirstValue] (
	@nodeTypeId int,
	@metricKey varchar(20),
	@nodeId int,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
)
returns varchar(1024)

begin
	declare @value varchar(1024)
	SELECT top 1 @value = [first]
	FROM [summary].[tbnodemetricvalue_5Minute]  
	where timestamp >= @fromTimeStamp and timestamp < @toTimeStamp  and  nodeid = @nodeId and nodeTypeId=@nodeTypeId and metricKey = @metricKey and [first] is not null
	order by timestamp asc
	return @value
end
GO

CREATE function [Aggregation].[fnGetLastObservationValue] (
	@nodeId int,
	@nodeTypeId int,
	@metricKey varchar(20),
	@timeStamp DATETIME
)
returns varchar(1024)

begin
	declare @value varchar(1024)

	SELECT TOP (1) @value = [value]
	FROM [Aggregation].[tbObservations]
	Where nodeid= @nodeId and [nodeTypeId] = @nodeTypeId and metrickey = @metricKey and timestamp < @timeStamp
	order by timestamp desc

	if @value is null
	Begin
		declare @MM VARCHAR(3)
		select @MM = SUBSTRING(CONVERT(VARCHAR(5),@timeStamp,108), 3, 5)

		if @MM = ':00'
		Begin
			SELECT TOP (1) @value = [value]
			FROM [summary].[tbnodemetricvalue_5Minute]
			Where nodeid= @nodeId and [nodeTypeId] = @nodeTypeId and metrickey = @metricKey and timestamp < @timeStamp
			order by timestamp desc
		End
	End

	return @value
end
GO

CREATE function [Aggregation].[fnGetLastValue] (
	@nodeTypeId int,
	@metricKey varchar(20),
	@nodeId int,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
)
returns varchar(1024)

begin
	declare @value varchar(1024)
	SELECT top 1 @value = [last]
	FROM [summary].[tbnodemetricvalue_5Minute]  
	where timestamp >= @fromTimeStamp and timestamp < @toTimeStamp  and  nodeid = @nodeId and nodeTypeId=@nodeTypeId and metricKey = @metricKey and [last] is not null
	order by timestamp desc
	return @value
end
GO

CREATE function [Aggregation].[fnGetMaxCountValue] (
	@nodeTypeId int,
	@metricKey varchar(20),
	@nodeId int,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
)
returns int

begin
	declare @value FLOAT,
	@count int

	select  @value = [Aggregation].[fnGetModeValue](@nodeTypeId,@metricKey,@nodeId,@fromTimeStamp, @toTimeStamp)

	select @count = Count(*) 
	FROM  [summary].[tbnodemetricvalue_5Minute]
	where timestamp >= @fromTimeStamp and timestamp < @toTimeStamp  and [nodeId] = @nodeId and [nodeTypeId]=@nodeTypeId and [metricKey]=@metricKey and [mode] = @value 
	HAVING COUNT(*)>1

	return @count
end
GO

CREATE function [Aggregation].[fnGetMedianValue] (
	@nodeTypeId int,
	@metricKey varchar(20),
	@nodeId int,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
)
returns FLOAT

begin
	declare @value FLOAT

	SELECT  distinct @value = PERCENTILE_CONT(0.5) 
		WITHIN GROUP (ORDER BY [median])
		OVER (PARTITION BY [nodeId],[nodeTypeId],[metricKey] ) 
	FROM  [summary].[tbnodemetricvalue_5Minute]
	where timestamp >= @fromTimeStamp and timestamp < @toTimeStamp  and [nodeId] = @nodeId and [nodeTypeId]=@nodeTypeId and [metricKey]=@metricKey
	
	return @value
end
GO

CREATE function [Aggregation].[fnGetMinute] (
	@timestamp datetime
)
returns datetime

begin
	declare @value varchar(2)
	
	declare @yymmdd varchar(10) 
	SELECT  @yymmdd = Format(@timestamp, 'yyyy-MM-dd')
	
	declare @hh varchar(2) 
	SELECT  @hh = Format(@timestamp, 'HH') 

	declare @minute varchar(2)
	SELECT  @minute = Format(@timestamp, 'mm')

	declare @MM varchar(2)
	select @MM = CASE 
    WHEN @minute >= 0 and @minute < 5 THEN '00'
    WHEN @minute >= 5 and @minute < 10 THEN '05'
    WHEN @minute >= 10 and @minute < 15 THEN '10'
    WHEN @minute >= 15 and @minute < 20 THEN '15'
    WHEN @minute >= 20 and @minute < 25 THEN '20'
    WHEN @minute >= 25 and @minute < 30 THEN '25'
    WHEN @minute >= 30 and @minute < 35 THEN '30'
    WHEN @minute >= 35 and @minute < 40 THEN '35'
    WHEN @minute >= 40 and @minute < 45 THEN '40'
    WHEN @minute >= 45 and @minute < 50 THEN '45'
    WHEN @minute >= 50 and @minute < 55 THEN '50'
    WHEN @minute >= 55 and @minute < 60 THEN '55'
    ELSE '55'
	END 

	declare @date varchar(23)
	set @date =  @yymmdd + ' ' + @hh + ':' + @MM + ':00.000'

	return convert(varchar,@date,121)
end
GO

CREATE function [Aggregation].[fnGetModeValue] (
	@nodeTypeId int,
	@metricKey varchar(20),
	@nodeId int,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
)
returns FLOAT

begin
	declare @value FLOAT

	SELECT TOP 1  @value = [mode]
	FROM  [summary].[tbnodemetricvalue_5Minute]
	where timestamp >= @fromTimeStamp and timestamp < @toTimeStamp  and [nodeId] = @nodeId and [nodeTypeId]=@nodeTypeId and [metricKey]=@metricKey and [mode] is not null
	GROUP BY [mode]
	HAVING COUNT(*)>1
	ORDER BY COUNT(*) DESC, [mode]

	return @value
end
GO

CREATE function [Aggregation].[fnGetPreviousNodeStatusValue] (
	@nodeId int,
	@nodeTypeId int,
	@timeStamp DATETIME
)
returns int

begin
	declare @elapsedtime int = null ,
	@lastTimeStamp DATETIME,
	@nodeStatusMetricKey  varchar(255)

	SELECT  @nodeStatusMetricKey=value
	FROM [Aggregation].[tbConfiguration]
	where [key] = 'NodeStatus';

	SELECT TOP (1) @lastTimeStamp = [timestamp]
	FROM [summary].tbNodeStatus
	Where nodeid= @nodeId and [nodeTypeId] = @nodeTypeId and [originalStatus] = 1 and [timestamp] < @timeStamp 
	order by [timestamp] desc

	if @lastTimeStamp is null
	BEGIN
		SELECT TOP (1) @lastTimeStamp = [timestamp]
		FROM [summary].[tbnodemetricvalue_5Minute]
		Where nodeid= @nodeId and [nodeTypeId] = @nodeTypeId and [metricKey] = @nodeStatusMetricKey and [originalStatus] = 1 and [timestamp] < @timeStamp 
		order by [timestamp] desc
	END

	if @lastTimeStamp is not null
	BEGIN
		set @elapsedtime = DATEDIFF(MINUTE, @lastTimeStamp, @timeStamp);
	END

	return @elapsedtime
End
GO

CREATE function [Aggregation].[fnGetStdDevValue] (
	@nodeTypeId int,
	@metricKey varchar(20),
	@nodeId int,
	@fromTimeStamp DATETIME, 
	@toTimeStamp   DATETIME
)
returns FLOAT

begin
	declare @value FLOAT
	SELECT top 1 @value = STDEV(TRY_CAST([value] AS FLOAT))
	FROM [Aggregation].[tbRawData]  with (nolock)
	where timestamp >= @fromTimeStamp and timestamp < @toTimeStamp  and nodeid = @nodeId and nodeTypeId=@nodeTypeId and metricKey = @metricKey

	return @value
end
GO

CREATE function [Aggregation].[fnRoundTime] (@Time DATETIME, @RoundToMin INT)
RETURNS DATETIME
AS
BEGIN
RETURN ROUND(CAST(CAST(CONVERT(VARCHAR,
@Time,121) AS DATETIME) AS FLOAT) * (1440/@RoundToMin),0)/(1440/@RoundToMin)
END
GO

CREATE FUNCTION [Aggregation].[fnStringList2Table]
(
    @List varchar(MAX)
)
RETURNS 
@ParsedList table
(
    item int
)
AS
BEGIN
    DECLARE @item varchar(max), @Pos int

    SET @List = LTRIM(RTRIM(@List))+ ','
    SET @Pos = CHARINDEX(',', @List, 1)

    WHILE @Pos > 0
    BEGIN
        SET @item = LTRIM(RTRIM(LEFT(@List, @Pos - 1)))
        IF @item <> ''
        BEGIN
            INSERT INTO @ParsedList (item) 
            VALUES (CAST(@item AS int))
        END
        SET @List = RIGHT(@List, LEN(@List) - @Pos)
        SET @Pos = CHARINDEX(',', @List, 1)
    END

    RETURN
END
GO

CREATE FUNCTION [Aggregation].[fnGetStats]
(
)
RETURNS
@List table
(
	AverageProcessingTime_HourlyJob int,
	AverageProcessingTime_DailyJob int,
	CommandTimeout int,
	Job_StartTimeDelay int,
	Job_SuspendProcessingAfter datetime,
	JobProcessingHasIssues bit,
	JobProcessingIsSuspended bit,
	LastErrJobAggregationDateTime datetime,
	LastErrJobElapsedtime int,
	LastErrJobId int,
	LastErrJobInterval varchar(255),
	LastErrJobStartDateTime datetime,
	LastErrJobEndDateTime datetime,
	LastErrJobStatus varchar(255),
	LastJobAggregationDateTime datetime,
	LastJobElapsedtime int,
	LastJobId int,
	LastJobInterval varchar(255),
	LastJobStartDateTime datetime,
	LastJobEndDateTime datetime,
	LastHourlyJob_Scheduled_DateTime datetime,
	LastJobStatus varchar(255),
	NextJobAggregationDateTime datetime,
	NextJobId int,
	NextJobInterval varchar(255),
	NextJobStartAfterDateTime datetime,
	NextJobStartDateTime datetime,
	NextJobStatus varchar(255),
	NextJobWillStartInMinutes int,
	NumberOfDaysForStats int,
	NumberOfRefresh int,
	ProjectName varchar(50),
	Refresh_Enabled bit,
	Refresh_Interval int,
	Retention_ActivityLog_NumberOfHours int,
	SP_CommandTimeout int,
	SuspendAfter datetime
)
AS
BEGIN
	Declare @AverageProcessingTime_HourlyJob int
	Declare @AverageProcessingTime_DailyJob int
	Declare @CommandTimeout int
	Declare @Job_StartTimeDelay int
	Declare @Job_SuspendProcessingAfter datetime
	Declare @JobProcessingHasIssues bit = 0
	Declare @JobProcessingIsSuspended bit = 0
	Declare @LastErrJobAggregationDateTime datetime
	Declare @LastErrJobElapsedtime int
	Declare @LastErrJobId int
	Declare @LastErrJobInterval varchar(255)
	Declare @LastErrJobStartDateTime datetime
	Declare @LastErrJobEndDateTime datetime
	Declare @LastErrJobStatus varchar(255)
	Declare @LastJobAggregationDateTime datetime
	Declare @LastJobElapsedtime int
	Declare @LastJobId int
	Declare @LastJobInterval varchar(255)
	Declare @LastJobStartDateTime datetime
	Declare @LastJobEndDateTime datetime
	Declare @LastJobStatus varchar(255)
	Declare @LastHourlyJob_Scheduled_DateTime datetime
	Declare @NextJobAggregationDateTime datetime
	Declare @NextJobId int
	Declare @NextJobInterval varchar(255)
	Declare @NextJobStartAfterDateTime datetime
	Declare @NextJobStartDateTime datetime
	Declare @NextJobStatus varchar(255)
	Declare @NextJobWillStartInMinutes int
	Declare @NumberOfDaysForStats int
	Declare @NumberOfRefresh int
	Declare @ProjectName varchar(50)
	Declare @Refresh_Enabled bit
	Declare @Refresh_Interval int
	Declare @Retention_ActivityLog_NumberOfHours int
	Declare @SP_CommandTimeout int
	Declare @SuspendAfter datetime
	Declare @StartDate Date
	Declare @EndDate Date

	SELECT TOP (1) @CommandTimeout = [value] FROM [Aggregation].[tbConfiguration] WHERE [key]  = 'CommandTimeout';
	SELECT TOP (1) @Job_StartTimeDelay = [value] FROM [Aggregation].[tbConfiguration] WHERE [key]  = 'Job_StartTimeDelay';
	SELECT TOP (1) @Job_SuspendProcessingAfter = [value] FROM [Aggregation].[tbConfiguration] WHERE [key]  = 'Job_SuspendProcessingAfter';
	SELECT TOP (1) @LastHourlyJob_Scheduled_DateTime = [value] FROM [Aggregation].[tbConfiguration] WHERE [key]  = 'LastHourlyJob_Scheduled_DateTime';
	SELECT TOP (1) @NumberOfDaysForStats = [value] FROM [Aggregation].[tbConfiguration] WHERE [key]  = 'NumberOfDaysForStats';
	SELECT TOP (1) @NumberOfRefresh = [value] FROM [Aggregation].[tbConfiguration] WHERE [key]  = 'NumberOfRefresh';
	SELECT TOP (1) @ProjectName = [value] FROM [Aggregation].[tbConfiguration] WHERE [key]  = 'ProjectName';
	SELECT TOP (1) @Refresh_Enabled = [value] FROM [Aggregation].[tbConfiguration] WHERE [key]  = 'Refresh_Enabled';
	SELECT TOP (1) @Refresh_Interval = [value] FROM [Aggregation].[tbConfiguration] WHERE [key]  = 'Refresh_Interval';
	SELECT TOP (1) @Retention_ActivityLog_NumberOfHours = [value] FROM [Aggregation].[tbConfiguration] WHERE [key]  = 'Retention_ActivityLog_NumberOfHours';
	SELECT TOP (1) @SP_CommandTimeout = [value] FROM [Aggregation].[tbConfiguration] WHERE [key]  = 'SP_CommandTimeout';


	SELECT TOP (1) @NextJobId = j.id, @NextJobStartAfterDateTime = [startAfterDateTime],  
	@NextJobStartDateTime = [startDateTime], 
	@NextJobInterval = i.[description], 
	@NextJobStatus = s.description,
	@NextJobAggregationDateTime = [aggregationStartDateTime]
	FROM [Aggregation].[tbJob] J Inner Join
		[Aggregation].[tbInterval] I on j.interval = I.id Inner Join
		[Aggregation].[tbStatus] S on j.status= s.id
	WHERE status in (1,2)
	ORDER BY [status] desc, [priority], [interval], [startAfterDateTime]

	set @NextJobWillStartInMinutes = DATEDIFF(minute, getdate(), @NextJobStartAfterDateTime)

	select @EndDate = CONVERT(date, max([startDateTime])), @StartDate =CONVERT(date, max([startDateTime]-@NumberOfDaysForStats))
	FROM [Aggregation].[tbJob] 

	SELECT @AverageProcessingTime_HourlyJob = avg(DATEDIFF(s, [startDateTime],[endDateTime])) 
	FROM [Aggregation].[tbJob] 
	Where interval = 1 and CONVERT(date, [startDateTime]) >= @StartDate and CONVERT(date, [startDateTime]) <= @EndDate
	
	SELECT @AverageProcessingTime_DailyJob = avg(DATEDIFF(s, [startDateTime],[endDateTime])) 
	FROM [Aggregation].[tbJob] 
	Where interval = 2 and CONVERT(date, [startDateTime]) >= @StartDate and CONVERT(date, [startDateTime]) <= @EndDate
	
	SELECT @SuspendAfter = [value]
	FROM [Aggregation].[tbConfiguration]
	WHERE [key] = 'Job_SuspendProcessingAfter'


	SELECT TOP (1)  @LastErrJobId = j.id, @LastErrJobAggregationDateTime= [aggregationStartDateTime],
		@LastErrJobStartDateTime = [startDateTime], @LastErrJobEndDateTime = [endDateTime],  @LastErrJobElapsedtime = datediff(s,[startDateTime],[endDateTime]),
		@LastErrJobInterval = I.description, @LastErrJobStatus = s.description
	FROM [Aggregation].[tbJob] J Inner Join
		[Aggregation].[tbInterval] I on j.interval = I.id Inner Join
		[Aggregation].[tbStatus] S on j.status= s.id
	WHERE status in (5) 
	ORDER BY J.[id] desc
	
	SELECT TOP (1)  @LastJobId = j.id, @LastJobAggregationDateTime= [aggregationStartDateTime],
		@LastJobStartDateTime = [startDateTime], @LastJobEndDateTime = [endDateTime], @LastJobElapsedtime = datediff(s,[startDateTime],[endDateTime]),
		@LastJobInterval = I.description, @LastJobStatus = s.description
	FROM [Aggregation].[tbJob] J Inner Join
		[Aggregation].[tbInterval] I on j.interval = I.id Inner Join
		[Aggregation].[tbStatus] S on j.status= s.id
	WHERE status in (3) 
	ORDER BY J.[id] desc

	SELECT @JobProcessingIsSuspended = [value]
	FROM [Aggregation].[tbConfiguration]
	WHERE [key] = 'Disabled'

	IF (datediff(MINUTE,@LastJobStartDateTime,getdate()) > 65)
	BEGIN
		SET @JobProcessingHasIssues = 1
	END

	INSERT INTO @List (AverageProcessingTime_HourlyJob, AverageProcessingTime_DailyJob,JobProcessingHasIssues, JobProcessingIsSuspended,LastErrJobAggregationDateTime,LastErrJobElapsedtime,LastErrJobId,
		LastErrJobInterval,LastErrJobStartDateTime,LastErrJobEndDateTime,LastErrJobStatus,LastJobAggregationDateTime,LastJobElapsedtime,LastJobId,LastJobInterval,LastJobStartDateTime,
		LastJobEndDateTime,LastJobStatus,NextJobAggregationDateTime,NextJobId,NextJobInterval,NextJobStartAfterDateTime,NextJobStartDateTime,NextJobStatus,NextJobWillStartInMinutes,
		CommandTimeout, Job_StartTimeDelay, Job_SuspendProcessingAfter,LastHourlyJob_Scheduled_DateTime, NumberOfDaysForStats, 
		NumberOfRefresh, ProjectName, Refresh_Enabled, Refresh_Interval, Retention_ActivityLog_NumberOfHours, SP_CommandTimeout
	) 
	VALUES ( @AverageProcessingTime_HourlyJob, @AverageProcessingTime_DailyJob,@JobProcessingHasIssues, @JobProcessingIsSuspended,@LastErrJobAggregationDateTime,@LastErrJobElapsedtime,@LastErrJobId,
		@LastErrJobInterval,@LastErrJobStartDateTime,@LastErrJobEndDateTime,@LastErrJobStatus,@LastJobAggregationDateTime,@LastJobElapsedtime,@LastJobId,@LastJobInterval,@LastJobStartDateTime,
		@LastJobEndDateTime,@LastJobStatus,@NextJobAggregationDateTime,@NextJobId,@NextJobInterval,@NextJobStartAfterDateTime, @NextJobStartDateTime,@NextJobStatus,@NextJobWillStartInMinutes,
		@CommandTimeout, @Job_StartTimeDelay, @Job_SuspendProcessingAfter, @LastHourlyJob_Scheduled_DateTime, @NumberOfDaysForStats, 
		@NumberOfRefresh, @ProjectName, @Refresh_Enabled, @Refresh_Interval, @Retention_ActivityLog_NumberOfHours, @SP_CommandTimeout
	)

    RETURN
END
GO

CREATE FUNCTION [Aggregation].[fnGetListOfJobs]
(
	@jobId bigint,
    @fromDate datetime,
	@toDate datetime
)
RETURNS varchar(Max) -- or whatever length you need
AS
BEGIN
    Declare @jobs varchar(Max);
	
	SELECT @jobs = stuff((select ','+convert(varchar(10),id)
	FROM [Aggregation].[tbJob]
	WHERE [aggregationStartDateTime] = @fromDate and [aggregationEndDateTime] = @toDate 
	ORDER BY [id] DESC
		for xml path (''), type).value('.','nvarchar(max)')
		,1,1,'')

    RETURN  @jobs
END
GO


CREATE FUNCTION [Aggregation].[fnGetMetricKeys]
(
	@nodeTypeId int
)
RETURNS
@List table
(
	MetricKey varchar(255)
)
AS
BEGIN
	declare @selectionValue bit = 1

	SELECT TOP (1) @selectionValue = [include]
	FROM [Aggregation].[tbNodeStatusConfig]
		where nodeTypeId = @nodeTypeId

	if @selectionValue = null
	begin
		set @selectionValue = 1
	end

	if @selectionValue = 1
	begin
		INSERT INTO @List (MetricKey) 
		SELECT [metricKey]	
		FROM [Aggregation].[rtbNodeTypeMetric]
		WHERE [nodeTypeId] =  @nodeTypeId AND 
			[metricKey] <> '9999' AND 
			[metricKey] IN (select [metricKey] from [Aggregation].[tbNodeStatusConfig] where [nodeTypeId] = @nodeTypeId  and [include]=1)
	end
	else
	begin
		INSERT INTO @List (MetricKey) 
		SELECT [metricKey]	
		FROM [Aggregation].[rtbNodeTypeMetric]
		WHERE [nodeTypeId] =  @nodeTypeId AND 
			[metricKey] <> '9999' AND 
			[metricKey] not in	(select [metricKey] from [Aggregation].[tbNodeStatusConfig] where [nodeTypeId] = @nodeTypeId  and [include]=0) 
	end

    RETURN
END
GO
