/***************************************
***   Static data management script  ***
***************************************/

-- This script will manage the static data from
-- your Team Database project for [dbo].[BravoVetsStatus].

PRINT 'Updating static data table [dbo].[BravoVetsStatus]'

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
[BravoVetsStatusId] int,
[StatusName] nvarchar(128),
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
INSERT INTO @tblTempTable ([BravoVetsStatusId], [StatusName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('1', 'Submitted', '2014-01-27 16:48:32.757', '2014-01-27 16:48:32.757', 'False')
INSERT INTO @tblTempTable ([BravoVetsStatusId], [StatusName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('2', 'In Process', '2014-01-27 16:48:32.647', '2014-01-27 16:48:32.647', 'False')
INSERT INTO @tblTempTable ([BravoVetsStatusId], [StatusName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('3', 'Approved', '2014-01-27 16:48:32.760', '2014-01-27 16:48:32.760', 'False')
INSERT INTO @tblTempTable ([BravoVetsStatusId], [StatusName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('4', 'Active', '2014-01-27 16:48:32.667', '2014-01-27 16:48:32.667', 'False')
INSERT INTO @tblTempTable ([BravoVetsStatusId], [StatusName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('5', 'Inactive', '2014-01-27 16:48:32.700', '2014-01-27 16:48:32.700', 'False')
INSERT INTO @tblTempTable ([BravoVetsStatusId], [StatusName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('6', 'Suspended', '2014-01-27 16:48:32.713', '2014-01-27 16:48:32.713', 'False')


-- 3: Insert any new items into the table from the table variable
SET IDENTITY_INSERT [dbo].[BravoVetsStatus] ON
INSERT INTO [dbo].[BravoVetsStatus] ([BravoVetsStatusId], [StatusName], [CreateDateUtc], [ModifiedDateUtc], [Deleted])
SELECT tmp.[BravoVetsStatusId], tmp.[StatusName], tmp.[CreateDateUtc], tmp.[ModifiedDateUtc], tmp.[Deleted]
FROM @tblTempTable tmp
LEFT JOIN [dbo].[BravoVetsStatus] tbl ON tbl.[BravoVetsStatusId] = tmp.[BravoVetsStatusId]
WHERE tbl.[BravoVetsStatusId] IS NULL
SET IDENTITY_INSERT [dbo].[BravoVetsStatus] OFF

-- 4: Update any modified values with the values from the table variable
UPDATE LiveTable SET
LiveTable.[StatusName] = tmp.[StatusName],
LiveTable.[CreateDateUtc] = tmp.[CreateDateUtc],
LiveTable.[ModifiedDateUtc] = tmp.[ModifiedDateUtc],
LiveTable.[Deleted] = tmp.[Deleted]
FROM [dbo].[BravoVetsStatus] LiveTable 
INNER JOIN @tblTempTable tmp ON LiveTable.[BravoVetsStatusId] = tmp.[BravoVetsStatusId]

-- 5: Delete any missing records from the target
IF @DeleteMissingRecords = 1
BEGIN
	DELETE FROM [dbo].[BravoVetsStatus] FROM [dbo].[BravoVetsStatus] LiveTable
	LEFT JOIN @tblTempTable tmp ON LiveTable.[BravoVetsStatusId] = tmp.[BravoVetsStatusId]
	WHERE tmp.[BravoVetsStatusId] IS NULL
END

PRINT 'Finished updating static data table [dbo].[BravoVetsStatus]'

-- Note: If you are not using the new GDR version of DBPro
-- then remove this go command.
GO