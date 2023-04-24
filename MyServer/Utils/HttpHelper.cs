using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MyServer.Utils
{
    public static class HttpHelper
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static string GetPostRequestContent(HttpListenerRequest request)
        {
            string s = string.Empty;
            try
            {
                if (!request.HasEntityBody)
                {
                    logger.Error("No client data was sent with the request.");
                    return string.Empty;
                }
                Stream body = request.InputStream;
                Encoding encoding = request.ContentEncoding;
                StreamReader reader = new StreamReader(body, encoding);
                //if (request.ContentType != null)
                //    logger.Debug("Client data content type {0}", request.ContentType);
                //logger.Debug("Client data content length {0}", request.ContentLength64);
                // Convert the data to a string and display it on the console.
                s = reader.ReadToEnd();
                body.Close();
                reader.Close();
                // If you are finished with the request, it should be closed also.
            }
            catch (Exception ex)
            {
                logger.Error($"GetPostRequestContent Error {ex} ");
            }
            return s;
        }

        public static Dictionary<string, string> GetRequestParameters(string row)
        {
            if (string.IsNullOrEmpty(row)) return null;
            var kvs = Regex.Split(row, "&");
            if (kvs == null || kvs.Length <= 0) return null;
            return kvs.ToDictionary(e => Regex.Split(e, "=")[0], e => { var p = Regex.Split(e, "="); return p.Length > 1 ? p[1] : ""; });
        }
    }
}
