using Atles.Core.Results.Types;
using OneOf;

namespace Atles.Core.Results;

public class QueryResult<TResult> : OneOfBase<TResult, Failure>
{
    protected QueryResult(OneOf<TResult, Failure> input) : base(input) { }

    public static implicit operator QueryResult<TResult>(TResult value) => new(value);
    public static implicit operator QueryResult<TResult>(Failure value) => new(value);
}
