using System;
using System.Collections.Generic;
using System.Linq;
using Atles.Domain.Models.Posts;
using Atles.Domain.PostReactions;
using Atles.Domain.Posts;
using AutoFixture;
using NUnit.Framework;

namespace Atles.Domain.Models.Tests.Posts
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
        public void Should_increase_reaction_count()
        {
            var sut = Fixture.Build<Post>().With(x => x.PostReactionCounts, new List<PostReactionCount>()).Create();

            sut.IncreaseReactionCount(PostReactionType.Celebrate);

            Assert.AreEqual(1, sut.PostReactionCounts.FirstOrDefault(x => x.Type == PostReactionType.Celebrate).Count);
        }

        [Test]
        public void Should_decrease_reaction_count()
        {
            var postReactionCount = new PostReactionCount(Guid.NewGuid(), PostReactionType.Insightful);
            postReactionCount.IncreaseCount();

            var sut = Fixture.Build<Post>().With(x => x.PostReactionCounts, new List<PostReactionCount> { postReactionCount }).Create();

            sut.DecreaseReactionCount(PostReactionType.Insightful);

            Assert.AreEqual(1, sut.PostReactionCounts.FirstOrDefault(x => x.Type == PostReactionType.Insightful).Count);
        }
    }
}
