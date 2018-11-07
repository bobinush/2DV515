using TinyCsvParser.Mapping;

namespace webapi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public sealed class UserClassMap : CsvMapping<User>
    {
        public UserClassMap()
        : base()
        {
            MapProperty(0, m => m.Name);
            MapProperty(1, m => m.Id);
        }
    }
}