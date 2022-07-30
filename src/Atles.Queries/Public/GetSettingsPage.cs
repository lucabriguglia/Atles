using Atles.Core.Queries;
using Atles.Models.Public;

namespace Atles.Queries.Public
{
    public class GetSettingsPage : QueryBase<SettingsPageModel>
    {
        public Guid UserId { get; set; }
    }
}
