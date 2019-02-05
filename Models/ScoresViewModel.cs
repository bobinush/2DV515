namespace mvc.Models
{


    public class ScoreViewModel
    {
        public ScoreViewModel(Page p, double scoreContent, double scoreLocation)
        {
            ID = p.ID;
            Url = p.Url;
            ScoreContent = scoreContent;
            ScoreLocation = 0.8 * scoreLocation;
        }
        public int ID { get; set; }
        public string Url { get; set; }
        public double Score { get { return ScoreContent + ScoreLocation; } }
        // + 0.5 * scores.Distance[i];
        public double ScoreContent { get; set; }
        public double ScoreLocation { get; set; }
        public double ScoreDistance { get; set; }
    }
}