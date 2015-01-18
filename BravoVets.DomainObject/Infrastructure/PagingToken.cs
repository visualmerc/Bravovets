using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainObject.Infrastructure
{
    public class PagingToken
    {
        public int StartRecord { get; set; }

        public int TotalRecords { get; set; }
    }
}
