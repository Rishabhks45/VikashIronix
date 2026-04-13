namespace SharedKernel.Utilities.Helpers;

public static class ValidationHelper
{
    public static ValidationResult ValidateModel<T>(T model) where T : class
    {
        if (model is null)
        {
            return new ValidationResult
            {
                IsValid = false,
                Errors = new Collection<ValidationError>
                {
                    new ValidationError
                    {
                        PropertyName = nameof(model),
                        ErrorMessage = $"{nameof(model)} cannot be null"
                    }
                }
            };
        }

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);

        var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

        if (isValid)
        {
            return new ValidationResult
            {
                IsValid = true,
                Errors = new Collection<ValidationError>()
            };
        }

        var errors = validationResults.Select(result => new ValidationError
        {
            PropertyName = result.MemberNames.FirstOrDefault() ?? "Unknown",
            ErrorMessage = result.ErrorMessage ?? "Unknown validation error"
        }).ToList();

        return new ValidationResult
        {
            IsValid = false,
            Errors = new Collection<ValidationError>(errors)
        };
    }

    public static void ValidateModelOrThrow<T>(T model) where T : class
    {
        var result = ValidateModel(model);

        if (!result.IsValid)
        {
            var errorMessages = string.Join("; ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            throw new BadRequestException($"Validation failed: {errorMessages}");
        }
    }

    public static bool IsValid<T>(T model) where T : class
    {
        return ValidateModel(model).IsValid;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public Collection<ValidationError> Errors { get; init; } = new();
}

public class ValidationError
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

