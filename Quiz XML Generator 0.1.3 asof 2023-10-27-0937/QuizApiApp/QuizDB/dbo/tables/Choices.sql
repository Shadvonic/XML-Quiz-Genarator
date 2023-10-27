CREATE TABLE [dbo].[Choices]
(
	[Id] INT NOT NULL PRIMARY KEY  IDENTITY, 
    [ChoiceText] NVARCHAR(MAX) NOT NULL, 
    [QuestionId] INT NOT NULL
)
