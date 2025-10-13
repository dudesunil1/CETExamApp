using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models.Attributes
{
    public class OptionalEmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // If value is null or empty, it's valid (optional field)
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return true;
            }

            // If value is provided, validate it as an email
            var emailAttribute = new EmailAddressAttribute();
            return emailAttribute.IsValid(value);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Please enter a valid email address for {name}";
        }
    }
}
