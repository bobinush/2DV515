using System;
using System.Collections.Generic;
using System.Linq;

namespace webapi.Models
{
    public class CentroidViewModel
    {
        public CentroidViewModel()
        {
            Words = new List<Word>();
            Blogs = new List<Blog>();
        }
        public string Name { get; set; }
        public List<Word> Words { get; set; }
        public int NumberOfBlogs { get { return Blogs.Count; } }
        public List<Blog> Blogs { get; set; }
        public List<string> PreviousBlogs { get; set; }

        public void ClearAssignments()
        {
            PreviousBlogs = Blogs.Select(x => x.Name).ToList();
            Blogs.Clear();
        }

        public double Pearson(Blog b)
        {
            double sum1 = 0, sum2 = 0, sum1sq = 0, sum2sq = 0, psum = 0;
            int n = 0;
            foreach (var cWord in Words)
            {
                double d = b.Words.First(x => x.Key == cWord.Key).Value;
                // foreach (var bWord in b.Words)
                // {
                // if (cWord.Key == bWord.Key)
                // {
                sum1 += cWord.Value;
                sum2 += d;
                sum1sq += cWord.Value * cWord.Value;
                sum2sq += d * d;
                psum += cWord.Value * d;
                n++;
                // }
                // }
            }
            if (n == 0)
            {
                return 0;
            }
            double num = psum - ((sum1 * sum2) / n);
            double den = Math.Sqrt((sum1sq - Math.Pow(sum1, 2.0) / n) * (sum2sq - Math.Pow(sum2, 2.0) / n));
            return 1.0 - num / den;
        }

        internal bool IsClusterSameAsPrevious()
        {
            return !Blogs.Select(x => x.Name).Except(PreviousBlogs).Any() && !PreviousBlogs.Except(Blogs.Select(x => x.Name)).Any();
        }
    }
}