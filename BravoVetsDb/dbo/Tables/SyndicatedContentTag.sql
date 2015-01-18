CREATE TABLE [dbo].[SyndicatedContentTag] (
    [SyndicatedContentTagId] INT            IDENTITY (1, 1) NOT NULL,
    [SyndicatedContentId]    INT         NOT NULL,
    [Tag]                    NVARCHAR (100) NOT NULL,
    [CreateDateUtc]             DATETIME       NOT NULL,
    [ModifiedDateUtc]           DATETIME       NOT NULL,
    [Deleted]                BIT            NOT NULL DEFAULT 0,
    CONSTRAINT [PK_SyndicatedContentTag] PRIMARY KEY CLUSTERED ([SyndicatedContentTagId] ASC),
    CONSTRAINT [FK_SyndicatedContentTag_SyndicatedContent] FOREIGN KEY ([SyndicatedContentId]) REFERENCES [dbo].[SyndicatedContent] ([SyndicatedContentId])
);

