
namespace DevSprintReview {
    public static class GradeParser {
        public static int Parse(string label) {
            if (label == "Deixou a desejar") return 1;
            if (label == "Poderia ter feito mais") return 2;
            if (label == "Boa atuação") return 3;
            if (label == "Excelente") return 4;
            return -1;
        }
    }
}