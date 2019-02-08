using System.ComponentModel;


namespace RatingsCalculator.ViewModels
{
    public class RatingsResultViewModel
    {
        [DisplayName("Initial Rating")]
        public string InitialRating { get; set; }
        [DisplayName("Final Rating")]
        public string FinalRating { get; set; }
        [DisplayName("Rating Change")]
        public string RatingChange { get; set; }
        [DisplayName("Best Win")]
        public string BestWin { get; set; }
        [DisplayName("Worst Loss")]
        public string WorstLoss { get; set; }
        [DisplayName("Total Matches")]
        public string MatchCount { get; set; }
        [DisplayName("Wins")]
        public string WinCount { get; set; }
        [DisplayName("Losses")]
        public string LossCount { get; set; }
    }
}