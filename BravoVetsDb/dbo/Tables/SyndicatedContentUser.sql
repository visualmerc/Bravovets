CREATE TABLE [dbo].[SyndicatedContentUser] (
    [SyndicatedContentUserId]		INT   IDENTITY (1, 1) NOT NULL,
    [SyndicatedContentId]			INT   NOT NULL,
    [BravoVetsUserId]				INT      NOT NULL,
	ActivityTypeId					INT	  NOT NULL,
	AddtionalText					NVARCHAR(255) NULL,
    [BravoVetsStatusId]             INT      NOT NULL,
    [CreateDateUtc]					DATETIME NOT NULL,
    [ModifiedDateUtc]				DATETIME NOT NULL,
    [Deleted]						BIT      NOT NULL DEFAULT 0,
    CONSTRAINT [PK_SyndicatedContentUser] PRIMARY KEY CLUSTERED ([SyndicatedContentUserId] ASC),
    CONSTRAINT [FK_SyndicatedContentUser_BravoVetsUser] FOREIGN KEY ([BravoVetsUserId]) REFERENCES [dbo].[BravoVetsUser] ([BravoVetsUserId]),
    CONSTRAINT [FK_SyndicatedContentUser_Status] FOREIGN KEY ([BravoVetsStatusId]) REFERENCES [dbo].[BravoVetsStatus] ([BravoVetsStatusId]),
    CONSTRAINT [FK_SyndicatedContentUser_SyndicatedContent] FOREIGN KEY ([SyndicatedContentId]) REFERENCES [dbo].[SyndicatedContent] ([SyndicatedContentId]), 
    CONSTRAINT [FK_SyndicatedContentUser_ActivityType] FOREIGN KEY (ActivityTypeId) REFERENCES dbo.ActivityType(ActivityTypeId)
);

