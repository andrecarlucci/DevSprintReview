using System.Collections.Generic;

namespace DevSprintReview.Entities {
    public class Sprint {
        public string Name { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}