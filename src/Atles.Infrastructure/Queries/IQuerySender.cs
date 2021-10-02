using System.Threading.Tasks;

namespace Atles.Infrastructure.Queries
{
    public interface IQuerySender
    {
        //Task<TResult> Send<TQuery, TResult>(TQuery query) where TQuery : IQuery;
        Task<TResult> Send<TResult>(IQuery<TResult> query);
    }
}
