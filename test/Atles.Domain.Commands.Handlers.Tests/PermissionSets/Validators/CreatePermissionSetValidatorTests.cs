using Atles.Core;
using Atles.Core.Queries;
using Atles.Domain.Commands.Handlers.PermissionSets.Validators;
using Atles.Domain.Commands.PermissionSets;
using Atles.Domain.Rules;
using Atles.Domain.Rules.PermissionSets;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Commands.Handlers.Tests.PermissionSets.Validators
{
    [TestFixture]
    public class CreatePermissionSetValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<CreatePermissionSet>().With(x => x.Name, string.Empty).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new CreatePermissionSetValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<CreatePermissionSet>().With(x => x.Name, new string('*', 51)).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new CreatePermissionSetValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<CreatePermissionSet>();

            var querySiteId = Guid.NewGuid();
            var queryName = string.Empty;

            var dispatcher = new Mock<IDispatcher>();
            dispatcher
                .Setup(x => x.Get(It.IsAny<IsPermissionSetNameUnique>()))
                .Callback<IQuery<bool>>(q =>
                {
                    var query = q as IsPermissionSetNameUnique;
                    querySiteId = query.SiteId;
                    queryName = query.Name;
                })
                .ReturnsAsync(false);

            var sut = new CreatePermissionSetValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
            Assert.AreEqual(command.SiteId, querySiteId);
            Assert.AreEqual(command.Name, queryName);
        }
    }
}
