CREATE TABLE [dbo].[NcsSubmission]
(
	[Id] INT NOT NULL IDENTITY,
	[UKPRN] INT NOT NULL,
	[TouchpointId] VARCHAR(50) NOT NULL,
	[CustomerID] UNIQUEIDENTIFIER NOT NULL,
	[DateOfBirth] DATE NOT NULL,
	[HomePostCode] VARCHAR(50),
	[ActionPlanId] UNIQUEIDENTIFIER NOT NULL,
	[SessionDate] DATE NOT NULL,
	[SubContractorId] VARCHAR(50),
	[AdviserName] VARCHAR(50),
	[OutcomeId] UNIQUEIDENTIFIER NOT NULL,
	[OutcomeType] INT NOT NULL,
	[OutcomeEffectiveDate] DATE NOT NULL,
	[OutcomePriorityCustomer] INT NOT NULL,
	[ReturnPeriod] INT NOT NULL,
	[CollectionYear] INT NOT NULL,
	[DssJobId] UNIQUEIDENTIFIER NOT NULL,
	[DssTimestamp] DATETIME NOT NULL,
	[CreatedOn] DATETIME DEFAULT (GETDATE()) NOT NULL
	CONSTRAINT [PK_NcsSubmission] PRIMARY KEY CLUSTERED ([Id] ASC )
)
