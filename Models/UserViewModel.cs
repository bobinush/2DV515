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
            Pearson = new Pearson();
            Euclidean = new Euclidean();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public double Score { get; set; }
        public Pearson Pearson { get; set; }
        public Euclidean Euclidean { get; set; }
    }

    public class Pearson
    {
        public Pearson()
        {
            Movies = new List<MovieViewModel>();
        }
        public double Score { get; set; }
        public ICollection<UserViewModel> SimilarUsers { get; set; }
        public ICollection<MovieViewModel> Movies { get; set; }
    }

    public class Euclidean
    {
        public Euclidean()
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