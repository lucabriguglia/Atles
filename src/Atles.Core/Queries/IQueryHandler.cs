using System.Threading.Tasks;
using Atles.Core.Results;

namespace Atles.Core.Queries;

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<QueryResult<TResult>> Handle(TQuery query);
}
