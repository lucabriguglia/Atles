using Atles.Domain.Posts;
using AutoFixture;
using NUnit.Framework;

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
        public void Increase_likes_count()
        {
            var sut = Fixture.Create<Post>();

            var currentCount = sut.LikesCount;

            sut.IncreaseLikesCount();

            Assert.AreEqual(currentCount + 1, sut.LikesCount, nameof(sut.LikesCount));
        }

        [Test]
        public void Decrease_likes_count()
        {
            var sut = Fixture.Create<Post>();
            sut.IncreaseLikesCount();

            var currentCount = sut.LikesCount;

            sut.DecreaseLikesCount();

            Assert.AreEqual(currentCount - 1, sut.LikesCount, nameof(sut.LikesCount));
        }

        [Test]
        public void Decrease_likes_count_less_than_zero()
        {
            var sut = Fixture.Create<Post>();

            sut.DecreaseLikesCount();

            Assert.AreEqual(0, sut.LikesCount, nameof(sut.LikesCount));
        }

        [Test]
        public void Increase_dislikes_count()
        {
            var sut = Fixture.Create<Post>();

            var currentCount = sut.DislikesCount;

            sut.IncreaseDislikesCount();

            Assert.AreEqual(currentCount + 1, sut.DislikesCount, nameof(sut.DislikesCount));
        }

        [Test]
        public void Decrease_dislikes_count()
        {
            var sut = Fixture.Create<Post>();
            sut.IncreaseDislikesCount();

            var currentCount = sut.DislikesCount;

            sut.DecreaseDislikesCount();

            Assert.AreEqual(currentCount - 1, sut.DislikesCount, nameof(sut.DislikesCount));
        }

        [Test]
        public void Decrease_dislikes_count_less_than_zero()
        {
            var sut = Fixture.Create<Post>();

            sut.DecreaseDislikesCount();

            Assert.AreEqual(0, sut.DislikesCount, nameof(sut.DislikesCount));
        }
    }
}
