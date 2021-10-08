using Atles.Domain.Forums.Commands;
using Atles.Domain.Forums.Rules;
using Atles.Domain.Forums.Validators;
using Atles.Domain.PermissionSets;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using OpenCqrs.Queries;

namespace Atles.Domain.Tests.Forums.Validators
{
    [TestFixture]
    public class CreateForumValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<CreateForum>().With(x => x.Name, string.Empty).Create();

            var queries = new Mock<IQuerySender>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(queries.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<CreateForum>().With(x => x.Name, new string('*', 51)).Create();

            var queries = new Mock<IQuerySender>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(queries.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<CreateForum>();

            var queries = new Mock<IQuerySender>();
            queries.Setup(x => x.Send(new IsForumNameUnique { SiteId = command.SiteId, CategoryId = command.CategoryId, Name = command.Name })).ReturnsAsync(false);

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(queries.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_slug_is_empty()
        {
            var command = Fixture.Build<CreateForum>().With(x => x.Slug, string.Empty).Create();

            var queries = new Mock<IQuerySender>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(queries.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Slug, command);
        }

        [Test]
        public void Should_have_validation_error_when_slug_is_too_long()
        {
            var command = Fixture.Build<CreateForum>().With(x => x.Slug, new string('*', 51)).Create();

            var queries = new Mock<IQuerySender>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(queries.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Slug, command);
        }

        [Test]
        public void Should_have_validation_error_when_slug_is_not_unique()
        {
            var command = Fixture.Create<CreateForum>();

            var queries = new Mock<IQuerySender>();
            queries.Setup(x => x.Send(new IsForumSlugUnique { SiteId = command.SiteId, Slug = command.Slug })).ReturnsAsync(false);

            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(queries.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Slug, command);
        }

        [Test]
        public void Should_have_validation_error_when_description_is_too_long()
        {
            var command = Fixture.Build<CreateForum>().With(x => x.Description, new string('*', 201)).Create();

            var queries = new Mock<IQuerySender>();
            var permissionSetRules = new Mock<IPermissionSetRules>();

            var sut = new CreateForumValidator(queries.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Description, command);
        }

        [Test]
        public void Should_have_validation_error_when_permission_set_is_not_valid()
        {
            var command = Fixture.Create<CreateForum>();

            var queries = new Mock<IQuerySender>();

            var permissionSetRules = new Mock<IPermissionSetRules>();
            permissionSetRules.Setup(x => x.IsValidAsync(command.SiteId, command.PermissionSetId.Value)).ReturnsAsync(false);

            var sut = new CreateForumValidator(queries.Object, permissionSetRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.PermissionSetId, command);
        }
    }
}
