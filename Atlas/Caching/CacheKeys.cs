using System;

namespace Atlas.Caching
{
    public static class CacheKeys
    {
        public static string ForumGroups(Guid siteId) => $"ForumGroups | Site: {siteId}";
        public static string Forums(Guid forumGroupId) => $"Forums | ForumGroup: {forumGroupId}";
    }
}
