using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Enum;
using BravoVets.DomainObject.Infrastructure;

namespace BravoVets.DomainService.Contract
{
    public interface IPublishQueueDomainService
    {

        /// <summary>
        /// Send content to the social calendar -- queue content to be published
        /// </summary>
        /// <param name="queue"></param>
        /// <returns>Whether the content was successfully queued</returns>
        QueueContent QueueContentForPublish(QueueContent queue);

        /// <summary>
        /// Create a new QueueContent item, based on an existing item
        /// </summary>
        /// <param name="queueContentId">PK for Queue Content table, for the source item</param>
        /// <returns>The new QueueContent item</returns>
        QueueContent CloneQueuedContent‏(int queueContentId);

        /// <summary>
        /// Remove a particular queued item from the publishing queue
        /// </summary>
        /// <param name="queueContentId">PK for Queue Content table</param>
        /// <returns>boolean indicating whether it was deleted</returns>
        bool DequeueContent(int queueContentId);

        /// <summary>
        /// Get a particular queued message
        /// </summary>
        /// <param name="queueContentId">PK for Queue Content table</param>
        /// <returns>A fully populated QueueContent object</returns>
        QueueContent GetQueuedMessage(int queueContentId);

        /// <summary>
        /// Get Queued Content for a user, to populate the social Calendar
        /// </summary>
        /// <param name="bravoVetsUserId">PK for BravoVets users</param>
        /// <param name="startDate">Start date for the publish date of queued messages</param>
        /// <param name="endDate">End date for the publish date of queued messages</param>
        /// <returns>A dictionary for dates, with a collection of queued messages</returns>
        Dictionary<DateTime, List<QueueContent>> GetQueuedContentForUserByDay(int bravoVetsUserId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get a count of how many queued content items are active for a particular user
        /// </summary>
        /// <param name="bravoVetsUserId">PK for BravoVets users</param>
        /// <param name="startMonth">DateTime -- only the month and year are used</param>
        /// <param name="endMonth">DateTime -- only the month and year are used</param>
        /// <returns>Dictionary with one DateTime per month, plus a count of queued items</returns>
        Dictionary<DateTime, int> GetQueuedContentCountForUserByMonth(int bravoVetsUserId, DateTime startMonth, DateTime endMonth);

        List<QueueContent> GetQueuedContentForUser(int bravoVetsUserId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// For the delivery process -- get all the queued content that has not been previously shared and whose publish time is
        /// before the specified UTC time
        /// </summary>
        /// <param name="currentUtcDateTime">Any unpublished content before this date is retrieved</param>
        /// <param name="deliverySessionId"></param>
        /// <returns>A list of unpublished queued content</returns>
        List<QueueContent> GetAllQueuedContentForPublish(DateTime currentUtcDateTime, out Guid deliverySessionId);

        /// <summary>
        /// For the delivery process -- get all the queued content that has not been previously shared and whose publish time is
        /// before the specified UTC time, for the specified country. the "IsPublished" column must be false. The BravoVetsStatusId must be Active
        /// </summary>
        /// <param name="currentUtcDateTime">Any unpublished content before this date is retrieved</param>        
        /// <param name="bravoVetsCountryId">The country of origin/publishing for the queued content</param>
        /// <param name="deliverySessionId">An identifier created for the delivery</param>
        /// <returns>A list of unpublished queued content</returns>
        List<QueueContent> GetAllQueuedContentForPublish(DateTime currentUtcDateTime, int bravoVetsCountryId, out Guid deliverySessionId);

        /// <summary>
        /// After a set of content is shared, this method will update it to indicate it has been published
        /// </summary>
        /// <param name="deliveredQueue"></param>
        /// <returns>An indicator that the content was successfully updated</returns>
        bool MarkContentAsPublished(List<QueueContent> deliveredQueue);

        /// <summary>
        /// After a set of content is shared, track the errored items
        /// </summary>
        /// <param name="erroredQueue"></param>
        /// <returns>An indicator of whether the errored queue was processed</returns>
        bool AddContentToErroredQueue(List<QueueContent> erroredQueue);

        /// <summary>
        /// Copies attachment(s) from syndicated content to queued item
        /// </summary>
        /// <param name="queueContentId">PK for queue contents</param>
        /// <param name="syndicatedContentId">PK for syndicated content</param>
        void AssociateSyndicatedContentAttachments(int queueContentId, int syndicatedContentId);

        /// <summary>
        /// Upload new attachments for a queued item
        /// </summary>
        /// <param name="attachment">QueueContentAttachment object</param>
        void AddNewAttachments(List<QueueContentAttachment> attachment);

        // TODO: BVRP should see content that was posted or shared in Social Calendar list only, removed from Trending Topics
        /// <summary>
        /// Show the content that a particular user shared via the social calendar
        /// </summary>
        /// <param name="bravoVetsUserId">PK for the user table</param>
        /// <param name="pagingToken">paging token for records</param>
        /// <returns>A selection of records published by a user</returns>
        List<QueueContent> GetPublishedContentsByUser(int bravoVetsUserId, PagingToken pagingToken);

        /// <summary>
        /// Seed a QueueContent with some minimal default values
        /// </summary>
        /// <param name="country">country of origin for content</param>
        /// <param name="bravoVetsUserId">Author ID</param>
        /// <returns>the QueueContent object</returns>
        QueueContent GetMinimalEmptyQueueContent(BravoVetsCountryEnum country, int bravoVetsUserId);

        QueueContentAttachment GetQueueContentAttachment(int queueContentAttachmentId);

        void DeleteAttachment(int queueContentAttachmentId);

        /// <summary>
        /// After a Queued item is shared, record its disposition
        /// </summary>
        /// <param name="deliverySessionId">ID generated server side when the timer job runs</param>
        /// <param name="queueContentId">The PK of the QueueContent item</param>
        /// <param name="wasSuccess">whether the sharing worked</param>
        /// <param name="errorMessage">Only pass if there was an error</param>
        void LogQueuedContentDelivery(Guid deliverySessionId, int queueContentId, bool wasSuccess, string errorMessage);
    }
}
