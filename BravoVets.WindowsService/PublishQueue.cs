using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;
using System.Timers;
using log4net;
using Timer = System.Timers.Timer;
using log4net.Config;

namespace BravoVets.WindowsService
{
    public partial class PublishQueue : ServiceBase
    {
        private Timer timer;

        internal static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string _serviceUrl = ConfigurationManager.AppSettings["ServiceUrl"];

        private bool _isTimeVariable = false;

        public PublishQueue()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                XmlConfigurator.Configure();
                this.InitializeTimer();
            }
            catch (Exception ex)
            {
                Logger.Error("Error starting publishing queue", ex);
            }
        }

        protected override void OnStop()
        {
            timer.Stop();

        }

        public void PublishContent()
        {
            /*
             * Query to find any Syndicated Content Queue whose publish date has passed
             * Also check "IsPublished"
             * Iterate through avaialable content
             * Find the platform it is targeted for
             * structure the message appropriately
             * Use helper code to push content to the platform
             * Mark the content as pushed
             * */
            try
            {
                var requester = new WebClient();
                var response = requester.DownloadString(this._serviceUrl);

                Logger.Info(string.Format("Publish Service Log Event. Called service url {0} at {1}", this._serviceUrl, DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                Logger.Error("Error publishing content BravoVets.WindowsService.PublishQueue", ex);
            }
        }

        private void InitializeTimer()
        {
            Logger.Info(string.Format("Publish Service starting {0}", DateTime.UtcNow));
            if (timer == null)
            {
                timer = new Timer();
                timer.AutoReset = true;
                timer.Interval = this.RetrieveTimerInterval();
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Start();
                Logger.Info(string.Format("Timer is now working with an interval of {0}", timer.Interval));
            }
        }

        private void timer_Elapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            PublishContent();
            if (_isTimeVariable)
            {
                timer.Interval = this.RetrieveTimerInterval();
            }
        }

        private double RetrieveTimerInterval()
        {
            double minutes;
            if (double.TryParse(ConfigurationManager.AppSettings["IntervalMinutes"], out minutes))
            {
                return minutes * 60000;
            }

            var utcNow = DateTime.UtcNow;
            var timeTilTop = 60 - utcNow.Minute;
            this._isTimeVariable = true;

            return Convert.ToDouble(timeTilTop * 60000);
        }

    }
}
