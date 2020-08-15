using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Models.Public.Members
{
    public interface IMemberModelBuilder
    {
        Task<MemberPageModel> BuildMemberPageModelAsync(Guid memberId, IList<Guid> forumIds);
        Task<SettingsPageModel> BuildSettingsPageModelAsync(Guid memberId);
    }
}