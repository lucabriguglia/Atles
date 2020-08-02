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
