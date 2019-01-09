namespace webapi.Models
{
    public class MetricsViewModel
    {
        public MetricsViewModel(int numberOfPages)
        {
            Content = new double[numberOfPages];
            Location = new double[numberOfPages];
            Distance = new double[numberOfPages];
        }
        public double[] Content { get; set; }
        public double[] Location { get; set; }
        public double[] Distance { get; set; }
    }
}