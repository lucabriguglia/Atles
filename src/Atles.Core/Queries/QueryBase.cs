using System;

namespace Atles.Core.Queries;

public abstract class QueryBase<TResult> : IQuery<TResult>
{
    public Guid SiteId { get; set; }
}

// TODO: Temporary name until all queries have been converted from classes to records
public abstract record QueryRecordBase<TResult>(Guid SiteId) : IQuery<TResult>;
