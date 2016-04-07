using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DevSprintReview.Entities;

namespace DevSprintReview {
    public static class ReviewedParser {

        public static List<Person> Parse(string firstLine, ICollection<Person> pessoas) {
            var reviewed = new List<Person>();
            var columns = firstLine.Split(',');
            for (int i = 2; i < 2 + pessoas.Count; i++) {
                var name = ParseReviewed(columns[i]);
                if (name == null) {
                    break;
                }
                var person = pessoas.FirstOrDefault(p => p.Name.ToUpper() == name.ToUpper());
                reviewed.Add(person);
            }
            return reviewed;
        }

        private static string ParseReviewed(string label) {
            var match = Regex.Match(label, @"\[(.+)\]");
            if (match.Groups.Count < 2) {
                return null;
            }
            return match.Groups[1].Value;
        }
    }
}