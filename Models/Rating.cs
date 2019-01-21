using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace mvc.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public double Score { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public Movie Movie { get; set; }

        // only used when importing really
        [JsonIgnore]
        [NotMapped]
        public string Title { get; set; }
    }

    public class RatingClassMap : CsvMapping<Rating>
    {
        public RatingClassMap()
        {
            MapProperty(0, m => m.UserId);
            MapProperty(1, m => m.Title);
            // MapProperty(1, m => m.Movie, new MovieTypeConverter());
            MapProperty(2, m => m.Score);
        }
    }
    public class MovieTypeConverter : ITypeConverter<Movie>
    {
        public Type TargetType => typeof(Movie);

        Type ITypeConverter<Movie>.TargetType => throw new NotImplementedException();

        public bool TryConvert(string value, out Movie result)
        {
            result = new Movie
            {
                Title = value
            };
            return true;
        }
    }
}