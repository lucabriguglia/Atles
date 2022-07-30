using System;

namespace Atles.Data.Caching
{
    public static class CacheKeys
    {
        public static string CurrentSite(string siteName) => $"Current Site | SiteNme: {siteName}";
        public static string CurrentForums(Guid siteId) => $"Current Forums | SiteId: {siteId}";
        public static string Categories(Guid siteId) => $"Categories | SiteId: {siteId}";
        public static string Forum(Guid forumId) => $"Forum | ForumId: {forumId}";
        public static string PermissionSet(Guid permissionSetId) => $"PermissionSet | PermissionSetId: {permissionSetId}";
        public static string UserRanks(Guid siteId) => $"UserRanks | SiteId: {siteId}";
    }
}
