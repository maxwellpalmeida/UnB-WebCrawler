using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnBWebCrawler.Classes.Common
{
    public static class Useful
    {
        public static HtmlDocument getHtmlDoc(string url, int qtCalls = 0)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            try
            {
                var httpClient = new HttpClient();
                var html = httpClient.GetStringAsync(url);//.ConfigureAwait(false);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html.Result);
                return htmlDocument;
            }
            catch (Exception)
            {
                if (qtCalls <= 4)
                {
                    Thread.Sleep(3000);
                    return getHtmlDoc(url, qtCalls++);
                }
                else
                    throw;
            }
        }
    }
}
