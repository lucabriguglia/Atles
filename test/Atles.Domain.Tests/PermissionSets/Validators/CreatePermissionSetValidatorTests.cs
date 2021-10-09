using Atles.Domain.PermissionSets.Commands;
using Atles.Domain.PermissionSets.Rules;
using Atles.Domain.PermissionSets.Validators;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using OpenCqrs;
using OpenCqrs.Queries;
using System;

namespace Atles.Domain.Tests.PermissionSets.Validators
{
    [TestFixture]
    public class CreatePermissionSetValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<CreatePermissionSet>().With(x => x.Name, string.Empty).Create();

            var sender = new Mock<ISender>();

            var sut = new CreatePermissionSetValidator(sender.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<CreatePermissionSet>().With(x => x.Name, new string('*', 51)).Create();

            var sender = new Mock<ISender>();

            var sut = new CreatePermissionSetValidator(sender.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<CreatePermissionSet>();

            var querySiteId = Guid.NewGuid();
            var queryName = string.Empty;

            var sender = new Mock<ISender>();
            sender
                .Setup(x => x.Send(It.IsAny<IsPermissionSetNameUnique>()))
                .Callback<IQuery<bool>>(q =>
                {
                    var query = q as IsPermissionSetNameUnique;
                    querySiteId = query.SiteId;
                    queryName = query.Name;
                })
                .ReturnsAsync(false);

            var sut = new CreatePermissionSetValidator(sender.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
            Assert.AreEqual(command.SiteId, querySiteId);
            Assert.AreEqual(command.Name, queryName);
        }
    }
}
