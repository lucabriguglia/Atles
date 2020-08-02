using System;
using System.Threading.Tasks;
using Atlas.Models.Public;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers
{
    [Route("api/public/member")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IContextService _contextService;
        private readonly IPublicModelBuilder _modelBuilder;

        public MemberController(IContextService contextService, IPublicModelBuilder modelBuilder)
        {
            _contextService = contextService;
            _modelBuilder = modelBuilder;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MemberPageModel>> Index(Guid id)
        {
            var model = await _modelBuilder.BuildMemberPageModelAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return model;
        }
    }
}
