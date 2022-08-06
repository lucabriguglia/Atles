namespace Atles.Validators.ValidationRules;

public interface ICategoryValidationRules
{
    Task<bool> IsCategoryNameUnique(Guid siteId, Guid id, string name);
    Task<bool> IsCategoryValid(Guid siteId, Guid id);
}