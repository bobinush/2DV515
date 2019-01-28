using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace mvc.Models
{
    public class MovieGenreP
    {
        public int MovieId { get; set; }
        public MovieP Movie { get; set; }

        public int GenreId { get; set; }
        public GenreP Genre { get; set; }
    }
}