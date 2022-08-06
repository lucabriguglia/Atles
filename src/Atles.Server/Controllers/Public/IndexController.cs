using Atles.Core;
using Atles.Domain;
using Atles.Models;
using Atles.Models.Public;
using Atles.Queries.Public;
using Atles.Server.Services;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Atles.Server.Controllers.Public;

[Route("api/public")]
public class IndexController : SiteControllerBase
{
    private readonly ISecurityService _securityService;
    private readonly IDispatcher _dispatcher;

    public IndexController(ISecurityService securityService,
        IDispatcher dispatcher) : base(dispatcher)
    {
        _securityService = securityService;
        _dispatcher = dispatcher;
    }

    [HttpGet("index-model")]
    public async Task<IndexPageModel> Index()
    {
        var modelToFilter = await _dispatcher.Get(new GetIndexPage { SiteId = CurrentSite.Id });

        var filteredModel = await GetFilteredIndexModel(CurrentSite.Id, modelToFilter.AsT0);  // TODO: Refactoring

        return filteredModel;
    }

    private async Task<IndexPageModel> GetFilteredIndexModel(Guid siteId, IndexPageModel modelToFilter)
    {
        var result = new IndexPageModel();

        foreach (var categoryToFilter in modelToFilter.Categories)
        {
            var category = new IndexPageModel.CategoryModel { Name = categoryToFilter.Name };

            foreach (var forumToFilter in categoryToFilter.Forums)
            {
                var permissionSetId = forumToFilter.PermissionSetId ?? categoryToFilter.PermissionSetId;
                var permissions = await _dispatcher.Get(new GetPermissions { SiteId = siteId, PermissionSetId = permissionSetId });
                var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions.AsT0); // TODO: Refactoring
                if (!canViewForum) continue;
                var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions.AsT0); // TODO: Refactoring
                var forum = new IndexPageModel.ForumModel
                {
                    Id = forumToFilter.Id,
                    Name = forumToFilter.Name,
                    Slug = forumToFilter.Slug,
                    Description = forumToFilter.Description,
                    TotalTopics = forumToFilter.TotalTopics,
                    TotalReplies = forumToFilter.TotalReplies,
                    LastTopicId = forumToFilter.LastTopicId,
                    LastTopicTitle = forumToFilter.LastTopicTitle,
                    LastTopicSlug = forumToFilter.LastTopicSlug,
                    LastPostTimeStamp = forumToFilter.LastPostTimeStamp,
                    LastPostUserId = forumToFilter.LastPostUserId,
                    LastPostUserDisplayName = forumToFilter.LastPostUserDisplayName,
                    CanViewTopics = canViewTopics
                };
                category.Forums.Add(forum);
            }

            result.Categories.Add(category);
        }

        return result;
    }

    [Authorize]
    [HttpPost("preview")]
    public async Task<string> Preview([FromBody] string content)
    {
        return await Task.FromResult(Markdown.ToHtml(content));
    }

    [HttpGet("current-site")]
    public async Task<CurrentSiteModel> GetCurrentSite()
    {
        return await Task.FromResult(CurrentSite);
    }

    [HttpGet("current-user")]
    public async Task<CurrentUserModel> GetCurrentUser()
    {
        return await Task.FromResult(CurrentUser);
    }

    [HttpGet("search")]
    public async Task<SearchPageModel> Search([FromQuery] int page = 1, [FromQuery] string search = null)
    {
        var accessibleForumIds = new List<Guid>();

        foreach (var forum in CurrentForums)
        {
            var permissions = await _dispatcher.Get(new GetPermissions { SiteId = CurrentSite.Id, PermissionSetId = forum.PermissionSetId });
            var canViewForum = _securityService.HasPermission(PermissionType.ViewForum, permissions.AsT0);
            var canViewTopics = _securityService.HasPermission(PermissionType.ViewTopics, permissions.AsT0);
            var canViewRead = _securityService.HasPermission(PermissionType.Read, permissions.AsT0);
            if (canViewForum && canViewTopics && canViewRead)
            {
                accessibleForumIds.Add(forum.Id);
            }
        }

        var model = await _dispatcher.Get(new GetSearchPage 
        { 
            SiteId = CurrentSite.Id,
            AccessibleForumIds = accessibleForumIds, 
            Options = new QueryOptions(page, search) 
        });

        return model.AsT0;
    }

    [HttpGet("cookie-consent")]
    public async Task<CookieConsentModel> CookieConsent()
    {
        var consentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
        var showBanner = !consentFeature?.CanTrack ?? false;
        var consentCookie = consentFeature?.CreateConsentCookie();
        return await Task.FromResult(new CookieConsentModel { ShowBanner = showBanner, ConsentCookie = consentCookie });
    }
}
