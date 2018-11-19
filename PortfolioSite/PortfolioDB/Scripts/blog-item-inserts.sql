/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

INSERT INTO [dbo].[BlogItem] ([Name], [TimeStamp], [Comments])
VALUES('NeuralNetwork', GETDATE(), ''),
('CNN', GETDATE(), ''),
('OtherModelling', GETDATE(), ''),
('Convolution', GETDATE(), ''),
('TrackingImages', GETDATE(), ''),
('CleaningImages', GETDATE(), '')
SELECT [Id], [Name], [TimeStamp], [Comments]
FROM [dbo].[BlogItem] tbl2
WHERE NOT EXISTS (Select [Id], [Name] From [dbo].[BlogItem] tbl1 WHERE tbl1.Name = tbl2.Name)