using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProfSite.Tests.Infrastructure
{
    public class MockWebService
    {
        public struct MockCall
        {
            public string Url { get; set; }
            public Func<string,object, MockWebResponse> WebAction { get; set; }

            //TODO:  This does not work right now because the count gets updated
            // on a separate thread.  We need to address that before we can use this count.
            public int NumberOfCalls { get; set; }
        }

        public class MockWebResponse
        {
            public MockWebResponse()
            {
                StatusCode = HttpStatusCode.OK;
            }

            public HttpStatusCode StatusCode { get; set; }
            public string StatusDescription { get; set; }
            public dynamic Data { get; set; }
            public string ContentType { get; set; }
        }

        private HttpListener newHttpListener;

        private Dictionary<string, MockCall> actions = new Dictionary<string, MockCall>();


        private Thread thread;

        private void AddAction(MockCall mockCall)
        {
            var url = FormatUrl(mockCall.Url);

            if (actions.ContainsKey(mockCall.Url))
                throw new Exception("Duplicate URL added");

            actions.Add(url, mockCall);
        }

        private string FormatUrl(string urlIn)
        {
            string urlOut = urlIn.ToLower().Trim();
            // strip trailing slash
            if (urlOut.EndsWith("/")) urlOut = urlOut.Remove(urlOut.Length - 1);

            return urlOut;
        }

        public void Run(params MockCall[] mockCalls)
        {

            if (mockCalls == null)
                throw new Exception("You have to setup the calls first");

            foreach (var mockCall in mockCalls)
            {
                AddAction(mockCall);
            }

            thread = new Thread(Start);
            thread.Start();
        }

        private void Start()
        {
            //Info on HttpListener:  http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx
            newHttpListener = new HttpListener();

            // Turning this on might help if there are problems.
            //newHttpListener.IgnoreWriteExceptions = true;

            foreach (var action in actions)
            {

                var actionUrl = action.Key;
                if (!actionUrl.EndsWith("/"))
                    actionUrl += "/";

                newHttpListener.Prefixes.Add(actionUrl);
            }

            newHttpListener.Start();

            do
            {
                GetContext();
            } while (true);
        }

        private void GetContext()
        {
            HttpListenerContext context = newHttpListener.GetContext();

            var requestedUrl = FormatUrl(context.Request.Url.OriginalString.ToLower());

            var queryString = context.Request.Url.Query;

            var actionUrl = requestedUrl;
            if (!string.IsNullOrEmpty(queryString))
            {
                var startIndex = requestedUrl.IndexOf("?");
                if (startIndex >0 )
                    actionUrl = requestedUrl.Remove(startIndex);
            }
            


            if (!actions.ContainsKey(actionUrl))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                Debug.WriteLine(string.Format("URL not found for mock web service {0}", requestedUrl));
            }
            else
            {
                var mockCall = actions[actionUrl];

                var action = mockCall.WebAction;
                mockCall.NumberOfCalls++;

                dynamic requestData = GetRequestData(context);
                var response = action(queryString,requestData);

                string responseString = GetResponseAsString(context.Request.ContentType, response);

                // If the mock response specifies the content type, use it. Otherwise return the same
                // content type as the request
                context.Response.ContentType = !string.IsNullOrEmpty(response.ContentType)
                                                   ? response.ContentType
                                                   : context.Request.ContentType;
                context.Response.StatusCode = (int)response.StatusCode;

                var enc = new UTF8Encoding();
                byte[] byteArr = enc.GetBytes(responseString);

                context.Response.OutputStream.Write(byteArr, 0, byteArr.Length);
            }

            context.Response.Close();
        }

        private object GetRequestData(HttpListenerContext context)
        {
            var body = new StreamReader(context.Request.InputStream).ReadToEnd();

            if (context.Request.ContentType != null && context.Request.ContentType.ToLower().Contains("json"))
            {
                return JsonConvert.DeserializeObject(body, typeof(ExpandoObject));
            }
            return body;
        }

        private string GetResponseAsString(string contentType, MockWebResponse response)
        {
            if (contentType != null && contentType.ToLower().Contains("json"))
            {
                //var json = response.Data == null ? string.Empty : JsonHelper.ToJsonFromObject(response.Data).ToString();
                var json = response.Data ?? string.Empty;
                return json;
            }
            return response.Data.ToString();
        }

        public void Stop()
        {
            if (thread != null)
                thread.Abort();
            if (newHttpListener != null)
            {
                if (newHttpListener.IsListening)
                    newHttpListener.Stop();

                newHttpListener.Close();
            }

        }

    }
}
