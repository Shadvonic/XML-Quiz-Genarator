CREATE TABLE [dbo].[Quizzes]
(
	[Id]  uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [QuizName] NVARCHAR(MAX) NOT NULL
)
