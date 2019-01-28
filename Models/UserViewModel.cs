using System.Collections.Generic;
using Newtonsoft.Json;

namespace mvc.Models
{
    public class UserViewModel
    {
        public UserViewModel() { }

        public UserViewModel(User u)
        {
            Id = u.Id;
            Name = u.Name;
            Distance = new DistanceMetric();
            Ratings = u.Ratings;
        }
        public UserViewModel(UserP u)
        {
            Id = u.Id;
            Name = u.Name;
            Distance = new DistanceMetric();
            RatingsP = u.Ratings;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<RatingP> RatingsP { get; set; }
        public double Score { get; set; }
        public DistanceMetric Distance { get; set; }
    }

    public class DistanceMetric
    {
        public DistanceMetric()
        {
            Movies = new List<MovieViewModel>();
        }
        public double Score { get; set; }
        public ICollection<UserViewModel> SimilarUsers { get; set; }
        public ICollection<MovieViewModel> Movies { get; set; }
    }

    public class MovieViewModel
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public double Score { get; set; }
        [JsonIgnore]
        public double SimilarityScore { get; set; }
    }
}