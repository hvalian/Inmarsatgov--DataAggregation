MERGE INTO [Aggregation].[tbInterval] AS Target
USING 
    (SELECT [id], [description]
       FROM (VALUES
				('1', 'Hourly'),
				('2', 'Daily')
            ) X ([id], [description])
    ) AS Source
ON (Target.[id] = Source.[id])
WHEN NOT MATCHED BY Target THEN 
    INSERT ([id], [description])
    VALUES ([id], [description])
WHEN MATCHED THEN UPDATE SET 
        Target.[description] = Source.[description]
WHEN NOT MATCHED BY Source THEN
	DELETE;

MERGE INTO [Aggregation].[tbPriority] AS Target
USING 
    (SELECT [id], [description]
       FROM (VALUES
				('1', 'High'),
				('2', 'Normal'),
				('3', 'Low')
            ) X ([id], [description])
    ) AS Source
ON (Target.[id] = Source.[id])
WHEN NOT MATCHED BY Target THEN 
    INSERT ([id], [description])
    VALUES ([id], [description])
WHEN MATCHED THEN UPDATE SET 
        Target.[description] = Source.[description]
WHEN NOT MATCHED BY Source THEN
	DELETE;

MERGE INTO [Aggregation].[tbStatus] AS Target
USING 
    (SELECT [id], [description]
       FROM (VALUES
				('1', 'Created'),
				('2', 'Started'),
				('3', 'Completed'),
				('4', 'CompletedWithError'),
				('5', 'Abend'),
				('6', 'Canceled')
            ) X ([id], [description])
    ) AS Source
ON (Target.[id] = Source.[id])
WHEN NOT MATCHED BY Target THEN 
    INSERT ([id], [description])
    VALUES ([id], [description])
WHEN MATCHED THEN UPDATE SET 
        Target.[description] = Source.[description]
WHEN NOT MATCHED BY Source THEN
	DELETE;

MERGE INTO [Aggregation].[tbJobType] AS Target
USING 
    (SELECT [id], [description]
       FROM (VALUES
				('1', 'Normal'),
				('2', 'Rerun'),
				('3', 'Refresh')
            ) X ([id], [description])
    ) AS Source
ON (Target.[id] = Source.[id])
WHEN NOT MATCHED BY Target THEN 
    INSERT ([id], [description])
    VALUES ([id], [description])
WHEN MATCHED THEN UPDATE SET 
        Target.[description] = Source.[description]
WHEN NOT MATCHED BY Source THEN
	DELETE;

MERGE INTO [Aggregation].[tbNodeStatusConfig] AS Target
USING 
    (SELECT [nodeTypeId],[metricKey],[include]
       FROM (VALUES
                (10,'clock_delta_count',1),
                (10,'digital_rx_power',1),
                (10,'dvb_s2_crc32_error',1),
                (10,'dvb_s2_crc8_error',1),
                (10,'fast_fade_correction',1),
                (10,'local_fo_correction',1),
                (10,'lost_pl_lock_count',1),
                (10,'power_in_dbm',1),
                (10,'rx_cof',1),
                (10,'rx_power',1),
                (10,'SNR_cal',1),
                (10,'temperature_celcius',1),
                (10,'time_tics',1),  
                (4,'3',0),
                (4,'4',0),
                (4,'5',0),
                (4,'6',0),
                (4,'7',0),
                (4,'8',0),  
                (4,'9',0),  
                (2,'1395',0),
                (2,'1393',0),
                (2,'1668',0)  
            ) X ([nodeTypeId],[metricKey],[include])
    ) AS Source
ON (Target.[nodeTypeId] = Source.[nodeTypeId] and Target.[metricKey] = Source.[metricKey])
WHEN NOT MATCHED BY Target THEN 
    INSERT ([nodeTypeId],[metricKey],[include])
    VALUES ([nodeTypeId],[metricKey],[include])
WHEN MATCHED THEN UPDATE SET 
        Target.[include] = Source.[include]
WHEN NOT MATCHED BY Source THEN
	DELETE;

MERGE INTO [Aggregation].[tbActivity] AS Target
USING 
    (SELECT [id], [description]
       FROM (VALUES
				('1', 'Change Configuration'),
				('2', 'Refresh a Day'),
				('3', 'Reset Aggregation Clock')
            ) X ([id], [description])
    ) AS Source
ON (Target.[id] = Source.[id])
WHEN NOT MATCHED BY Target THEN 
    INSERT ([id], [description])
    VALUES ([id], [description])
WHEN MATCHED THEN UPDATE SET 
        Target.[description] = Source.[description]
WHEN NOT MATCHED BY Source THEN
	DELETE;

DECLARe @ActionViewOpenPeriod Varchar(50);
DECLARE @CommandTimeout Varchar(50);
DECLARE @Disabled Varchar(50);
DECLARE @HistoricalViewOpenPeriod Varchar(50)
DECLARE @JobStartTimeDelay Varchar(50);
DECLARE @Job_SuspendProcessingAfter Varchar(50);
DECLARE @LastHourlyJob_Scheduled_DateTime Varchar(50);
DECLARE @NodeStatus Varchar(50);
DECLARE @NumberOfDaysForStats Varchar(50);
DECLARE @NumberOfRefresh Varchar(50);
DECLARE @ProjectName Varchar(50);
DECLARE @RefreshEnabled Varchar(50);
DECLARE @RefreshInterval Varchar(50);
DECLARE @ReRunStartDelayTime Varchar(50);
DECLARE @RetentionActivityLogNumberOfHours Varchar(50);
DECLARE @SPCommandTimeout Varchar(50);

SELECT @ActionViewOpenPeriod = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'ActionViewOpenPeriod';
IF @ActionViewOpenPeriod IS NULL 
	SET @ActionViewOpenPeriod = '2023/06/01'

SELECT @CommandTimeout = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'CommandTimeout';
IF @CommandTimeout IS NULL 
	SET @CommandTimeout = '300'

SELECT @Disabled = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'Disabled';
IF @Disabled IS NULL 
	SET @Disabled = '0'

SELECT @HistoricalViewOpenPeriod = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'HistoricalViewOpenPeriod';
IF @HistoricalViewOpenPeriod IS NULL 
	SET @HistoricalViewOpenPeriod = '2023/01/01'

SELECT @JobStartTimeDelay = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'Job_StartTimeDelay';
IF @JobStartTimeDelay IS NULL 
	SET @JobStartTimeDelay = '125'

SELECT @Job_SuspendProcessingAfter = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'Job_SuspendProcessingAfter';
IF @Job_SuspendProcessingAfter IS NULL 
	SET @Job_SuspendProcessingAfter = '2099/12/31 23:59:59'

SELECT @LastHourlyJob_Scheduled_DateTime = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'LastHourlyJob_Scheduled_DateTime';
IF @LastHourlyJob_Scheduled_DateTime IS NULL 
	SET @LastHourlyJob_Scheduled_DateTime = '2022/12/31 23:59:59'

SELECT @NodeStatus = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'NodeStatus';
IF @NodeStatus IS NULL 
	SET @NodeStatus = '9999'

SELECT @NumberOfDaysForStats = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'NumberOfDaysForStats';
IF @NumberOfDaysForStats IS NULL 
	SET @NumberOfDaysForStats = '7';

SELECT @NumberOfRefresh = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'NumberOfRefresh';
IF @NumberOfRefresh IS NULL 
	SET @NumberOfRefresh = '2';

SELECT @ProjectName = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'ProjectName';
IF @ProjectName IS NULL 
	SET @ProjectName = 'NoInstanceName'

SELECT @RefreshEnabled = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'Refresh_Enabled';
IF @RefreshEnabled IS NULL 
	SET @RefreshEnabled = '0'
	
SELECT @RefreshInterval = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'Refresh_Interval';
IF @RefreshInterval IS NULL 
	SET @RefreshInterval = '1'

SELECT @ReRunStartDelayTime = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'ReRun_StartTimeDelay';
IF @ReRunStartDelayTime IS NULL 
	SET @ReRunStartDelayTime = '30'

SELECT @RetentionActivityLogNumberOfHours = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'Retention_ActivityLog_NumberOfHours';
IF @RetentionActivityLogNumberOfHours IS NULL 
	SET @RetentionActivityLogNumberOfHours = '72'

SELECT @SPCommandTimeout = [value]
FROM [Aggregation].[tbConfiguration]
WHERE [key] = 'SP_CommandTimeout';
IF @SPCommandTimeout IS NULL 
	SET @SPCommandTimeout = '1500'

MERGE INTO [Aggregation].tbConfiguration AS Target
USING 
    (SELECT [key] ,[value], [description], [type], [readOnly], [immediate]
       FROM (VALUES
            ('ActionViewOpenPeriod',@ActionViewOpenPeriod, 'Action View - OpenPeriod', 'datetime', 0, 1),
            ('CommandTimeout',@CommandTimeout, 'Command Timeout (Seconds)', 'int', 0, 1),
            ('Disabled',@Disabled, 'Disabled', 'bit', 1, 1),
            ('HistoricalViewOpenPeriod',@HistoricalViewOpenPeriod, 'Historical View - OpenPeriod', 'datetime', 0, 1),
            ('Job_StartTimeDelay',@JobStartTimeDelay, 'Job Start Delay (Minutes)', 'int', 0, 0),
            ('Job_SuspendProcessingAfter',@Job_SuspendProcessingAfter, 'Suspend Job Processing After', 'datetime', 0, 1),
            ('LastHourlyJob_Scheduled_DateTime',@LastHourlyJob_Scheduled_DateTime, 'Last Hourly Job Scheduled DateTime', 'datetime', 1, 1),
            ('NodeStatus',@NodeStatus, 'Node Status MetricKey', 'varchar', 1, 1),
            ('NumberOfDaysForStats',@NumberOfDaysForStats, 'Past # Of Days To Calculate Avg. Processing Time', 'int', 0, 1),
            ('NumberOfRefresh',@NumberOfRefresh, 'Number Of Refresh', 'int', 0, 0),
            ('ProjectName',@ProjectName, 'Instance Name', 'varchar', 0, 1), 
            ('Refresh_Enabled',@RefreshEnabled, 'Refresh Enabled', 'bit', 0, 0),
            ('Refresh_Interval',@RefreshInterval, 'Refresh  Interval (In Days)', 'int', 0, 0),
            ('ReRun_StartTimeDelay',@ReRunStartDelayTime, 'ReRun Job Start Delay (Minutes)', 'int', 0, 1),
            ('Retention_ActivityLog_NumberOfHours',@RetentionActivityLogNumberOfHours, 'ActivityLog Retention Period (Number Of Hours)', 'int', 0, 1),
            ('SP_CommandTimeout',@SPCommandTimeout, 'SP Command Timeout (Seconds)', 'int', 0, 1)
            ) X ([key] ,[value], [description], [type], [readOnly], [immediate])
    ) AS Source
ON (Target.[key] = Source.[key])
WHEN NOT MATCHED BY Target THEN 
    INSERT ([key] ,[value], [description], [type], [readOnly], [immediate])
    VALUES ([key] ,[value], [description], [type], [readOnly], [immediate])
WHEN MATCHED THEN UPDATE SET 
        Target.[value] = Source.[value],
        Target.[description] = Source.[description],
        Target.[type] = Source.[type],
        Target.[readOnly] = Source.[readOnly],
        Target.[immediate] = Source.[immediate]
WHEN NOT MATCHED BY Source THEN
	DELETE;

MERGE INTO [Aggregation].[tbUser] AS Target
USING 
    (SELECT [userId], [name], [active], [hasAdminAccess], [hasAccessToConfiguration], [hasAccessToQueue], [hasAccessToRefreshJob], [hasAccessToUpdateClock]
       FROM (VALUES
				('INMARSATGOV\homayoon.valian', 'Homayoon Valian', 1, 1, 1, 1, 1, 1)
            ) X ([userId], [name], [active], [hasAdminAccess], [hasAccessToConfiguration], [hasAccessToQueue], [hasAccessToRefreshJob], [hasAccessToUpdateClock])
    ) AS Source
ON (Target.[userId] = Source.[userId])
WHEN NOT MATCHED BY Target THEN 
    INSERT ([userId], [name], [active], [hasAdminAccess], [hasAccessToConfiguration], [hasAccessToQueue], [hasAccessToRefreshJob], [hasAccessToUpdateClock])
    VALUES ([userId], [name], [active], [hasAdminAccess], [hasAccessToConfiguration], [hasAccessToQueue], [hasAccessToRefreshJob], [hasAccessToUpdateClock])
WHEN MATCHED THEN UPDATE SET 
        Target.[name] = Source.[name],
        Target.[active] = Source.[active],
        Target.[hasAdminAccess] = Source.[hasAdminAccess],
        Target.[hasAccessToConfiguration] = Source.[hasAccessToConfiguration],
        Target.[hasAccessToQueue] = Source.[hasAccessToQueue],
        Target.[hasAccessToRefreshJob] = Source.[hasAccessToRefreshJob],
        Target.[hasAccessToUpdateClock] = Source.[hasAccessToUpdateClock];

MERGE INTO [Aggregation].[tbWebServer] AS Target
USING 
    (SELECT [serverName],[serverIP],[active], [InstanceName]
       FROM (VALUES
                ('http://igenms-agapp01/','172.28.40.38',1, 'PRD - ENMS'),
                ('http://igmsc-app01/','172.28.40.171',1, 'PRD - MSC'),
                ('http://devenmsweb/','172.28.40.134',1, 'DEV - MSC')
            ) X ([serverName],[serverIP],[active], [InstanceName])
    ) AS Source
ON (Target.[serverName] = Source.[serverName])
WHEN NOT MATCHED BY Target THEN 
    INSERT ([serverName],[serverIP],[active], [InstanceName])
    VALUES ([serverName],[serverIP],[active], [InstanceName])
WHEN MATCHED THEN UPDATE SET 
        Target.[serverIP] = Source.[serverIP],
        Target.[active] = Source.[active],
        Target.[InstanceName] = Source.[InstanceName]
WHEN NOT MATCHED BY Source THEN
	DELETE;