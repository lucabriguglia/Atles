using Atlas.Domain.Members;
using Atlas.Domain.Members.Commands;
using Atlas.Domain.Members.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atlas.Tests.Domain.Members.Validators
{
    [TestFixture]
    public class UpdateMemberValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_display_name_is_empty()
        {
            var command = Fixture.Build<UpdateMember>().With(x => x.DisplayName, string.Empty).Create();

            var memberRules = new Mock<IMemberRules>();

            var sut = new UpdateMemberValidator(memberRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }

        [Test]
        public void Should_have_validation_error_when_display_name_is_too_long()
        {
            var command = Fixture.Build<UpdateMember>().With(x => x.DisplayName, new string('*', 51)).Create();

            var memberRules = new Mock<IMemberRules>();

            var sut = new UpdateMemberValidator(memberRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }

        [Test]
        public void Should_have_validation_error_when_display_name_is_not_unique()
        {
            var command = Fixture.Create<UpdateMember>();

            var memberRules = new Mock<IMemberRules>();
            memberRules.Setup(x => x.IsDisplayNameUniqueAsync(command.DisplayName, command.Id)).ReturnsAsync(false);

            var sut = new UpdateMemberValidator(memberRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }
    }
}
