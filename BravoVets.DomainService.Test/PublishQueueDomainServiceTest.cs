using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Enum;
using BravoVets.DomainObject.Infrastructure;
using BravoVets.DomainService.Contract;
using BravoVets.DomainService.Service;
using BravoVets.DomainService.Test.Infrastructure;
using Xunit;

namespace BravoVets.DomainService.Test
{
    public class PublishQueueDomainServiceTest : AbstractServiceBaseTest
    {
        private IPublishQueueDomainService _queueService;

        private int _testUpdateQueueId = 1;

        private int _testUserWithCalendar = 3;

        private int _testAssociationAttachmentQueueId = 10;

        private int _testSyndicatedContentAttachmentSource = 6;

        private int _testNewAttachmentQueueid = 9;

        public PublishQueueDomainServiceTest()
        {
            this._queueService = new PublishQueueDomainService();
        }

        [Fact]
        public void PublishQueueCanCreateContent()
        {
            var testContent = this.createTestQueueContent();
            var newQueue = this._queueService.QueueContentForPublish(testContent);
            Assert.True(newQueue.QueueContentId > 0, "Did not insert queued content");
        }

        [Fact]
        public void PublishQueueCanUpdateContent()
        {
            var testContent = this.createTestQueueContent();
            var publishDate = testContent.PublishDateUtc;

            testContent.QueueContentId = this._testUpdateQueueId;

            var updatedQueue = this._queueService.QueueContentForPublish(testContent);
            Assert.True(updatedQueue.PublishDateUtc == publishDate, "Did not update item");
        }

        [Fact]
        public void PublishQueueCanCloneContent()
        {
            var newQueue = this._queueService.CloneQueuedContent‏(4);
            Assert.True(newQueue.QueueContentId > 0, "Did not successfully clone content");
        }

        [Fact]
        public void PublishQueueCanDequeueContent()
        {
            var testContent = this.createTestQueueContent();
            var newQueue = this._queueService.QueueContentForPublish(testContent);
            var queueId = newQueue.QueueContentId;
            Assert.True(queueId > 0, "Did not insert queued content");

            var didDequeue = this._queueService.DequeueContent(queueId);
            Assert.True(didDequeue, "Did not dequeue content");
        }

        [Fact]
        public void PublishQueueCanGetContent()
        {
            var testContent = this.createTestQueueContent();
            var newQueue = this._queueService.QueueContentForPublish(testContent);
            Assert.True(newQueue.QueueContentId > 0, "Did not insert queued content");
            var queueId = newQueue.QueueContentId;
            var publishDate = newQueue.PublishDateUtc;

            var retrievedQueue = this._queueService.GetQueuedMessage(queueId);
            Assert.True(retrievedQueue.QueueContentId == queueId, "did not find the right queued message");
            Assert.True(retrievedQueue.PublishDateUtc == publishDate, "did not retrieve publish date for queued message");
        }

        [Fact]
        public void PublishQueueCanGetContentOverDateRange()
        {
            var daysOfcontent = this._queueService.GetQueuedContentForUserByDay(
                this._testUserWithCalendar,
                new DateTime(2014, 4, 1),
                new DateTime(2014, 6, 1));

            Assert.True(daysOfcontent.Count > 0, "Did not get calendar content.");
            var hasElements = false;
            foreach (var dayGroup in daysOfcontent.Where(dayGroup => dayGroup.Value.Count > 0))
            {
                hasElements = true;
            }
            Assert.True(hasElements, "Did not get any elements");
        }

        [Fact]
        public void PublishQueueCanGetQueuedMessages()
        {
            Guid deliverySessionId;
            var queues = this._queueService.GetAllQueuedContentForPublish(new DateTime(2014, 4, 15), out deliverySessionId);
            foreach (var queueContent in queues)
            {
                Assert.False(queueContent.IsPublished);
            }
            Assert.True(queues.Count > 0, "Did not get any queued items.");
        }

        [Fact]
        public void PublishQueueCanGetQueuedMessagesForCountry()
        {
            Guid deliverySessionId;
            var queues = this._queueService.GetAllQueuedContentForPublish(new DateTime(2014, 4, 15), 2, out deliverySessionId);
            foreach (var queueContent in queues)
            {
                Assert.False(queueContent.IsPublished, "This has already been published");
                Assert.True(queueContent.BravoVetsCountryId == 2, "Wrong country");
            }
            Assert.True(queues.Count > 0, "Did not get any queued items.");
        }

        [Fact]
        public void PublishQueueCanUpdatePublishStatus()
        {
            var queuesToUpdate = new List<QueueContent>();

            Guid deliverySessionId;
            var queues = this._queueService.GetAllQueuedContentForPublish(new DateTime(2014, 4, 15), out deliverySessionId);
            foreach (var queueContent in queues)
            {
                Assert.False(queueContent.IsPublished);
                if (queuesToUpdate.Count < 2)
                {
                    queuesToUpdate.Add(queueContent);
                }
            }
            Assert.True(queues.Count > 0, "Did not get any queued items.");

            var didUpdate = this._queueService.MarkContentAsPublished(queuesToUpdate);

            Assert.True(didUpdate);

        }

        [Fact]
        public void PublishQueueCanAssociateAttachment()
        {
            this._queueService.AssociateSyndicatedContentAttachments(this._testAssociationAttachmentQueueId, this._testSyndicatedContentAttachmentSource);

            var queue = this._queueService.GetQueuedMessage(this._testAssociationAttachmentQueueId);
            Assert.True(queue.QueueContentAttachments.Count > 0, "Did not associate and get queued content");
        }

        [Fact]
        public void PublishQueueCanCreateNewAttachment()
        {
            var attachments = new List<QueueContentAttachment>();
            attachments.Add(this.createTestAttachment(this._testNewAttachmentQueueid));
            attachments.Add(this.createTestAttachment(this._testNewAttachmentQueueid));
            this._queueService.AddNewAttachments(attachments);

            var queue = this._queueService.GetQueuedMessage(this._testNewAttachmentQueueid);
            Assert.True(queue.QueueContentAttachments.Count > 0, "Did not create, associate and get queued content");
        }

        // [Fact]
        public void PublishQueueCanCreateErrorQueue()
        {
            Assert.True(false);
        }

        [Fact]
        public void PublishQueueCanGetPublishedContent()
        {
            var publishedItems = this._queueService.GetPublishedContentsByUser(this._testUserWithCalendar,
                new PagingToken {StartRecord = 0, TotalRecords = 50});
            Assert.True(publishedItems.Count > 0, "Did not get any published items");
        }

        [Fact]
        public void PublishQueueCanGetContentCountByMonth()
        {
            var startMonth = new DateTime(2013, 11, 15);
            var endMonth = new DateTime(2014, 5, 12);
            var goodDictionary = this._queueService.GetQueuedContentCountForUserByMonth(
                this._testUserWithCalendar,
                startMonth,
                endMonth);
            Assert.True(goodDictionary.Count == 7, "Did not create a good dictionary");
        }

        [Fact]
        public void TestHourlyTimerMethods()
        {
            var hourlyMinutes = "60";

            var straightDate = this.RetrieveTimerInterval(hourlyMinutes);
            Assert.Equal(straightDate, 60 * 60000);

            var daName = "hourly";
            var nameDate = this.RetrieveTimerInterval(daName);
            Assert.True(nameDate >= 60000 && nameDate <= (60 * 60000));
        }

        #region private methods

        private QueueContent createTestQueueContent()
        {
            var random = new Random();

            var tc = new QueueContent();
            tc.AccessCode = Guid.NewGuid().ToString("N");
            tc.BravoVetsCountryId = (int)BravoVetsCountryEnum.US;
            tc.BravoVetsStatusId = (int)BravoVetsStatusEnum.Active;
            tc.BravoVetsUserId = random.Next(1, 5);
            tc.ContentText = string.Format("{0} test. A bunch of test text {1} to test out the content text. {2}",
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            tc.CreateDateUtc = DateTime.UtcNow;
            tc.Deleted = false;
            tc.IsPublished = false;
            tc.LinkUrl = string.Format("http://www.{0}.com", Guid.NewGuid().ToString("N"));
            tc.ModifiedDateUtc = DateTime.UtcNow;
            tc.PlatformPublishId = (int) SocialPlatformEnum.Facebook;
            tc.PublishDateUtc = DateTime.UtcNow.AddDays(random.Next(5, 35));
            tc.SyndicatedContentPostTypeId = (int)SyndicatedContentPostTypeEnum.Original;
            tc.SyndicatedContentId = random.Next(1, 20);

            return tc;
        }

        private QueueContentAttachment createTestAttachment(int queueContentId)
        {
            var random = new Random();

            var tqc = new QueueContentAttachment();
            tqc.AttachmentExtension = "png";
            tqc.AttachmentFileName = string.Format("Attachment{0}", Guid.NewGuid().ToString("N"));
            tqc.CreateDateUtc = DateTime.UtcNow;
            tqc.ModifiedDateUtc = DateTime.UtcNow;
            tqc.QueueContentId = queueContentId;
            tqc.AttachmentFile = new byte[] { Convert.ToByte(2) };
            return tqc;
        }

        private double RetrieveTimerInterval(string intervalMinutes)
        {
            double minutes;
            if (double.TryParse(intervalMinutes, out minutes))
            {
                return minutes * 60000;
            }

            var utcNow = DateTime.UtcNow;
            var timeTilTop = 60 - utcNow.Minute;

            return Convert.ToDouble(timeTilTop * 60000);
        }

        #endregion
    }
}
