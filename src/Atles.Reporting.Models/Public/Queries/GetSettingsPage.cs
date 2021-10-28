using System;
using Atles.Infrastructure.Queries;

namespace Atles.Reporting.Models.Public.Queries
{
    public class GetSettingsPage : QueryBase<SettingsPageModel>
    {
        public Guid UserId { get; set; }
    }
}
