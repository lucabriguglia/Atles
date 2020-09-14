using Atles.Domain.PermissionSets;
using Atles.Domain.PermissionSets.Commands;
using Atles.Domain.PermissionSets.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Tests.PermissionSets.Validators
{
    [TestFixture]
    public class CreatePermissionSetValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<CreatePermissionSet>().With(x => x.Name, string.Empty).Create();

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreatePermissionSetValidator(permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<CreatePermissionSet>().With(x => x.Name, new string('*', 51)).Create();

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreatePermissionSetValidator(permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<CreatePermissionSet>();

            var permissionSetRules = new Mock<IPermissionSetRules>();
            permissionSetRules.Setup(x => x.IsNameUniqueAsync(command.SiteId, command.Name)).ReturnsAsync(false);

            var sut = new CreatePermissionSetValidator(permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }
    }
}
