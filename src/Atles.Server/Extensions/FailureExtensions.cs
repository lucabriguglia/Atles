using Atles.Core.Results.Types;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Extensions;

public static class FailureExtensions
{
    public static ActionResult ToActionResult(this Failure failure)
    {
        var (failureType, title, message) = failure;

        var problemDetails = new Models.ProblemDetails
        {
            Title = title ?? failureType.ToString(),
            Detail = message ?? failureType.ToString(),
            Status = failureType.ToStatusCode()
        };

        return failureType switch
        {
            FailureType.NotFound => new NotFoundObjectResult(problemDetails),
            _ => new UnprocessableEntityObjectResult(problemDetails)
        };
    }

    private static int ToStatusCode(this FailureType failureType)
    {
        return failureType switch
        {
            FailureType.NotFound => 404,
            FailureType.Error => 422,
            _ => 400
        };
    }
}