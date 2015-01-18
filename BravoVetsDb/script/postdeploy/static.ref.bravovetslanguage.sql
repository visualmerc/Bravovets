/***************************************
***   Static data management script  ***
***************************************/

-- This script will manage the static data from
-- your Team Database project for [dbo].[BravoVetsLanguage].

PRINT 'Updating static data table [dbo].[BravoVetsLanguage]'

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
[BravoVetsLanguageId] int,
[LanguageName] nvarchar(128),
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
INSERT INTO @tblTempTable ([BravoVetsLanguageId], [LanguageName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('1', 'English', '2014-01-15 20:28:14.240', '2014-01-15 20:28:14.240', 'False')
INSERT INTO @tblTempTable ([BravoVetsLanguageId], [LanguageName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('2', 'French', '2014-01-15 20:28:14.260', '2014-01-15 20:28:14.260', 'False')
INSERT INTO @tblTempTable ([BravoVetsLanguageId], [LanguageName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('3', 'Italian', '2014-01-15 20:28:14.263', '2014-01-15 20:28:14.263', 'False')
INSERT INTO @tblTempTable ([BravoVetsLanguageId], [LanguageName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('4', 'German', '2014-01-15 20:28:14.267', '2014-01-15 20:28:14.267', 'False')
INSERT INTO @tblTempTable ([BravoVetsLanguageId], [LanguageName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('5', 'Spanish', '2014-01-15 20:28:14.267', '2014-01-15 20:28:14.267', 'False')
INSERT INTO @tblTempTable ([BravoVetsLanguageId], [LanguageName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES ('6', 'Dutch', '2014-01-15 20:28:14.270', '2014-01-15 20:28:14.270', 'False')


-- 3: Insert any new items into the table from the table variable
SET IDENTITY_INSERT [dbo].[BravoVetsLanguage] ON
INSERT INTO [dbo].[BravoVetsLanguage] ([BravoVetsLanguageId], [LanguageName], [CreateDateUtc], [ModifiedDateUtc], [Deleted])
SELECT tmp.[BravoVetsLanguageId], tmp.[LanguageName], tmp.[CreateDateUtc], tmp.[ModifiedDateUtc], tmp.[Deleted]
FROM @tblTempTable tmp
LEFT JOIN [dbo].[BravoVetsLanguage] tbl ON tbl.[BravoVetsLanguageId] = tmp.[BravoVetsLanguageId]
WHERE tbl.[BravoVetsLanguageId] IS NULL
SET IDENTITY_INSERT [dbo].[BravoVetsLanguage] OFF

-- 4: Update any modified values with the values from the table variable
UPDATE LiveTable SET
LiveTable.[LanguageName] = tmp.[LanguageName],
LiveTable.[CreateDateUtc] = tmp.[CreateDateUtc],
LiveTable.[ModifiedDateUtc] = tmp.[ModifiedDateUtc],
LiveTable.[Deleted] = tmp.[Deleted]
FROM [dbo].[BravoVetsLanguage] LiveTable 
INNER JOIN @tblTempTable tmp ON LiveTable.[BravoVetsLanguageId] = tmp.[BravoVetsLanguageId]

-- 5: Delete any missing records from the target
IF @DeleteMissingRecords = 1
BEGIN
	DELETE FROM [dbo].[BravoVetsLanguage] FROM [dbo].[BravoVetsLanguage] LiveTable
	LEFT JOIN @tblTempTable tmp ON LiveTable.[BravoVetsLanguageId] = tmp.[BravoVetsLanguageId]
	WHERE tmp.[BravoVetsLanguageId] IS NULL
END

PRINT 'Finished updating static data table [dbo].[BravoVetsLanguage]'

-- Note: If you are not using the new GDR version of DBPro
-- then remove this go command.
GO