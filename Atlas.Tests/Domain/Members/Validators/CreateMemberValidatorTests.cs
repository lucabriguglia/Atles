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
    public class CreateMemberValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_display_name_is_empty()
        {
            var command = Fixture.Build<CreateMember>().With(x => x.DisplayName, string.Empty).Create();

            var memberRules = new Mock<IMemberRules>();

            var sut = new CreateMemberValidator(memberRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }

        [Test]
        public void Should_have_validation_error_when_display_name_is_too_long()
        {
            var command = Fixture.Build<CreateMember>().With(x => x.DisplayName, new string('*', 51)).Create();

            var memberRules = new Mock<IMemberRules>();

            var sut = new CreateMemberValidator(memberRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }

        [Test]
        public void Should_have_validation_error_when_display_name_is_not_unique()
        {
            var command = Fixture.Create<CreateMember>();

            var memberRules = new Mock<IMemberRules>();
            memberRules.Setup(x => x.IsDisplayNameUniqueAsync(command.DisplayName)).ReturnsAsync(false);

            var sut = new CreateMemberValidator(memberRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.DisplayName, command);
        }

        [Test]
        public void Should_have_validation_error_when_user_id_is_empty()
        {
            var command = Fixture.Build<CreateMember>().With(x => x.UserId, string.Empty).Create();

            var memberRules = new Mock<IMemberRules>();

            var sut = new CreateMemberValidator(memberRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.UserId, command);
        }

        [Test]
        public void Should_have_validation_error_when_email_is_empty()
        {
            var command = Fixture.Build<CreateMember>().With(x => x.Email, string.Empty).Create();

            var memberRules = new Mock<IMemberRules>();

            var sut = new CreateMemberValidator(memberRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Email, command);
        }

        [Test]
        public void Should_have_validation_error_when_email_is_not_valid()
        {
            var command = Fixture.Build<CreateMember>().With(x => x.Email, "email").Create();

            var memberRules = new Mock<IMemberRules>();

            var sut = new CreateMemberValidator(memberRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Email, command);
        }
    }
}
