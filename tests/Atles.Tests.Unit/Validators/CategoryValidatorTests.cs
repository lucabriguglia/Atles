using Atles.Models.Admin.Categories;
using Atles.Validators;
using Atles.Validators.ValidationRules;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators;

[TestFixture]
public class CategoryValidatorTests : TestFixtureBase
{
    [Test]
    public async Task Should_have_validation_error_when_name_is_empty()
    {
        var model = Fixture.Build<CategoryFormModel.CategoryModel>().With(x => x.Name, string.Empty).Create();

        var categoryValidationRules = new Mock<ICategoryValidationRules>();
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new CategoryValidator(categoryValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_name_is_too_long()
    {
        var model = Fixture.Build<CategoryFormModel.CategoryModel>().With(x => x.Name, new string('*', 51)).Create();

        var categoryValidationRules = new Mock<ICategoryValidationRules>();
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new CategoryValidator(categoryValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_name_is_not_unique()
    {
        var model = Fixture.Create<CategoryFormModel.CategoryModel>();

        var categoryValidationRules = new Mock<ICategoryValidationRules>();
        categoryValidationRules
            .Setup(rules => rules.IsCategoryNameUnique(model.SiteId, model.Id, model.Name))
            .ReturnsAsync(false);
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new CategoryValidator(categoryValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_permission_set_is_not_valid()
    {
        var model = Fixture.Create<CategoryFormModel.CategoryModel>();

        var categoryValidationRules = new Mock<ICategoryValidationRules>();
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();
        permissionSetValidationRules
            .Setup(rules => rules.IsPermissionSetValid(model.SiteId, model.PermissionSetId))
            .ReturnsAsync(false);

        var validator = new CategoryValidator(categoryValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.PermissionSetId);
    }
}