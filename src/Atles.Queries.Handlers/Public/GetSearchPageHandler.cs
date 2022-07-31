using System.Threading.Tasks;
using Atles.Core;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Models.Public;
using Atles.Queries.Public;

namespace Atles.Queries.Handlers.Public
{
    public class GetSearchPageHandler : IQueryHandler<GetSearchPage, SearchPageModel>
    {
        private readonly IDispatcher _dispatcher;

        public GetSearchPageHandler(IDispatcher sender)
        {
            _dispatcher = sender;
        }

        public async Task<QueryResult<SearchPageModel>> Handle(GetSearchPage query)
        {
            // TODO: To be moved to a service
            var queryResult = await _dispatcher.Get(new GetSearchPosts { AccessibleForumIds = query.AccessibleForumIds, Options = query.Options });
            var posts = queryResult.AsT0;

            var result = new SearchPageModel
            {
                Posts = posts
            };

            return result;
        }
    }
}
