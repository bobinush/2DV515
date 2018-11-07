using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TinyCsvParser.Mapping;

namespace webapi.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Movie { get; set; }
        public double Score { get; set; }

        public User User { get; set; }
    }

    public sealed class RatingClassMap : CsvMapping<Rating>
    {
        public RatingClassMap()
        {
            MapProperty(0, m => m.UserId);
            MapProperty(1, m => m.Movie);
            MapProperty(2, m => m.Score);
        }
    }
}