CREATE TABLE [dbo].[SyndicatedContentAttachment]
(
	[SyndicatedContentAttachmentId]		INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	[SyndicatedContentId]				INT NOT NULL,
	AttachmentFileName					NVARCHAR(255) NOT NULL,
	AttachmentExtension					NVARCHAR(15) NOT NULL,
	AttachmentFile						VARBINARY(MAX) NOT NULL,
	DisplayInUi							BIT NOT NULL DEFAULT 1,
	[CreateDateUtc]						DATETIME NOT NULL,
    [ModifiedDateUtc]					DATETIME NOT NULL 

)
