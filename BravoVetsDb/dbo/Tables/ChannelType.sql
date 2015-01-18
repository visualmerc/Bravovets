CREATE TABLE [dbo].[ChannelType]
(
	[ChannelTypeId] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	[ChannelTypeName] NVARCHAR (128) NOT NULL,
    [CreateDateUtc]       DATETIME       NOT NULL,
    [ModifiedDateUtc]     DATETIME       NOT NULL,
    [Deleted]          BIT            NOT NULL
)
