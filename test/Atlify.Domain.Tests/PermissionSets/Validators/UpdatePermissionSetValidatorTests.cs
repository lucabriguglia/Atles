using Atlify.Domain.PermissionSets;
using Atlify.Domain.PermissionSets.Commands;
using Atlify.Domain.PermissionSets.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atlify.Domain.Tests.PermissionSets.Validators
{
    [TestFixture]
    public class UpdatePermissionSetValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<UpdatePermissionSet>().With(x => x.Name, string.Empty).Create();

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdatePermissionSetValidator(permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<UpdatePermissionSet>().With(x => x.Name, new string('*', 51)).Create();

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdatePermissionSetValidator(permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<UpdatePermissionSet>();

            var permissionSetRules = new Mock<IPermissionSetRules>();
            permissionSetRules.Setup(x => x.IsNameUniqueAsync(command.SiteId, command.Name, command.Id)).ReturnsAsync(false);

            var sut = new UpdatePermissionSetValidator(permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }
    }
}
