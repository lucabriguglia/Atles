using System.Security.Claims;
using Atles.Core.Extensions;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Core.Utilities;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public;

public class GetCurrentUserHandler : IQueryHandler<GetCurrentUser, CurrentUserModel>
{
    private readonly AtlesDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetCurrentUserHandler(AtlesDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<QueryResult<CurrentUserModel>> Handle(GetCurrentUser query)
    {
        var defaultUser = new CurrentUserModel();

        if (_httpContextAccessor.HttpContext is null) return defaultUser;

        var claimsPrincipal = _httpContextAccessor.HttpContext.User;
        if (claimsPrincipal.Identity is null || !claimsPrincipal.Identity.IsAuthenticated) return defaultUser;

        var identityUserId = claimsPrincipal.GetUserId();
        if (string.IsNullOrEmpty(identityUserId)) return defaultUser;

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId);
        if (user is null) return defaultUser;

        return new CurrentUserModel
        {
            Id = user.Id,
            IdentityUserId = user.IdentityUserId,
            Email = user.Email,
            DisplayName = user.DisplayName,
            GravatarHash = user.Email.ToGravatarEmailHash(),
            IsSuspended = user.Status == UserStatusType.Suspended
        };
    }
}