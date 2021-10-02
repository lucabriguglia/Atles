using System;

namespace Atles.Infrastructure.Queries
{
    public interface IQuery
    {
        Guid SiteId { get; set; }
    }

    public interface IQuery<TResult>
    {
        Guid SiteId { get; set; }
    }
}
