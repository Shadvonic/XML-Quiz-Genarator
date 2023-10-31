CREATE TABLE [dbo].[Choices]
(
	[Id] uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [ChoiceText] NVARCHAR(MAX) NOT NULL, 
    [QuestionId] uniqueidentifier NOT NULL
)
