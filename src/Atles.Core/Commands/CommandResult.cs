using Atles.Core.Results;
using OneOf;

namespace Atles.Core.Commands;

public class CommandResult : OneOfBase<Success, Failure>
{
    protected CommandResult(OneOf<Success, Failure> input) : base(input) { }

    public static implicit operator CommandResult(Success value) => new(value);
    public static implicit operator CommandResult(Failure value) => new(value);

    public bool Success(out Success success, out Failure failure) => TryPickT0(out success, out failure);
}
