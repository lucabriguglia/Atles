using System.Threading.Tasks;
using Atles.Core.Results;

namespace Atles.Core.Queries;

public interface IQueryProcessor
{
    Task<QueryResult<TResult>> Process<TResult>(IQuery<TResult> query);
}
