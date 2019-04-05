CREATE TABLE [dbo].[NcsSubmission]
(
	[UKPRN] INT NOT NULL,
	[TouchpointId] VARCHAR(10) NOT NULL,
	[CustomerId] UNIQUEIDENTIFIER NOT NULL,
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
	[CollectionYear] INT NOT NULL,
	[DssJobId] UNIQUEIDENTIFIER NOT NULL,
	[DssTimestamp] DATETIME NOT NULL,
	[CreatedOn] DATETIME NOT NULL
	CONSTRAINT [PK_NcsSubmission] PRIMARY KEY CLUSTERED ([UKPRN] ASC, [TouchpointId] ASC, [CustomerId] ASC, [ActionPlanId] ASC, [OutcomeId] ASC )
)
