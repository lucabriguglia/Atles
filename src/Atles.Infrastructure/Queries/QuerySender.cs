using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Atles.Infrastructure.Queries
{
    public class QuerySender : IQuerySender
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly ConcurrentDictionary<Type, object> _queryHandlerWrappers = new();

        public QuerySender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> Send<TResult>(IQuery<TResult> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var queryType = query.GetType();

            var handler = (QueryHandlerWrapperBase<TResult>)_queryHandlerWrappers.GetOrAdd(queryType,
                t => Activator.CreateInstance(typeof(QueryHandlerWrapper<,>).MakeGenericType(queryType, typeof(TResult))));

            return await handler.Handle(query, _serviceProvider);
        }
    }
}
