CREATE TABLE [dbo].[OutcomeRates]
(
	[Id] INT NOT NULL,
	[OutcomePriorityCustomer] INT NOT NULL, 
	[CustomerSatisfaction] INT NOT NULL, 
	[CareerManagement] INT NOT NULL, 
	[Jobs] INT NOT NULL, 
	[Learning] INT NOT NULL, 
	[EffectiveFrom] DATE NOT NULL, 
	[EffectiveTo] DATE NULL
	CONSTRAINT [PK_OutcomeRates] PRIMARY KEY CLUSTERED ([Id] Asc )
)
