using Atles.Models.Admin.Users;
using Atles.Validators.Users;
using Atles.Validators.ValidationRules;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators;

[TestFixture]
public class UpdateUserValidatorTests : TestFixtureBase
{
    [Test]
    public async Task Should_have_validation_error_when_display_name_is_empty()
    {
        var model = Fixture.Build<EditUserPageModel.UserModel>().With(x => x.DisplayName, string.Empty).Create();

        var userValidationRules = new Mock<IUserValidationRules>();

        var validator = new UpdateUserValidator(userValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Test]
    public async Task Should_have_validation_error_when_display_name_is_too_long()
    {
        var model = Fixture.Build<EditUserPageModel.UserModel>().With(x => x.DisplayName, new string('*', 51)).Create();

        var userValidationRules = new Mock<IUserValidationRules>();

        var validator = new UpdateUserValidator(userValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Test]
    public async Task Should_have_validation_error_when_display_name_is_not_unique()
    {
        var model = Fixture.Create<EditUserPageModel.UserModel>();

        var userValidationRules = new Mock<IUserValidationRules>();

        var validator = new UpdateUserValidator(userValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }
}
