using Atles.Models.Admin.Users;
using Atles.Validators.ValidationRules;
using FluentValidation;

namespace Atles.Validators.Users;

public class CreateUserValidator : AbstractValidator<CreateUserPageModel.UserModel>
{
    private readonly string[] _blackListedWords = { "Password" };

    public CreateUserValidator(IUserValidationRules userValidationRules)
    {
        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email not valid.");

        RuleFor(model => model.Email)
            .MustAsync(UserEmailBeUnique).WithMessage(model => $"Email {model.Email} is already in use.");

        // https://stackoverflow.com/questions/64205825/regex-with-fluent-validation-in-c-sharp-how-to-not-allow-spaces-and-certain-sp
        RuleFor(model => model.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 100).WithMessage("Password must be at least 6 and at max 100 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain one or more capital letters.")
            .Matches("[a-z]").WithMessage("Password must contain one or more lowercase letters.")
            .Matches(@"\d").WithMessage("Password must contain one or more digits.")
            .Matches(@"[][""!@$%^&*#£(){}:;<>,.?/+_=|'~\\-]").WithMessage("Password must contain one or more special characters.")
            .Matches("^[^ “”]*$").WithMessage("Password must not contain the characters “” or spaces.")
            .Must(password => !_blackListedWords.Any(word => password.Contains(word, StringComparison.OrdinalIgnoreCase))).WithMessage("Password contains a word that is not allowed.");

        RuleFor(model => model.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(model => model.Password).WithMessage("Confirm password does not match password.");

        async Task<bool> UserEmailBeUnique(CreateUserPageModel.UserModel model, string email, CancellationToken cancellation) =>
            await userValidationRules.IsUserEmailUnique(Guid.Empty, email);
    }
}
