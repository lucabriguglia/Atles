using Atles.Models.Admin.Sites;
using Atles.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators;

[TestFixture]
public class UpdateSiteValidatorTests : TestFixtureBase
{
    [Test]
    public async Task Should_have_validation_error_when_title_is_empty()
    {
        var model = Fixture.Build<SettingsPageModel.SiteModel>().With(x => x.Title, string.Empty).Create();

        var sut = new SiteValidator();

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    public async Task Should_have_validation_error_when_title_is_too_long()
    {
        var model = Fixture.Build<SettingsPageModel.SiteModel>().With(x => x.Title, new string('*', 51)).Create();

        var sut = new SiteValidator();

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Test]
    public async Task Should_have_validation_error_when_theme_is_empty()
    {
        var model = Fixture.Build<SettingsPageModel.SiteModel>().With(x => x.Theme, string.Empty).Create();

        var sut = new SiteValidator();

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Theme);
    }

    [Test]
    public async Task Should_have_validation_error_when_theme_is_too_long()
    {
        var model = Fixture.Build<SettingsPageModel.SiteModel>().With(x => x.Theme, new string('*', 251)).Create();

        var sut = new SiteValidator();

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Theme);
    }

    [Test]
    public async Task Should_have_validation_error_when_css_is_empty()
    {
        var model = Fixture.Build<SettingsPageModel.SiteModel>().With(x => x.Css, string.Empty).Create();

        var sut = new SiteValidator();

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Css);
    }

    [Test]
    public async Task Should_have_validation_error_when_css_is_too_long()
    {
        var model = Fixture.Build<SettingsPageModel.SiteModel>().With(x => x.Css, new string('*', 251)).Create();

        var sut = new SiteValidator();

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Css);
    }

    [Test]
    public async Task Should_have_validation_error_when_language_is_empty()
    {
        var model = Fixture.Build<SettingsPageModel.SiteModel>().With(x => x.Language, string.Empty).Create();

        var sut = new SiteValidator();

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Language);
    }

    [Test]
    public async Task Should_have_validation_error_when_language_is_too_long()
    {
        var model = Fixture.Build<SettingsPageModel.SiteModel>().With(x => x.Language, new string('*', 11)).Create();

        var sut = new SiteValidator();

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Language);
    }

    [Test]
    public async Task Should_have_validation_error_when_privacy_is_empty()
    {
        var model = Fixture.Build<SettingsPageModel.SiteModel>().With(x => x.Privacy, string.Empty).Create();

        var sut = new SiteValidator();

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Privacy);
    }

    [Test]
    public async Task Should_have_validation_error_when_terms_is_empty()
    {
        var model = Fixture.Build<SettingsPageModel.SiteModel>().With(x => x.Terms, string.Empty).Create();

        var sut = new SiteValidator();

        var result = await sut.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Terms);
    }
}
