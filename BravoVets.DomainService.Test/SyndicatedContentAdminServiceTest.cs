using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Test
{
    using System.Drawing;
    using System.IO;

    using BravoVets.DomainObject;
    using BravoVets.DomainObject.Enum;
    using BravoVets.DomainObject.Infrastructure;
    using BravoVets.DomainService.Contract;
    using BravoVets.DomainService.Service;
    using BravoVets.DomainService.Test.Infrastructure;
    using BravoVets.DomainService.Test.Properties;

    using Xunit;

    public class SyndicatedContentAdminServiceTest : AbstractServiceBaseTest
    {
        private ISyndicatedContentAdminService _contentAdminService = new SyndicatedContentAdminService();

        private int _testSyndicatedContentId = 6;

        private int _deleterSyndicatedContentId = 10;

        [Fact]
        public void AdminServiceCanCreateTrendingTopic()
        {
            var trendingTopic = SyndicatedContentTypeEnum.TrendingTopics;

            var topic = this.BuildSyndicatedContent(trendingTopic);

            var newTopic = this._contentAdminService.QueueNewTrendingTopic(topic);
            Assert.True(newTopic.SyndicatedContentId > 0);
        }

        [Fact]
        public void AdminServiceCanGetTrendingTopics()
        {
            int recordCount;
            var pageToken = new PagingToken { StartRecord = 1, TotalRecords = 10 };

            var trendingTopicsList = this._contentAdminService.GetTrendingTopicsAdminList(
                false,
                BravoVetsCountryEnum.GB,
                ContentSortEnum.ContentDate,
                pageToken,
                out recordCount);

            Assert.True(recordCount > 0, "did not get a count");
        }

        [Fact]
        public void AdminServiceCanGetIndividualContent()
        {
            var goodContent = this._contentAdminService.GetSyndicatedContent(this._testSyndicatedContentId);
            Assert.True(goodContent.ContentText.Length > 0, "Did not get a syndicated content object");
        }

        [Fact]
        public void AdminServiceCanDeleteSyndicatedContent()
        {
            var topic =
                this._contentAdminService.GetMinimalEmptySyndicatedContent(
                    SyndicatedContentTypeEnum.TrendingTopics,
                    BravoVetsCountryEnum.ES,
                    "Test Service");
            topic.Title = string.Format("Test Trending Topic {0}", Guid.NewGuid().ToString().Substring(0, 8));
            topic.ContentText = string.Format(
                "Generated on {0}, by Mr. {1}",
                DateTime.UtcNow.ToLongDateString(),
                Guid.NewGuid());

            var newTopic = this._contentAdminService.QueueNewTrendingTopic(topic);
            Assert.True(newTopic.SyndicatedContentId > 0);

            var newId = newTopic.SyndicatedContentId;
            var didDelete = this._contentAdminService.DeleteSyndicatedContent(newId);
            Assert.True(didDelete);
        }

        [Fact]
        public void AdminServiceCanUpdateSyndicatedContent()
        {
            var oldContent = this._contentAdminService.GetSyndicatedContent(this._testSyndicatedContentId);
            var oldTitle = oldContent.Title;
            oldContent.Title = "Biggie Smalls";
            var newContent = this._contentAdminService.UpdateSyndicatedContent(oldContent);
            Assert.True(oldTitle != newContent.Title, "Did not update Syndicated content");
            Assert.True(newContent.Title == "Biggie Smalls", "Did not updated syndicated content title");
        }

        [Fact]
        public void AdminServiceCanGetFeaturedContent()
        {
            var contentList = this._contentAdminService.GetFeaturedContent(
                SyndicatedContentTypeEnum.SocialTips,
                BravoVetsCountryEnum.GB);
            Assert.True(contentList.Count > 0, "Did not get any Featured Content");
        }

        [Fact]
        public void ContentServiceCanSaveImages()
        {
            Image testImage = Resources.Broadway;

            var testList = new List<SyndicatedContentAttachment>();

            for (int i = 0; i < 3; i++)
            {
                var attachment = this.HydrateSyndicatedContentAttachment(testImage, this._testSyndicatedContentId);
                testList.Add(attachment);
            }

            var processedList = this._contentAdminService.SaveSyndicatedContentAttachments(testList);

            foreach (var syndicatedContentAttachment in processedList)
            {
                Assert.True(
                    syndicatedContentAttachment.SyndicatedContentAttachmentId > 0,
                    "Did not insert a syndicated content attachment.");
            }
        }

        [Fact]
        public void AdminServiceCanAssociateFeaturedContent()
        {
            var newAttachment = this._contentAdminService.AssociateFeaturedContent(3, 15);
            Assert.True(
                newAttachment.SyndicatedContentAttachmentId > 0,
                "Did not create an attachment from featured content.");
        }

        [Fact]
        public void AdminServiceCanDeleteSyndicatedContentAttachment()
        {
            Image testImage = Resources.Broadway;

            var testList = new List<SyndicatedContentAttachment>();

            for (int i = 0; i < 1; i++)
            {
                var attachment = this.HydrateSyndicatedContentAttachment(testImage, this._testSyndicatedContentId);
                testList.Add(attachment);
            }

            var processedList = this._contentAdminService.SaveSyndicatedContentAttachments(testList);

            foreach (var syndicatedContentAttachment in processedList)
            {
                var id = syndicatedContentAttachment.SyndicatedContentAttachmentId;
                Console.WriteLine("Deleting syndicated content ID {0}", id);
                var deleted = this._contentAdminService.DeleteSyndicatedContentAttachment(id);
                Assert.True(deleted, "did not delete an attachment");
            }
        }

        [Fact]
        public void AdminServiceCanCreateLink()
        {
            var contentLink = HydrateSyndicatedContentLink(null, this._testSyndicatedContentId);

            var newLink = this._contentAdminService.SaveContentLink(contentLink);
            Assert.True(newLink.SyndicatedContentLinkId > 0, "Did not create a link");
        }

        [Fact]
        public void AdminServiceCanCreateTestLink()
        {
            var contentLink = this.GenerateTestLink2();

            var newLink = this._contentAdminService.SaveContentLink(contentLink);
            Assert.True(newLink.SyndicatedContentLinkId > 0, "Did not create a link");
        }

        [Fact]
        public void AdminServiceCanDeleteStackedLink()
        {
            var linkList = new List<SyndicatedContentLink>();
            for (int i = 0; i < 3; i++)
            {
                linkList.Add(HydrateSyndicatedContentLink(null, this._deleterSyndicatedContentId));
            }

            var newLink = this._contentAdminService.SaveContentLinks(linkList);
            foreach (var syndicatedContentLink in linkList)
            {
                Assert.True(syndicatedContentLink.SyndicatedContentLinkId > 0, "Did not create a link");
            }

            var didDelete = this._contentAdminService.DeleteSyndicatedContent(this._deleterSyndicatedContentId);
            Assert.True(didDelete, "Did not delete the syndicated content");
        }

        [Fact]
        public void AdminServiceCanCreateAttachmentLink()
        {
            Image testImage = Resources.Broadway;
            var attachment = this.HydrateSyndicatedContentAttachment(testImage, this._testSyndicatedContentId);
            var finalAttachment = this._contentAdminService.SaveSyndicatedContentAttachment(attachment);
            var attachmentId = finalAttachment.SyndicatedContentAttachmentId;
            Assert.True(attachmentId > 0, "Did not insert new attachment");

            var attachLink = this.HydrateSyndicatedContentLink(attachmentId, this._testSyndicatedContentId);
            var finalLink = this._contentAdminService.SaveContentLink(attachLink);
            Assert.True(finalLink.SyndicatedContentLinkId > 0, "did not insert a link");
            Assert.True(finalLink.SyndicatedContentAttachmentId == attachmentId, "did not associate attachment");

        }

        [Fact]
        public void AdminServiceCanCascadeDeleteLink()
        {
            Image testImage = Resources.Broadway;
            var attachment = this.HydrateSyndicatedContentAttachment(testImage, this._testSyndicatedContentId);
            var finalAttachment = this._contentAdminService.SaveSyndicatedContentAttachment(attachment);
            var attachmentId = finalAttachment.SyndicatedContentAttachmentId;
            Assert.True(attachmentId > 0, "Did not insert new attachment");

            var attachLink = this.HydrateSyndicatedContentLink(attachmentId, this._testSyndicatedContentId);
            var finalLink = this._contentAdminService.SaveContentLink(attachLink);
            Assert.True(finalLink.SyndicatedContentLinkId > 0, "did not insert a link");
            Assert.True(finalLink.SyndicatedContentAttachmentId == attachmentId, "did not associate attachment");

            var didDelete = this._contentAdminService.DeleteSyndicatedContentLink(finalLink.SyndicatedContentLinkId);
            Assert.True(didDelete, "did not delete link");

            var ghost = this._contentAdminService.GetSyndicatedContentAttachment(attachmentId);
            //Assert.Null(ghost);
        }

        [Fact]
        public void AdminServiceCanCascadeDeleteAttachment()
        {
            Image testImage = Resources.Broadway;
            var attachment = this.HydrateSyndicatedContentAttachment(testImage, this._testSyndicatedContentId);
            var finalAttachment = this._contentAdminService.SaveSyndicatedContentAttachment(attachment);
            var attachmentId = finalAttachment.SyndicatedContentAttachmentId;
            Assert.True(attachmentId > 0, "Did not insert new attachment");

            var attachLink = this.HydrateSyndicatedContentLink(attachmentId, this._testSyndicatedContentId);
            var finalLink = this._contentAdminService.SaveContentLink(attachLink);
            var linkId = finalLink.SyndicatedContentLinkId;
            Console.Write("LinkId: {0}", linkId);
            Assert.True(linkId > 0, "did not insert a link");
            Assert.True(finalLink.SyndicatedContentAttachmentId == attachmentId, "did not associate attachment");

            var didDelete = this._contentAdminService.DeleteSyndicatedContentAttachment(attachmentId);
            Assert.True(didDelete, "did not delete attachment");
        }

        [Fact]
        public void AdminServiceCanGetContentWithAttachments()
        {
            var goodContent = this._contentAdminService.GetSyndicatedContent(this._testSyndicatedContentId);
            Assert.True(goodContent.ContentText.Length > 0, "Did not get a syndicated content object");
            Assert.True(goodContent.SyndicatedContentAttachments.Count > 0, "Did not get attachments");
        }

        [Fact]
        public void AdminServiceCanDeleteAssociatedQueueItems()
        {
            var resource = this.BuildSyndicatedContent(SyndicatedContentTypeEnum.BravectoResources);

            var newResource = this._contentAdminService.QueueNewTrendingTopic(resource);
            var contentId = newResource.SyndicatedContentId;
            Console.WriteLine("SyndicatedContent created. ID: {0}", contentId);
            Assert.True(contentId > 0);

            var queueService = new PublishQueueDomainService();
            var rawQueue = this.createTestQueueContent(contentId);
            var newQueue = queueService.QueueContentForPublish(rawQueue);
            var queueId = newQueue.QueueContentId;
            Console.WriteLine("QueueItem created. ID: {0}", queueId);

            var didDelete = this._contentAdminService.DeleteSyndicatedContent(contentId);
            Assert.True(didDelete, "did not delete the content");
        }

        #region private methods

        private SyndicatedContentAttachment HydrateSyndicatedContentAttachment(Image testImage, int syndicatedContentId)
        {
            var attachment = new SyndicatedContentAttachment();
            attachment.AttachmentExtension = "jpg";
            attachment.AttachmentFileName = string.Format("Broad{0}.jpg", Guid.NewGuid().ToString("N").Substring(1, 4));
            attachment.CreateDateUtc = DateTime.UtcNow;
            attachment.ModifiedDateUtc = DateTime.UtcNow;
            attachment.SyndicatedContentId = syndicatedContentId;
            attachment.AttachmentFile = this.ImageToByte(testImage, "jpg");
            return attachment;
        }

        private SyndicatedContentLink HydrateSyndicatedContentLink(
            int? syndicatedContentAttachmentId,
            int syndicatedContentId)
        {
            var contentLink = new SyndicatedContentLink();
            contentLink.CreateDateUtc = DateTime.UtcNow;
            contentLink.LinkTitle = string.Format("Link Title {0}", Guid.NewGuid());
            contentLink.LinkUrl = string.Format("http://www.{0}.com", Guid.NewGuid());
            contentLink.ModifiedDateUtc = DateTime.UtcNow;
            contentLink.SyndicatedContentId = syndicatedContentId;
            contentLink.SyndicatedContentAttachmentId = syndicatedContentAttachmentId;
            return contentLink;
        }

        private SyndicatedContentLink GenerateTestLink()
        {
            var contentLink = new SyndicatedContentLink();
            contentLink.CreateDateUtc = DateTime.UtcNow;
            contentLink.LinkTitle = string.Format("You are the man now, dog");
            contentLink.LinkUrl = "http://ytmnd.com/";
            contentLink.ModifiedDateUtc = DateTime.UtcNow;
            contentLink.SyndicatedContentId = 57;
            contentLink.SyndicatedContentAttachmentId = null;
            return contentLink;
        }

        private SyndicatedContentLink GenerateTestLink2()
        {
            var contentLink = new SyndicatedContentLink();
            contentLink.CreateDateUtc = DateTime.UtcNow;
            contentLink.LinkTitle = string.Format("BravoVets Documentation");
            contentLink.LinkUrl = "none";
            contentLink.ModifiedDateUtc = DateTime.UtcNow;
            contentLink.SyndicatedContentId = 57;
            contentLink.SyndicatedContentAttachmentId = 390;
            return contentLink;
        }

        //http://ytmnd.com/

        private SyndicatedContent BuildSyndicatedContent(SyndicatedContentTypeEnum contentType)
        {
            var topic = this._contentAdminService.GetMinimalEmptySyndicatedContent(
                contentType,
                BravoVetsCountryEnum.GB,
                "Test Service");
            topic.Title = string.Format("Test syndicated content {0}", Guid.NewGuid().ToString().Substring(0, 8));
            topic.ContentText = string.Format(
                "Generated on {0}, by Mr. {1}",
                DateTime.UtcNow.ToLongDateString(),
                Guid.NewGuid());
            //topic.ContentText = string.Empty;
            return topic;
        }

        private QueueContent createTestQueueContent(int syndicatedContentId)
        {
            var random = new Random();

            var tc = new QueueContent();
            tc.AccessCode = Guid.NewGuid().ToString("N");
            tc.BravoVetsCountryId = (int)BravoVetsCountryEnum.US;
            tc.BravoVetsStatusId = (int)BravoVetsStatusEnum.Active;
            tc.BravoVetsUserId = random.Next(1, 5);
            tc.ContentText = string.Format(
                "{0} test. A bunch of test text {1} to test out the content text. {2}",
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid());
            tc.CreateDateUtc = DateTime.UtcNow;
            tc.Deleted = false;
            tc.IsPublished = false;
            tc.LinkUrl = string.Format("http://www.{0}.com", Guid.NewGuid().ToString("N"));
            tc.ModifiedDateUtc = DateTime.UtcNow;
            tc.PlatformPublishId = (int)SocialPlatformEnum.Facebook;
            tc.PublishDateUtc = DateTime.UtcNow.AddDays(random.Next(5, 35));
            tc.SyndicatedContentPostTypeId = (int)SyndicatedContentPostTypeEnum.LinkPostImage;
            tc.SyndicatedContentId = syndicatedContentId;

            return tc;
        }

        #endregion


        #region featured content import
/*
        [Fact]
        public void ContentServiceCanSaveImagesTwo()
        {
            Image testImage = Image.FromFile(@"C:\Users\Rob\Pictures\DonerFeaturedImages\SocialTips\bullet_cat.jpg");
            Image testImageTmb =
                Image.FromFile(@"C:\Users\Rob\Pictures\DonerFeaturedImages\SocialTips\bullet_cat_tmb.jpg");

            var fc = new FeaturedContent();
            fc.BravoVetsCountryId = 2;
            fc.ContentExtension = "png";
            fc.ContentFile = ImageToByte(testImage, "png");
            fc.ContentThumbnail = ImageToByte(testImageTmb, "png");
            fc.ContentFileName = "bullet_cat";
            fc.CreateDateUtc = DateTime.UtcNow;
            fc.ModifiedDateUtc = DateTime.UtcNow;
            fc.SyndicatedContentTypeId = (int)SyndicatedContentTypeEnum.SocialTips;

            this._contentAdminService.AddUpdateFeaturedContent(fc);

            Assert.True(true);
        }

        [Fact]
        public void ContentServiceCanBulkImportSocialTips()
        {
            var pd = GetPictureList("SocialTips");
            const string Extenz = "jpg";

            foreach (string fileName in pd)
            {
                var imagePath = string.Format(
                    @"C:\Users\Rob\Downloads\bravoVets-featuredImages-rd2\Social Tips\{0}.{1}",
                    fileName,
                    Extenz);

                var thumbPath = string.Format(
                    @"C:\Users\Rob\Downloads\bravoVets-featuredImages-rd2\Social Tips\{0}_tmb.{1}",
                    fileName,
                    Extenz);

                var fc = this.GetFeaturedContent(
                    imagePath,
                    thumbPath,
                    Extenz,
                    fileName,
                    SyndicatedContentTypeEnum.SocialTips);

                this._contentAdminService.AddUpdateFeaturedContent(fc);

            }

            Assert.True(true);
        }

        [Fact]
        public void ContentServiceCanBulkImportTrendingTopics()
        {
            var pd = GetPictureList("TrendingTopics");
            const string Extenz = "jpg";

            foreach (string fileName in pd)
            {
                var imagePath = string.Format(
                    @"C:\Users\Rob\Downloads\bravoVets-featuredImages-rd2\Trending Topics\{0}.{1}",
                    fileName,
                    Extenz);

                var thumbPath = string.Format(
                    @"C:\Users\Rob\Downloads\bravoVets-featuredImages-rd2\Trending Topics\{0}_tmb.{1}",
                    fileName,
                    Extenz);

                var fc = this.GetFeaturedContent(
                    imagePath,
                    thumbPath,
                    Extenz,
                    fileName,
                    SyndicatedContentTypeEnum.TrendingTopics);

                this._contentAdminService.AddUpdateFeaturedContent(fc);

            }

            Assert.True(true);
        }

        [Fact]
        public void ContentServiceCanBulkImportTrendingTopicsPng()
        {
            var pd = GetPictureList("TrendingTopicsPng");
            const string Extenz = "png";

            foreach (string fileName in pd)
            {
                var imagePath = string.Format(
                    @"C:\Users\Rob\Pictures\DonerFeaturedImages\TrendingTopics\{0}.{1}",
                    fileName,
                    Extenz);

                var thumbPath = string.Format(
                    @"C:\Users\Rob\Pictures\DonerFeaturedImages\TrendingTopics\{0}_tmb.{1}",
                    fileName,
                    Extenz);

                var fc = this.GetFeaturedContent(
                    imagePath,
                    thumbPath,
                    Extenz,
                    fileName,
                    SyndicatedContentTypeEnum.TrendingTopics);

                this._contentAdminService.AddUpdateFeaturedContent(fc);

            }

            Assert.True(true);
        }

        [Fact]
        public void ContentServiceCanBulkImportBravectoResources()
        {
            var pd = GetPictureList("BravectoResources");
            const string Extenz = "jpg";

            foreach (string fileName in pd)
            {
                var imagePath = string.Format(
                    @"C:\Users\Rob\Downloads\bravoVets-featuredImages-rd2\Bravecto Resources\{0}.{1}",
                    fileName,
                    Extenz);

                var thumbPath = string.Format(
                    @"C:\Users\Rob\Downloads\bravoVets-featuredImages-rd2\Bravecto Resources\{0}_tmb.{1}",
                    fileName,
                    Extenz);

                Image testImage = Image.FromFile(imagePath);
                Image testImageTmb = Image.FromFile(thumbPath);

                var fc = this.GetFeaturedContent(
                    imagePath,
                    thumbPath,
                    Extenz,
                    fileName,
                    SyndicatedContentTypeEnum.BravectoResources);

                this._contentAdminService.AddUpdateFeaturedContent(fc);

            }

            Assert.True(true);
        } */

        private FeaturedContent GetFeaturedContent(
            string imagePath,
            string thumbPath,
            string extenz,
            string fileName,
            SyndicatedContentTypeEnum contentType)
        {
            Image testImage = Image.FromFile(imagePath);
            Image testImageTmb = Image.FromFile(thumbPath);

            var fc = new FeaturedContent();
            fc.BravoVetsCountryId = 2;
            fc.ContentExtension = extenz;
            fc.ContentFile = this.ImageToByte(testImage, extenz);
            fc.ContentThumbnail = this.ImageToByte(testImageTmb, extenz);
            fc.ContentFileName = fileName;
            fc.CreateDateUtc = DateTime.UtcNow;
            fc.ModifiedDateUtc = DateTime.UtcNow;
            fc.SyndicatedContentTypeId = (int)contentType;
            return fc;
        }

        private static List<string> GetPictureList(string whichType)
        {
            var pd = new List<string>();

            switch (whichType)
            {
                case "SocialTips":
                    pd.Add("Analyzing Social");
                    pd.Add("Facebook");
                    pd.Add("Twitter");
                    break;
                case "TrendingTopics":
                    pd.Add("Flea & Tick");
                    pd.Add("Fun");
                    pd.Add("Pet Health");
                    break;
                case "BravectoResources":
                    pd.Add("Bravecto & Your Business");
                    pd.Add("Products");
                    pd.Add("Vets Practice");
                    pd.Add("Webinars");
                    break;
                case "TrendingTopicsPng":
                    pd.Add("ElephantInRoom");
                    break;

            }


            return pd;
        }

        public byte[] ImageToByte(Image img, string whatType)
        {
            byte[] byteArray = new byte[0];
            using (var stream = new MemoryStream())
            {
                switch (whatType)
                {
                    case "png":
                        img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case "gif":
                        img.Save(stream, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    default:
                        img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                }
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        #endregion

    }
}


