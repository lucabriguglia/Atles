using Atles.Domain;
using AutoFixture;
using NUnit.Framework;

namespace Atles.Tests.Unit.Domain
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
            var sut = Fixture.Build<Post>().With(x => x.PostReactionSummaries, new List<PostReactionSummary>()).Create();

            sut.AddReactionToSummary(PostReactionType.Celebrate);

            Assert.AreEqual(1, sut.PostReactionSummaries.FirstOrDefault(x => x.Type == PostReactionType.Celebrate).Count);
        }

        [Test]
        public void Should_decrease_reaction_count()
        {
            var postReactionCount = new PostReactionSummary(Guid.NewGuid(), PostReactionType.Insightful);
            postReactionCount.IncreaseCount();

            var sut = Fixture.Build<Post>().With(x => x.PostReactionSummaries, new List<PostReactionSummary> { postReactionCount }).Create();

            sut.RemoveReactionFromSummary(PostReactionType.Insightful);

            Assert.AreEqual(1, sut.PostReactionSummaries.FirstOrDefault(x => x.Type == PostReactionType.Insightful).Count);
        }
    }
}
