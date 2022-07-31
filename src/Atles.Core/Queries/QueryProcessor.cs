using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Core.Services;

namespace Atles.Core.Queries;

public class QueryProcessor : IQueryProcessor
{
    private readonly IServiceProviderWrapper _serviceProvider;

    private static readonly ConcurrentDictionary<Type, object> QueryHandlerWrappers = new();

    public QueryProcessor(IServiceProviderWrapper serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<QueryResult<TResult>> Process<TResult>(IQuery<TResult> query)
    {
        if (query == null)
        {
            return new Failure(FailureType.NullArgument, "Query", $"Query of type {typeof(IQuery<TResult>)} is null.");
        }

        var queryType = query.GetType();

        var handler = (QueryHandlerWrapperBase<TResult>)QueryHandlerWrappers.GetOrAdd(queryType,
            t => Activator.CreateInstance(typeof(QueryHandlerWrapper<,>).MakeGenericType(queryType, typeof(TResult))));

        return await handler.Handle(query, _serviceProvider);
    }
}