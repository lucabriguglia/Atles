using MediatR;
using System;

namespace Atles.Reporting
{
    public abstract class QueryBase<T> : IRequest<T>
    {
        /// <summary>
        /// The unique identifier of the Site whose the request belongs to.
        /// </summary>
        public Guid SiteId { get; set; }
    }
}
