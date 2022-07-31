namespace Atles.Validators.Categories;

public interface ICategoryValidationRules
{
    Task<bool> IsCategoryNameUnique(Guid siteId, string name, Guid? id = null);
}