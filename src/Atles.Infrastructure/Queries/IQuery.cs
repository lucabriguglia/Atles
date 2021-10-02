using System;

namespace Atles.Infrastructure.Queries
{
    public interface IQuery<TResult>
    {
        Guid SiteId { get; set; }
    }
}
