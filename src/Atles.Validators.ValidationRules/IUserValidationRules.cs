namespace Atles.Validators.ValidationRules;

public interface IUserValidationRules
{
    Task<bool> IsUserEmailUnique(Guid id, string email);
    Task<bool> IsUserDisplayNameUnique(Guid id, string displayName);
}
