namespace Atles.Core.Results;

public record Failure(FailureType FailureType, string Title = "", string Description = "");