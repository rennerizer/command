using System.ComponentModel.DataAnnotations;

namespace BoardGames.Api.Attributes
{
    public class SortOrderValidatorAttribute : ValidationAttribute
    {
        public string[] AllowedValues { get; set; } = new[] { "ASC", "DESC" };

        public SortOrderValidatorAttribute()
            : base("Value must be one of the following: {0}.") { }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var stringValue = value as string;

            if (!string.IsNullOrEmpty(stringValue) && AllowedValues.Contains(stringValue))
                return ValidationResult.Success;

            return new ValidationResult(
                FormatErrorMessage(string.Join(",", AllowedValues))
            );
        }
    }
}
