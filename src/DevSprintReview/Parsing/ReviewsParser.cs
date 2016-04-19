using System.Linq;
using System.Collections.Generic;
using MiniBiggy;
using DevSprintReview.Entities;
using System;

namespace DevSprintReview {
    public static class ReviewsParser {
        public static List<Review> Parse(string[,] grid, List<Person> reviewed, PersistentList<Person> pessoas) {
            var reviews = new List<Review>();
            for (int i = 0; i < grid.GetLength(0); i++) {
                var reviewer = pessoas.FirstOrDefault(p => p.Email.ToUpper() == grid[i, 0].ToUpper());
                if (reviewer == null) {
                    throw new Exception("Não reconheço o e-mail " + grid[i, 0]);
                }
                var r1 = reviews.FirstOrDefault(r => r.Reviewer == reviewer);
                if (r1 != null) {
                    reviews.Remove(r1);
                }
                var review = new Review(reviewer);
                for (int j = 0; j < reviewed.Count; j++) {
                    var grade = GradeParser.Parse(grid[i, (j * 2) + 1]);
                    var comment = grid[i, (j * 2) + 2];
                    if (grade < 3 && String.IsNullOrEmpty(comment)) {
                        grade = -1;
                    }
                    review.AddPersonReview(reviewed[j].Name, grade, comment);
                }
                review.Comment = grid[i, grid.GetLength(1) - 1];
                reviews.Add(review);
            }
            return reviews;
        }
    }
}