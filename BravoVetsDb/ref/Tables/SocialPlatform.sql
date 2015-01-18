CREATE TABLE [dbo].[SocialPlatform] (
    [SocialPlatformId]   INT            IDENTITY (1, 1) NOT NULL,
    [SocialPlatformName] NVARCHAR (128) NOT NULL,
    [Url]                NVARCHAR (256) NOT NULL,
    [CreateDateUtc]         DATETIME       NOT NULL,
    [ModifiedDateUtc]       DATETIME       NOT NULL,
    [Deleted]            BIT            NOT NULL,
    CONSTRAINT [pk_SocialPlatformId] PRIMARY KEY CLUSTERED ([SocialPlatformId] ASC)
);

