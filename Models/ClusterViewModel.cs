using System;
using System.Collections.Generic;
using System.Linq;

namespace mvc.Models
{
    public class ClusterViewModel
    {
        public ClusterViewModel()
        {
            Result = new List<ClusterList>();
        }
        public int IterationsDone { get; set; }
        public List<ClusterList> Result { get; set; } // List instead of ICollection so we can do .IndexOf() in the view.
    }

    public class ClusterList
    {
        public ClusterList()
        {
            Blogs = new List<string>();
        }
        public string Name { get; set; }
        public int NumberOfBlogs { get; set; }
        public ICollection<string> Blogs { get; set; }
    }
}