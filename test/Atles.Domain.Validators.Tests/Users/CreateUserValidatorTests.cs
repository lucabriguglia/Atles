using Atles.Domain.Models.Users.Commands;
using Atles.Domain.Validators.Users;
using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Atles.Domain.Validators.Tests.Users
{
    [TestFixture]
    public class CreateUserValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_user_id_is_empty()
        {
            var command = Fixture.Build<CreateUser>().With(x => x.IdentityUserId, string.Empty).Create();

            var sut = new CreateUserValidator();

            sut.ShouldHaveValidationErrorFor(x => x.IdentityUserId, command);
        }

        [Test]
        public void Should_have_validation_error_when_email_is_empty()
        {
            var command = Fixture.Build<CreateUser>().With(x => x.Email, string.Empty).Create();

            var sut = new CreateUserValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Email, command);
        }

        [Test]
        public void Should_have_validation_error_when_email_is_not_valid()
        {
            var command = Fixture.Build<CreateUser>().With(x => x.Email, "email").Create();

            var sut = new CreateUserValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Email, command);
        }
    }
}
