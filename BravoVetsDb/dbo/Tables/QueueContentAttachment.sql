CREATE TABLE [dbo].[QueueContentAttachment]
(
	[QueueContentAttachmentId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [QueueContentId] INT NOT NULL,
	[SyndicatedContentAttachmentId] [int] NULL,
	[AttachmentFileName] [nvarchar](255)  NULL,
	[AttachmentExtension] [nvarchar](15) NULL,
	[AttachmentFile] [varbinary](max)  NULL,
	[CreateDateUtc] [datetime] NULL,
	[ModifiedDateUtc] [datetime] NULL
)
