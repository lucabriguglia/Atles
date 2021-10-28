using OpenCqrs;
using OpenCqrs.Queries;
using System.Threading.Tasks;
using Atles.Reporting.Models.Public;
using Atles.Reporting.Models.Public.Queries;

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
