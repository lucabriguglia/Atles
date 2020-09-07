using Docs;
using Docs.Models;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers.Public
{
    [Route("api/public/documentation")]
    [ApiController]
    public class DocumentationController : ControllerBase
    {
        private readonly IDocumentationService _documentationService;

        public DocumentationController(IDocumentationService documentationService)
        {
            _documentationService = documentationService;
        }

        [HttpGet]
        public DocumentationModel Get()
        {
            return _documentationService.GetLatest();
        }
    }
}