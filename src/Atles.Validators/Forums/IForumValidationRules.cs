namespace Atles.Validators.Forums;

public interface IForumValidationRules
{
    Task<bool> IsForumNameUnique(Guid siteId, Guid categoryId, string name, Guid? id = null);
    Task<bool> IsForumSlugUnique(Guid siteId, string slug, Guid? id = null);
    Task<bool> IsForumValid(Guid siteId, Guid id);
}