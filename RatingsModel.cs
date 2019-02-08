using System.Collections.Generic;


namespace RatingsCalculator.Models
{
    public class RatingsModel
    {
        public int InitialRating { get; set; }
        public int FinalRating { get; set; }
        public List<int> WinList { get; set; }
        public List<int> LossList { get; set; }
        public int BestWin { get; set; }
        public int WorstLoss { get; set; }
        public int RatingChange { get; set; }

        public RatingsModel()
        {
            WinList = new List<int>();
            LossList = new List<int>();
        }
    }
}