using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using Xunit;

namespace BravoVets.DomainService.Test
{
    public class WindowsServiceTest
    {
        internal static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string _serviceUrl = ConfigurationManager.AppSettings["ServiceUrl"];

        public WindowsServiceTest()
        {
            XmlConfigurator.Configure();
        }

        [Fact]
        public void TesterCanRunServiceCode()
        {
            var content = this.PublishContent();
            var myMilliseconds = 60000 * Convert.ToDouble(ConfigurationManager.AppSettings["IntervalMinutes"]);
            Console.WriteLine("Current millisecond value: {0}", myMilliseconds);
            Assert.True(content.Length > 0, "did not get web response");
        }
        
        private string PublishContent()
        {
            try
            {
                var requester = new WebClient();
                var response = requester.DownloadString(this._serviceUrl);

                Logger.Info(string.Format("Test Publish Service Log Event. Called service url {0} at {1}", this._serviceUrl, DateTime.UtcNow));
                return response;
            }
            catch (Exception ex)
            {
                Logger.Error("Error publishing content BravoVets.WindowsService.PublishQueue", ex);
                throw ex;
            }
        }
    }
}
