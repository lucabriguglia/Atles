using Atlas.Domain.Forums;
using Atlas.Domain.Forums.Commands;
using Atlas.Domain.Forums.Validators;
using Atlas.Domain.PermissionSets;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atlas.Tests.Domain.Forums.Validators
{
    [TestFixture]
    public class UpdateForumValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<UpdateForum>().With(x => x.Name, string.Empty).Create();

            var forumRules = new Mock<IForumRules>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<UpdateForum>().With(x => x.Name, new string('*', 101)).Create();

            var forumRules = new Mock<IForumRules>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<UpdateForum>();

            var forumRules = new Mock<IForumRules>();
            forumRules.Setup(x => x.IsNameUniqueAsync(command.SiteId, command.CategoryId, command.Name, command.Id)).ReturnsAsync(false);

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_permission_set_is_not_valid()
        {
            var command = Fixture.Create<UpdateForum>();

            var forumRules = new Mock<IForumRules>();

            var permissionSetRules = new Mock<IPermissionSetRules>();
            permissionSetRules.Setup(x => x.IsValid(command.SiteId, command.PermissionSetId.Value)).ReturnsAsync(false);

            var sut = new UpdateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.PermissionSetId, command);
        }
    }
}
