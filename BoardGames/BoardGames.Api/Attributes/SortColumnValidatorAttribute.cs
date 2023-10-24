using System.ComponentModel.DataAnnotations;

namespace BoardGames.Api.Attributes
{
    public class SortColumnValidatorAttribute : ValidationAttribute
    {
        public Type EntityType { get; set; }

        public SortColumnValidatorAttribute(Type entityType)
            : base("Value must match an existing property.")
        {
            EntityType = entityType;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (EntityType != null)
            {
                var stringValue = value as string;

                if (!string.IsNullOrEmpty(stringValue) && 
                    EntityType.GetProperties().Any(p => p.Name == stringValue)) 
                { 
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
