CREATE TABLE [dbo].[Questions]
(
	[Id]  uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [QuizId] uniqueidentifier NOT NULL, 
    [QuestionText] NVARCHAR(MAX) NOT NULL, 
    [CorrectChoiceIndex] INT NOT NULL, 
    [CorrectExplanation] NVARCHAR(MAX) NULL, 
    [IncorrectExplanation] NCHAR(10) NULL
)
