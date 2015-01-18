CREATE TABLE [dbo].[BravoVetsCountry] (
    [BravoVetsCountryId]     INT            IDENTITY (1, 1) NOT NULL,
    [CountryName]   NVARCHAR (128) NOT NULL,
    [CountryNameResourceKey]   NVARCHAR (128) NOT NULL,
	CountryIsoCode	NVARCHAR(3)	  NOT NULL,
    [BravoVetsLanguageId]   INT     NOT NULL,
    [LanguageCode] NVARCHAR (3)		NOT NULL,
	CultureName	   NVARCHAR(6)	  NOT NULL,
	Active			BIT				NOT NULL DEFAULT(1),
    [CreateDateUtc]   DATETIME       NOT NULL,
    [ModifiedDateUtc] DATETIME       NOT NULL,
    [Deleted]      BIT            NOT NULL,
    CONSTRAINT [pk_RegionId] PRIMARY KEY CLUSTERED ([BravoVetsCountryId] ASC),
    CONSTRAINT [FK_Region_Language] FOREIGN KEY ([BravoVetsLanguageId]) REFERENCES [dbo].[BravoVetsLanguage] ([BravoVetsLanguageId])
);

