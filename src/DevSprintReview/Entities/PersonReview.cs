using System.Diagnostics;

namespace DevSprintReview.Entities {
    [DebuggerDisplay("{Reviewed} : {Grade}")]
    public class PersonReview {
        public string Reviewed { get; set; }
        public int Grade { get; set; }
        public string Comment { get; set; }

        public PersonReview(string reviewed, int grade, string comment) {
            Reviewed = reviewed;
            Grade = grade;
            Comment = comment;
        }
    }
}