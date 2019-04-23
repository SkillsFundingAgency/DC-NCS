﻿CREATE TABLE [dbo].[Source]
(
	[SourceId] INT IDENTITY (1, 1) NOT NULL,
	[UKPRN] INT NOT NULL,
	[TouchpointId] VARCHAR(10) NOT NULL,
	[SubmissionDate] DATETIME NOT NULL,
	[DssJobId] UNIQUEIDENTIFIER NOT NULL,
	[CreatedOn] DATETIME NOT NULL,
	CONSTRAINT [PK_Source] PRIMARY KEY CLUSTERED ([SourceId] ASC)
)
