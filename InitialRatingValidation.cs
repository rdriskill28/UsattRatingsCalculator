using System.ComponentModel.DataAnnotations;


namespace RatingsCalculator.Validation
{
    public class InitialRatingValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object obj, ValidationContext validationContext)
        {
            int tempInt;
            string value = (obj ?? string.Empty).ToString();

            if (!int.TryParse(value, out tempInt))
                return new ValidationResult("Must be a valid integer (whole number)!");
            
            if (tempInt < 0)
                return new ValidationResult("Negative ratings are not allowed!");

            return ValidationResult.Success;
        }
    }
}