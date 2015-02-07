CREATE TABLE [dbo].[CurrencyValue]
(
	[Id]			INT				NOT NULL PRIMARY KEY IDENTITY,     
	[Day]			CHAR(8)			NOT NULL,
	[Source]		VARCHAR(10)		NOT NULL,
	[Target]		VARCHAR(10)		NOT NULL,
	[ValueSource]	VARCHAR(100)	NOT NULL,
	[ValueType]		VARCHAR(50)		NOT NULL,
	[Value]			DECIMAL(6, 4)	NOT NULL
)
