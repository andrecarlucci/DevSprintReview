using System.Text.RegularExpressions;

namespace DevSprintReview {
    public static class FileToGrid {
        public static string[,] ToGrid(string file, int numCols) {
            var matches = Regex.Matches(file, "\\\"(.*?)\\\"", RegexOptions.Multiline);
            var total = matches.Count;
            var columns = numCols;
            var lines = total / columns;

            var grid = new string[lines - 1, columns-1];
            for (int i = 1; i < lines; i++) {
                for (int j = 1; j < columns; j++) {
                    var m = ((i) * columns) + (j);
                    grid[i - 1, j - 1] = matches[m].Groups[1].Value;
                }
            }
            return grid;
        }
    }
}