using Atlas.Domain.Categories;
using Atlas.Domain.Categories.Commands;
using Atlas.Domain.Categories.Validators;
using Atlas.Domain.PermissionSets;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atlas.Domain.Tests.Categories.Validators
{
    [TestFixture]
    public class UpdateCategoryValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<UpdateCategory>().With(x => x.Name, string.Empty).Create();

            var categoryRules = new Mock<ICategoryRules>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateCategoryValidator(categoryRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<UpdateCategory>().With(x => x.Name, new string('*', 51)).Create();

            var categoryRules = new Mock<ICategoryRules>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateCategoryValidator(categoryRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<UpdateCategory>();

            var categoryRules = new Mock<ICategoryRules>();
            categoryRules.Setup(x => x.IsNameUniqueAsync(command.SiteId, command.Name, command.Id)).ReturnsAsync(false);

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateCategoryValidator(categoryRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_permission_set_is_not_valid()
        {
            var command = Fixture.Create<UpdateCategory>();

            var categoryRules = new Mock<ICategoryRules>();

            var permissionSetRules = new Mock<IPermissionSetRules>();
            permissionSetRules.Setup(x => x.IsValidAsync(command.SiteId, command.PermissionSetId)).ReturnsAsync(false);

            var sut = new UpdateCategoryValidator(categoryRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.PermissionSetId, command);
        }
    }
}
