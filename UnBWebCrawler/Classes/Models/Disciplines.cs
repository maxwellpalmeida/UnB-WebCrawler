using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UnBWebCrawler.Classes.Models
{
    public class Disiplines : Common.PageContent
    {
        public Disiplines(string url) : base(url)
        {
            this.TeachClasses = new List<TeachClass>();
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Code;
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        private bool _Vacant;
        public bool Vacant
        {
            get { return _Vacant; }
            set { _Vacant = value; }
        }

        private string _Credits;
        public string Credits
        {
            get { return _Credits; }
            set { _Credits = value; }
        }

        private string _Requirements;
        public string Requirements
        {
            get { return _Requirements; }
            set { _Requirements = value; }
        }

        private List<TeachClass> _TeachClasses;
        public List<TeachClass> TeachClasses
        {
            get { return _TeachClasses; }
            set { _TeachClasses = value; }
        }

        public async Task LoadAsync()
        {
            var Document = base.Content;

            Regex rgx = new Regex(@"Alunos não atendidos por falta de vagas", RegexOptions.IgnoreCase);
            var matches = rgx.Matches(Document.DocumentNode.InnerHtml);
            if (matches.Count > 0)
            {
                this.Vacant = false;
            }
            else
            {
                this.Vacant = true;
            }

            var divTablesResponsive = Document.DocumentNode.Descendants("div").Where(w => w.GetAttributeValue("class", "").Equals("table-responsive") && !string.IsNullOrWhiteSpace(w.GetAttributeValue("style", "")));
            foreach (var divTable in divTablesResponsive)
            {
                var tableDisipline = divTable.Descendants("table").Where(w => w.GetAttributeValue("class", "").Equals("table table-striped table-bordered tabela-oferta"))?.FirstOrDefault();
                //?.Descendants("tbody")?.FirstOrDefault();
                if (tableDisipline != null)
                {
                    this.Credits = Document.DocumentNode.Descendants("div").Where(w => w.GetAttributeValue("class", "").Equals("table-responsive") && string.IsNullOrWhiteSpace(w.GetAttributeValue("style", "")))?.FirstOrDefault()?.FirstChild?.LastChild?.LastChild?.InnerText ?? "";

                    var teachClass = new TeachClass();
                    this.TeachClasses.Add(teachClass);

                    int aux = 0;
                    foreach (var tbodyClass in tableDisipline.LastChild.ChildNodes)
                    {
                        aux++;
                        if (aux == 1)
                        {
                            teachClass.Name = tbodyClass.Descendants("td")?.ToList()[0]?.InnerText ?? "";
                            continue;
                        }

                        if (aux == 2)
                        {
                            var tableVacancies = tbodyClass.Descendants("table")?.FirstOrDefault();
                            if (tableVacancies != null)
                            {
                                foreach (var trVancacies in tableVacancies.Descendants("tr"))
                                {
                                    var qualifier = trVancacies.Descendants("td")?.ToList()[1]?.InnerText ?? "";
                                    var value = trVancacies.Descendants("td")?.LastOrDefault()?.InnerText ?? "";
                                    switch (qualifier)
                                    {
                                        case "Vagas":
                                            teachClass.TotalVacancy = value;
                                            break;
                                        case "Ocupadas":
                                            teachClass.FilledVacancy = value;
                                            break;
                                        case "Restantes":
                                            teachClass.LeftVacancy = value;
                                            break;
                                    }
                                }
                            }
                            continue;
                        }

                        if (aux == 3)
                        {
                            teachClass.Shift = tbodyClass.Descendants("td").FirstOrDefault(w => w.GetAttributeValue("colspan", "").Any())?.InnerText ?? "";
                            continue;
                        }

                        if (aux == 4)
                        {
                            var tablesSchedule = tbodyClass.Descendants("table");
                            if (tablesSchedule != null)
                            {
                                foreach (var table in tablesSchedule)
                                {
                                    if (table.Descendants("td").Any(w => !string.IsNullOrWhiteSpace(w.GetAttributeValue("style", ""))))
                                    {
                                        var schedule = new TeachClassSchedules();

                                        int auxTr = 0;
                                        foreach (var tdTable in table.Descendants("td"))
                                        {
                                            auxTr++;

                                            if (auxTr == 1)
                                            {
                                                schedule.WeekDay = tdTable.InnerText ?? "";
                                            }

                                            if (auxTr == 2)
                                            {
                                                schedule.StartTime = tdTable.InnerText ?? "";
                                            }

                                            if (auxTr == 3)
                                            {
                                                schedule.EndTime = tdTable.InnerText ?? "";
                                            }

                                            if (auxTr == 5)
                                            {
                                                schedule.Class = tdTable.InnerText ?? "";
                                            }
                                        }
                                        teachClass.AddSchedule(schedule);
                                    }
                                    else
                                        teachClass.Professor = table.InnerText;
                                }
                            }
                            continue;
                        }

                        if (aux == 5)
                        {
                            teachClass.Professor = tbodyClass.Descendants("td")?.FirstOrDefault()?.InnerText ?? "";
                            continue;
                        }

                        if (aux == 6)
                        {
                            teachClass.Remark = tbodyClass.Descendants("img")?.FirstOrDefault()?.GetAttributeValue("alt", "") ?? "";
                            continue;
                        }
                    }

                    this.LoadRequirements();
                }
            }
        }

        private void LoadRequirements()
        {
            var document = Common.Useful.getHtmlDoc("https://wwwsec.serverweb.unb.br/graduacao/disciplina.aspx?cod=" + this.Code);
            var page = document.DocumentNode;

            this.Requirements = string.Join(", ", document.DocumentNode.Descendants("table").Where(w => w.GetAttributeValue("id", "").Equals("datatable"))?.FirstOrDefault()
                ?.Descendants("tr")?.ElementAt(5)?.Descendants("a")?.Select(w => w.InnerText));
        }
    }

}
