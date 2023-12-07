IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spCastDateTimeTypeData]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spCastDateTimeTypeData]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spCastFloatTypeData]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spCastFloatTypeData]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spCastIntTypeData]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spCastIntTypeData]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spCastStringTypeData]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spCastStringTypeData]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spCleanUpJob]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spCleanUpJob]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spPopulateDailySummary]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spPopulateDailySummary]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spPopulateHourlySummary]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spPopulateHourlySummary]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spUpdateMetricDefaultValue]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spUpdateMetricDefaultValue]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spUpdateNodeTypeDefaultValue]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spUpdateNodeTypeDefaultValue]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbMetricDefaultValue]') AND type in (N'U'))
DROP TABLE [Aggregation].[tbMetricDefaultValue]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbTask]') AND type in (N'U'))
DROP TABLE [Aggregation].[tbTask]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbStaging]') AND type in (N'U'))
DROP TABLE [Aggregation].[tbStaging]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbObservationIds]') AND type in (N'U'))
DROP TABLE [Aggregation].[tbObservationIds]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbNodeStatus]') AND type in (N'U'))
DROP TABLE [Aggregation].[tbNodeStatus]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[tbNodeType]') AND type in (N'U'))
DROP TABLE [Aggregation].[tbNodeType]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[vwChildNodeStatus]') AND type='v' )
DROP VIEW [Aggregation].[vwChildNodeStatus]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id  = OBJECT_ID(N'[Aggregation].[fnGetLastId]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
DROP FUNCTION [Aggregation].[fnGetLastId]
GO

IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Aggregation].[spCleanUpLogs]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Aggregation].[spCleanUpLogs]
GO