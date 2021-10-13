using Atles.Models.Public;
using Atles.Reporting.Public.Queries;
using Microsoft.AspNetCore.Mvc;
using OpenCqrs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atles.Server.Controllers
{
    [ApiController]
    public abstract class SiteControllerBase : ControllerBase
    {
        private readonly ISender _sender;

        protected SiteControllerBase(ISender sender)
        {
            _sender = sender;
        }

        protected async Task<CurrentSiteModel> CurrentSite() => await _sender.Send(new GetCurrentSite());
        protected async Task<CurrentUserModel> CurrentUser() => await _sender.Send(new GetCurrentUser());
        protected async Task<IList<CurrentForumModel>> CurrentForums() => await _sender.Send(new GetCurrentForums());
    }
}