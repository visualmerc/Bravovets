--================================================
--========= syndicated content updates ===========

ALTER TABLE [dbo].[SyndicatedContent]
ALTER COLUMN [LinkUrl] [NVARCHAR](512) NULL
GO

ALTER TABLE [dbo].[SyndicatedContent]
ADD [LinkUrlName] NVARCHAR (255)  NULL
GO

ALTER TABLE [dbo].[SyndicatedContentLink]
ALTER COLUMN [LinkUrl] [NVARCHAR](512) NULL
GO

--=====================================
--========= Country updates ===========

-- activate the United States entry
UPDATE [dbo].[BravoVetsCountry]
	SET [Active] = 1
	WHERE BravoVetsCountryId = 1
GO

DELETE FROM [dbo].[BravoVetsCountry]
      WHERE BravoVetsCountryId = 233
GO

--=====================================
--========= Orphaned content ===========

CREATE PROCEDURE [dbo].[FindOrphanedSyndicatedContent]

AS
	-- Run before delete sproc
	DECLARE @RecCount INT

	SELECT @RecCount = COUNT(SyndicatedContentUserId) FROM
	[dbo].[SyndicatedContentUser]
	WHERE SyndicatedContentId IN
		(SELECT SyndicatedContentId
		FROM [dbo].[SyndicatedContent]
		WHERE [BravoVetsStatusId] != 4)

	PRINT 'Number of SyndicatedContentUser Orphans ' + CAST(@RecCount AS VARCHAR)

	SELECT @RecCount = COUNT(SyndicatedContentTagId) FROM
	[dbo].SyndicatedContentTag
	WHERE SyndicatedContentId IN
		(SELECT SyndicatedContentId
		FROM [dbo].[SyndicatedContent]
		WHERE [BravoVetsStatusId] != 4)

	PRINT 'Number of SyndicatedContentTag Orphans ' + CAST(@RecCount AS VARCHAR)

	SELECT @RecCount = COUNT(SyndicatedContentAttachmentId) FROM
	[dbo].SyndicatedContentAttachment
	WHERE SyndicatedContentId IN
		(SELECT SyndicatedContentId
		FROM [dbo].[SyndicatedContent]
		WHERE [BravoVetsStatusId] != 4)

	PRINT 'Number of SyndicatedContentAttachment Orphans ' + CAST(@RecCount AS VARCHAR)

	SELECT @RecCount = COUNT(SyndicatedContentId) FROM
	[dbo].[SyndicatedContent]
		WHERE [BravoVetsStatusId] != 4

	PRINT 'Number of SyndicatedContent Orphans ' +  + CAST(@RecCount AS VARCHAR)

	PRINT 'Done'
RETURN 0
