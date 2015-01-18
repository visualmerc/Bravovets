CREATE TABLE [dbo].[BravoVetsLanguage] (
    [BravoVetsLanguageId]   INT            IDENTITY (1, 1) NOT NULL,
    [LanguageName] NVARCHAR (128) NOT NULL,
    [CreateDateUtc]   DATETIME       NOT NULL,
    [ModifiedDateUtc] DATETIME       NOT NULL,
    [Deleted]      BIT            NOT NULL,
    CONSTRAINT [pk_LanguageId] PRIMARY KEY CLUSTERED ([BravoVetsLanguageId] ASC)
);

