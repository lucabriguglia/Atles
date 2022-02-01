using Atles.Domain.Commands.Handlers.Posts.Validators;
using Atles.Domain.Commands.Posts;
using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Atles.Domain.Commands.Handlers.Tests.Posts.Validators
{
    [TestFixture]
    public class UpdateReplyValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_content_is_empty()
        {
            var command = Fixture.Build<UpdateReply>().With(x => x.Content, string.Empty).Create();

            var sut = new UpdateReplyValidator();

            sut.ShouldHaveValidationErrorFor(x => x.Content, command);
        }
    }
}
