using OpenCqrs.Queries;
using System;

namespace Atles.Infrastructure.Queries
{
    public abstract class QueryBase<TResult> : IQuery<TResult>
    {
        public Guid SiteId { get; set; }
    }
}
