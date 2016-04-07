
using System.Collections.Generic;
using System.Diagnostics;

namespace DevSprintReview.Entities {
    [DebuggerDisplay("{Email}")]
    public class Person {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Area { get; set; }
    }
}