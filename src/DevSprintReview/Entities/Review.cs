using System.Collections.Generic;
using System.Diagnostics;

namespace DevSprintReview.Entities {
    [DebuggerDisplay("Review: {Reviewer.Name}")]
    public class Review {
        public Person Reviewer { get; set; }
        public List<PersonReview> PersonReviews { get; set; } = new List<PersonReview>();
        public string Comment { get; set; }

        public Review() {

        }

        public Review(Person reviewer) {
            Reviewer = reviewer;
        }

        public void AddPersonReview(string reviewed, int grade) {
            PersonReviews.Add(new PersonReview(reviewed, grade));
        }
    }
}