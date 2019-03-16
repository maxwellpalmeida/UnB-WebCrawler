using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UnBWebCrawler.Classes.Models
{
    public class Departments : Common.PageContent
    {
        public Departments(string url) : base(url)
        { }

        public Task CompletedTask { get; set; }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private List<Disiplines> _Disciplines = new List<Disiplines>();
        public List<Disiplines> Disciplines
        {
            get { return _Disciplines; }
            set { _Disciplines = value; }
        }

        public async Task LoadDisciplinesAsync()
        {
            var tableDisciplines = (base.Content).DocumentNode.Descendants("table").Where(w => w.GetAttributeValue("id", "").Equals("datatable")).FirstOrDefault();
            if (tableDisciplines != null)
            {
                var trs = tableDisciplines.Descendants("tr");
                if (trs != null)
                {
                    foreach (var tr in trs)
                    {
                        if (tr.Descendants("th").Any()) // Ignore header
                            continue;

                        var a = tr.Descendants("td").FirstOrDefault(w => w.Descendants("a")?.Any() == true)?.Descendants("a")?.FirstOrDefault();
                        if (a != null)
                        {
                            string Codigo;
                            string conteudo = a.GetAttributeValue("href", "");

                            var rgx = new Regex(@"cod=\d+", RegexOptions.IgnoreCase);
                            var sub = rgx.Match(conteudo);
                            Codigo = sub.Groups[0].ToString().Replace("cod=", "");

                            this.Disciplines.Add(new Disiplines(url: "https://wwwsec.serverweb.unb.br/graduacao/" + conteudo)
                            {
                                Name = a.InnerText,
                                Code = Codigo,
                                Vacant = false
                            });

                            //this.Disciplines.Add(Task.Run<Disiplines>( () =>
                            //{
                            //    var disc = new Disiplines(url: "https://wwwsec.serverweb.unb.br/graduacao/" + conteudo)
                            //    {
                            //        Name = a.InnerText,
                            //        Code = Codigo,
                            //        Vacant = false
                            //    };

                            //     disc.LoadAsync();
                            //    return disc;
                            //}));

                            // this.Disciplines.Add(Task.Factory.StartNew<Disiplines>(() =>
                            //{
                            //    var disc = new Disiplines(url: "https://wwwsec.serverweb.unb.br/graduacao/" + conteudo)
                            //    {
                            //        Name = a.InnerText,
                            //        Code = Codigo,
                            //        Vacant = false
                            //    };

                            //    disc.LoadAsync();
                            //    return disc;
                            //}));
                        }
                    }
                }
            }



            //this.Disciplines.Clear();
            //this.Disciplines.Add(new Disiplines(url: "https://wwwsec.serverweb.unb.br/graduacao/oferta_dados.aspx?cod=205702&dep=383") { Name = "teste", Code = "205702" });

            foreach (var disciplina in this.Disciplines)
            {
                await disciplina.LoadAsync();
            }
        }
    }
}
