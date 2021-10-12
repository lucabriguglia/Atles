using Atles.Data;
using Atles.Domain.Users;
using Atles.Models.Public.Users;
using Atles.Reporting.Public.Queries;
using Atles.Reporting.Shared.Queries;
using Microsoft.EntityFrameworkCore;
using OpenCqrs;
using OpenCqrs.Queries;
using System.Threading.Tasks;

namespace Atles.Reporting.Handlers.Public
{
    public class GetSettingsPageHandler : IQueryHandler<GetSettingsPage, SettingsPageModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly ISender _sender;

        public GetSettingsPageHandler(AtlesDbContext dbContext, ISender sender)
        {
            _dbContext = dbContext;
            _sender = sender;
        }

        public async Task<SettingsPageModel> Handle(GetSettingsPage query)
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
                GravatarHash = _sender.Send(new GenerateEmailHashForGravatar { Email = user.Email }).GetAwaiter().GetResult(),
                IsSuspended = user.Status == UserStatusType.Suspended
            };

            return result;
        }
    }
}
