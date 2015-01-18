CREATE TABLE [dbo].[FeaturedContent]
(
	[FeaturedContentId]			INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	SyndicatedContentTypeId		INT NOT NULL,
    BravoVetsCountryId			INT NOT NULL,
	ContentFileName				NVARCHAR(255) NOT NULL,
	ContentExtension			NVARCHAR(15) NOT NULL,
	ContentThumbnail			VARBINARY(MAX) NULL,
	ContentFile					VARBINARY(MAX) NOT NULL,
	[CreateDateUtc]				DATETIME NOT NULL,
    [ModifiedDateUtc]			DATETIME NOT NULL, 
    CONSTRAINT [FK_FeaturedContent_ToCountry] FOREIGN KEY (BravoVetsCountryId) REFERENCES BravoVetsCountry(BravoVetsCountryId) 

)

GO


