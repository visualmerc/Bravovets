CREATE TABLE [dbo].[QueueContent]
(
	[QueueContentId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [BravoVetsCountryId] INT NOT NULL, 
    [BravoVetsUserId] INT NOT NULL,
    [SyndicatedContentPostTypeId] INT NULL, 
    [ContentText] NVARCHAR(4000) NOT NULL, 
    [SyndicatedContentId] INT NULL, 
    [LinkUrl] NVARCHAR(255) NULL, 
    [BravoVetsStatusId] INT NOT NULL,
	[PublishDateUtc] [datetime] NOT NULL,
	[PlatformPublishId] INT NOT NULL,
    [AccountName]       NVARCHAR (255) NULL,
	[AccessCode] [varchar](2000) NULL,
	[IsPublished] [bit] NOT NULL DEFAULT 0,
	PublishError NVARCHAR(255) NULL, 
	[CreateDateUtc] [datetime] NOT NULL,
	[ModifiedDateUtc] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL, 
    CONSTRAINT [FK_QueueContent_ToCountry] FOREIGN KEY (BravoVetsCountryId) REFERENCES dbo.BravoVetsCountry(BravoVetsCountryId), 
    CONSTRAINT [FK_QueueContent_ToSyndicatedContentPostType] FOREIGN KEY (SyndicatedContentPostTypeId) REFERENCES dbo.SyndicatedContentPostType(SyndicatedContentPostTypeId), 
    CONSTRAINT [FK_QueueContent_ToBravoVetsStatus] FOREIGN KEY (BravoVetsStatusId) REFERENCES dbo.BravoVetsStatus(BravoVetsStatusId), 
    CONSTRAINT [FK_QueueContent_ToPlatform] FOREIGN KEY (PlatformPublishId) REFERENCES dbo.SocialPlatform(SocialPlatformId), 
    CONSTRAINT [FK_QueueContent_ToUser] FOREIGN KEY (BravoVetsUserId) REFERENCES dbo.BravoVetsUser(BravoVetsUserId)

)
