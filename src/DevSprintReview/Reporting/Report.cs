using System.Linq;
using System.Collections.Generic;
using System.Text;
using System;
using DevSprintReview.Entities;
using MiniBiggy;

namespace DevSprintReview {
    public class Report {
        private Sprint _sprint;
        private Dictionary<string, Person> _reviewers;
        private HashSet<string> _areas;
        private List<Person> _reviewed;
        private PersistentList<Person> _pessoas;
        
        public Report(Sprint sprint, PersistentList<Person> pessoas) {
            _sprint = sprint;
            _pessoas = pessoas;
        }

        public void Calcule() {
            _areas = new HashSet<string>(_sprint.Reviews.Select(r => r.Reviewer.Area).Distinct());
            _reviewers = _sprint.Reviews.ToDictionary(k => k.Reviewer.Email, v => v.Reviewer);
            var reviewedNames = _sprint.Reviews.SelectMany(r => r.PersonReviews).Select(pr => pr.Reviewed).Distinct().ToList();
            _reviewed = reviewedNames.Select(n => _pessoas.First(p => p.Name == n)).ToList();
        }

        public IndividualReport GenerateIndividualReport(Person person) {
            var sb = new StringBuilder();
            var reviewedName = person.Name;
            sb.AppendLine($"{reviewedName,-20} Area: {"Self",-15} \t Nota: {Format(CalculeNotaSelf(reviewedName))}");
            foreach (var area in _areas) {
                var nota = CalculeNotaPorDevPorArea(person, area);
                sb.AppendLine($"{reviewedName,-20} Area: {area,-15} \t Nota: {Format(CalculeNotaPorDevPorArea(person, area))}");
            }
            return new IndividualReport { Person = person, Report = sb.ToString() };
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
                sb.AppendLine(GenerateIndividualReport(reviewed).Report);
                sb.AppendLine("----------------------------------------------------------");
            }
            //sb.AppendLine($"{"Time",-20} Area: {"Self",-15} \t Nota: {Format(CalculeNotaTimeSelf())}");
            foreach (var area in _areas) {
                sb.AppendLine($"{"Time",-20} Area: {area,-15} \t Nota: {Format(CalculeNotaTimePorArea(area))}");
            }
            sb.AppendLine($"{"Time",-20} Area: {"Todas",-15} \t Nota: {Format(CalculeNotaTime())}");
            sb.AppendLine("----------------------------------------------------------");
            sb.AppendLine("Comentarios");

            sb.AppendLine("----------------------------------------------------------");
            foreach (var review in _sprint.Reviews) {
                sb.AppendLine($"{review.Reviewer.Name}: {review.Comment}");
            }
            return sb.ToString();
        }

        private double CalculeNotaTimeSelf() {
            var notas = _reviewed.Select(r => CalculeNotaSelf(r.Name)).Where(n => n > 0);
            return (double) notas.Sum() / notas.Count();
        }

        private string Format(double nota) {
            if (nota < 0) {
                return "N/A";
            }
            return $"{nota:#.0}";
        }

        private int CalculeNotaSelf(string reviewed) {
            var selfReview = _sprint.Reviews.FirstOrDefault(r => r.Reviewer.Name == reviewed);
            if (selfReview == null) {
                return -1;
            }
            return selfReview.PersonReviews.FirstOrDefault(pr => pr.Reviewed == reviewed).Grade;
        }

        private double CalculeNotaPorDevPorArea(Person reviewed, string area) {
            var notas = _sprint.Reviews.Where(r => r.Reviewer.Area == area)
                                       .SelectMany(r => r.PersonReviews.Where(pr => pr.Reviewed == reviewed.Name && pr.Grade > 0))
                                       .Select(pr => pr.Grade)
                                       .ToList();
            if (reviewed.Area == area) {
                var self = CalculeNotaSelf(reviewed.Name);
                if (self > 0) {
                    notas.Remove(self);
                }
            }
            var tam = notas.Count;
            if (tam == 0) {
                return -1;
            }
            return (double)notas.Sum() / tam;
        }

        private double CalculeNotaTimePorArea(string area) {
            var values = _sprint.Reviews.Where(r => r.Reviewer.Area == area)
                                .SelectMany(e => e.PersonReviews.Where(pr => pr.Grade > 0).Select(pr => pr.Grade))
                                .ToList();
            return (double)values.Sum() / values.Count();
        }

        private double CalculeNotaTime() {
            var prs = _sprint.Reviews.SelectMany(r => r.PersonReviews).Where(pr => pr.Grade > 0);
            var items = prs.Count();
            var total = prs.Sum(e => e.Grade);
            return ((double) total) / items;
        }
    }

}