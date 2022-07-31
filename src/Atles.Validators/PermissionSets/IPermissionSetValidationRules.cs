namespace Atles.Validators.PermissionSets;

public interface IPermissionSetValidationRules
{
    Task<bool> IsPermissionSetValid(Guid siteId, Guid id);
}