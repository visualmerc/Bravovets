CREATE TABLE [dbo].[BravoVetsStatus] (
    [BravoVetsStatusId]     INT            IDENTITY (1, 1) NOT NULL,
    [StatusName]   NVARCHAR (128) NOT NULL,
    [CreateDateUtc]   DATETIME       NOT NULL,
    [ModifiedDateUtc] DATETIME       NOT NULL,
    [Deleted]      BIT            NOT NULL,
    CONSTRAINT [pk_StatusId] PRIMARY KEY CLUSTERED ([BravoVetsStatusId] ASC)
);

