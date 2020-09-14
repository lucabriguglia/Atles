using Atles.Data.Caching;
using Docs;
using Docs.Models;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Public
{
    [Route("api/public/documentation")]
    [ApiController]
    public class DocumentationController : ControllerBase
    {
        private readonly IDocumentationService _documentationService;
        private readonly ICacheManager _cacheManager;

        public DocumentationController(IDocumentationService documentationService, ICacheManager cacheManager)
        {
            _documentationService = documentationService;
            _cacheManager = cacheManager;
        }

        [HttpGet]
        public DocumentationModel Get()
        {
            return _cacheManager.GetOrSet("DomainDocumentation", () => _documentationService.GetLatest());
        }
    }
}