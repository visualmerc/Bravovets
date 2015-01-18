using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Service
{
    using System.Reflection;

    using BravoVets.DomainObject.Infrastructure;

    using log4net;

    public class DomainServiceBase
    {
        internal static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal static BravoVetsException GenerateBravoVetsServiceException(Exception ex, string originator)
        {
            // If this error has already been processed, pass it along
            if (ex.GetType() == typeof(BravoVetsException))
            {
                return (BravoVetsException)ex;
            }

            Guid errorId = Guid.NewGuid();
            Logger.Error(errorId.ToString("N"), ex);
            var processedError = new BravoVetsException();
            processedError.ErrorId = errorId;
            processedError.Originator = originator;
            processedError.BvInnerException = ex.InnerException;
            processedError.BvMessage = ex.Message;
            return processedError;
        }
    }
}
