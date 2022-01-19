using System;
using Atles.Domain.Models.PermissionSets.Commands;
using Atles.Domain.Models.PermissionSets.Rules;
using Atles.Domain.Validators.PermissionSets;
using Atles.Infrastructure;
using Atles.Infrastructure.Queries;
using AutoFixture;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;

namespace Atles.Domain.Validators.Tests.PermissionSets
{
    [TestFixture]
    public class UpdatePermissionSetValidatorTests : TestFixtureBase
    {
        [Test]
        public void Should_have_validation_error_when_name_is_empty()
        {
            var command = Fixture.Build<UpdatePermissionSet>().With(x => x.Name, string.Empty).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new UpdatePermissionSetValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_too_long()
        {
            var command = Fixture.Build<UpdatePermissionSet>().With(x => x.Name, new string('*', 51)).Create();

            var dispatcher = new Mock<IDispatcher>();

            var sut = new UpdatePermissionSetValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
        }

        [Test]
        public void Should_have_validation_error_when_name_is_not_unique()
        {
            var command = Fixture.Create<UpdatePermissionSet>();

            var querySiteId = Guid.NewGuid();
            var queryName = string.Empty;
            Guid? queryId = null;

            var dispatcher = new Mock<IDispatcher>();
            dispatcher
                .Setup(x => x.Get(It.IsAny<IsPermissionSetNameUnique>()))
                .Callback<IQuery<bool>>(q => 
                {
                    var query = q as IsPermissionSetNameUnique;
                    querySiteId = query.SiteId;
                    queryName = query.Name;
                    queryId = query.Id;
                })
                .ReturnsAsync(false);

            var sut = new UpdatePermissionSetValidator(dispatcher.Object);

            sut.ShouldHaveValidationErrorFor(x => x.Name, command);
            Assert.AreEqual(command.SiteId, querySiteId);
            Assert.AreEqual(command.Name, queryName);
            Assert.AreEqual(command.Id, queryId);
        }
    }
}
