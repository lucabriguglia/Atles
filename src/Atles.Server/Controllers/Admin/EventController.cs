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
        private readonly ISender _sender;

        public EventController(ISender sender) : base(sender)
        {
            _sender = sender;
        }

        [HttpGet("target-model/{id}")]
        public async Task<TargetEventsComponentModel> Target(Guid id)
        {
            var site = await CurrentSite();

            return await _sender.Send(new GetTargetEventsComponent 
            { 
                SiteId = site.Id, 
                Id = id 
            });
        }
    }
}
