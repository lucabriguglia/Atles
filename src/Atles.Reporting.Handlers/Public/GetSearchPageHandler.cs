using Atles.Models.Public.Search;
using Atles.Reporting.Public.Queries;
using OpenCqrs;
using OpenCqrs.Queries;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Public
{
    public class GetSearchPageHandler : IQueryHandler<GetSearchPage, SearchPageModel>
    {
        private readonly ISender _sender;

        public GetSearchPageHandler(ISender sender)
        {
            _sender = sender;
        }

        public async Task<SearchPageModel> Handle(GetSearchPage query)
        {
            var result = new SearchPageModel
            {
                Posts = await _sender.Send(new GetSearchPosts { AccessibleForumIds = query.AccessibleForumIds, Options = query.Options })
            };

            return result;
        }
    }
}
