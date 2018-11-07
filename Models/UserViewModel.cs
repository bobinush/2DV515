namespace webapi.Models
{
    public class UserViewModel
    {
        public UserViewModel(User u)
        {
            Id = u.Id;
            Name = u.Name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public double Euclidean { get; set; }
        public double Pearson { get; set; }
    }
}