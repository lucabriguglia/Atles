namespace Atles.Validators.PermissionSets;

public interface IPermissionSetValidationRules
{
    Task<bool> IsPermissionSetNameUnique(Guid siteId, string name, Guid? id = null);
    Task<bool> IsPermissionSetValid(Guid siteId, Guid id);
}