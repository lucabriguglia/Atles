using System.Threading.Tasks;

namespace Atles.Core.Queries
{
    public interface IQueryProcessor
    {
        Task<TResult> Process<TResult>(IQuery<TResult> query);
    }
}
