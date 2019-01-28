using System;
using System.Collections.Generic;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace mvc.Models
{
    public class GenreP
    {
        // public GenreP()
        // {
        //     Movies = new HashSet<MovieGenreP>();
        // }
        public int Id { get; set; }
        public string Title { get; set; }

        public ICollection<MovieGenreP> Movies { get; set; }

        public override bool Equals(Object obj)
        {
            GenreP g = obj as GenreP;
            return g != null && g.Title == this.Title;
        }

        public override int GetHashCode()
        {
            // https://docs.microsoft.com/en-us/visualstudio/ide/reference/generate-equals-gethashcode-methods?view=vs-2017
            var hashCode = 352033288;
            hashCode = hashCode * -1521134295 + Title.GetHashCode();
            return hashCode;
        }
    }
}