using Atlas.Domain.Users;
using Atlas.Domain.Users.Commands;
using Atlas.Domain.Users.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atlas.Domain.Tests.Users.Validators
{
    [TestFixture]
    public class CreateUserValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_user_id_is_empty()
        {
            var command = Fixture.Build<CreateUser>().With(x => x.IdentityUserId, string.Empty).Create();

            var userRules = new Mock<IUserRules>();

            var sut = new CreateUserValidator(userRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.IdentityUserId, command);
        }

        [Test]
        public void Should_have_validation_error_when_email_is_empty()
        {
            var command = Fixture.Build<CreateUser>().With(x => x.Email, string.Empty).Create();

            var userRules = new Mock<IUserRules>();

            var sut = new CreateUserValidator(userRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Email, command);
        }

        [Test]
        public void Should_have_validation_error_when_email_is_not_valid()
        {
            var command = Fixture.Build<CreateUser>().With(x => x.Email, "email").Create();

            var userRules = new Mock<IUserRules>();

            var sut = new CreateUserValidator(userRules.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Email, command);
        }
    }
}
