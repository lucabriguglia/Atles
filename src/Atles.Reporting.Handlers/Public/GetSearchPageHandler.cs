using System.Threading.Tasks;
using Atles.Core;
using Atles.Core.Queries;
using Atles.Models.Public;
using Atles.Queries.Public;

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
