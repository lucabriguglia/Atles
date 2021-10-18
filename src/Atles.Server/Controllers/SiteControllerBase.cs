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
        private readonly IDispatcher _dispatcher;

        protected SiteControllerBase(IDispatcher sender)
        {
            _dispatcher = sender;
        }

        protected async Task<CurrentSiteModel> CurrentSite() => await _dispatcher.Get(new GetCurrentSite());
        protected async Task<CurrentUserModel> CurrentUser() => await _dispatcher.Get(new GetCurrentUser());
        protected async Task<IList<CurrentForumModel>> CurrentForums() => await _dispatcher.Get(new GetCurrentForums());
    }
}