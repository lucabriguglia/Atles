namespace Atles.Validators.ValidationRules;

public interface ICategoryValidationRules
{
    Task<bool> IsCategoryNameUnique(Guid siteId, string name, Guid? id = null);
    Task<bool> IsCategoryValid(Guid siteId, Guid id);
}