CREATE TABLE [dbo].[SyndicatedContentLink]
(
	[SyndicatedContentLinkId]			INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	[SyndicatedContentId]				INT NOT NULL,
	LinkTitle							NVARCHAR (255) NOT NULL,
    [LinkUrl]							NVARCHAR (512) NOT NULL,
	[SyndicatedContentAttachmentId]		INT NULL,
	[CreateDateUtc]						DATETIME NOT NULL,
    [ModifiedDateUtc]					DATETIME NOT NULL, 
    CONSTRAINT [FK_SyndicatedContentLink_ToSyndicatedContent] FOREIGN KEY (SyndicatedContentId) REFERENCES dbo.SyndicatedContent(SyndicatedContentId) 
)
