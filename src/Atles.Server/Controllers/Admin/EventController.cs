﻿using System;
using System.Threading.Tasks;
using Atles.Core;
using Atles.Models.Admin.Events;
using Atles.Queries.Admin;
using Microsoft.AspNetCore.Mvc;

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
