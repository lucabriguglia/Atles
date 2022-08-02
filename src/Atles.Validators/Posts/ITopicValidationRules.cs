namespace Atles.Validators.Posts;

public interface ITopicValidationRules
{
    Task<bool> IsTopicValid(Guid siteId, Guid forumId, Guid id);
}