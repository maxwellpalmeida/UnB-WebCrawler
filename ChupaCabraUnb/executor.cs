using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Net.Http;
using System.Net;
using UnBWebCrawler.Classes.Models;
using System.Threading;

namespace UnBWebCrawler
{
    public class executor : Classes.Common.PageContent
    {
        public int ContadorDepartamentosConcluidos { get; set; }

        public executor() : base("https://wwwsec.serverweb.unb.br/graduacao/oferta_dep.aspx?cod=1")
        { }

        public Task<int> DepartmentsQuantity { get; set; }

        private List<Task<Departments>> _Departments = new List<Task<Departments>>();
        public List<Task<Departments>> Departments
        {
            get { return _Departments; }
            set { _Departments = value; }
        }

        public static Task<Task<T>>[] Interleaved<T>(IEnumerable<Task<T>> tasks)
        {
            var inputTasks = tasks.ToList();

            var buckets = new TaskCompletionSource<Task<T>>[inputTasks.Count];
            var results = new Task<Task<T>>[buckets.Length];
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new TaskCompletionSource<Task<T>>();
                results[i] = buckets[i].Task;
            }

            int nextTaskIndex = -1;
            Action<Task<T>> continuation = completed =>
            {
                var bucket = buckets[Interlocked.Increment(ref nextTaskIndex)];
                bucket.TrySetResult(completed);
            };

            foreach (var inputTask in inputTasks)
                inputTask.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

            return results;
        }

        private async Task<Departments> PrepareDepartmentTask(HtmlNode a)
        {
            var depart = new Classes.Models.Departments(url: "https://wwwsec.serverweb.unb.br/graduacao/" + a.GetAttributeValue("href", ""))
            {
                Name = a.InnerText,
            };

            await depart.LoadDisciplinesAsync();

            this.ContadorDepartamentosConcluidos++;
            Console.WriteLine(string.Format("Departamentos analisados: {0}", this.ContadorDepartamentosConcluidos));

            return depart;
        }

        public async Task loadAsync()
        {
            var tableDepartments = (base.Content).DocumentNode.Descendants("table").Where(w => w.GetAttributeValue("id", "").Equals("datatable")).FirstOrDefault();
            if (tableDepartments != null)
            {
                var trs = tableDepartments.Descendants("tbody")?.FirstOrDefault()?.Descendants("tr");
                this.DepartmentsQuantity = Task.Factory.StartNew<int>(() => { return trs.Count(); });
                foreach (var tr in trs)
                {
                    var a = tr.Descendants("td").FirstOrDefault(w => w.Descendants("a")?.Any() == true)?.Descendants("a")?.FirstOrDefault();
                    if (a != null)
                    {
                        this.Departments.Add(this.PrepareDepartmentTask(a));
                    }
                }
            }

            this.Departments.ForEach(w => w.Start());
        }

        public async void ConsolidateData()
        {
            var excelApp = new Excel.Application();
            excelApp.Visible = true;
            excelApp.Workbooks.Add();
            Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;

            workSheet.Cells[1, "A"] = "Departamento";
            workSheet.Cells[1, "B"] = "Disciplina";
            workSheet.Cells[1, "C"] = "Créditos";
            workSheet.Cells[1, "D"] = "Pre-requisito(s)";
            workSheet.Cells[1, "E"] = "Turma";
            workSheet.Cells[1, "F"] = "Vagas";
            workSheet.Cells[1, "G"] = "Vagas Ocupadas";
            workSheet.Cells[1, "H"] = "Vagas Restantes";
            workSheet.Cells[1, "I"] = "Turno Disciplina";
            workSheet.Cells[1, "J"] = "Turno Aulas";
            workSheet.Cells[1, "K"] = "Primeiro Horário";
            workSheet.Cells[1, "L"] = "Horários completo";
            workSheet.Cells[1, "M"] = "Local";
            workSheet.Cells[1, "N"] = "Professor";
            workSheet.Cells[1, "O"] = "Observação";
            workSheet.Cells[1, "P"] = "Com vaga";
            workSheet.Cells[1, "Q"] = "Segunda";
            workSheet.Cells[1, "R"] = "Terça";
            workSheet.Cells[1, "S"] = "Quarta";
            workSheet.Cells[1, "T"] = "Quinta";
            workSheet.Cells[1, "U"] = "Sexta";
            workSheet.Cells[1, "V"] = "Sábado";
            workSheet.Cells[1, "W"] = "Domingo";
            workSheet.Cells[1, "X"] = "URL de acesso a Disciplina";

            while (this.Departments.Count <= (await this.DepartmentsQuantity))
            {
                int aux = 2;
                foreach (var bucket in Interleaved(this.Departments))
                {
                    var departmentTask = await bucket;
                    var department = await departmentTask;

                    int AuxDisc = 0;
                    department.CompletedTask = null;
                    foreach (var discipline in department.Disciplines)
                    {
                        foreach (var teachClass in discipline.TeachClasses)
                        {
                            workSheet.Cells[aux, "A"] = department.Name;
                            workSheet.Cells[aux, "B"] = discipline.Name;
                            workSheet.Cells[aux, "C"] = discipline.Credits;
                            workSheet.Cells[aux, "D"] = discipline.Requirements;
                            workSheet.Cells[aux, "E"] = teachClass.Name;
                            workSheet.Cells[aux, "F"] = teachClass.TotalVacancy;
                            workSheet.Cells[aux, "G"] = teachClass.FilledVacancy;
                            workSheet.Cells[aux, "H"] = teachClass.LeftVacancy;
                            workSheet.Cells[aux, "I"] = teachClass.Shift;
                            workSheet.Cells[aux, "J"] = teachClass.getTurnoAulas();
                            workSheet.Cells[aux, "K"] = teachClass.getPrimeiroHorario();
                            workSheet.Cells[aux, "L"] = teachClass.getDemaisHorario();
                            workSheet.Cells[aux, "M"] = teachClass.getLocais();
                            workSheet.Cells[aux, "N"] = teachClass.Professor;
                            workSheet.Cells[aux, "O"] = teachClass.Remark;
                            workSheet.Cells[aux, "P"] = Useful.getBoolTranscript(discipline.Vacant);
                            workSheet.Cells[aux, "Q"] = Useful.getBoolTranscript(teachClass.Monday);
                            workSheet.Cells[aux, "R"] = Useful.getBoolTranscript(teachClass.Tuesday);
                            workSheet.Cells[aux, "S"] = Useful.getBoolTranscript(teachClass.Wednesday);
                            workSheet.Cells[aux, "T"] = Useful.getBoolTranscript(teachClass.Thursday);
                            workSheet.Cells[aux, "U"] = Useful.getBoolTranscript(teachClass.Friday);
                            workSheet.Cells[aux, "V"] = Useful.getBoolTranscript(teachClass.Saturday);
                            workSheet.Cells[aux, "W"] = Useful.getBoolTranscript(teachClass.Sunday);

                            Excel.Range rangeToHoldHyperlink = workSheet.get_Range(string.Format("X{0}", aux), Type.Missing);
                            workSheet.Hyperlinks.Add(rangeToHoldHyperlink, discipline.url);

                            workSheet.Columns.AutoFit();
                            workSheet.Rows.AutoFit();

                            aux++;

                            //Console.WriteLine($"{AuxTeachClass++} de {discipline.TeachClasses.Count}");
                        }

                        Console.WriteLine($"Departamento {department.Name} Disciplinas: {AuxDisc++} de {department.Disciplines.Count}");
                    }
                    //  Console.WriteLine($"{this.Departments.Count(w => w.CompletedTask == null)}/{this.Departments.Count} departamentos lançados no excel.");
                }
                Console.WriteLine(aux + " turmas localizadas e populadas no arquivo excel.");
            }
        }

        public void ConsolidarDadosold()
        {
            Console.Clear();
            Console.WriteLine("Disciplinas com vagas: ");

            //if (!File.Exists(@"C:\Users\fujioka\Desktop\DisciplinasComVaga.txt"))
            //{
            //    File.Create(@"C:\Users\fujioka\Desktop\DisciplinasComVaga.txt");
            //}

            using (StreamWriter file = new StreamWriter(@"C:\Users\fujioka\Desktop\DisciplinasComVaga.txt"))
            {
                //this.Departments.Where(w => w.Disciplines.Any(d => d.Vacant)).ToList().ForEach(departamento =>
                //{
                //    Console.WriteLine("Departamento: " + departamento.Name);
                //    Console.WriteLine("Disciplinas: ");

                //    file.WriteLine("Departamento: " + departamento.Name);
                //    file.WriteLine("Disciplinas: ");

                //    departamento.Disciplines.ForEach(disciplina =>
                //        {
                //            if (disciplina.Vacant)
                //            {
                //                Console.WriteLine(string.Format("   {0} - {1}", disciplina.Code, disciplina.Name));
                //                file.WriteLine(string.Format("   {0} - {1}", disciplina.Code, disciplina.Name));
                //            }
                //        });

                //    Console.WriteLine("   ");
                //    Console.WriteLine("   ");

                //    file.WriteLine("   ");
                //    file.WriteLine("   ");
                //});
            }

            Console.WriteLine(@"Arquivo com as disciplinas gerado automaticamente no diretório 'C:\Users\fujioka\Desktop\DisciplinasComVaga.txt'");
        }
    }
}
