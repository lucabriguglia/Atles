namespace Atles.Validators.ValidationRules;

public interface IPermissionSetValidationRules
{
    Task<bool> IsPermissionSetNameUnique(Guid siteId, Guid id, string name);
    Task<bool> IsPermissionSetValid(Guid siteId, Guid id);
}