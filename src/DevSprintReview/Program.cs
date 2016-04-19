using MiniBiggy;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DevSprintReview.Entities;
using DevSprintReview.Mails;

namespace DevSprintReview {
    public class Program {
        public static void Main(string[] args) {
            bool sendmail = false;
            
            var filename = args[0];
            var lines = File.ReadAllLines(filename);

            if (args.Length == 2) {
                sendmail = args[1] == "sendmail";
            }

            var pessoas = CreateList<Person>.UsingPath("people.js")
                                            .UsingPrettyJsonSerializer()
                                            .SavingWhenRequested();

            var sprints = CreateList<Sprint>.UsingPath("sprints.js")
                                            .UsingPrettyJsonSerializer()
                                            .SavingWhenRequested();

            var file = File.ReadAllText(filename);
            file = CommentFixer.RemoveNewLinesFromComments(file);
            var reviewed = ReviewedParser.Parse(file, pessoas);
            var grid = FileToGrid.ToGrid(file, reviewed.Count + 3);

            var reviews = ReviewsParser.Parse(grid, reviewed, pessoas);

            var sprintName = Path.GetFileNameWithoutExtension(filename);
            var sprint = sprints.FirstOrDefault(s => s.Name == sprintName);
            if (sprint == null) {
                sprint = new Sprint();
                sprints.Add(sprint);
            }
            sprint.Name = sprintName;
            sprint.Reviews = reviews;

            Console.WriteLine("Result: ----------------------------");

            var report = new Report(sprint, pessoas);
            report.Calcule();
            Console.WriteLine(report);
            sprints.Save();

            if (!sendmail) {
                return;
            }
            Console.WriteLine("Sending e-mails:");

            foreach (var person in reviewed) {
                var individualReport = report.GenerateIndividualReport(person);
                Console.Write($"{person.Name}: {person.Email} sending...");
                EmailSender.Send(person.Email, person.Name,
                    sprintName, 
                    String.Format(EmailTemplate, individualReport.Person.Name, individualReport.Report)
                );
                Console.WriteLine("OK!");
            }
        }

        public static string EmailTemplate = @"Olá {0},

Segue sua review.
As notas estão agrupadas por área (média simples de todas as pessoas da área), assim você pode ver como seu trabalho está sendo visto por cada uma delas.
A regra usada para as notas é a seguinte:

Deixou a desejar: 1
Poderia ter feito mais: 2
Boa atuação: 3
Excelente: 4

Suas notas por área (self é a sua própria avaliação):

{1}

Qualquer dúvida é só falar.

Atenciosamente,

André Carlucci
";
    }
}
