using Atles.Infrastructure.Queries;
using Atles.Models.Public.Users;
using System;

namespace Atles.Reporting.Public.Queries
{
    public class GetSettingsPage : QueryBase<SettingsPageModel>
    {
        public Guid UserId { get; set; }
    }
}
