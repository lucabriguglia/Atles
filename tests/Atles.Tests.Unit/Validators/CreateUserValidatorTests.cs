using Atles.Models.Admin.Users;
using Atles.Validators.Users;
using Atles.Validators.ValidationRules;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators;

[TestFixture]
public class CreateUserValidatorTests : TestFixtureBase
{
    [Test]
    public async Task Should_have_validation_error_when_email_is_empty()
    {
        var model = Fixture.Build<CreateUserPageModel.UserModel>().With(x => x.Email, string.Empty).Create();

        var userValidationRules = new Mock<IUserValidationRules>();

        var sut = new CreateUserValidator(userValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Test]
    public async Task Should_have_validation_error_when_email_is_not_valid()
    {
        var model = Fixture.Build<CreateUserPageModel.UserModel>().With(x => x.Email, "email").Create();

        var userValidationRules = new Mock<IUserValidationRules>();

        var sut = new CreateUserValidator(userValidationRules.Object);

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
