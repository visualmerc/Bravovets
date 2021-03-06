﻿CREATE TABLE [dbo].[SyndicatedContent] (
    [SyndicatedContentId]			INT				IDENTITY (1, 1) NOT NULL,
    BravoVetsCountryId				INT             NOT NULL,
	SyndicatedContentTypeId			INT             NOT NULL,
	SyndicatedContentPostTypeId		INT				NOT NULL,
    [Title]							NVARCHAR (100)  NOT NULL,
	Author							NVARCHAR(200)	NULL,
    [Subject]						NVARCHAR (100)  NOT NULL,
    [Summary]						NVARCHAR (255)  NULL,
    [ContentText]					NVARCHAR (4000) NULL,
    [LinkUrlName]					NVARCHAR (255)  NULL,
    [LinkUrl]						NVARCHAR (512)  NULL,
    [BravoVetsStatusId]				INT             NOT NULL,
	NumberOfFavorites				INT				NOT NULL DEFAULT 0,
	NumberOfShares					INT				NOT NULL DEFAULT 0,
	NumberOfViews					INT				NOT NULL DEFAULT 0,
	PublishDateUtc					DATETIME		NOT NULL,
    [CreateDateUtc]					DATETIME        NOT NULL,
    [ModifiedDateUtc]				DATETIME        NOT NULL,
    [Deleted]						BIT             NOT NULL DEFAULT 0,
    CONSTRAINT [PK_SyndicatedContent] PRIMARY KEY CLUSTERED ([SyndicatedContentId] ASC),
    CONSTRAINT [FK_SyndicatedContent_Region] FOREIGN KEY ([BravoVetsCountryId]) REFERENCES [dbo].BravoVetsCountry ([BravoVetsCountryId]),
    CONSTRAINT [FK_SyndicatedContent_Status] FOREIGN KEY ([BravoVetsStatusId]) REFERENCES [dbo].[BravoVetsStatus] ([BravoVetsStatusId]),
    CONSTRAINT [FK_SyndicatedContent_PostType] FOREIGN KEY (SyndicatedContentPostTypeId) REFERENCES [dbo].[SyndicatedContentPostType]([SyndicatedContentPostTypeId]), 
    CONSTRAINT [FK_SyndicatedContent_ContentType] FOREIGN KEY (SyndicatedContentTypeId) REFERENCES dbo.SyndicatedContentType(SyndicatedContentTypeId)
);

