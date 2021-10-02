using System;
using System.Threading.Tasks;

namespace Atles.Infrastructure.Queries
{
    internal class QueryHandlerWrapper<TQuery, TResult> : QueryHandlerWrapperBase<TResult> where TQuery : IQuery<TResult>
    {
        public override Task<TResult> Handle(IQuery<TResult> query, IServiceProvider serviceProvider)
        {
            var handler = GetHandler<IQueryHandler<TQuery, TResult>>(serviceProvider);
            return handler.Handle((TQuery)query);
        }
    }
}
