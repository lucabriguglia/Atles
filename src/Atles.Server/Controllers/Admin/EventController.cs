using System;
using System.Threading.Tasks;
using Atles.Models.Admin.Events;
using Atles.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin
{
    [Route("api/admin/events")]
    public class EventController : AdminControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IEventModelBuilder _modelBuilder;

        public EventController(IContextService contextService, IEventModelBuilder modelBuilder)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
        }

        [HttpGet("target-model/{id}")]
        public async Task<TargetEventsComponentModel> Target(Guid id)
        {
            var site = await _contextService.CurrentSiteAsync();

            return await _modelBuilder.BuildTargetModelAsync(site.Id, id);
        }
    }
}
