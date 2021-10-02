using System.Threading.Tasks;

namespace Atles.Infrastructure.Queries
{
    public interface IQuerySender
    {
        Task<TResult> Send<TResult>(IQuery<TResult> query);
    }
}
