using System;

namespace Atlas.Caching
{
    public static class CacheKeys
    {
        public static string Site(string name) => $"Site | Site: {name}";
        public static string ForumGroups(Guid siteId) => $"ForumGroups | Site: {siteId.ToString()}";
        public static string Forums(Guid forumGroupId) => $"Forums | ForumGroup: {forumGroupId.ToString()}";
    }
}
