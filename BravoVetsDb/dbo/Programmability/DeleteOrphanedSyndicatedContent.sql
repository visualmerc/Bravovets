CREATE PROCEDURE [dbo].[DeleteOrphanedSyndicatedContent]
AS

	-- This script will delete any syndicated content (and its related tables) that is not in an "Active" state
	-- These entries will come from administrators creating the content, then being interrrupted through some error, browser crash, etc.

	DELETE FROM
	[dbo].[SyndicatedContentUser]
	WHERE SyndicatedContentId IN
		(SELECT SyndicatedContentId
		FROM [dbo].[SyndicatedContent]
		WHERE [BravoVetsStatusId] != 4)

	DELETE FROM
	[dbo].SyndicatedContentTag
	WHERE SyndicatedContentId IN
		(SELECT SyndicatedContentId
		FROM [dbo].[SyndicatedContent]
		WHERE [BravoVetsStatusId] != 4)

	DELETE FROM
	[dbo].SyndicatedContentAttachment
	WHERE SyndicatedContentId IN
		(SELECT SyndicatedContentId
		FROM [dbo].[SyndicatedContent]
		WHERE [BravoVetsStatusId] != 4)

	DELETE FROM [dbo].[SyndicatedContent]
		WHERE [BravoVetsStatusId] != 4

RETURN 0
