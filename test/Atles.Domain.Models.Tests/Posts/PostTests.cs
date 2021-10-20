using Atles.Domain.PostReactions;
using Atles.Domain.Posts;
using AutoFixture;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atles.Domain.Models.Tests
{
    [TestFixture]
    public class PostTests : TestFixtureBase
    {
        [Test]
        public void Should_delete_post()
        {
            var sut = Fixture.Create<Post>();

            sut.Delete();

            Assert.AreEqual(PostStatusType.Deleted, sut.Status, nameof(sut.Status));
        }

        [Test]
        public void Should_add_reaction()
        {
            var sut = Fixture.Build<Post>().With(x => x.PostReactionCounts, new List<PostReactionCount>()).Create();

            sut.AddReaction(PostReactionType.Celebrate);

            Assert.AreEqual(1, sut.PostReactionCounts.FirstOrDefault(x => x.Type == PostReactionType.Celebrate).Count);
        }

        [Test]
        public void Should_remove_reaction()
        {
            var postReactionCount = new PostReactionCount(Guid.NewGuid(), PostReactionType.Insightful);
            postReactionCount.IncreaseCount();

            var sut = Fixture.Build<Post>().With(x => x.PostReactionCounts, new List<PostReactionCount> { postReactionCount }).Create();

            sut.RemoveReaction(PostReactionType.Insightful);

            Assert.AreEqual(1, sut.PostReactionCounts.FirstOrDefault(x => x.Type == PostReactionType.Insightful).Count);
        }
    }
}
