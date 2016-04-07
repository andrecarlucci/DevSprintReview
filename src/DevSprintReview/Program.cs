using MiniBiggy;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DevSprintReview.Entities;

namespace DevSprintReview {
    public class Program {
        public static void Main(string[] args) {
            var filename = args[0];
            var lines = File.ReadAllLines(filename);
            
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

            var report = new Report(sprint);
            report.Calcule();
            Console.WriteLine(report);
            sprints.Save();            
        }
    }
}
