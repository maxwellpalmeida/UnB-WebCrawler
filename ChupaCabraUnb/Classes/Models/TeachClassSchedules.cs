using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnBWebCrawler
{
    public class TeachClassSchedules
    {
        private string _Class;
        public string Class
        {
            get { return _Class; }
            set { _Class = value; }
        }

        private string _WeekDay;
        public string WeekDay
        {
            get { return _WeekDay; }
            set { _WeekDay = value; }
        }

        private string _StartTime;
        public string StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }

        private string _EndTime;
        public string EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
    }
}
