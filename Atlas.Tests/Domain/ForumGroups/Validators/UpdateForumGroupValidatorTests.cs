using Atlas.Domain.ForumGroups;
using Atlas.Domain.ForumGroups.Commands;
using Atlas.Domain.ForumGroups.Validators;
using Atlas.Domain.PermissionSets;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atlas.Tests.Domain.ForumGroups.Validators
{
    [TestFixture]
    public class UpdateForumGroupValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<UpdateForumGroup>().With(x => x.Name, string.Empty).Create();

            var forumGroupRules = new Mock<IForumGroupRules>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateForumGroupValidator(forumGroupRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<UpdateForumGroup>().With(x => x.Name, new string('*', 101)).Create();

            var forumGroupRules = new Mock<IForumGroupRules>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateForumGroupValidator(forumGroupRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<UpdateForumGroup>();

            var forumGroupRules = new Mock<IForumGroupRules>();
            forumGroupRules.Setup(x => x.IsNameUniqueAsync(command.SiteId, command.Name, command.Id)).ReturnsAsync(false);

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new UpdateForumGroupValidator(forumGroupRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_permission_set_is_not_valid()
        {
            var command = Fixture.Create<UpdateForumGroup>();

            var forumGroupRules = new Mock<IForumGroupRules>();

            var permissionSetRules = new Mock<IPermissionSetRules>();
            permissionSetRules.Setup(x => x.IsValid(command.SiteId, command.PermissionSetId.Value)).ReturnsAsync(false);

            var sut = new UpdateForumGroupValidator(forumGroupRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.PermissionSetId, command);
        }
    }
}
