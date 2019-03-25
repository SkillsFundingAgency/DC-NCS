DECLARE @SummaryOfChanges_OutcomeRates TABLE ([Id] INT, [Action] VARCHAR(100));

MERGE INTO [OutcomeRates] AS Target
USING (VALUES		
	(1, 0, 10, 10, 20, CONVERT(DATETIME, N'2018-10-01T00:00:00.000'), NULL, N'Community'),
	(2, 1, 45, 50, 70, CONVERT(DATETIME, N'2018-10-01T00:00:00.000'), NULL, N'Community')
)
	AS Source([Id], [OutcomePriorityCustomer], [CustomerSatisfaction], [CareerManagement], [JobsAndLearning], [EffectiveFrom], [EffectiveTo], [Delivery])
	ON Target.[Id] = Source.[Id]
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT Target.[Id] ,
							Target.[OutcomePriorityCustomer],
							Target.[CustomerSatisfaction],
							Target.[CareerManagement],
							Target.[JobsAndLearning],
							Target.[EffectiveFrom],
							Target.[EffectiveTo],
							Target.[Delivery]
					EXCEPT 
						SELECT Source.[Id] ,
							Source.[OutcomePriorityCustomer],
							Source.[CustomerSatisfaction],
							Source.[CareerManagement],
							Source.[JobsAndLearning],
							Source.[EffectiveFrom],
							Source.[EffectiveTo],
							Source.[Delivery]													      
				)
		  THEN UPDATE SET 
						Target.[Id] = Source.[Id],
						Target.[OutcomePriorityCustomer] = Source.[OutcomePriorityCustomer],
						Target.[CustomerSatisfaction] = Source.[CustomerSatisfaction],
						Target.[CareerManagement] = Source.[CareerManagement],
						Target.[JobsAndLearning] = Source.[JobsAndLearning],
						Target.[EffectiveFrom] = Source.[EffectiveFrom],
						Target.[EffectiveTo] = Source.[EffectiveTo],
						Target.[Delivery] = Source.[Delivery]
	WHEN NOT MATCHED BY TARGET THEN INSERT([Id], [OutcomePriorityCustomer], [CustomerSatisfaction], [CareerManagement], [JobsAndLearning], [EffectiveFrom], [EffectiveTo], [Delivery]) 
		VALUES ([Id], [OutcomePriorityCustomer], [CustomerSatisfaction], [CareerManagement], [JobsAndLearning], [EffectiveFrom], [EffectiveTo], [Delivery])
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT isnull(deleted.Id,Inserted.[Id]),$action INTO @SummaryOfChanges_OutcomeRates	([Id], [Action])
;

	DECLARE @AddCount_OutcomeRates INT, @UpdateCount_OutcomeRates INT, @DeleteCount_OutcomeRates INT
	SET @AddCount_OutcomeRates  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_OutcomeRates WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_OutcomeRates = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_OutcomeRates WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_OutcomeRates = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_OutcomeRates WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		      %s - Added %i - Update %i - Delete %i',10,1,'AuditEventType', @AddCount_OutcomeRates, @UpdateCount_OutcomeRates, @DeleteCount_OutcomeRates) WITH NOWAIT;

