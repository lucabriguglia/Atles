using System.Threading.Tasks;

namespace Atles.Models
{
    public interface IQuery
    {
    }

    public class QueryBase : IQuery
    {
    }

    public interface IResult
    {
    }

    public class ResultBase : IResult
    {
    }

    public interface IQuerySender
    {
        Task<TResult> Send<TQuery, TResult>(TQuery query) 
            where TQuery : IQuery 
            where TResult : IResult;
    }

    public class QuerySender : IQuerySender
    {
        public Task<TResult> Send<TQuery, TResult>(TQuery query)
            where TQuery : IQuery
            where TResult : IResult
        {
            throw new System.NotImplementedException();
        }
    }
}
