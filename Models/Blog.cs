using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc.Models
{
    public class Blog
    {
        public Blog()
        {
            Words = new List<BlogWord>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BlogWord> Words { get; set; }
    }
}