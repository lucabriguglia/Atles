using Atles.Domain.Rules.Posts;

namespace Atles.Validators.Posts;

public interface ITopicValidationRules
{
    Task<bool> IsTopicValid(Guid siteId, Guid forumId, Guid id);
}