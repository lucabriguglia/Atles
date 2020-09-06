using Docs;
using Docs.Models;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Server.Controllers.Admin
{
    [Route("api/admin/documentation")]
    public class DocumentationController : AdminControllerBase
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