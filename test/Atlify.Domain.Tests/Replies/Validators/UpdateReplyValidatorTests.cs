using Atlify.Domain.Posts.Commands;
using Atlify.Domain.Posts.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Atlify.Domain.Tests.Replies.Validators
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
