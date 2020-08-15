using Atlas.Domain.Forums;
using Atlas.Domain.Forums.Commands;
using Atlas.Domain.Forums.Validators;
using Atlas.Domain.PermissionSets;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atlas.Domain.Tests.Forums.Validators
{
    [TestFixture]
    public class CreateForumValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<CreateForum>().With(x => x.Name, string.Empty).Create();

            var forumRules = new Mock<IForumRules>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<CreateForum>().With(x => x.Name, new string('*', 51)).Create();

            var forumRules = new Mock<IForumRules>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<CreateForum>();

            var forumRules = new Mock<IForumRules>();
            forumRules.Setup(x => x.IsNameUniqueAsync(command.SiteId, command.CategoryId, command.Name)).ReturnsAsync(false);

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_slug_is_empty()
        {
            var command = Fixture.Build<CreateForum>().With(x => x.Slug, string.Empty).Create();

            var forumRules = new Mock<IForumRules>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Slug, command);
        }

        [Test]
        public void Should_have_validation_error_when_slug_is_too_long()
        {
            var command = Fixture.Build<CreateForum>().With(x => x.Slug, new string('*', 51)).Create();

            var forumRules = new Mock<IForumRules>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Slug, command);
        }

        [Test]
        public void Should_have_validation_error_when_slug_is_not_unique()
        {
            var command = Fixture.Create<CreateForum>();

            var forumRules = new Mock<IForumRules>();
            forumRules.Setup(x => x.IsSlugUniqueAsync(command.SiteId, command.Slug)).ReturnsAsync(false);

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Slug, command);
        }

        [Test]
        public void Should_have_validation_error_when_description_is_too_long()
        {
            var command = Fixture.Build<CreateForum>().With(x => x.Description, new string('*', 201)).Create();

            var forumRules = new Mock<IForumRules>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Description, command);
        }

        [Test]
        public void Should_have_validation_error_when_permission_set_is_not_valid()
        {
            var command = Fixture.Create<CreateForum>();

            var forumRules = new Mock<IForumRules>();

            var permissionSetRules = new Mock<IPermissionSetRules>();
            permissionSetRules.Setup(x => x.IsValidAsync(command.SiteId, command.PermissionSetId.Value)).ReturnsAsync(false);

            var sut = new CreateForumValidator(forumRules.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.PermissionSetId, command);
        }
    }
}
