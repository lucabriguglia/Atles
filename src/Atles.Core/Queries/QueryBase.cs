using System;

namespace Atles.Core.Queries;

public abstract class QueryBase<TResult> : IQuery<TResult>
{
    public Guid SiteId { get; set; }
}