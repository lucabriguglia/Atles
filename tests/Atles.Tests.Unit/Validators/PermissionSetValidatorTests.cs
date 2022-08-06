using Atles.Models.Admin.PermissionSets;
using Atles.Validators;
using Atles.Validators.ValidationRules;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators;

[TestFixture]
public class PermissionSetValidatorTests : TestFixtureBase
{
    [Test]
    public async Task Should_have_validation_error_when_name_is_empty()
    {
        var model = Fixture.Build<PermissionSetFormModel.PermissionSetModel>().With(x => x.Name, string.Empty).Create();

        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new PermissionSetValidator(permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_name_is_too_long()
    {
        var model = Fixture.Build<PermissionSetFormModel.PermissionSetModel>().With(x => x.Name, new string('*', 51)).Create();

        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new PermissionSetValidator(permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_name_is_not_unique()
    {
        var model = Fixture.Create<PermissionSetFormModel.PermissionSetModel>();

        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();
        permissionSetValidationRules
            .Setup(rules => rules.IsPermissionSetNameUnique(model.SiteId, model.Id, model.Name))
            .ReturnsAsync(false);

        var validator = new PermissionSetValidator(permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
