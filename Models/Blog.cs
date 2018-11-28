using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    public class Blog
    {
        public Blog()
        {
            Words = new List<Word>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Word> Words { get; set; }
    }
}