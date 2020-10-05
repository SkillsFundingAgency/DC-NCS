/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'OutcomeRates'))
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Learning'
          AND Object_ID = Object_ID(N'dbo.OutcomeRates'))
	BEGIN
		DELETE FROM [dbo].[OutcomeRates]  
	END
END
