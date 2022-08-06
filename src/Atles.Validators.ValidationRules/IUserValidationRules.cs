namespace Atles.Validators.ValidationRules;

public interface IUserValidationRules
{
    Task<bool> IsUserDisplayNameUnique(Guid id, string displayName);
}
