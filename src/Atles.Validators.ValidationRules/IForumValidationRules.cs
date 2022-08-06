namespace Atles.Validators.ValidationRules;

public interface IForumValidationRules
{
    Task<bool> IsForumNameUnique(Guid siteId, Guid categoryId, Guid id, string name);
    Task<bool> IsForumSlugUnique(Guid siteId, Guid id, string slug);
    Task<bool> IsForumValid(Guid siteId, Guid id);
}