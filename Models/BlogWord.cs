namespace webapi.Models
{
    public class BlogWord
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public double Value { get; set; }
        public int BlogId { get; set; }
    }
}