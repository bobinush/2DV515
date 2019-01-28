using System;
using System.Collections.Generic;

namespace mvc.Models
{
    public class MovieP
    {
        public MovieP()
        {
            Genres = new HashSet<MovieGenreP>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Imdb { get; set; }
        public string Tmdb { get; set; }
        public int NumberOfRatings { get; set; }

        public ICollection<TagP> Tags { get; set; }
        public ICollection<RatingP> Ratings { get; set; }
        public ICollection<MovieGenreP> Genres { get; set; }
    }
}