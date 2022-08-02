using Atles.Commands.UserRanks;
using Atles.Validators.UserRanks;
using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Atles.Tests.Unit.Validators;

[Ignore("WIP")]
[TestFixture]
public class CreateUserRankValidatorTests : TestFixtureBase
{
    [Test]
    public async Task Should_have_validation_error_when_name_is_empty()
    {
        var model = Fixture.Build<CreateUserRank>().With(x => x.Name, string.Empty).Create();

        var validator = new CreateUserRankValidator();

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_name_is_too_long()
    {
        var model = Fixture.Build<CreateUserRank>().With(x => x.Name, new string('*', 51)).Create();

        var validator = new CreateUserRankValidator();

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_have_validation_error_when_name_is_not_unique()
    {
        var model = Fixture.Create<CreateUserRank>();

        var validator = new CreateUserRankValidator();

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task Should_not_have_validation_error_when_description_is_empty()
    {
        var model = Fixture.Build<CreateUserRank>().With(x => x.Description, string.Empty).Create();

        var validator = new CreateUserRankValidator();

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Test]
    public async Task Should_have_validation_error_when_description_is_too_long()
    {
        var model = Fixture.Build<CreateUserRank>().With(x => x.Description, new string('*', 51)).Create();

        var validator = new CreateUserRankValidator();

        var result = await validator.TestValidateAsync(model);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
