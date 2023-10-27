CREATE TABLE [dbo].[Questions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [QuizId] INT NOT NULL, 
    [QuestionText] NVARCHAR(MAX) NOT NULL, 
    [CorrectChoiceIndex] INT NOT NULL, 
    [CorrectExplanation] NVARCHAR(MAX) NULL, 
    [IncorrectExplanation] NCHAR(10) NULL
)
