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

BEGIN
   IF NOT EXISTS (SELECT * From [dbo].[BlogItem] where Name = 'NeuralNetwork')
   BEGIN
        INSERT INTO [dbo].[BlogItem] ([Name], [TimeStamp], [Comments])
		VALUES('NeuralNetwork', GETDATE(), '');
   END
END

BEGIN
   IF NOT EXISTS (SELECT * From [dbo].[BlogItem] where Name = 'CNN')
   BEGIN
        INSERT INTO [dbo].[BlogItem] ([Name], [TimeStamp], [Comments])
		VALUES ('CNN', GETDATE(), '');
   END
END

BEGIN
   IF NOT EXISTS (SELECT * From [dbo].[BlogItem] where Name = 'OtherModelling')
   BEGIN
        INSERT INTO [dbo].[BlogItem] ([Name], [TimeStamp], [Comments])
		VALUES('OtherModelling', GETDATE(), '');
   END
END

BEGIN
   IF NOT EXISTS (SELECT * From [dbo].[BlogItem] where Name = 'Convolution')
   BEGIN
        INSERT INTO [dbo].[BlogItem] ([Name], [TimeStamp], [Comments])
		VALUES ('Convolution', GETDATE(), '');
   END
END

BEGIN
   IF NOT EXISTS (SELECT * From [dbo].[BlogItem] where Name = 'TrackingImages')
   BEGIN
        INSERT INTO [dbo].[BlogItem] ([Name], [TimeStamp], [Comments])
		VALUES ('TrackingImages', GETDATE(), '');
   END
END

BEGIN
   IF NOT EXISTS (SELECT * From [dbo].[BlogItem] where Name = 'CleaningImages')
   BEGIN
        INSERT INTO [dbo].[BlogItem] ([Name], [TimeStamp], [Comments])
		VALUES ('CleaningImages', GETDATE(), '');
   END
END