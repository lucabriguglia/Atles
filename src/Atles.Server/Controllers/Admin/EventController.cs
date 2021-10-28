using System;
using System.Threading.Tasks;
using Atles.Reporting.Models.Admin.Events;
using Atles.Reporting.Models.Admin.Events.Queries;
using Microsoft.AspNetCore.Mvc;
using OpenCqrs;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/events")]
    public class EventController : AdminControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public EventController(IDispatcher dispatcher) : base(dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet("target-model/{id}")]
        public async Task<TargetEventsComponentModel> Target(Guid id)
        {
            var site = await CurrentSite();

            return await _dispatcher.Get(new GetTargetEventsComponent 
            { 
                SiteId = site.Id, 
                Id = id 
            });
        }
    }
}
