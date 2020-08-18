using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlify.Models.Public.Members
{
    public interface IMemberModelBuilder
    {
        Task<MemberPageModel> BuildMemberPageModelAsync(Guid memberId, IList<Guid> forumIds);
        Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid memberId);
    }
}