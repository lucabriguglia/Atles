using Atles.Commands.Forums;
using Atles.Validators.Forums;
using Atles.Validators.PermissionSets;
using Atles.Validators.ValidationRules;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators;

[TestFixture]
public class UpdateForumValidatorTests : TestFixtureBase
{
    [Test]
    public async Task Should_have_validation_error_when_name_is_empty()
    {
        var model = Fixture.Build<UpdateForum>().With(x => x.Name, string.Empty).Create();

        var forumValidationRules = new Mock<IForumValidationRules>();
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new UpdateForumValidator(forumValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_name_is_too_long()
    {
        var model = Fixture.Build<UpdateForum>().With(x => x.Name, new string('*', 51)).Create();

        var forumValidationRules = new Mock<IForumValidationRules>();
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new UpdateForumValidator(forumValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_name_is_not_unique()
    {
        var model = Fixture.Create<UpdateForum>();

        var forumValidationRules = new Mock<IForumValidationRules>();
        forumValidationRules
            .Setup(rules => rules.IsForumNameUnique(model.SiteId, model.CategoryId, model.Name, model.ForumId))
            .ReturnsAsync(false);
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new UpdateForumValidator(forumValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_slug_is_empty()
    {
        var model = Fixture.Build<UpdateForum>().With(x => x.Slug, string.Empty).Create();

        var forumValidationRules = new Mock<IForumValidationRules>();
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new UpdateForumValidator(forumValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Slug);
    }

    [Test]
    public async Task Should_have_validation_error_when_slug_is_too_long()
    {
        var model = Fixture.Build<UpdateForum>().With(x => x.Slug, new string('*', 51)).Create();

        var forumValidationRules = new Mock<IForumValidationRules>();
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new UpdateForumValidator(forumValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Slug);
    }

    [Test]
    public async Task Should_have_validation_error_when_slug_is_not_unique()
    {
        var model = Fixture.Create<UpdateForum>();

        var forumValidationRules = new Mock<IForumValidationRules>();
        forumValidationRules
            .Setup(rules => rules.IsForumSlugUnique(model.SiteId, model.Slug, model.ForumId))
            .ReturnsAsync(false);
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new UpdateForumValidator(forumValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Slug);
    }

    [Test]
    public async Task Should_have_validation_error_when_description_is_too_long()
    {
        var model = Fixture.Build<UpdateForum>().With(x => x.Description, new string('*', 201)).Create();

        var forumValidationRules = new Mock<IForumValidationRules>();
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();

        var validator = new UpdateForumValidator(forumValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public async Task Should_have_validation_error_when_permission_set_is_not_valid()
    {
        var model = Fixture.Create<UpdateForum>();

        var forumValidationRules = new Mock<IForumValidationRules>();
        var permissionSetValidationRules = new Mock<IPermissionSetValidationRules>();
        permissionSetValidationRules
            .Setup(rules => rules.IsPermissionSetValid(model.SiteId, model.PermissionSetId.Value))
            .ReturnsAsync(false);

        var validator = new UpdateForumValidator(forumValidationRules.Object, permissionSetValidationRules.Object);

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.PermissionSetId);
    }
}