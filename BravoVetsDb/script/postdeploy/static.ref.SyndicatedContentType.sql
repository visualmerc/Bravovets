/***************************************
***   Static data management script  ***
***************************************/

-- This script will manage the static data from
-- your Team Database project for [dbo].[SyndicatedContentType].

PRINT 'Updating static data table [dbo].[SyndicatedContentType]'

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
[SyndicatedContentTypeId] int,
[SyndicatedContentTypeName] nvarchar(128),
[CreateDateUtc] datetime
)

-- 2: Populate the table variable with data
-- This is where you manage your data in source control. You
-- can add and modify entries, but because of potential foreign
-- key contraint violations this script will not delete any
-- removed entries. If you remove an entry then it will no longer
-- be added to new databases based on your schema, but the entry
-- will not be deleted from databases in which the value already exists.
INSERT INTO @tblTempTable ([SyndicatedContentTypeId], [SyndicatedContentTypeName], [CreateDateUtc]) VALUES ('1', 'Trending Topics', '2014-03-01 02:00:51.090')
INSERT INTO @tblTempTable ([SyndicatedContentTypeId], [SyndicatedContentTypeName], [CreateDateUtc]) VALUES ('2', 'Social Tips', '2014-03-01 02:00:51.090')
INSERT INTO @tblTempTable ([SyndicatedContentTypeId], [SyndicatedContentTypeName], [CreateDateUtc]) VALUES ('3', 'Bravecto Resources', '2014-03-01 02:00:51.090')


-- 3: Insert any new items into the table from the table variable
SET IDENTITY_INSERT [dbo].[SyndicatedContentType] ON
INSERT INTO [dbo].[SyndicatedContentType] ([SyndicatedContentTypeId], [SyndicatedContentTypeName], [CreateDateUtc])
SELECT tmp.[SyndicatedContentTypeId], tmp.[SyndicatedContentTypeName], tmp.[CreateDateUtc]
FROM @tblTempTable tmp
LEFT JOIN [dbo].[SyndicatedContentType] tbl ON tbl.[SyndicatedContentTypeId] = tmp.[SyndicatedContentTypeId]
WHERE tbl.[SyndicatedContentTypeId] IS NULL
SET IDENTITY_INSERT [dbo].[SyndicatedContentType] OFF

-- 4: Update any modified values with the values from the table variable
UPDATE LiveTable SET
LiveTable.[SyndicatedContentTypeName] = tmp.[SyndicatedContentTypeName],
LiveTable.[CreateDateUtc] = tmp.[CreateDateUtc]
FROM [dbo].[SyndicatedContentType] LiveTable 
INNER JOIN @tblTempTable tmp ON LiveTable.[SyndicatedContentTypeId] = tmp.[SyndicatedContentTypeId]

-- 5: Delete any missing records from the target
IF @DeleteMissingRecords = 1
BEGIN
	DELETE FROM [dbo].[SyndicatedContentType] FROM [dbo].[SyndicatedContentType] LiveTable
	LEFT JOIN @tblTempTable tmp ON LiveTable.[SyndicatedContentTypeId] = tmp.[SyndicatedContentTypeId]
	WHERE tmp.[SyndicatedContentTypeId] IS NULL
END

PRINT 'Finished updating static data table [dbo].[SyndicatedContentType]'

-- Note: If you are not using the new GDR version of DBPro
-- then remove this go command.
GO