using System;

namespace Atlas.Server.Caching
{
    public static class CacheKeys
    {
        public static string Site(string siteName) => $"Site | SiteNme: {siteName}";
        public static string ForumGroups(Guid siteId) => $"ForumGroups | SiteId: {siteId.ToString()}";
        public static string Forums(Guid forumGroupId) => $"Forums | ForumGroupId: {forumGroupId.ToString()}";
    }
}
