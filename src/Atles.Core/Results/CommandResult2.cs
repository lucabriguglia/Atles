using System.Collections.Generic;
using Atles.Core.Events;
using Atles.Core.Results.Types;
using OneOf;
using OneOf.Types;

namespace Atles.Core.Results;

public class CommandResult2 : OneOfBase<Success<IList<IEvent>>, Failure>
{
    protected CommandResult2(OneOf<Success<IList<IEvent>>, Failure> input) : base(input) { }

    public static implicit operator CommandResult2(Success<IList<IEvent>> value) => new(value);
    public static implicit operator CommandResult2(Failure value) => new(value);

    public bool Success(out Success<IList<IEvent>> success, out Failure failure) => TryPickT0(out success, out failure);
}