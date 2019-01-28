using System;
using System.Collections.Generic;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace mvc.Models
{
    public class TagP
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public int Timestamp { get; set; }
        public int MovieId { get; set; }
        public int UserId { get; set; }

        public UserP User { get; set; }
        public MovieP Movie { get; set; }
    }
}