using Atles.Core.Extensions;
using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Public;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public;

public class GetSettingsPageHandler : IQueryHandler<GetSettingsPage, SettingsPageModel>
{
    private readonly AtlesDbContext _dbContext;
    public GetSettingsPageHandler(AtlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QueryResult<SettingsPageModel>> Handle(GetSettingsPage query)
    {
        var result = new SettingsPageModel();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x =>
                x.Id == query.UserId &&
                x.Status != UserStatusType.Deleted);

        if (user == null)
        {
            return null;
        }

        result.User = new SettingsPageModel.UserModel
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            GravatarHash = user.Email.ToGravatarEmailHash(),
            IsSuspended = user.Status == UserStatusType.Suspended
        };

        return result;
    }
}