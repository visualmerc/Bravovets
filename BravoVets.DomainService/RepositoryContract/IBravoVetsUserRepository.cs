using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.DomainObject;

namespace BravoVets.DomainService.RepositoryContract
{
    public interface IBravoVetsUserRepository : IBaseRepository<BravoVetsUser>
    {
        BravoVetsUser GetByMerckId(int lfwUserId);

        BravoVetsUser Get(int bravoVetsUserId, bool fullGraph);

        bool AcceptTermsAndConditions(int bravoVetsUserId);

        bool ReverseTermsAndConditions(int bravoVetsUserId);
    }
}
