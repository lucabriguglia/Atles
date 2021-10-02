using System.Threading.Tasks;

namespace Atles.Infrastructure.Queries
{
    //public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery
    //{
    //    Task<TResult> Handle(TQuery query);
    //}

    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(TQuery query);
    }
}
