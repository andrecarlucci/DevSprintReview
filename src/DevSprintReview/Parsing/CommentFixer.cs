using System;
using System.Linq;
using System.Collections.Generic;
using MiniBiggy;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DevSprintReview.Entities;

namespace DevSprintReview
{
    public static class CommentFixer {

        private static bool _inZone;
        private static StringBuilder _newFile = new StringBuilder();

        public static string RemoveNewLinesFromComments(string file) {
            _newFile.Clear();
            for (int i = 0; i < file.Length; i++) {
                if (InZoneDoubleQuotes(file, i)) {
                    i++;
                    continue;
                }
                if(InZoneProcessed(file[i])) continue;
                if(NewLineIgnored(file[i])) continue;
                ProcessOutZone(file[i]);
            }
            return _newFile.ToString();
        }

        private static bool InZoneDoubleQuotes(string file, int index) {
            if (!_inZone || file.Length-1 == index) {
                return false;
            }
            if (file[index] == '"' && file[index+1] == '"') {
                _newFile.Append('\'');
                return true;
            }
            return false;
        }

        private static bool InZoneProcessed(char c) {
            if (!_inZone && c == '"') {
                _newFile.Append(c);
                return _inZone = true;
            }
            return false;
        }

        private static bool NewLineIgnored(char c) {
            if (_inZone && (c == '\n' || c == '\r')) {
                return true;
            }
            _newFile.Append(c);
            return false;
        }
        private static void ProcessOutZone(char c) {
            if (_inZone && c == '"') {
                _inZone = false;
            }
        }
    }
}