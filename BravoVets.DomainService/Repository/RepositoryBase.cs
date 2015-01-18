using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BravoVets.DomainService.Repository
{
    using System.Data.Entity.Validation;
    using System.Reflection;

    using BravoVets.DomainObject.Infrastructure;

    using log4net;

    public class RepositoryBase
    {
        internal static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal static BravoVetsException GenerateBravoVetsRepositoryException(Exception ex, string originator)
        {
            var errorId = Guid.NewGuid();
            var processedError = new BravoVetsException();
            processedError.ErrorId = errorId;
            processedError.Originator = originator;
            processedError.BvInnerException = ex.InnerException;
            if (ex.GetType() == typeof(DbEntityValidationException))
            {
                var validationText = GatherValidationExceptions((DbEntityValidationException)ex);
                processedError.BvMessage = string.Format("{0} {1}", validationText, ex.Message);
                Logger.Error(string.Format("{0} {1}", errorId.ToString("N"), validationText));
            }
            else
            {
                processedError.BvMessage = string.Format("{0}", ex.Message);
                Logger.Error(errorId.ToString("N"), ex);
            }
            return processedError;
        }

        private static string GatherValidationExceptions(DbEntityValidationException dbException)
        {
            var validationErrors = new StringBuilder();
            var x = 0;
            foreach (var validation in dbException.EntityValidationErrors)
            {
                var y = 0;
                foreach (var error in validation.ValidationErrors)
                {
                    validationErrors.AppendLine(string.Format("{0}.{1} {2}", x, y, error.ErrorMessage));
                    y++;
                }
                x++;
            }
            return validationErrors.ToString();
        }
    }
}
