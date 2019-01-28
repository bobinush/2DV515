using System;
using System.Collections.Generic;

namespace mvc.Models
{
    public class UserP
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<TagP> Tags { get; set; }
        public ICollection<RatingP> Ratings { get; set; }

        public override bool Equals(Object obj)
        {
            UserP g = obj as UserP;
            return g != null && g.Id == this.Id;
        }

        public override int GetHashCode()
        {
            // https://docs.microsoft.com/en-us/visualstudio/ide/reference/generate-equals-gethashcode-methods?view=vs-2017
            var hashCode = 352033288;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            return hashCode;
        }

        public double CalcEuclidean(UserP user, int minRatings = 0)
        {
            double sim = 0;
            int n = 0;
            // Iterate over all rating combinations
            foreach (var rA in Ratings)
            {
                foreach (var rB in user.Ratings)
                {
                    if (rA.MovieId == rB.MovieId &&
                    (rA.Movie.NumberOfRatings > minRatings || rB.Movie.NumberOfRatings > minRatings))
                    {
                        sim += Math.Pow(rA.Score - rB.Score, 2.0);
                        n++;
                    }
                }
            }
            // No ratings in common, return 0 else calculate inverted score
            return n == 0 ? 0 : 1 / (1 + sim);
        }

        public double CalcPearson(UserP user, int minRatings = 0)
        {
            double sum1 = 0, sum2 = 0, sum1sq = 0, sum2sq = 0, psum = 0;
            int n = 0;
            // Iterate over all rating combinations
            foreach (var rA in Ratings)
            {
                foreach (var rB in user.Ratings)
                {
                    if (rA.MovieId == rB.MovieId &&
                    (rA.Movie.NumberOfRatings > minRatings || rB.Movie.NumberOfRatings > minRatings))
                    {
                        sum1 += rA.Score;
                        sum2 += rB.Score;
                        sum1sq += rA.Score * rA.Score;
                        sum2sq += rB.Score * rB.Score;
                        psum += rA.Score * rB.Score;
                        n++;
                    }
                }
            }
            if (n == 0)
            {
                return 0;
            }
            double num = psum - ((sum1 * sum2) / n);
            double den = Math.Sqrt((sum1sq - Math.Pow(sum1, 2.0) / n) * (sum2sq - Math.Pow(sum2, 2.0) / n));
            return num / den;
        }
    }
}