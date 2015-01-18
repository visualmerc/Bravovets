CREATE TABLE [dbo].[BravoVetsUserActivity] (
    [BravoVetsUserActivityId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [BravoVetsUserId]         INT            NOT NULL,
    [ActivityTypeId]          INT            NOT NULL,
    [Description]             NVARCHAR (128) NULL,
    [Url]                     NVARCHAR (256) NULL,
    [CreateDateUtc]              DATETIME       NOT NULL,
    [ModifiedDateUtc]            DATETIME       NOT NULL,
    [Deleted]                 BIT            NOT NULL DEFAULT 0,
    CONSTRAINT [PK_BravoVetsUserActivity] PRIMARY KEY CLUSTERED ([BravoVetsUserActivityId] ASC),
    CONSTRAINT [FK_BravoVetsUserActivity_ActivityType] FOREIGN KEY ([ActivityTypeId]) REFERENCES [dbo].[ActivityType] ([ActivityTypeId]),
    CONSTRAINT [FK_BravoVetsUserActivity_BravoVetsUser] FOREIGN KEY ([BravoVetsUserId]) REFERENCES [dbo].[BravoVetsUser] ([BravoVetsUserId])
);

