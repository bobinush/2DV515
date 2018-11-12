using System.Collections.Generic;
using Newtonsoft.Json;

namespace webapi.Models
{
    public class UserViewModel
    {
        public UserViewModel()
        {

        }
        public UserViewModel(User u)
        {
            Id = u.Id;
            Name = u.Name;
            Movies = new List<MovieViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public double Score { get; set; }
        public List<UserViewModel> SimilarUsers { get; set; }
        public List<MovieViewModel> Movies { get; set; }
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