DECLARE @SummaryOfChanges_FamFunding TABLE ([Id] INT, [Action] VARCHAR(100));

MERGE INTO [FamFunding] AS Target
USING (VALUES		
	(1, N'0000000108', N'South West and Oxfordshire', N'Adviza', 10028942, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 2021),
	(2, N'0000000106', N'North East and Cumbria', N'EDT', 10001298, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 2021),
	(3, N'0000000109', N'Yorkshire and the Humber', N'EDT', 10001298, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 2021),
	(4, N'0000000102', N'East Midlands', N'Futures', 10001647, NULL, NULL, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 12000.00, 2021),
	(5, N'0000000101', N'East of England plus Buckinghamshire', N'Futures', 10001647, NULL, NULL, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 2021),
	(6, N'0000000105', N'North West', N'Growth', 10004177, NULL, NULL, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 2021),
	(7, N'0000000103', N'London', N'Prospects', 10005262, NULL, NULL, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 2021),
	(8, N'0000000104', N'West Midlands', N'Prospects', 10005262, NULL, NULL, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 15833.33, 2021),
	(9, N'0000000107', N'South East', N'CXK', 10001648, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 19666.67, 2021)
)
	AS Source([Id], [TouchpointId], [Area], [PrimeContractor], [UKPRN], [April], [May], [June], [July], [August], [September], [October], [November], [December], [January], [February], [March], [CollectionYear])
	ON Target.[Id] = Source.[Id]
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT Target.[Id] ,
							Target.[TouchpointId],
							Target.[Area],
							Target.[PrimeContractor],
							Target.[UKPRN],
							Target.[April],
							Target.[May],
							Target.[June],
							Target.[July],
							Target.[August],
							Target.[September],
							Target.[October],
							Target.[November],
							Target.[December],
							Target.[January],
							Target.[February],
							Target.[March],
							Target.[CollectionYear]
					EXCEPT 
						SELECT Source.[Id] ,
							Source.[TouchpointId],
							Source.[Area],
							Source.[PrimeContractor],
							Source.[UKPRN],
							Source.[April],
							Source.[May],
							Source.[June],
							Source.[July],
							Source.[August],
							Source.[September],
							Source.[October],
							Source.[November],
							Source.[December],
							Source.[January],
							Source.[February],
							Source.[March],
							Source.[CollectionYear]													      
				)
		  THEN UPDATE SET 
						Target.[Id] = Source.[Id],
						Target.[TouchpointId] = Source.[TouchpointId],
						Target.[Area] = Source.[Area],
						Target.[PrimeContractor] = Source.[PrimeContractor],
						Target.[UKPRN] = Source.[UKPRN],
						Target.[April] = Source.[April],
						Target.[May] = Source.[May],
						Target.[June] = Source.[June],
						Target.[July] = Source.[July],
						Target.[August] = Source.[August],
						Target.[September] = Source.[September],
						Target.[October] = Source.[October],
						Target.[November] = Source.[November],
						Target.[December] = Source.[December],
						Target.[January] = Source.[January],
						Target.[February] = Source.[February],
						Target.[March] = Source.[March],
						Target.[CollectionYear] = Source.[CollectionYear]
	WHEN NOT MATCHED BY TARGET THEN INSERT([Id], [TouchpointId], [Area], [PrimeContractor], [UKPRN], [April], [May], [June], [July], [August], [September], [October], [November], [December], [January], [February], [March], [CollectionYear]) 
		VALUES ([Id], [TouchpointId], [Area], [PrimeContractor], [UKPRN], [April], [May], [June], [July], [August], [September], [October], [November], [December], [January], [February], [March], [CollectionYear])
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT isnull(deleted.Id,Inserted.[Id]),$action INTO @SummaryOfChanges_FamFunding	([Id], [Action])
;

	DECLARE @AddCount_FamFunding INT, @UpdateCount_FamFunding INT, @DeleteCount_FamFunding INT
	SET @AddCount_FamFunding  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FamFunding WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_FamFunding = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FamFunding WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_FamFunding = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_FamFunding WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		      %s - Added %i - Update %i - Delete %i',10,1,'AuditEventType', @AddCount_FamFunding, @UpdateCount_FamFunding, @DeleteCount_FamFunding) WITH NOWAIT;

