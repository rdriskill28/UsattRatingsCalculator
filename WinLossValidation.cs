using System.ComponentModel.DataAnnotations;


namespace RatingsCalculator.Validation
{
    public class WinLossValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object obj, ValidationContext validationContext)
        {
            string value = (obj ?? string.Empty).ToString();
            
            foreach (char c in value)
            {
                if ((c < '0' || c > '9') && c != ',')
                    return new ValidationResult("Only numbers and a comma to separate them are allowed");
            }

            return ValidationResult.Success;
        }
    }
}