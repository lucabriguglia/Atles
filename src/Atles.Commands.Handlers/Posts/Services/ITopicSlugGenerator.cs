namespace Atles.Commands.Handlers.Posts.Services
{
    public interface ITopicSlugGenerator
    {
        Task<string> GenerateTopicSlug(Guid forumId, string title);
    }
}
