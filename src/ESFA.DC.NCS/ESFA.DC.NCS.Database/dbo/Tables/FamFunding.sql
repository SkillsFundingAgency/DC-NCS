CREATE TABLE [dbo].[FamFunding]
(
	[Id] INT NOT NULL,
	[TouchpointId] VARCHAR(10) NOT NULL,
	[Area] VARCHAR(100),
	[PrimeContractor] VARCHAR(100),
	[UKPRN] INT NOT NULL,
	[April]  DECIMAL(10, 2) NULL,
	[May]  DECIMAL(10, 2) NULL,
	[June]  DECIMAL(10, 2) NULL,
	[July]  DECIMAL(10, 2) NULL,
	[August]  DECIMAL(10, 2) NULL,
	[September]  DECIMAL(10, 2) NULL,
	[October]  DECIMAL(10, 2) NULL,
	[November]  DECIMAL(10, 2) NULL,
	[December]  DECIMAL(10, 2) NULL,
	[January]  DECIMAL(10, 2) NULL,
	[February]  DECIMAL(10, 2) NULL,
	[March]  DECIMAL(10, 2) NULL,
	[CollectionYear] INT NOT NULL,
	CONSTRAINT [PK_FamFunding] PRIMARY KEY CLUSTERED ([Id] Asc )
)
