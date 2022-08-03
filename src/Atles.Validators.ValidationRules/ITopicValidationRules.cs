namespace Atles.Validators.ValidationRules;

public interface ITopicValidationRules
{
    Task<bool> IsTopicValid(Guid siteId, Guid forumId, Guid id);
}