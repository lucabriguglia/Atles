namespace Atles.Core.Results.Types;

public record Failure(FailureType FailureType, string Title = "", string Description = "");