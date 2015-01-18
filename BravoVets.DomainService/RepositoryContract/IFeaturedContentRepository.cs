using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.RepositoryContract
{
    using BravoVets.DomainObject;
    using BravoVets.DomainObject.Enum;

    public interface IFeaturedContentRepository : IBaseRepository<FeaturedContent>
    {
        List<FeaturedContentSlim> GetFeaturedContent(SyndicatedContentTypeEnum contentType, BravoVetsCountryEnum country);
    }
}
