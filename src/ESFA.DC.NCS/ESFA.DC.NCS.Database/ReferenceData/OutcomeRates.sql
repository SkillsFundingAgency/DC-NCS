DECLARE @SummaryOfChanges_OutcomeRates TABLE ([Id] INT, [Action] VARCHAR(100));

MERGE INTO [OutcomeRates] AS Target
USING (VALUES		
	(1, 0, 10, 10, 20, 20, CONVERT(DATETIME, N'2018-10-01T00:00:00.000'), CONVERT(DATETIME, N'2020-09-30T00:00:00.000')),
	(2, 1, 45, 50, 70, 70, CONVERT(DATETIME, N'2018-10-01T00:00:00.000'), CONVERT(DATETIME, N'2020-09-30T00:00:00.000')),
	(3, 0, 10, 10, 30, 20, CONVERT(DATETIME, N'2020-10-01T00:00:00.000'), NULL),
	(4, 1, 45, 50, 70, 60, CONVERT(DATETIME, N'2020-10-01T00:00:00.000'), NULL)
)
	AS Source([Id], [OutcomePriorityCustomer], [CustomerSatisfaction], [CareerManagement], [Jobs], [Learning], [EffectiveFrom], [EffectiveTo])
	ON Target.[Id] = Source.[Id]
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT Target.[Id] ,
							Target.[OutcomePriorityCustomer],
							Target.[CustomerSatisfaction],
							Target.[CareerManagement],
							Target.[Jobs],
							Target.[Learning],
							Target.[EffectiveFrom],
							Target.[EffectiveTo]
					EXCEPT 
						SELECT Source.[Id] ,
							Source.[OutcomePriorityCustomer],
							Source.[CustomerSatisfaction],
							Source.[CareerManagement],
							Source.[Jobs],
							Source.[Learning],
							Source.[EffectiveFrom],
							Source.[EffectiveTo]													      
				)
		  THEN UPDATE SET 
						Target.[Id] = Source.[Id],
						Target.[OutcomePriorityCustomer] = Source.[OutcomePriorityCustomer],
						Target.[CustomerSatisfaction] = Source.[CustomerSatisfaction],
						Target.[CareerManagement] = Source.[CareerManagement],
						Target.[Jobs] = Source.[Jobs],
						Target.[Learning] = Source.[Learning],
						Target.[EffectiveFrom] = Source.[EffectiveFrom],
						Target.[EffectiveTo] = Source.[EffectiveTo]
	WHEN NOT MATCHED BY TARGET THEN INSERT([Id], [OutcomePriorityCustomer], [CustomerSatisfaction], [CareerManagement], [Jobs], [Learning], [EffectiveFrom], [EffectiveTo]) 
		VALUES ([Id], [OutcomePriorityCustomer], [CustomerSatisfaction], [CareerManagement], [Jobs], [Learning], [EffectiveFrom], [EffectiveTo])
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT isnull(deleted.Id,Inserted.[Id]),$action INTO @SummaryOfChanges_OutcomeRates	([Id], [Action])
;

	DECLARE @AddCount_OutcomeRates INT, @UpdateCount_OutcomeRates INT, @DeleteCount_OutcomeRates INT
	SET @AddCount_OutcomeRates  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_OutcomeRates WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_OutcomeRates = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_OutcomeRates WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_OutcomeRates = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_OutcomeRates WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		      %s - Added %i - Update %i - Delete %i',10,1,'AuditEventType', @AddCount_OutcomeRates, @UpdateCount_OutcomeRates, @DeleteCount_OutcomeRates) WITH NOWAIT;

