﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnBWebCrawler.Classes.Models
{
    public class TeachClass
    {
        public TeachClass()
        {
            this.Schedules = new List<TeachClassSchedules>();
        }

        private int _Restantes;
        public int Restantes
        {
            get { return _Restantes; }
            set { _Restantes = value; }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _TotalVacancy;
        public string TotalVacancy
        {
            get { return _TotalVacancy; }
            set { _TotalVacancy = value; }
        }

        private string _LeftVacancy;
        public string LeftVacancy
        {
            get { return _LeftVacancy; }
            set { _LeftVacancy = value; }
        }

        private string _FilledVacancy;
        public string FilledVacancy
        {
            get { return _FilledVacancy; }
            set { _FilledVacancy = value; }
        }

        private string _Shift;
        public string Shift
        {
            get { return _Shift; }
            set { _Shift = value; }
        }

        private string _Professor;
        public string Professor
        {
            get { return _Professor; }
            set { _Professor = value; }
        }

        private string _Remark;
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }

        private List<TeachClassSchedules> _Schedules;
        public List<TeachClassSchedules> Schedules
        {
            get { return _Schedules; }
            set { _Schedules = value; }
        }

        private bool _Monday;
        public bool Monday
        {
            get { return _Monday; }
            set { _Monday = value; }
        }

        private bool _Tuesday;
        public bool Tuesday
        {
            get { return _Tuesday; }
            set { _Tuesday = value; }
        }

        private bool _Wednesday;
        public bool Wednesday
        {
            get { return _Wednesday; }
            set { _Wednesday = value; }
        }

        private bool _Thursday;
        public bool Thursday
        {
            get { return _Thursday; }
            set { _Thursday = value; }
        }

        private bool _Friday;
        public bool Friday
        {
            get { return _Friday; }
            set { _Friday = value; }
        }

        private bool _Saturday;
        public bool Saturday
        {
            get { return _Saturday; }
            set { _Saturday = value; }
        }

        private bool _Sunday;
        public bool Sunday
        {
            get { return _Sunday; }
            set { _Sunday = value; }
        }

        public string getClassShift()
        {
            List<Useful.eShift> shifts = new List<Useful.eShift>();
            foreach (var schedule in this.Schedules)
            {
                switch (int.Parse(schedule.StartTime.Substring(0, 2)))
                {
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        shifts.Add(Useful.eShift.Manha);
                        break;

                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                        shifts.Add(Useful.eShift.Tarde);
                        break;

                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 0:
                        shifts.Add(Useful.eShift.Noite);
                        break;
                }
            }

            if (shifts.Distinct().Count() > 1)
                return Useful.eShift.Misto.ToString();
            else
                return shifts.FirstOrDefault().ToString();
        }

        public void AddSchedule(TeachClassSchedules schedule)
        {
            this.Schedules.Add(schedule);

            switch (schedule.WeekDay.ToLower())
            {
                case "segunda":
                    this.Monday = true;
                    break;

                case "terça":
                    this.Tuesday = true;
                    break;

                case "quarta":
                    this.Wednesday = true;
                    break;

                case "quinta":
                    this.Thursday = true;
                    break;

                case "sexta":
                    this.Friday = true;
                    break;

                case "sabado":
                    this.Saturday = true;
                    break;

                case "domingo":
                    this.Sunday = true;
                    break;
            }
        }

        public string getLocations()
        {
            List<string> Locations = new List<string>();
            foreach (var schedule in this.Schedules)
            {
                Locations.Add(schedule.Class);
            }

            return string.Join("   //   ", Locations);
        }

        public string getFirstSchedule()
        {
            var firtsSchedule = this.Schedules.FirstOrDefault();
            if (firtsSchedule != null)
            {
                return firtsSchedule.StartTime;
            }
            else
            {
                return "";
            }
        }

        public string getAllOthersSchedules()
        {
            List<string> schedules = new List<string>();
            int aux = 0;
            bool retryIncludeWeek = false;
            foreach (var schedule in this.Schedules)
            {
                if (schedules.Count == 0)
                {
                    schedules.Add(string.Format("{0} a {1}", schedule.StartTime, schedule.EndTime));
                }
                else
                {
                    if (!schedules.Contains(string.Format("{0} a {1}", schedule.StartTime, schedule.EndTime)))
                    {
                        retryIncludeWeek = true;
                        break;
                    }
                }

                aux++;
            }

            if (retryIncludeWeek)
            {
                schedules.Clear();
                aux = 0;
                foreach (var schedule in this.Schedules)
                {
                    schedules.Add(string.Format("{0}:{1} a {2}. ", schedule.WeekDay, schedule.StartTime, schedule.EndTime));
                    aux++;
                }
            }

            return string.Join("   //   ", schedules);
        }

        public string getWeekDays()
        {
            List<string> diasSemana = new List<string>();
            foreach (var horario in this.Schedules)
            {
                diasSemana.Add(horario.WeekDay);
            }

            return string.Join(", ", diasSemana);
        }
    }
}
