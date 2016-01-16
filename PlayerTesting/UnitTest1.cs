using System;


using NUnit.Framework;

namespace PlayerTesting
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            Assert.That(1, Is.EqualTo(1));
        }
    }
}
