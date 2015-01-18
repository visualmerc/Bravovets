﻿/***************************************
***   Static data management script  ***
***************************************/

-- This script will manage the static data from
-- your Team Database project for [dbo].[SyndicatedContentPostType].

PRINT 'Updating static data table [dbo].[SyndicatedContentPostType]'

-- Set date format to ensure text dates are parsed correctly
-- SET DATEFORMAT ymd

-- Turn off affected rows being returned
SET NOCOUNT ON

-- Change this to 1 to delete missing records in the target
-- WARNING: Setting this to 1 can cause damage to your database
-- and cause failed deployment if there are any rows referencing
-- a record which has been deleted.
DECLARE @DeleteMissingRecords BIT
SET @DeleteMissingRecords = 0

-- 1: Define table variable
DECLARE @tblTempTable TABLE (
[SyndicatedContentPostTypeId] int,
[SyndicatedContentPostTypeName] nvarchar(200),
[CreateDateUtc] datetime,
[ModifiedDateUtc] datetime,
[Deleted] bit
)

-- 2: Populate the table variable with data
-- This is where you manage your data in source control. You
-- can add and modify entries, but because of potential foreign
-- key contraint violations this script will not delete any
-- removed entries. If you remove an entry then it will no longer
-- be added to new databases based on your schema, but the entry
-- will not be deleted from databases in which the value already exists.
INSERT INTO @tblTempTable ([SyndicatedContentPostTypeId], [SyndicatedContentPostTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('1', 'Original', '2014-01-27 16:42:06.957', '2014-01-27 16:42:06.957', 'False')
INSERT INTO @tblTempTable ([SyndicatedContentPostTypeId], [SyndicatedContentPostTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('2', 'RssPost', '2014-01-27 16:42:06.957', '2014-01-27 16:42:06.957', 'False')
INSERT INTO @tblTempTable ([SyndicatedContentPostTypeId], [SyndicatedContentPostTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('3', 'LinkPostImage', '2014-01-27 16:42:06.957', '2014-01-27 16:42:06.957', 'False')
INSERT INTO @tblTempTable ([SyndicatedContentPostTypeId], [SyndicatedContentPostTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('4', 'LinkPostPage', '2014-01-27 16:42:06.957', '2014-01-27 16:42:06.957', 'False')
INSERT INTO @tblTempTable ([SyndicatedContentPostTypeId], [SyndicatedContentPostTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('5', 'ImagePost', '2014-01-27 16:42:06.957', '2014-01-27 16:42:06.957', 'False')
INSERT INTO @tblTempTable ([SyndicatedContentPostTypeId], [SyndicatedContentPostTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('6', 'Video', '2014-01-27 16:42:06.957', '2014-01-27 16:42:06.957', 'False')
INSERT INTO @tblTempTable ([SyndicatedContentPostTypeId], [SyndicatedContentPostTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('7', 'TextOnly', '2014-01-27 16:42:06.957', '2014-01-27 16:42:06.957', 'False')


-- 3: Insert any new items into the table from the table variable
SET IDENTITY_INSERT [dbo].[SyndicatedContentPostType] ON
INSERT INTO [dbo].[SyndicatedContentPostType] ([SyndicatedContentPostTypeId], [SyndicatedContentPostTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted])
SELECT tmp.[SyndicatedContentPostTypeId], tmp.[SyndicatedContentPostTypeName], tmp.[CreateDateUtc], tmp.[ModifiedDateUtc], tmp.[Deleted]
FROM @tblTempTable tmp
LEFT JOIN [dbo].[SyndicatedContentPostType] tbl ON tbl.[SyndicatedContentPostTypeId] = tmp.[SyndicatedContentPostTypeId]
WHERE tbl.[SyndicatedContentPostTypeId] IS NULL
SET IDENTITY_INSERT [dbo].[SyndicatedContentPostType] OFF

-- 4: Update any modified values with the values from the table variable
UPDATE LiveTable SET
LiveTable.[SyndicatedContentPostTypeName] = tmp.[SyndicatedContentPostTypeName],
LiveTable.[CreateDateUtc] = tmp.[CreateDateUtc],
LiveTable.[ModifiedDateUtc] = tmp.[ModifiedDateUtc],
LiveTable.[Deleted] = tmp.[Deleted]
FROM [dbo].[SyndicatedContentPostType] LiveTable 
INNER JOIN @tblTempTable tmp ON LiveTable.[SyndicatedContentPostTypeId] = tmp.[SyndicatedContentPostTypeId]

-- 5: Delete any missing records from the target
IF @DeleteMissingRecords = 1
BEGIN
	DELETE FROM [dbo].[SyndicatedContentPostType] FROM [dbo].[SyndicatedContentPostType] LiveTable
	LEFT JOIN @tblTempTable tmp ON LiveTable.[SyndicatedContentPostTypeId] = tmp.[SyndicatedContentPostTypeId]
	WHERE tmp.[SyndicatedContentPostTypeId] IS NULL
END

PRINT 'Finished updating static data table [dbo].[SyndicatedContentPostType]'

-- Note: If you are not using the new GDR version of DBPro
-- then remove this go command.
GO




