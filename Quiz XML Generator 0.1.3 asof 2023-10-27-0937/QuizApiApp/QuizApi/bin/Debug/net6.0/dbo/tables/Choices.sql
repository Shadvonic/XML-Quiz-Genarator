﻿CREATE TABLE [dbo].[Choices]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY , 
    [ChoiceText] NVARCHAR(MAX) NOT NULL, 
    [QuestionId] UNIQUEIDENTIFIER NOT NULL
)
