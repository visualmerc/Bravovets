CREATE TABLE [dbo].[SyndicatedContentPostType]
(
	[SyndicatedContentPostTypeId] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	SyndicatedContentPostTypeName NVARCHAR(200) NOT NULL,
	[CreateDateUtc]       DATETIME       NOT NULL,
    [ModifiedDateUtc]     DATETIME       NOT NULL,
    [Deleted]          BIT            NOT NULL

)
