IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwNode]') AND type='v' )
DROP VIEW [Aggregation].[vwNode]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwNodeMetric]') AND type='v' )
DROP VIEW [Aggregation].[vwNodeMetric]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwNodeMetricValue]') AND type='v' )
DROP VIEW [Aggregation].[vwNodeMetricValue]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwNodeParent]') AND type='v' )
DROP VIEW [Aggregation].[vwNodeParent]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwNodeType]') AND type='v' )
DROP VIEW [Aggregation].[vwNodeType]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwNodeTypeMetric]') AND type='v' )
DROP VIEW [Aggregation].[vwNodeTypeMetric]
GO

CREATE view [Aggregation].[vwNode] as
SELECT [id]
      ,[name]
      ,[active]
      ,[nodeTypeId]
      ,[connectorId]
      ,[connectorCreds]
      ,[sourceInfo]
      ,[pollerId]
      ,[useMetricTemplate]
      ,[nodeTypeTemplateId]
      ,[pollingInterval]
      ,[dateCreated]
      ,[dateUpdated]
      ,[pollPolicy]
      ,[lastPollDate]
      ,[nextPollDate]
      ,cast([nodeData] as XML) as nodeData
      ,[processorName]
	  ,activationDate
	  ,deactivationDate
    FROM [IGENMSDB].[igenms].[dbo].[vwNode]
    WHERE [nodeTypeId]  IN ('2', '3', '4', '8', '10', '12')
GO

CREATE view [Aggregation].[vwNodeMetric]
as
SELECT [id]
      ,[nodeId]
      ,[nodeTypeId]
      ,[metricKey]
      ,[metricValueType]
      ,[sampleValue]
      ,[description]
      ,[metricValue1Label]
      ,[metricValue2Label]
      ,[metricValue3Label]
      ,[active]
      ,[pollingInterval]
      ,[dateCreated]
      ,[dateUpdated]
      ,[metricSourceInfo]
      ,[name]
      ,[metricUnits]
	  ,cast(nodeMetricData as XML) nodeMetricData
    FROM [IGENMSDB].[igenms].[dbo].[vwNodeMetric]
    WHERE [nodeTypeId]  IN ('2', '3', '4', '8', '10', '12')
GO

CREATE view [Aggregation].[vwNodeMetricValue] as 
SELECT [id]
      ,[nodeId]
      ,[nodeMetricId]
      ,[nodeTypeId]
      ,[metricKey]
      ,[metricValueType]
      ,[timestamp]
      ,[value]
      ,[metricvalue1]
      ,[metricvalue2]
      ,[metricvalue3]
      ,[epochTimestamp]
      ,[lastPolledTimestamp]
      ,[backFillMetricValueID]
      ,[isbackFilled]
    FROM [IGENMSDB].[igenms].[dbo].[tbNodeMetricValue]
    WHERE [nodeTypeId]  IN ('2', '3', '4', '8', '10', '12')
GO

CREATE view [Aggregation].[vwNodeParent] as
SELECT P.[id]
      ,P.[nodeId]
      ,P.[parentnodeId]
      ,P.[active]
      ,P.[dateCreated]
      ,P.[dateUpdated]
    FROM [IGENMSDB].[igenms].[dbo].[tbNodeParent] P Inner Join
        [IGENMSDB].[igenms].[dbo].[vwNode] N on P.[nodeId] = N.[id]
    WHERE N.[nodeTypeId]  IN ('2', '3', '4', '8', '10', '12')
GO

CREATE view [Aggregation].[vwNodeType] as
SELECT [id]
      ,[name]
      ,[active]
      ,[description]
      ,[dateCreated]
      ,[dateUpdated]
	  ,cast([config] as XML) as [config]
    FROM [IGENMSDB].[igenms].[dbo].[vwNodeType]
    WHERE [id]  IN ('2', '3', '4', '8', '10', '12')
GO

CREATE view [Aggregation].[vwNodeTypeMetric]
as
SELECT [id]
      ,[nodeTypeId]
      ,[metricKey]
      ,[name]
      ,[metricValueType]
      ,[sampleValue]
      ,[description]
      ,[metricValue1Label]
      ,[metricValue2Label]
      ,[metricValue3Label]
      ,[pollingInterval]
      ,[dateCreated]
      ,[dateUpdated]
      ,[metricSourceInfo]
      ,[metricUnits]
      ,cast([rollUpAggregateConfig] as XML) as [rollUpAggregateConfig]
      ,[isCalculatedMetric]
      ,[displayName]
      ,cast([config] as XML) as [config]
      ,cast([preFillValueConfig] as XML) as [preFillValueConfig]
    FROM [IGENMSDB].[igenms].[dbo].[vwNodeTypeMetric]
    WHERE [nodeTypeId]  IN ('2', '3', '4', '8', '10', '12')
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spCleanRefTables]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spCleanRefTables]
GO

CREATE PROCEDURE [Aggregation].[spCleanRefTables]
AS
BEGIN
	Delete [Aggregation].[rtbNode]
	where [nodeTypeId] not in (2, 3, 4, 8, 10, 12);

	DELETE FROM [Aggregation].[rtbNodeMetric]
	where [nodeTypeId] not in (2, 3, 4, 8, 10, 12);

	Delete [Aggregation].[rtbNodeType]
	where [nodeTypeId] not in (2, 3, 4, 8, 10, 12);

	Delete FROM [Aggregation].[rtbNodeTypeMetric]
	where [nodeTypeId] not in (2, 3, 4, 8, 10, 12);
END
GO