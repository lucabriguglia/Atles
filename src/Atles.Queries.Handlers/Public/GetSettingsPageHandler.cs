﻿using System.Threading.Tasks;
using Atles.Core;
using Atles.Core.Queries;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Public;
using Atles.Queries.Handlers.Services;
using Atles.Queries.Public;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Public
{
    public class GetSettingsPageHandler : IQueryHandler<GetSettingsPage, SettingsPageModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IDispatcher _dispatcher;
        private readonly IGravatarService _gravatarService;
        public GetSettingsPageHandler(AtlesDbContext dbContext, IDispatcher sender, IGravatarService gravatarService)
        {
            _dbContext = dbContext;
            _dispatcher = sender;
            _gravatarService = gravatarService;
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
                GravatarHash = _gravatarService.GenerateEmailHash(user.Email),
                IsSuspended = user.Status == UserStatusType.Suspended
            };

            return result;
        }
    }
}
