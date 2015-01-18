using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.RepositoryContract
{
    public interface IBaseRepository<T>
    {
        T Get(int id);

        T Create(T obj);

        T Update(T obj);

        bool Delete(int id);

    }
}
