using System.ComponentModel.DataAnnotations;

namespace CETExamApp.Models.Attributes
{
    public class OptionalPhoneAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // If value is null or empty, it's valid (optional field)
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return true;
            }

            // If value is provided, validate it as a phone number
            var phoneAttribute = new PhoneAttribute();
            return phoneAttribute.IsValid(value);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Please enter a valid phone number for {name}";
        }
    }
}
