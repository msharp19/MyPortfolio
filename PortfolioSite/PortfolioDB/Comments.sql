CREATE TABLE [dbo].[Comments]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [BlogItemId] INT NOT NULL, 
    [BlogItemName] INT NOT NULL, 
    [Username] NVARCHAR(50) NOT NULL, 
    [DateSent] DATETIME NOT NULL DEFAULT GETDATE(), 
    [Content] NVARCHAR(1000) NOT NULL, 
    [SubCommentCount] INT NOT NULL DEFAULT 0, 
    [ParentId] INT NULL, 
    [Email] NVARCHAR(150) NULL, 
    CONSTRAINT [FK_Comments_ToBlogItem] FOREIGN KEY ([BlogItemId]) REFERENCES [BlogItem]([Id]), 
    CONSTRAINT [FK_Comments_ToComment] FOREIGN KEY ([ParentId]) REFERENCES [Comments]([Id])
)
