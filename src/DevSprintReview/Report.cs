using System.Linq;
using System.Collections.Generic;
using System.Text;
using System;
using DevSprintReview.Entities;

namespace DevSprintReview {
    public class Report {
        private Sprint _sprint;
        private Dictionary<string, Person> _reviewers;
        private HashSet<string> _areas;
        private List<string> _reviewed;

        public Report(Sprint sprint) {
            _sprint = sprint;
        }

        public void Calcule() {
            _areas = new HashSet<string>(_sprint.Reviews.Select(r => r.Reviewer.Area).Distinct());
            _reviewers = _sprint.Reviews.ToDictionary(k => k.Reviewer.Email, v => v.Reviewer);
            _reviewed = _sprint.Reviews.SelectMany(r => r.PersonReviews).Select(pr => pr.Reviewed).Distinct().ToList();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine("Reviewers");
            sb.AppendLine("----------------------------------------------------------");
            foreach (var reviewer in _reviewers.Values.OrderBy(x => x.Area).ThenBy(x => x.Name)) {
                sb.AppendLine($"{reviewer.Name,-20} Area: {reviewer.Area,-15} Email: {reviewer.Email}");
            }
            sb.AppendLine("----------------------------------------------------------");
            foreach (var reviewer in _reviewers.Values.OrderBy(x => x.Area).ThenBy(x => x.Name)) {
                sb.Append(reviewer.Email + ";");
            }
            sb.AppendLine();
            sb.AppendLine("----------------------------------------------------------");
            foreach (var reviewed in _reviewed) {
                sb.AppendLine($"{reviewed,-20} Area: {"Self",-15} \t Nota: {CalculeNotaSelf(reviewed):#.0}");
                foreach (var area in _areas) {
                    var nota = CalculeNotaPorDevPorArea(reviewed, area);
                    if (nota < 0) {
                        sb.AppendLine($"{reviewed,-20} Area: {area,-15} \t Nota: N/A");
                    }
                    else {
                        sb.AppendLine($"{reviewed,-20} Area: {area,-15} \t Nota: {CalculeNotaPorDevPorArea(reviewed, area):#.0}");
                    }
                }
                sb.AppendLine("----------------------------------------------------------");
            }
            foreach (var area in _areas) {
                sb.AppendLine($"{"Time",-20} Area: {area,-15} \t Nota: {CalculePorArea(area):#.0}");
            }
            sb.AppendLine("----------------------------------------------------------");
            sb.AppendLine($"{"Time",-20} Area: {"Todas",-15} \t Nota: {CalculeNotaTime():#.0}");
            sb.AppendLine("----------------------------------------------------------");
            sb.AppendLine("Comentarios");

            sb.AppendLine("----------------------------------------------------------");
            foreach (var review in _sprint.Reviews) {
                sb.AppendLine($"{review.Reviewer.Name}: {review.Comment}");
            }
            return sb.ToString();
        }

        private double CalculeNotaSelf(string reviewed) {
            return _sprint.Reviews.FirstOrDefault(r => r.Reviewer.Name == reviewed)
                                       .PersonReviews.FirstOrDefault(pr => pr.Reviewed == reviewed)
                                       .Grade;
        }

        private double CalculeNotaPorDevPorArea(string reviewed, string area) {
            var notas = _sprint.Reviews.Where(r => r.Reviewer.Area == area)
                                       .SelectMany(r => r.PersonReviews.Where(pr => pr.Reviewed == reviewed && pr.Grade > 0))
                                       .Select(pr => pr.Grade)
                                       .ToList();
            
            var tam = notas.Count;
            if (tam == 0) {
                return -1;
            }
            return (double)notas.Sum() / tam;
        }

        private double CalculePorArea(string area) {
            var values = _sprint.Reviews.Where(r => r.Reviewer.Area == area)
                                .SelectMany(e => e.PersonReviews.Where(pr => pr.Grade > 0).Select(pr => pr.Grade))
                                .ToList();
            return (double)values.Sum() / values.Count();
        }

        private double CalculeNotaTime() {
            var prs = _sprint.Reviews.SelectMany(r => r.PersonReviews).Where(pr => pr.Grade > 0);
            var items = prs.Count();
            var total = prs.Sum(e => e.Grade);
            return (double) total / items;
        }
    }
}