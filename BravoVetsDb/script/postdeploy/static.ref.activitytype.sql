/***************************************
***   Static data management script  ***
***************************************/

-- This script will manage the static data from
-- your Team Database project for [dbo].[ActivityType].

PRINT 'Updating static data table [dbo].[ActivityType]'

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
[ActivityTypeId] int,
[ActivityTypeName] nvarchar(128),
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
INSERT INTO @tblTempTable ([ActivityTypeId], [ActivityTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('1', 'Hide', '2014-01-27 16:42:06.957', '2014-01-27 16:42:06.957', 'False')
INSERT INTO @tblTempTable ([ActivityTypeId], [ActivityTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('2', 'Show', '2014-01-27 16:42:06.970', '2014-01-27 16:42:06.970', 'False')
INSERT INTO @tblTempTable ([ActivityTypeId], [ActivityTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('3', 'FacebookShare', '2014-01-27 16:42:06.983', '2014-01-27 16:42:06.983', 'False')
INSERT INTO @tblTempTable ([ActivityTypeId], [ActivityTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('4', 'TwitterShare', '2014-01-27 16:42:06.983', '2014-01-27 16:42:06.983', 'False')
INSERT INTO @tblTempTable ([ActivityTypeId], [ActivityTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('5', 'ScheduleFacebook', '2014-01-27 16:42:06.990', '2014-01-27 16:42:06.990', 'False')
INSERT INTO @tblTempTable ([ActivityTypeId], [ActivityTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('6', 'ScheduleTwitter', '2014-01-27 16:42:06.990', '2014-01-27 16:42:06.990', 'False')
INSERT INTO @tblTempTable ([ActivityTypeId], [ActivityTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('7', 'Favorite', '2014-01-27 16:42:07.020', '2014-01-27 16:42:07.020', 'False')
INSERT INTO @tblTempTable ([ActivityTypeId], [ActivityTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('8', 'Expand', '2014-01-27 16:42:07.023', '2014-01-27 16:42:07.023', 'False')


-- 3: Insert any new items into the table from the table variable
SET IDENTITY_INSERT [dbo].[ActivityType] ON
INSERT INTO [dbo].[ActivityType] ([ActivityTypeId], [ActivityTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted])
SELECT tmp.[ActivityTypeId], tmp.[ActivityTypeName], tmp.[CreateDateUtc], tmp.[ModifiedDateUtc], tmp.[Deleted]
FROM @tblTempTable tmp
LEFT JOIN [dbo].[ActivityType] tbl ON tbl.[ActivityTypeId] = tmp.[ActivityTypeId]
WHERE tbl.[ActivityTypeId] IS NULL
SET IDENTITY_INSERT [dbo].[ActivityType] OFF

-- 4: Update any modified values with the values from the table variable
UPDATE LiveTable SET
LiveTable.[ActivityTypeName] = tmp.[ActivityTypeName],
LiveTable.[CreateDateUtc] = tmp.[CreateDateUtc],
LiveTable.[ModifiedDateUtc] = tmp.[ModifiedDateUtc],
LiveTable.[Deleted] = tmp.[Deleted]
FROM [dbo].[ActivityType] LiveTable 
INNER JOIN @tblTempTable tmp ON LiveTable.[ActivityTypeId] = tmp.[ActivityTypeId]

-- 5: Delete any missing records from the target
IF @DeleteMissingRecords = 1
BEGIN
	DELETE FROM [dbo].[ActivityType] FROM [dbo].[ActivityType] LiveTable
	LEFT JOIN @tblTempTable tmp ON LiveTable.[ActivityTypeId] = tmp.[ActivityTypeId]
	WHERE tmp.[ActivityTypeId] IS NULL
END

PRINT 'Finished updating static data table [dbo].[ActivityType]'

-- Note: If you are not using the new GDR version of DBPro
-- then remove this go command.
GO