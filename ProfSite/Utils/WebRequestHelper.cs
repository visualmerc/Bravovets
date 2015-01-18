using System;
using System.Data.Entity.Validation;
using System.IO;
using System.Net;
using System.Text;
using BravoVets.DomainObject.Infrastructure;

namespace ProfSite.Utils
{
    public static class WebRequestHelper
    {
        public static string GetWebResponse(WebRequest httpWebRequest, out bool success)
        {
            var url = httpWebRequest.RequestUri;

            HttpWebResponse webResponse;
            try
            {
                webResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                string json = ReadResponse(webResponse);

                webResponse.Close();
                success = true;
                return json;
            }
            catch (WebException e)
            {
                success = false;
                webResponse = e.Response as HttpWebResponse;

                if (webResponse != null)
                {
                    string json = ReadResponse(webResponse);

                    webResponse.Close();
                    return json;
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                success = false;
                return string.Empty;
            }
        }

        private static string ReadResponse(WebResponse webResponse)
        {
            string response = "";
            Stream responseStream = webResponse.GetResponseStream();

            

            if (responseStream == null)
                throw new Exception("Response stream is null");

            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(responseStream, encode);
            
            var read = new Char[256];
            // Reads 256 characters at a time.     
            int count = readStream.Read(read, 0, 256);
            
            while (count > 0)
            {
                String str = new String(read, 0, count);
                response += str;
                count = readStream.Read(read, 0, 256);
            }

            return response;
        }
    }
}