/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
--====== Ref data section =====
:r .\static.ref.activitytype.sql
:r .\static.ref.bravovetslanguage.sql
:r .\static.ref.bravovetsstatus.sql
:r .\static.ref.socialplatform.sql
:r .\static.ref.BravoVetsCountry.sql
:r .\static.ref.channeltype.sql
:r .\static.ref.SyndicatedContentPostType.sql
:r .\static.ref.SyndicatedContentType.sql
:r .\static.ref.FeaturedContent.sql

--====== prod data section =====
:r .\static.content.SyndicatedContent.sql
:r .\static.content.SyndicatedContentAttachment.sql

-- =============================
-- Test data section
:r .\static.tst.MerckLfwUser.sql
:r .\CreateTestUsers.sql
--:r .\static.tst.CreateBigTestData.sql
--:r .\static.tst.CreateBigTestData_02.sql
