using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RatingsCalculator.Models;
using RatingsCalculator.ViewModels;


namespace RatingsCalculator.Controllers
{
    public class RatingsController : Controller
    {
        #region ActionResult
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Calculate(RatingsViewModel vm)
        {
            if (!ModelState.IsValid)            
                return View("Index", vm);

            RatingsModel model = ConvertToModel(vm);

            model = CalculateFinalRating(model);

            RatingsResultViewModel result = ConvertToResultViewModel(model);

            return View("Calculate", result);
        }
        #endregion

        #region Model <=> ViewModel Conversion
        private RatingsResultViewModel ConvertToResultViewModel(RatingsModel model)
        {
            var result = new RatingsResultViewModel
            {
                InitialRating = model.InitialRating.ToString(),
                FinalRating = model.FinalRating.ToString(),
                RatingChange = model.RatingChange.ToString(),
                BestWin = model.BestWin == 0 ? string.Empty : model.BestWin.ToString(),
                WorstLoss = model.WorstLoss == 0 ? string.Empty : model.WorstLoss.ToString(),
                MatchCount = (model.WinList.Count + model.LossList.Count).ToString(),
                WinCount = model.WinList.Count.ToString(),
                LossCount = model.LossList.Count.ToString()
            };
            return result;
        }

        private RatingsModel ConvertToModel(RatingsViewModel vm)
        {
            var model = new RatingsModel { InitialRating = Convert.ToInt32(vm.InitialRating) };
            
            // int.Parse is OK here because we restrict to only numbers and commas and empty results are removed
            if (!string.IsNullOrEmpty(vm.WinString) && vm.WinString.Contains(","))
                model.WinList = vm.WinString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();
            else if (!string.IsNullOrEmpty(vm.WinString))
                model.WinList.Add(Convert.ToInt32(vm.WinString));

            if (!string.IsNullOrEmpty(vm.LossString) && vm.LossString.Contains(","))
                model.LossList = vm.LossString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();
            else if (!string.IsNullOrEmpty(vm.LossString))
                model.LossList.Add(Convert.ToInt32(vm.LossString));

            if (model.WinList.Count > 0)
                model.BestWin = model.WinList.Max();
            if (model.LossList.Count > 0)
                model.WorstLoss = model.LossList.Min();

            return model;
        }
        #endregion

        #region Ratings Logic
        private RatingsModel CalculateFinalRating(RatingsModel model)
        {
            //Pass 1.  the result will determine how we shall calculate the final rating
            int ratingChange = MakeRatingPass(model.InitialRating, model.WinList, model.LossList);

            //51-75 point change means you set your initial rating to your rating after the first pass
            //then you do another pass
            if (ratingChange > 51 && ratingChange <= 75)
            {
                int passOneRating = model.InitialRating + ratingChange;
                int passTwoChange = MakeRatingPass(passOneRating, model.WinList, model.LossList);
                model.FinalRating = passOneRating + passTwoChange;
            }
            //over 75 point gain (with at least one loss) needs to use this formula:
            //Pass_2_rating = ( BestWin + WorstLoss ) / 4 + ( Pass_1_result ) / 2
            else if (ratingChange > 75 && model.LossList.Count > 0)
            {
                int adjustedRating = ((model.BestWin + model.WorstLoss) / 4) + ((model.InitialRating + ratingChange) / 2);

                //adjusted rating can never be lower than initial rating.  if it is then just use pass1 rating
                model.FinalRating = adjustedRating >= model.InitialRating ? adjustedRating : (model.InitialRating + ratingChange);
            }
            //over 75 point gain without a loss (adjust rating to implied median then run another pass)
            else if (ratingChange > 75)
            {
                int passOneRating = GetImpliedMedian(model.WinList);

                //adjusted rating can never be lower than initial rating.  if it is then just use pass1 rating
                if (passOneRating >= model.InitialRating)
                {
                    int passTwoChange = MakeRatingPass(passOneRating, model.WinList, model.LossList);
                    model.FinalRating = passOneRating + passTwoChange;
                }
                else
                    model.FinalRating = model.InitialRating + ratingChange;
            }
            // less than 51 change = Done
            else
                model.FinalRating = model.InitialRating + ratingChange;

            model.RatingChange = model.FinalRating - model.InitialRating;
            return model;
        }

        private int MakeRatingPass(int initialRating, List<int> winList, List<int> lossList)
        {            
            int ratingChange = 0;

            foreach (int win in winList)
            { 
                int ratingDifference = initialRating - win;
                ratingChange += GetPoints(ratingDifference, true);
            }

            foreach (int loss in lossList)
            { 
                int ratingDifference = initialRating - loss;
                ratingChange += GetPoints(ratingDifference, false);
            }

            return ratingChange;
        }

        private int GetPoints (int ratingDifference, bool isWin)
        {
            // ratingDifference should be yourRating - opponentRating
            // win as a higher rated player or loss as a lower rated player is expected.  All else is an upset
            bool isExpected = (ratingDifference >= 0 && isWin) || (ratingDifference < 0 && !isWin);

            // since we know if it is expected, we now only need the absolute value of the rating difference
            int difference = Math.Abs(ratingDifference);

            int points = 0;

            if (difference >= 0 && difference <= 12)
                points = 8;
            else if (difference >= 13 && difference <= 37)
                points = isExpected ? 7 : 10;
            else if (difference >= 38 && difference <= 62)
                points = isExpected ? 6 : 13;
            else if (difference >= 63 && difference <= 87)
                points = isExpected ? 5 : 16;
            else if (difference >= 88 && difference <= 112)
                points = isExpected ? 4 : 20;
            else if (difference >= 113 && difference <= 137)
                points = isExpected ? 3 : 25;
            else if (difference >= 138 && difference <= 162)
                points = isExpected ? 2 : 30;
            else if (difference >= 163 && difference <= 187)
                points = isExpected ? 2 : 35;
            else if (difference >= 188 && difference <= 212)
                points = isExpected ? 1 : 40;
            else if (difference >= 213 && difference <= 237)
                points = isExpected ? 1 : 45;
            else if (difference >= 238)
                points = isExpected ? 0 : 50;

            return isWin ? points : points * -1;
        }

        private int GetImpliedMedian(List<int> source)
        {
            // Create a copy of the input, and sort the copy
            int[] temp = source.ToArray();
            Array.Sort(temp);

            int count = temp.Length;
            if (count == 0)            
                throw new InvalidOperationException("Empty collection");  
          
            if (count % 2 == 0)
            {
                // count is even, average two middle elements
                int a = temp[count / 2 - 1];
                int b = temp[count / 2];
                return (a + b) / 2;
            }

            // count is odd, return the middle element
            return temp[count / 2];
        }
        #endregion
    }
}
