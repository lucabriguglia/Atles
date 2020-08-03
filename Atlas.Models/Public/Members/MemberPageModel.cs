using System;
using System.Collections.Generic;

namespace Atlas.Models.Public.Members
{
    public class MemberPageModelToFilter
    {
        public MemberModel Member { get; set; } = new MemberModel();

        public MemberTopicModelsToFilter MemberTopicModelsToFilter { get; set; } = new MemberTopicModelsToFilter();

        public int TotalMemberTopics { get; set; }
    }

    public class MemberTopicModelsToFilter
    {
        public IList<MemberTopicModel> Topics { get; set; } = new List<MemberTopicModel>();

        public IList<TopicPermission> TopicPermissions { get; set; } = new List<TopicPermission>();
    }

    public class MemberPageModel
    {
        public MemberModel Member { get; set; } = new MemberModel();

        public IList<MemberTopicModel> LastTopics { get; set; } = new List<MemberTopicModel>();
    }

    public class MemberModel
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public int TotalTopics { get; set; }
        public int TotalReplies { get; set; }
        public string GravatarHash { get; set; }
    }

    public class MemberTopicModel
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public string Title { get; set; }
        public DateTime TimeStamp { get; set; }
        public int TotalReplies { get; set; }
        public bool CanRead { get; set; }
    }

    public class TopicPermission
    {
        public Guid TopicId { get; set; }
        public Guid PermissionSetId { get; set; }
    }
}
