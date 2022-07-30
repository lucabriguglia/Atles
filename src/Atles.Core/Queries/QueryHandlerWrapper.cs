using System;
using System.Threading.Tasks;
using Atles.Core.Services;

namespace Atles.Core.Queries;

internal class QueryHandlerWrapper<TQuery, TResult> : QueryHandlerWrapperBase<TResult> where TQuery : IQuery<TResult>
{
    public override Task<TResult> Handle(IQuery<TResult> query, IServiceProviderWrapper serviceProvider)
    {
        var handler = GetHandler<IQueryHandler<TQuery, TResult>>(serviceProvider);

        if (handler == null)
        {
            throw new Exception($"Handler not found for query of type {typeof(TQuery)}");
        }

        return handler.Handle((TQuery)query);
    }
}