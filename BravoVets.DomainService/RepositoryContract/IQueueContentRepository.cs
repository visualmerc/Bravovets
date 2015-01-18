using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.DomainObject;
using BravoVets.DomainObject.Infrastructure;

namespace BravoVets.DomainService.RepositoryContract
{
    public interface IQueueContentRepository : IBaseRepository<QueueContent>
    {
        List<QueueContent> GetQueuedContentForUserByDateRange(int bravoVetsUserId, DateTime startDate, DateTime endDate);

        int GetQueuedContentCountForUserByDateRange(int bravoVetsUserId, DateTime startDate, DateTime endDate);

        List<QueueContent> GetUnpublishedQueueContents(DateTime publishDateTime);

        List<QueueContent> GetUnpublishedQueueContents(DateTime publishDateTime, int bravoVetsCountryId);

        bool UpdateContentAsPublished(List<QueueContent> publishedItems);

        List<QueueContent> GetPublishedContentsByUser(int bravoVetsUserId, PagingToken pagingToken);

        void CreateQueueLog(QueueContentDeliveryLog queueContentDeliveryLog);
    }
}
