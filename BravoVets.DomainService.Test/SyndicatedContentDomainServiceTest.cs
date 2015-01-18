using System;
using System.Collections.Generic;
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

    using Xunit;

    public class SyndicatedContentDomainServiceTest : AbstractServiceBaseTest
    {
        private ISyndicatedContentDomainService _contentService;

        #region set test syndicatedcontentid's

        private int _testUserId = 2;

        private int _falseCaseUserId = 33;

        private int _testCountryId = 2;

        // IDs that are set in the database as inital test data
        private int _presetFavoriteId = 2;

        private int _presetViewedId = 5;

        private int _presetHiddendId = 10;

        // IDs to test with
        private int _facebookShareTestId = 3;

        private int _favoriteTestId = 12;

        private int _unfavoriteTestId = 15;

        private int _hiderTestId = 9;

        #endregion

        public SyndicatedContentDomainServiceTest()
        {
            this._contentService = new SyndicatedContentDomainService();
        }

        [Fact]
        public void ContentServiceCanGetTrendingTopics()
        {
            var content = this._contentService.GetTrendingTopics(this._testUserId, this._testCountryId);
            foreach (var syndicatedContent in content)
            {
                var syndicatedContentId = syndicatedContent.SyndicatedContentId;
                Assert.True(
                    syndicatedContentId > 0,
                    string.Format("Trouble with syndicatedContentId {0}", syndicatedContentId));
            }
            Assert.True(content.Count > 0, "Did not get any syndicated content");
        }

        [Fact]
        public void ContentServiceCanGetTrendingTopicsWithAttachments()
        {
            var content = this._contentService.GetTrendingTopics(this._testUserId, this._testCountryId);
            bool attachmentsPresent = false;
            foreach (
                var syndicatedContent in
                    content.Where(syndicatedContent => syndicatedContent.SyndicatedContentAttachments.Count > 0))
            {
                Console.WriteLine("Found an attachment on {0}", syndicatedContent.SyndicatedContentId);
                attachmentsPresent = true;
                break;
            }
            Assert.True(attachmentsPresent, "Did not find any attachments");
        }

        [Fact]
        public void ContentServiceCanGetSocialTips()
        {
            var tips = this._contentService.GetSocialTips(this._testUserId, this._testCountryId);
            Assert.True(tips.Count > 0, "did not get any social tips");
        }

        [Fact]
        public void ContentServiceCanSocialTipsWithAttachments()
        {
            var content = this._contentService.GetSocialTips(this._testUserId, this._testCountryId);
            bool attachmentsPresent = false;
            foreach (
                var syndicatedContent in
                    content.Where(syndicatedContent => syndicatedContent.SyndicatedContentAttachments.Count > 0))
            {
                Console.WriteLine("Found an attachment on {0}", syndicatedContent.SyndicatedContentId);
                attachmentsPresent = true;
                break;
            }
            Assert.True(attachmentsPresent, "Did not find any attachments");
        }

        // [Fact]
        public void ContentServiceCanGetFavoriteTrendingTopics()
        {
            // Look for IsFavoritedByMe, IsViewedByMe, IsSharedByMe
            var content = this._contentService.GetTrendingTopics(this._testUserId, this._testCountryId);

            var favoriteContent = content.FirstOrDefault(c => c.SyndicatedContentId == this._presetFavoriteId);

            Assert.True(content.Count > 0, "Did not get a favorite syndicated content");

            Assert.True(favoriteContent.IsFavoritedByMe, "Did not find favorite content");
            Assert.False(favoriteContent.IsViewedByMe, "False positive on favorite content");
        }

        [Fact]
        public void ContentServiceUserDoesHaveHiddenContent()
        {
            var hasHidden = this._contentService.UserHasHiddenTrendingTopics(this._testUserId);
            Assert.True(hasHidden);
        }

        [Fact]
        public void ContentServiceUserDoesHaveSharedContent()
        {
            var hasShared = this._contentService.UserHasSharedTrendingTopics(this._testUserId);
            Assert.True(hasShared);
        }

        [Fact]
        public void ContentServiceUserDoesNotHaveHiddenContent()
        {
            var noHidden = this._contentService.UserHasHiddenTrendingTopics(this._falseCaseUserId);
            Assert.False(noHidden);
        }

        [Fact]
        public void ContentServiceUserDoesNotHaveSharedContent()
        {
            var noShared = this._contentService.UserHasSharedTrendingTopics(this._falseCaseUserId);
            Assert.False(noShared);
        }

        [Fact]
        public void ContentServiceCanGetViewedTrendingTopics()
        {
            // Look for IsFavoritedByMe, IsViewedByMe, IsSharedByMe
            var content = this._contentService.GetTrendingTopics(this._testUserId, this._testCountryId);

            var favoriteContent = content.FirstOrDefault(c => c.SyndicatedContentId == _presetViewedId);

            Assert.True(content.Count > 0, "Did not get any syndicated content");

            Assert.True(favoriteContent.IsViewedByMe, "Did not find Viewed content");
            Assert.False(favoriteContent.IsFavoritedByMe, "False positive on viewed content");
        }


        [Fact]
        public void ContentServiceCanExcludeHiddenContent()
        {
            var content = this._contentService.GetTrendingTopics(this._testUserId, this._testCountryId);
            foreach (var syndicatedContent in content)
            {
                Assert.True(syndicatedContent.SyndicatedContentId != _presetHiddendId, "Did not exclude a hidden item");
            }

            Assert.True(content.Count > 0, "Did not get any syndicated content");
        }

        [Fact]
        public void ContentServiceCanExcludeSharedContent()
        {
            var content = this._contentService.GetTrendingTopics(this._testUserId, this._testCountryId);
            foreach (var syndicatedContent in content)
            {
                Assert.True(
                    syndicatedContent.SyndicatedContentId != _facebookShareTestId,
                    "Did not exclude a shared item");
            }

            Assert.True(content.Count > 0, "Did not get any syndicated content");
        }

        [Fact]
        public void ContentServiceCanGetFilteredTrendingTopics_Favorites()
        {
            var favorites = ContentFilterEnum.Favorites;
            var content = this._contentService.GetFilteredTrendingTopics(
                this._testUserId,
                this._testCountryId,
                favorites);

            foreach (var syndicatedContent in content)
            {
                Assert.True(syndicatedContent.IsFavoritedByMe, "Did not mark a favorited content as favorite.");
            }

            Assert.True(content.Count > 0, "Did not get any favorites");
        }

        [Fact]
        public void ContentServiceCanGetFilteredTrendingTopics_Shared()
        {
            const ContentFilterEnum shared = ContentFilterEnum.GenericShare;

            var content = this._contentService.GetFilteredTrendingTopics(this._testUserId, this._testCountryId, shared);

            foreach (var syndicatedContent in content)
            {
                Assert.True(syndicatedContent.IsSharedByMe, "Did not share this content.");
            }

            Assert.True(content.Count > 0, "Did not get any favorites");
        }

        [Fact]
        public void ContentServiceCanGetFilteredBravectoResources_Favorites()
        {
            var favorites = ContentFilterEnum.Favorites;
            var content = this._contentService.GetFilteredBravectoResources(
                this._testCountryId,
                this._testUserId,
                favorites,
                ContentSortEnum.ContentDate,
                new PagingToken { StartRecord = 0, TotalRecords = 25 });

            foreach (var syndicatedContent in content)
            {
                Assert.True(syndicatedContent.IsFavoritedByMe, "Did not mark a favorited content as favorite.");
            }

            Assert.True(content.Count > 0, "Did not get any favorites from bravecto resources");
        }

        [Fact]
        public void ContentServiceCanGetFilteredSocialTips_Favorites()
        {
            const ContentFilterEnum favorites = ContentFilterEnum.Favorites;

            var content = this._contentService.GetFilteredSocialTips(
                this._testCountryId,
                this._testUserId,
                favorites,
                ContentSortEnum.ContentDate,
                new PagingToken { StartRecord = 0, TotalRecords = 50 });

            foreach (var syndicatedContent in content)
            {
                Assert.True(syndicatedContent.IsFavoritedByMe, "Did not mark a favorited content as favorite.");
            }

            Assert.True(content.Count > 0, "Did not get any favorites from social tips");
        }


        [Fact]
        public void ContentServicesCanShareContent()
        {
            var sharedContent = this._contentService.ShareContent(
                this._testUserId,
                this._facebookShareTestId,
                SocialPlatformEnum.Facebook);
            Assert.True(sharedContent.NumberOfShares > 0, "did not share content");
        }

        [Fact]
        public void ContentServicesCanFavoriteTrendingTopics()
        {
            bool shouldIncrement = true;

            var syndicatedContent = this._contentService.GetSyndicatedContent(this._testUserId, _favoriteTestId);
            if (syndicatedContent.IsFavoritedByMe)
            {
                shouldIncrement = false;
            }

            var content = this._contentService.GetSyndicatedContent(this._testUserId, _favoriteTestId);
            var numberOfFavorites = content.NumberOfFavorites;
            this._contentService.FavoriteContent(this._testUserId, _favoriteTestId);
            var newContent = this._contentService.GetSyndicatedContent(this._testUserId, _favoriteTestId);
            var updatedFavorites = newContent.NumberOfFavorites;
            if (shouldIncrement)
            {
                Assert.True(updatedFavorites == numberOfFavorites + 1, "did not increment");
            }
            else
            {
                Assert.True(updatedFavorites == numberOfFavorites, "incremented when it should not have");
            }

        }

        [Fact]
        public void ContentServicesCanUnfavoriteTrendingTopics()
        {
            bool shouldIncrement = true;

            var content = this._contentService.GetSyndicatedContent(this._testUserId, _unfavoriteTestId);
            if (content.IsFavoritedByMe)
            {
                shouldIncrement = false;
            }

            var numberOfFavorites = content.NumberOfFavorites;
            this._contentService.FavoriteContent(this._testUserId, _unfavoriteTestId);

            var newContent = this._contentService.GetSyndicatedContent(this._testUserId, _unfavoriteTestId);
            var updatedFavorites = newContent.NumberOfFavorites;

            if (shouldIncrement)
            {
                Assert.True(updatedFavorites == numberOfFavorites + 1, "did not increment");
            }
            else
            {
                Assert.True(updatedFavorites == numberOfFavorites, "incremented when it should not have");
            }

            this._contentService.UnfavoriteContent(this._testUserId, _unfavoriteTestId);

            var finalContent = this._contentService.GetSyndicatedContent(this._testUserId, _unfavoriteTestId);
            var finalFavorites = newContent.NumberOfFavorites;

            Assert.False(finalContent.IsFavoritedByMe);

            Assert.True(finalFavorites < updatedFavorites, "did not decrement the number of favorites");
        }

        [Fact]
        public void ContentServiceCanGetAttachments()
        {
            var attachments = this._contentService.GetAttachmentsById(4);
            Assert.True(attachments.Count > 0, "did not get any attachments");
        }

        [Fact]
        public void ContentServicesCanHideContent()
        {
            this._contentService.HideContent(this._testUserId, this._hiderTestId);

            var content = this._contentService.GetTrendingTopics(this._testUserId, this._testCountryId);
            foreach (var syndicatedContent in content)
            {
                Assert.True(syndicatedContent.SyndicatedContentId != this._hiderTestId, "Did not exclude a hidden item");
            }

            var hiddenContent = this._contentService.GetFilteredTrendingTopics(
                this._testCountryId,
                this._testUserId,
                ContentFilterEnum.Hidden);

            Assert.True(hiddenContent.Count > 0);
            var gotIt = hiddenContent.FirstOrDefault(s => s.SyndicatedContentId == this._hiderTestId);
            Assert.True(gotIt != null, "Did not find the hidden item in a list filtered for hidden content");
        }

        [Fact]
        public void ContentServiceCanGetBravectoResources()
        {
            var resources = this._contentService.GetBravectoResources(this._testUserId, this._testCountryId);
            Assert.True(resources.Count > 0, "Did not get any bravecto resources");
            foreach (var resource in resources)
            {
                Assert.True(
                    resource.SyndicatedContentTypeId == (int)SyndicatedContentTypeEnum.BravectoResources,
                    "Did not get bravecto resources");
            }
        }



        //[Fact]
        //public void ContentServicesCanUnhideContent()
        //{
        //    Assert.True(false);
        //}

        [Fact]
        public void ContentServiceCanSaveImages()
        {
            Image testImage = Image.FromFile(@"C:\Users\Rob\Pictures\SyndicatedContentAttachments\KittenPile.png");

            var attachment = new SyndicatedContentAttachment();
            attachment.AttachmentExtension = "jpg";
            attachment.AttachmentFileName = "SugarGlider.jpg";
            attachment.CreateDateUtc = DateTime.UtcNow;
            attachment.ModifiedDateUtc = DateTime.UtcNow;
            attachment.SyndicatedContentId = 57;
            attachment.AttachmentFile = ImageToByte(testImage, "png");
            attachment.DisplayInUi = false;

            this._contentService.SaveAttachment(attachment);
        }

        [Fact]
        public void ContentServiceCanSavePdf()
        {
            var testPdf = System.IO.File.ReadAllBytes(@"C:\Users\Rob\Desktop\Transfer\BravoVets Project Website.pdf");

            var attachment = new SyndicatedContentAttachment();
            attachment.AttachmentExtension = "pdf";
            attachment.AttachmentFileName = "BravoVetsDocs.pdf";
            attachment.CreateDateUtc = DateTime.UtcNow;
            attachment.ModifiedDateUtc = DateTime.UtcNow;
            attachment.SyndicatedContentId = 57;
            attachment.AttachmentFile = testPdf;
            attachment.DisplayInUi = false;

            this._contentService.SaveAttachment(attachment);
        }
        //System.IO.File.ReadAllBytes("myfile.pdf")

        /*        [Fact]
              public void ContentServiceCanBulkImportImages()
              {
                  var adder = 0;

                  //for (int i = 0; i < 4; i++)
                  //{

                  var pd = GetPictureList();

                  foreach (KeyValuePair<int, string> keyValuePair in pd)
                  {
                      var imagePath =
                          string.Format(
                              @"C:\Users\Rob\Downloads\LocalizedBvImages\bravoVets-images-trend-Social-ES\exported\social Tips\{0}",
                              keyValuePair.Value);

                      Image testImage = Image.FromFile(imagePath);

                      var attachment = new SyndicatedContentAttachment();
                      attachment.AttachmentExtension = "jpg";
                      attachment.AttachmentFileName = keyValuePair.Value;
                      attachment.CreateDateUtc = DateTime.UtcNow;
                      attachment.ModifiedDateUtc = DateTime.UtcNow;
                      attachment.SyndicatedContentId = keyValuePair.Key + adder;
                      attachment.AttachmentFile = ImageToByte(testImage, "jpg");
                      this._contentService.AddAttachment(attachment);
                  }

                  adder += 98;
                  //}

                  Assert.True(true);
              }

              private static Dictionary<int, string> GetPictureList()
              {
                  var pd = new Dictionary<int, string>();
                  pd.Add(449, "bravovets_socialMedia_1_es.jpg");
                  pd.Add(450, "bravovets_socialMedia_2_es.jpg");
                  pd.Add(451, "bravovets_socialMedia_3_es.jpg");
                  pd.Add(452, "bravovets_socialMedia_4_es.jpg");
                  pd.Add(453, "bravovets_socialMedia_5_es.jpg");
                  pd.Add(454, "bravovets_socialMedia_6_es.jpg");
                  pd.Add(455, "bravovets_socialMedia_7_es.jpg");
                  pd.Add(456, "bravovets_socialMedia_8_es.jpg");
                  pd.Add(457, "bravovets_gettingStarted_1_es.jpg");
                  pd.Add(458, "bravovets_gettingStarted_2_es.jpg");
                  pd.Add(459, "bravovets_gettingStarted_3_es.jpg");
                  pd.Add(460, "bravovets_gettingStarted_4_es.jpg");
                  pd.Add(461, "bravovets_growingYourBase_1_es.jpg");
                  pd.Add(462, "bravovets_growingYourBase_2_es.jpg");
                  pd.Add(463, "bravovets_growingYourBase_3_es.jpg");
                  pd.Add(464, "bravovets_growingYourBase_4_es.jpg");
                  pd.Add(465, "bravovets_growingYourBase_5_es.jpg");
                  pd.Add(466, "bravovets_growingYourBase_6_es.jpg");
                  pd.Add(467, "bravovets_understandingContent_1_es.jpg");
                  pd.Add(468, "bravovets_understandingContent_2_es.jpg");
                  pd.Add(469, "bravovets_understandingContent_3_es.jpg");
                  pd.Add(470, "bravovets_understandingContent_4_es.jpg");
                  pd.Add(471, "bravovets_understandingContent_5_es.jpg");
                  pd.Add(472, "bravovets_understandingContent_6_es.jpg");
                  pd.Add(473, "bravovets_understandingContent_7_es.jpg");
                  pd.Add(474, "bravovets_managingACalendar_1_es.jpg");
                  pd.Add(475, "bravovets_managingACalendar_2_es.jpg");
                  pd.Add(476, "bravovets_managingACalendar_3_es.jpg");
                  pd.Add(477, "bravovets_managingCommunities_1_es.jpg");
                  pd.Add(478, "bravovets_managingCommunities_2_es.jpg");
                  pd.Add(479, "bravovets_managingCommunities_3_es.jpg");
                  pd.Add(480, "bravovets_managingCommunities_4_es.jpg");
                  pd.Add(481, "bravovets_prTopics_1_es.jpg");
                  pd.Add(482, "bravovets_prTopics_2_es.jpg");
                  pd.Add(483, "bravovets_prTopics_3_es.jpg");
                  pd.Add(484, "bravovets_prTopics_4_es.jpg");
                  pd.Add(485, "bravovets_prTopics_5_es.jpg");
                  pd.Add(486, "bravovets_prTopics_6_es.jpg");
                  pd.Add(487, "bravovets_prTopics_7_es.jpg");
                  pd.Add(488, "bravovets_analytics_1_es.jpg");
                  pd.Add(489, "bravovets_analytics_2_es.jpg");
                  return pd;
              } */

        #region private methods

        public static byte[] ImageToByte(Image img, string whatType)
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
