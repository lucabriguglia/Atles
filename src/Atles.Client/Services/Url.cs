namespace Atles.Client.Services
{
    public static class Url
    {
        public static string Forum(string slug) => $"/forum/{slug}";
        public static string Topic(string forumSlug, string topicSlug) => $"/forum/{forumSlug}/{topicSlug}";
    }
}
