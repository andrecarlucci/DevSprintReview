
using System;
using static System.String;

namespace DevSprintReview {
    public static class GradeParser {
        public static int Parse(string label) {
            if (IsNullOrEmpty(label)) {
                return -1;
            }
            return Int32.Parse(label);
        }
    }
}