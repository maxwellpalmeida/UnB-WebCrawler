using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace UnBWebCrawler
{
    public static class Useful
    {
        public enum eShift
        {
            Manha = 1,
            Tarde = 2,
            Noite = 3,
            Misto = 4
        }

        public static string getBoolTranscript(bool boolean)
        {
            return (boolean ? "Sim" : "Não");
        }

        public static HtmlDocument getHtmlWeb(string url)
        {
            HtmlDocument retorno = new HtmlDocument();
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (i > 0)
                    {
                        Console.WriteLine(string.Join("Url {0} inacessível. Nova tentativa em 5 segundos", url));
                        Thread.Sleep(5000);
                    }

                    var web = new HtmlWeb();
                    var document = web.Load(url);
                    return document;
                }
                catch (Exception e)
                { }
            }

            return retorno;
        }

        public static string getWebClient(string url)
        {
            string retorno = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (i > 0)
                    {
                        Console.WriteLine(string.Join("Url {0} inacessível. Nova tentativa em 5 segundos", url));
                        Thread.Sleep(5000);
                    }

                    System.Net.WebClient wc = new System.Net.WebClient();
                    byte[] raw = wc.DownloadData(url);
                    return System.Text.Encoding.UTF8.GetString(raw);
                }
                catch (Exception e)
                { }
            }

            return retorno;
        }
    }
}
