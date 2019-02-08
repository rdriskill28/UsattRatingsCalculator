using System.ComponentModel;
using RatingsCalculator.Validation;


namespace RatingsCalculator.ViewModels
{
    public class RatingsViewModel
    {
        [InitialRatingValidation]
        [DisplayName("Initial Rating")]
        public string InitialRating { get; set; }
        [WinLossValidation]
        [DisplayName("Wins")]
        public string WinString { get; set; }
        [WinLossValidation]
        [DisplayName("Losses")]
        public string LossString { get; set; }
    }
}