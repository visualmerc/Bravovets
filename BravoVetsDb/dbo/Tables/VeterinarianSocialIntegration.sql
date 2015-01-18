CREATE TABLE [dbo].[VeterinarianSocialIntegration] (
    [VeterinarianSocialIntegrationId] INT            IDENTITY (1, 1) NOT NULL,
    [VeterinarianId]                  INT            NOT NULL,
    [SocialPlatformId]                INT            NOT NULL,
    [AccountName]                     NVARCHAR (255) NULL,
    [AccountPassword]                 NVARCHAR (255) NULL,
	AccessToken						  VARCHAR(2000)  NULL,
	AccessCode						  VARCHAR(2000)  NULL,
	PageId							  VARCHAR(2000)  NULL,
	[PageAccessToken]						  VARCHAR(2000)  NULL,
	NumberOfFollowers				  INT NOT NULL DEFAULT 0,
	FollowerDiff						 INT NOT NULL DEFAULT 0,
	LastFeed							VARCHAR(MAX) NULL,
    [CreateDateUtc]                      DATETIME       NOT NULL,
    [ModifiedDateUtc]                    DATETIME       NOT NULL,
    [Deleted]                         BIT            CONSTRAINT [DF_VeterinarianSocialIntegration_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_VeterinarianSocialIntegration] PRIMARY KEY CLUSTERED ([VeterinarianSocialIntegrationId] ASC),
    CONSTRAINT [FK_VeterinarianSocialIntegration_SocialPlatform] FOREIGN KEY ([SocialPlatformId]) REFERENCES [dbo].[SocialPlatform] ([SocialPlatformId]),
    CONSTRAINT [FK_VeterinarianSocialIntegration_Veterinarian] FOREIGN KEY ([VeterinarianId]) REFERENCES [dbo].[Veterinarian] ([VeterinarianId])

);

