CREATE TABLE [dbo].[Questions]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [QuizId] UNIQUEIDENTIFIER NOT NULL, 
    [QuestionText] NVARCHAR(MAX) NOT NULL, 
    [CorrectChoiceIndex] INT NOT NULL, 
    [CorrectExplanation] NVARCHAR(MAX) NULL, 
    [IncorrectExplanation] NVARCHAR(MAX) NULL
)
