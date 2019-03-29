CREATE TABLE [dbo].[OutcomeRates]
(
	[Id] INT NOT NULL,
	[OutcomePriorityCustomer] INT NOT NULL, 
	[CustomerSatisfaction] INT NOT NULL, 
	[CareerManagement] INT NOT NULL, 
	[JobsAndLearning] INT NOT NULL, 
	[EffectiveFrom] DATE NOT NULL, 
	[EffectiveTo] DATE NULL, 
	[Delivery] VARCHAR(50) NOT NULL
	CONSTRAINT [PK_OutcomeRates] PRIMARY KEY CLUSTERED ([Id] Asc )
)
