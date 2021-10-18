using Atles.Models.Public.Search;
using Atles.Reporting.Public.Queries;
using OpenCqrs;
using OpenCqrs.Queries;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Public
{
    public class GetSearchPageHandler : IQueryHandler<GetSearchPage, SearchPageModel>
    {
        private readonly IDispatcher _dispatcher;

        public GetSearchPageHandler(IDispatcher sender)
        {
            _dispatcher = sender;
        }

        public async Task<SearchPageModel> Handle(GetSearchPage query)
        {
            var result = new SearchPageModel
            {
                Posts = await _dispatcher.Get(new GetSearchPosts { AccessibleForumIds = query.AccessibleForumIds, Options = query.Options })
            };

            return result;
        }
    }
}
