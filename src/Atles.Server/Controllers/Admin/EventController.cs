using System;
using System.Threading.Tasks;
using Atles.Models.Admin.Events;
using Atles.Reporting.Admin.Events.Queries;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;
using OpenCqrs;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/events")]
    public class EventController : AdminControllerBase
    {
        private readonly IContextService _contextService;
        private readonly ISender _sender;

        public EventController(IContextService contextService, ISender sender) : base(sender)
        {
            _contextService = contextService;
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
