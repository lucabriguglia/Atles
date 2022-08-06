namespace Atles.Validators.ValidationRules;

public interface IUserValidationRules
{
    Task<bool> IsUserDisplayNameUnique(string displayName, Guid? id = null);
}
