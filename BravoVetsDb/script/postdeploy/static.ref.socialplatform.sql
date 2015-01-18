/***************************************
***   Static data management script  ***
***************************************/

-- This script will manage the static data from
-- your Team Database project for [dbo].[SocialPlatform].

PRINT 'Updating static data table [dbo].[SocialPlatform]'

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
[SocialPlatformId] int,
[SocialPlatformName] nvarchar(128),
[Url] nvarchar(256),
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
INSERT INTO @tblTempTable ([SocialPlatformId], [SocialPlatformName], [Url], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('1', 'Twitter', 'http://www.twitter.com', '2014-01-27 16:44:35.933', '2014-01-27 16:44:35.933', 'False')
INSERT INTO @tblTempTable ([SocialPlatformId], [SocialPlatformName], [Url], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('2', 'Facebook', 'http://www.facebook.com', '2014-01-27 16:44:35.960', '2014-01-27 16:44:35.960', 'False')


-- 3: Insert any new items into the table from the table variable
SET IDENTITY_INSERT [dbo].[SocialPlatform] ON
INSERT INTO [dbo].[SocialPlatform] ([SocialPlatformId], [SocialPlatformName], [Url], [CreateDateUtc], [ModifiedDateUtc], [Deleted])
SELECT tmp.[SocialPlatformId], tmp.[SocialPlatformName], tmp.[Url], tmp.[CreateDateUtc], tmp.[ModifiedDateUtc], tmp.[Deleted]
FROM @tblTempTable tmp
LEFT JOIN [dbo].[SocialPlatform] tbl ON tbl.[SocialPlatformId] = tmp.[SocialPlatformId]
WHERE tbl.[SocialPlatformId] IS NULL
SET IDENTITY_INSERT [dbo].[SocialPlatform] OFF

-- 4: Update any modified values with the values from the table variable
UPDATE LiveTable SET
LiveTable.[SocialPlatformName] = tmp.[SocialPlatformName],
LiveTable.[Url] = tmp.[Url],
LiveTable.[CreateDateUtc] = tmp.[CreateDateUtc],
LiveTable.[ModifiedDateUtc] = tmp.[ModifiedDateUtc],
LiveTable.[Deleted] = tmp.[Deleted]
FROM [dbo].[SocialPlatform] LiveTable 
INNER JOIN @tblTempTable tmp ON LiveTable.[SocialPlatformId] = tmp.[SocialPlatformId]

-- 5: Delete any missing records from the target
IF @DeleteMissingRecords = 1
BEGIN
	DELETE FROM [dbo].[SocialPlatform] FROM [dbo].[SocialPlatform] LiveTable
	LEFT JOIN @tblTempTable tmp ON LiveTable.[SocialPlatformId] = tmp.[SocialPlatformId]
	WHERE tmp.[SocialPlatformId] IS NULL
END

PRINT 'Finished updating static data table [dbo].[SocialPlatform]'

-- Note: If you are not using the new GDR version of DBPro
-- then remove this go command.
GO