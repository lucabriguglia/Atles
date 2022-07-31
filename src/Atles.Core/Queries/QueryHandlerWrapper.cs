using System.Threading.Tasks;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Core.Services;

namespace Atles.Core.Queries;

internal class QueryHandlerWrapper<TQuery, TResult> : QueryHandlerWrapperBase<TResult> where TQuery : IQuery<TResult>
{
    public override async Task<QueryResult<TResult>> Handle(IQuery<TResult> query, IServiceProviderWrapper serviceProvider)
    {
        var handler = GetHandler<IQueryHandler<TQuery, TResult>>(serviceProvider);

        if (handler == null)
        {
            return new Failure(FailureType.Error, "Query Handler", "Handler not found for query of type {typeof(TQuery)}");
        }

        return await handler.Handle((TQuery)query);
    }
}
