CREATE PROCEDURE [dbo].[FindOrphanedSyndicatedContent]


AS
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


RETURN 0
