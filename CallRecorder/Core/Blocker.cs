using System;
using System.Threading;
using System.Windows.Forms;

namespace CallRecorder.Core
{
    public static class Blocker
    {
        public static DateTime LimitDate { get; set; }
        public static bool InTrial()
        {
            return GetDateTimeNow() < LimitDate;
        }
        private static DateTime GetDateTimeNow()
        {
            DateTime dateTime = DateTime.MinValue;
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://www.microsoft.com");
                request.Method = "GET";
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
                request.ContentType = "application/x-www-form-urlencoded";
                request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string todaysDates = response.Headers["date"];

                    dateTime = DateTime.ParseExact(todaysDates, "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, System.Globalization.DateTimeStyles.AssumeUniversal);
                }
            }
            catch(Exception ex)
            {
                Utils.Log(ex.Message);
                Thread.Sleep(1000);
                Application.Exit();
            }

            return dateTime;
        }
    }
}
