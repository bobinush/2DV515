using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    public class Page
    {
        public Page()
        {
            Words = new List<PageWord>();
        }
        public int ID { get; set; }
        public string Url { get; set; }

        public List<PageWord> Words { get; set; }
    }

    public class PageWord
    {
        public int ID { get; set; }
        public int Value { get; set; }

        public int PageId { get; set; }
        public Page Page { get; set; }
    }
}