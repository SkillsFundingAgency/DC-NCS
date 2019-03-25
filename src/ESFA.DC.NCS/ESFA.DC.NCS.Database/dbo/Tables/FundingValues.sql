CREATE TABLE [dbo].[FundingValues]
(
	[UKPRN]  INT NOT NULL,
	[TouchpointId] VARCHAR(10) NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
	[ActionPlanId] UNIQUEIDENTIFIER NOT NULL,
	[OutcomeId] UNIQUEIDENTIFIER NOT NULL,
	[OutcomeType] INT NOT NULL,
	[OutcomeEffectiveDate] DATE NOT NULL,
	[OutcomePriorityGroup] INT NOT NULL,
	[Value]  INT NOT NULL,
	[Period]  VARCHAR(12) NOT NULL
	CONSTRAINT [PK_FundingValues] PRIMARY KEY CLUSTERED ([UKPRN] ASC, [TouchpointId] ASC, [CustomerId] ASC, [ActionPlanId] ASC, [OutcomeId] ASC)
)
