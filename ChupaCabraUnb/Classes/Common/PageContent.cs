using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UnBWebCrawler.Classes.Common
{
    public class PageContent
    {
        public PageContent(string url)
        {
            this.url = url;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            this._Content = Useful.getHtmlDoc(this.url);
        }

        private string _url;

        public string url
        {
            get { return _url; }
            set { _url = value; }
        }

        private HtmlDocument _Content;
        public HtmlDocument Content
        {
            get { return _Content; }
        }

        //private Task<HtmlDocument> _Content;
        //public Task<HtmlDocument> Content
        //{
        //    get { return _Content; }
        //}
    }
}
