using System.Threading.Tasks;

namespace Atles.Infrastructure.Queries
{
    public interface IQueryProcessor
    {
        Task<TResult> Process<TResult>(IQuery<TResult> query);
    }
}
