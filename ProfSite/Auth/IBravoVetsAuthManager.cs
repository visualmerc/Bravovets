using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BravoVets.DomainObject;

namespace ProfSite.Auth
{
    public interface IBravoVetsAuthManager
    {
        bool HydrateBravoVetsUser(int merckId, out int bravoVetsUserId);

        bool CreateBravoVetsUser(MerckUser user, out int bravoVetsUserId);

        void SyncUserWithLfw();

        bool InvalidateCurrentIdentityInfo();

        bool SignOutOfLfw();


    }
}
