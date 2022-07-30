using AutoFixture;
using NUnit.Framework;

namespace Atles.Tests.Unit
{
    [TestFixture]
    public abstract class TestFixtureBase
    {
        protected Fixture Fixture { get; private set; }

        [SetUp]
        protected void SetUpAutoFixture()
        {
            Fixture = new Fixture();

            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}
