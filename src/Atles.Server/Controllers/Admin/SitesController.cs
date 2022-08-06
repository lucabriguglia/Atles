using Atles.Commands.Sites;
using Atles.Core;
using Atles.Models.Admin.Sites;
using Atles.Queries.Admin;
using Atles.Server.Mapping;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Admin;

[Route("api/admin/sites")]
public class SitesController : AdminControllerBase
{
    private readonly IMapper<SettingsPageModel.SiteModel, UpdateSite> _updateSiteMapper;
    private readonly IValidator<SettingsPageModel.SiteModel> _siteValidator;

    public SitesController(
        IDispatcher dispatcher,
        IMapper<SettingsPageModel.SiteModel, UpdateSite> updateSiteMapper,
        IValidator<SettingsPageModel.SiteModel> siteValidator) : base(dispatcher)
    {
        _updateSiteMapper = updateSiteMapper;
        _siteValidator = siteValidator;
    }

    [HttpGet("settings")]
    public async Task<ActionResult<SettingsPageModel>> Settings() => 
        await ProcessGet(new GetSettingsPageModel(CurrentSite.Id));

    [HttpPost("update")]
    public async Task<ActionResult> Update(SettingsPageModel model) => 
        await ProcessPost(model.Site, _updateSiteMapper, _siteValidator);
}
