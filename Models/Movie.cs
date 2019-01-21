using System;
using System.Collections.Generic;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace mvc.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public List<Rating> Ratings { get; set; }
    }
}