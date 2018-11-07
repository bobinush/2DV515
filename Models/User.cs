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
            return n == 0 ? 0 : 1 / (1 + sim);
        }

        // public double CalcPearson(User user)
        // {

        // }
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