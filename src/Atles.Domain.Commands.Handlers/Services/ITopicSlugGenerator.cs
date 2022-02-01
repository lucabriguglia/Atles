namespace Atles.Domain.Commands.Handlers.Services
{
    public interface ITopicSlugGenerator
    {
        Task<string> GenerateTopicSlug(Guid forumId, string title);
    }
}
