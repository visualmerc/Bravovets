CREATE TABLE [dbo].[Veterinarian] (
    [VeterinarianId] INT            IDENTITY (1, 1) NOT NULL,
    [BravoVetsCountryId]       INT            NOT NULL,
    [BusinessName]   NVARCHAR (256) NOT NULL,
    [JoinDate]       DATETIME       NOT NULL,
    [CreateDateUtc]     DATETIME       NOT NULL,
    [ModifiedDateUtc]   DATETIME       NOT NULL,
    [Deleted]        BIT            NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Veterinarian] PRIMARY KEY CLUSTERED ([VeterinarianId] ASC), 
    CONSTRAINT [FK_Veterinarian_ToTable] FOREIGN KEY ([BravoVetsCountryId]) REFERENCES [dbo].BravoVetsCountry(BravoVetsCountryId)
);

