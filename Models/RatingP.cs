using System;
using Newtonsoft.Json;

namespace mvc.Models
{
    public class RatingP
    {
        // public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int Timestamp { get; set; }
        public double Score { get; set; }
        // [JsonIgnore]
        public UserP User { get; set; }
        // [JsonIgnore]
        public MovieP Movie { get; set; }
    }
}