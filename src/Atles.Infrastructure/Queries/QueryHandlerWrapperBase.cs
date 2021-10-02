using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Atles.Infrastructure.Queries
{
    internal abstract class QueryHandlerWrapperBase<TResult>
    {
        protected static THandler GetHandler<THandler>(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<THandler>();
        }

        public abstract Task<TResult> Handle(IQuery<TResult> query, IServiceProvider serviceProvider);
    }
}
