CREATE TABLE [dbo].[ActivityType] (
    [ActivityTypeId]   INT            IDENTITY (1, 1) NOT NULL,
    [ActivityTypeName] NVARCHAR (128) NOT NULL,
    [CreateDateUtc]       DATETIME       NOT NULL,
    [ModifiedDateUtc]     DATETIME       NOT NULL,
    [Deleted]          BIT            NOT NULL,
    CONSTRAINT [pk_ActivityTypeId] PRIMARY KEY CLUSTERED ([ActivityTypeId] ASC)
);

