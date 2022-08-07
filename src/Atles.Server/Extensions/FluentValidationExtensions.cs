using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Extensions;

public static class FluentValidationExtensions
{
    public static ActionResult ToActionResult(this ValidationResult validationResult)
    {
        return new BadRequestObjectResult(new Models.ProblemDetails
        {
            Title = "Request validation failed",
            Detail = validationResult.Errors.ToErrorMessage(),
            Status = 400
        });
    }

    public static string ToErrorMessage(this IEnumerable<ValidationFailure> validationFailures)
    {
        var errors = validationFailures.Select(x => $"\r\n - {x.ErrorMessage}").ToArray();
        return $"Errors: {string.Join("", errors)}";
    }
}
