namespace webapi.Models
{


    public class ScoreViewModel
    {
        public ScoreViewModel(Page p, double score)
        {
            ID = p.ID;
            Url = p.Url;
            Score = score;
        }
        public int ID { get; set; }
        public string Url { get; set; }
        public double Score { get; set; }
    }
}