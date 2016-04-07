using System.Diagnostics;

namespace DevSprintReview.Entities {
    [DebuggerDisplay("{Reviewed} : {Grade}")]
    public class PersonReview {
        public string Reviewed { get; set; }
        public int Grade { get; set; }

        public PersonReview(string reviewed, int grade) {
            Reviewed = reviewed;
            Grade = grade;
        }
    }
}