using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using UnBWebCrawler;

namespace VacantDisciplinesReader
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            UnBWebCrawler.executor access = new UnBWebCrawler.executor();

            var tasks = new List<Task>();
            tasks.Add(Task.Factory.StartNew(() => access.loadAsync()));
            tasks.Add(Task.Factory.StartNew(() => access.ConsolidateData()));
            Task.WaitAll(tasks.ToArray());
        }
    }
}
