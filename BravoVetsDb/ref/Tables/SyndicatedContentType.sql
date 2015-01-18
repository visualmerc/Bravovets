CREATE TABLE [dbo].[SyndicatedContentType]
(
	[SyndicatedContentTypeId] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	SyndicatedContentTypeName NVARCHAR (128) NOT NULL,
	[CreateDateUtc]         DATETIME       NOT NULL
)
