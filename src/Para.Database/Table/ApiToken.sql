CREATE TABLE [dbo].[ApiToken]
(
	[Id]			INT				NOT NULL PRIMARY KEY IDENTITY,     
	[Token]			CHAR(32)		NOT NULL DEFAULT REPLACE(NEWID(),'-',''),
	[Usage]			BIGINT			NOT NULL DEFAULT 0, 	
	[IsActive]		BIT				NOT NULL DEFAULT 1,    
	[CreatedAt]		DATETIME		NOT NULL DEFAULT GETDATE(),	      
    [DeletedAt]		DATETIME		NULL,
	[IsDeleted]		BIT				NOT NULL DEFAULT 0
)