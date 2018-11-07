using System;
using System.Collections.Generic;
using TinyCsvParser.Mapping;

namespace webapi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Rating> Ratings { get; set; }

        public double CalcEuclidean(User user)
        {
            double sim = 0;
            int n = 0;
            // Iterate over all rating combinations
            foreach (var rA in Ratings)
            {
                foreach (var rB in user.Ratings)
                {
                    if (rA.Movie == rB.Movie)
                    {
                        sim += Math.Pow(rA.Score - rB.Score, 2.0);
                        n++;
                    }
                }
            }
            // No ratings in common, return 0 else calculate inverted score
            return n == 0 ? 0 : 1 / (1 + sim);
        }

        public double CalcPearson(User user)
        {
            double sum1 = 0, sum2 = 0, sum1sq = 0, sum2sq = 0, psum = 0;
            int n = 0;
            // Iterate over all rating combinations
            foreach (var rA in Ratings)
            {
                foreach (var rB in user.Ratings)
                {
                    if (rA.Movie == rB.Movie)
                    {
                        sum1 += rA.Score;
                        sum2 += rB.Score;
                        sum1sq = Math.Pow(rA.Score, 2.0);
                        sum2sq = Math.Pow(rB.Score, 2.0);
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

    public sealed class UserClassMap : CsvMapping<User>
    {
        public UserClassMap()
        : base()
        {
            MapProperty(0, m => m.Name);
            MapProperty(1, m => m.Id);
        }
    }
}