using System;
using System.Threading.Tasks;
using Atles.Models.Admin.Events;
using Atles.Reporting.Admin.Events.Queries;
using Microsoft.AspNetCore.Mvc;
using OpenCqrs;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/events")]
    public class EventController : AdminControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public EventController(IDispatcher sender) : base(sender)
        {
            _dispatcher = sender;
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
