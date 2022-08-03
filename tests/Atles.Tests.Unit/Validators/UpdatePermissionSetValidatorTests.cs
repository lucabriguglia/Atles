using Atles.Commands.PermissionSets;
using Atles.Validators.PermissionSets;
using Atles.Validators.ValidationRules;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators;

[TestFixture]
public class UpdatePermissionSetValidatorTests : TestFixtureBase
{
    [Test]
    public async Task Should_have_validation_error_when_name_is_empty()
    {
        var model = Fixture.Build<UpdatePermissionSet>().With(x => x.Name, string.Empty).Create();

        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new UpdatePermissionSetValidator(permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_name_is_too_long()
    {
        var model = Fixture.Build<UpdatePermissionSet>().With(x => x.Name, new string('*', 51)).Create();

        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new UpdatePermissionSetValidator(permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_name_is_not_unique()
    {
        var model = Fixture.Create<UpdatePermissionSet>();

        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();
        permissionSetValidationRules
            .Setup(rules => rules.IsPermissionSetNameUnique(model.SiteId, model.Name, model.PermissionSetId))
            .ReturnsAsync(false);

        var validator = new UpdatePermissionSetValidator(permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
