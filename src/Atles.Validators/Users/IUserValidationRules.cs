namespace Atles.Validators.Users;

public interface IUserValidationRules
{
    Task<bool> IsUserDisplayNameUnique(string displayName, Guid? id = null);
}
